using EmployeePortal.common;
using EmployeePortal.Models;
using EmployeePortal.Services;
using EmployeePortal.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EmployeePortal.Controllers
{
    public class TimecardsController : Controller
    {
        private TimecardService timecardService = new TimecardService();
        private UserService userService = new UserService();

        private static Guid UserId { get; set; }
        private static string JobCode { get; set; }
        private static string AdjustmentType { get; set; }
        private static int _page;

        public ActionResult Index(int? page, string sortOrder)
        {
            GetApplicationUserId();
            Applicant employee = timecardService.GetEmployee(UserId);
            if (employee == null)
            {
                return RedirectToAction("Index", "Applicant");
            }
            // copy last week's timecard ANYTIME ***************************************************************************************************
            if (employee.TimecardCount > 0)
            {
                ViewBag.CopyTimecardEligible = true;
                ViewBag.HasTimecards = true;
            }
            else
            {
                ViewBag.CopyTimecardEligible = false;
                ViewBag.HasTimecards = false;
            }
            // wait until the CURRENT WEEK IS FINISHED before you can copy timecard -- UNCOMMENT THIS ...
            //if (lastTimecard != null)
            //{
            //    if (DateTime.Now > DateTime.Parse(lastTimecard.WeekEnding) && employee.TimecardCount > 0)
            //    {
            //        ViewBag.CopyTimecardEligible = true;
            //    }
            //    else
            //    {
            //        ViewBag.CopyTimecardEligible = false;
            //    }
            //}
            //**************************************************************************************************************************************
            ViewBag.EmployeeName = employee.FirstName + " " + employee.LastName;
            List<Timecard> timecards = timecardService.GetTimecards(employee.Id).ToList();
            int pageNumber, pagesize;
            AddSortingAndPaging(page, ref sortOrder, ref timecards, out pageNumber, out pagesize);
            if (timecards.Count > pagesize)
            {
                ViewBag.Paging = "ShowPaging";
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    return View(timecards.ToPagedList(pageNumber, pagesize));
                else
                    return View(timecards.OrderByDescending(c => c.CreatedDate).ToPagedList(pageNumber == 0 ? 1 : pageNumber, pagesize));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    return View(timecards.ToList());
                else
                    return View(timecards.OrderByDescending(c => c.CreatedDate).ToList());
            }
        }

        #region Copy
        public ActionResult ShowCopyModal()
        {
            return PartialView("_copyTimecard", new Timecard());
        }

        public ActionResult CopyLastTimecardJobCode()
        {
            ViewBag.WeekEndingSort = null;
            ViewBag.ApprovalStatusSort = null;
            ViewBag.CustomerCodeSort = null;
            ViewBag.JobCodeSort = null;
            ViewBag.TotalHoursSort = null;
            ViewBag.TotalMilesSort = null;
            ViewBag.AdjustmentHoursSort = null;
            ViewBag.RegularHoursSort = null;
            if (Session["sort"] != null)
                Session["sort"] = null;
            GetApplicationUserId();
            Applicant employee = timecardService.GetEmployee(UserId);
            if (employee == null)
            {
                RedirectToAction("Index", "Applicant");
            }
            Timecard lastTimecard = timecardService.GetLastTimecard(employee.Id);
            Timecard newTimecard;
            // NEXT WEEK SHOULD BE BASED ON THE LAST TIMECARD WEEKENDING - i.e IF WEEK ENDED ON 9/2/2016 - PICK UP NEW WEEK STARTING 9/3/2016
            if (lastTimecard != null)
            {
                IList<string> nextWeek = timecardService.GetNextWeek();
                newTimecard = new Timecard
                {
                    RegisterId = lastTimecard.RegisterId,
                    EmployeeID = lastTimecard.EmployeeID,
                    EmployeeName = lastTimecard.EmployeeName,
                    CreatedByIPAddress = GetLocalIPAddress().ToString(),
                    CreatedByComputerName = Environment.MachineName,
                    CreatedDate = DateTime.Now,
                    CreatedByDeviceType = GetDeviceType(),
                    WeekEnding = nextWeek[6],
                    JobCode = JobCode,
                    JobCodeId = timecardService.GetJobCodeId(JobCode),
                    ApprovalStatus = "Pending...",
                    TotalRegular = lastTimecard.TotalRegular,
                    TotalHours = lastTimecard.TotalRegular
                };
                var jobcode = timecardService.GetJobCode(newTimecard.JobCode);
                newTimecard.CustomerCode = timecardService.GetCustomerCode(jobcode.CustomerCode);
                // THIS LINE ALLOWS YOU TO SEE THE MODEL ERRORS - look at common/ModelErrorChecker.cs class
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    try
                    {
                        timecardService.AddTimecard(newTimecard);
                        List<TimecardEntry> entries = timecardService.GetTimecardEntries(lastTimecard.Id).ToList();
                        TimecardEntry entry;
                        foreach (var item in entries)
                        {
                            entry = new TimecardEntry
                            {
                                TimecardId = newTimecard.Id,
                                StartTime = item.StartTime,
                                StartMiles = item.StartMiles,
                                Break1 = item.Break1,
                                Break2 = item.Break2,
                                EndTime = item.EndTime,
                                EndMiles = item.EndMiles,
                                TotalHours = item.TotalHours,
                                Regular = item.Regular,
                                Overtime = item.Overtime,
                                Vacation = item.Vacation,
                                DayOfWeek = item.DayOfWeek,
                                TotalMiles = item.TotalMiles,
                                Doubletime = item.Doubletime,
                                SickTime = item.SickTime,
                                LunchIn = item.LunchIn,
                                LunchOut = item.LunchOut,
                                WorkDate = item.WorkDate.AddDays(7)
                            };
                            newTimecard.WeekEnding = DateTime.Parse(lastTimecard.WeekEnding).AddDays(7).ToShortDateString();
                            timecardService.AddTimeCardEntry(entry);
                        }
                        return RedirectToAction("Edit", "Timecards", new { id = newTimecard.Id });
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Timecards", new { id = newTimecard.Id });
            }
            return RedirectToAction("Create");
        }

        // YOU CAN use this method if you want to just copy last week's timecard without making them select a job code first
        //public ActionResult CopyLastTimecard()
        //{
        //    ViewBag.WeekEndingSort = null;
        //    ViewBag.ApprovalStatusSort = null;
        //    ViewBag.CustomerCodeSort = null;
        //    ViewBag.JobCodeSort = null;
        //    ViewBag.TotalHoursSort = null;
        //    ViewBag.TotalMilesSort = null;
        //    ViewBag.AdjustmentHoursSort = null;
        //    ViewBag.RegularHoursSort = null;
        //    if (Session["sort"] != null)
        //        Session["sort"] = null;
        //    GetApplicationUserId();
        //    Applicant employee = timecardService.GetEmployee(UserId);
        //    if (employee == null)
        //    {
        //        RedirectToAction("Index", "Applicant");
        //    }
        //    Timecard lastTimecard = timecardService.GetLastTimecard(employee.Id);
        //    Timecard newTimecard;
        //    if (lastTimecard != null)
        //    {
        //        IList<string> nextWeek = timecardService.GetNextWeek();
        //        newTimecard = new Timecard
        //        {
        //            RegisterId = lastTimecard.RegisterId,
        //            EmployeeID = lastTimecard.EmployeeID,
        //            EmployeeName = lastTimecard.EmployeeName,
        //            CreatedByIPAddress = GetLocalIPAddress().ToString(),
        //            CreatedByComputerName = Environment.MachineName,
        //            CreatedDate = DateTime.Now,
        //            CreatedByDeviceType = GetDeviceType(),
        //            WeekEnding = nextWeek[6],
        //            JobCodeId = lastTimecard.JobCodeId,
        //            JobCode = lastTimecard.JobCode,
        //            CustomerCode = lastTimecard.CustomerCode,
        //            ApprovalStatus = "Pending...",
        //            TotalRegular = lastTimecard.TotalRegular,
        //            TotalHours = lastTimecard.TotalRegular
        //        };
        //        // THIS LINE ALLOWS YOU TO SEE THE MODEL ERRORS - look at common/ModelErrorChecker.cs class
        //        var errors = ModelState.Values.SelectMany(v => v.Errors);
        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                timecardService.AddTimecard(newTimecard);
        //                List<TimecardEntry> entries = timecardService.GetTimecardEntries(lastTimecard.Id).ToList();
        //                TimecardEntry entry;
        //                foreach (var item in entries)
        //                {
        //                    entry = new TimecardEntry
        //                    {
        //                        TimecardId = newTimecard.Id,
        //                        StartTime = item.StartTime,
        //                        StartMiles = item.StartMiles,
        //                        Break1 = item.Break1,
        //                        Break2 = item.Break2,
        //                        EndTime = item.EndTime,
        //                        EndMiles = item.EndMiles,
        //                        TotalHours = item.TotalHours,
        //                        Regular = item.Regular,
        //                        Overtime = item.Overtime,
        //                        Vacation = item.Vacation,
        //                        DayOfWeek = item.DayOfWeek,
        //                        TotalMiles = item.TotalMiles,
        //                        Doubletime = item.Doubletime,
        //                        SickTime = item.SickTime,
        //                        LunchIn = item.LunchIn,
        //                        LunchOut = item.LunchOut,
        //                        WorkDate = item.WorkDate.AddDays(7)
        //                    };
        //                    newTimecard.WeekEnding = DateTime.Parse(lastTimecard.WeekEnding).AddDays(7).ToShortDateString();
        //                    timecardService.AddTimeCardEntry(entry);
        //                }
        //                return RedirectToAction("Edit", "Timecards", new { id = newTimecard.Id });
        //            }
        //            catch (DbEntityValidationException e)
        //            {
        //                foreach (var eve in e.EntityValidationErrors)
        //                {
        //                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //                    foreach (var ve in eve.ValidationErrors)
        //                    {
        //                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
        //                            ve.PropertyName, ve.ErrorMessage);
        //                    }
        //                }
        //                throw;
        //            }
        //        }
        //        return RedirectToAction("Edit", "Timecards", new { id = newTimecard.Id });
        //    }
        //    return RedirectToAction("Create");
        //}
        #endregion

        #region TestMethods
        public ActionResult CreateTestTimecard()
        {
            ViewBag.WeekEndingSort = null;
            ViewBag.ApprovalStatusSort = null;
            ViewBag.CustomerCodeSort = null;
            ViewBag.JobCodeSort = null;
            ViewBag.TotalHoursSort = null;
            ViewBag.TotalMilesSort = null;
            ViewBag.AdjustmentHoursSort = null;
            ViewBag.RegularHoursSort = null;
            if (Session["sort"] != null)
                Session["sort"] = null;
            GetApplicationUserId();
            Applicant employee = timecardService.GetEmployee(UserId);
            if (employee == null)
            {
                RedirectToAction("Index", "Applicant");
            }
            ViewBag.TimecardCount = employee.TimecardCount;
            Timecard timecard = new Timecard
            {
                RegisterId = UserId,
                EmployeeID = employee.Id,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                CreatedByIPAddress = GetLocalIPAddress().ToString(),
                CreatedByComputerName = Environment.MachineName,
                CreatedByDeviceType = GetDeviceType(),
                CreatedDate = DateTime.Now,
                JobCode = "JobCode",
                CustomerCode = "CustomerCode",
            };
            IList<string> currentWeek = timecardService.GetCurrentWeek();
            timecard.WeekEnding = currentWeek[6];
            timecard.JobCode = "006";
            timecard.JobCodeId = timecardService.GetJobCodeId(timecard.JobCode);
            var jobcode = timecardService.GetJobCode(timecard.JobCode);
            timecard.CustomerCode = timecardService.GetCustomerCode(jobcode.CustomerCode);
            timecard.ApprovalStatus = "Pending ...";
            timecard.TotalHours = 56.0m;
            timecard.TotalRegular = 56.0m;
            // THIS LINE ALLOWS YOU TO SEE THE MODEL ERRORS - look at common/ModelErrorChecker.cs class
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicantService applicantService = new ApplicantService();
                    timecardService.AddTimecard(timecard);
                    CreateTimecardEntries(timecard, currentWeek);
                    employee.TimecardCount++;
                    applicantService.UpdateApplicant(employee);
                    ViewBag.TimecardCount = employee.TimecardCount;
                    List<TimecardEntry> entries = timecardService.GetTimecardEntries(timecard.Id).ToList();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            entries[i].StartTime = "12:00am";
                            entries[i].Break1 = 15;
                            entries[i].Break2 = 15;
                            entries[i].LunchOut = "3:30am";
                            entries[i].LunchIn = "4:00am";
                            entries[i].EndTime = "8:30am";
                        }
                        else
                        {
                            entries[i].StartTime = "3:30pm";
                            entries[i].Break1 = 15;
                            entries[i].Break2 = 15;
                            entries[i].LunchOut = "7:30pm";
                            entries[i].LunchIn = "8:00pm";
                            entries[i].EndTime = "12:00am";
                        }
                        entries[i].Regular = 8.0m;
                        timecardService.UpdateTimecardEntry(entries[i]);
                    }
                    return RedirectToAction("Edit", timecard);
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            return View(timecard);
        }

        public ActionResult CreateTimecardList()
        {
            var db = new ApplicationDbContext();
            Random r = new Random();
            ViewBag.WeekEndingSort = null;
            ViewBag.ApprovalStatusSort = null;
            ViewBag.CustomerCodeSort = null;
            ViewBag.JobCodeSort = null;
            ViewBag.TotalHoursSort = null;
            ViewBag.TotalMilesSort = null;
            ViewBag.AdjustmentHoursSort = null;
            ViewBag.RegularHoursSort = null;
            if (Session["sort"] != null)
                Session["sort"] = null;
            GetApplicationUserId();
            Applicant employee = timecardService.GetEmployee(UserId);
            if (employee == null)
            {
                RedirectToAction("Index", "Applicant");
            }
            ViewBag.TimecardCount = employee.TimecardCount;
            string weekEnding = string.Empty;
            for (int counter = 0; counter < 26; counter++)
            {
                Timecard timecard = new Timecard
                {
                    RegisterId = UserId,
                    EmployeeID = employee.Id,
                    EmployeeName = employee.FirstName + " " + employee.LastName,
                    CreatedByIPAddress = GetLocalIPAddress().ToString(),
                    CreatedByComputerName = Environment.MachineName,
                    CreatedByDeviceType = GetDeviceType(),
                    CreatedDate = DateTime.Now,
                    CustomerCode = "CustomerCode",
                };
                IList<string> currentWeek = timecardService.GetCurrentWeek();
                IList<string> nextWeek = timecardService.GetNextWeek(counter);
                if (counter == 0)
                {
                    timecard.WeekEnding = currentWeek[6];
                }
                else
                {
                    timecard.WeekEnding = nextWeek[6];
                }
                r = new Random();
                int jobcodeid = r.Next(1, 8);
                if (jobcodeid == 5)
                    jobcodeid = 7;
                timecard.JobCode = db.JobCodes.Where(i => i.Id == jobcodeid).Select(c => c.Code).FirstOrDefault();
                timecard.JobCodeId = timecardService.GetJobCodeId(timecard.JobCode);
                var jobcode = timecardService.GetJobCode(timecard.JobCode);
                timecard.CustomerCode = timecardService.GetCustomerCode(jobcode.CustomerCode);
                timecard.ApprovalStatus = "Submitted";
                timecard.LastSubmittedDate = DateTime.Now;
                timecard.TotalRegular = 56.0m;
                timecard.TotalHours = 56.0m;
                timecard.SubmittedCount++;
                timecard.TimecardSubmitted = true;
                // THIS LINE ALLOWS YOU TO SEE THE MODEL ERRORS - look at common/ModelErrorChecker.cs class
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    try
                    {
                        ApplicantService applicantService = new ApplicantService();
                        timecardService.AddTimecard(timecard);
                        for (int i = 0; i < 7; i++)
                        {
                            TimecardEntry entry = new TimecardEntry
                            {
                                WorkDate = Convert.ToDateTime(DateTime.Parse(nextWeek[i])),
                                TimecardId = timecard.Id,
                                DayOfWeek = ApplicationConstants.Days[i]
                            };
                            timecardService.AddTimeCardEntry(entry);
                        }
                        employee.TimecardCount++;
                        applicantService.UpdateApplicant(employee);
                        ViewBag.TimecardCount = employee.TimecardCount;
                        List<TimecardEntry> entries = timecardService.GetTimecardEntries(timecard.Id).ToList();
                        for (int i = 0; i < entries.Count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                entries[i].StartTime = "12:00am";
                                entries[i].Break1 = 15;
                                entries[i].Break2 = 15;
                                entries[i].LunchOut = "3:30am";
                                entries[i].LunchIn = "4:00am";
                                entries[i].EndTime = "8:30am";
                            }
                            else
                            {
                                entries[i].StartTime = "3:30pm";
                                entries[i].Break1 = 15;
                                entries[i].Break2 = 15;
                                entries[i].LunchOut = "7:30pm";
                                entries[i].LunchIn = "8:00pm";
                                entries[i].EndTime = "12:00am";
                            }
                            entries[i].Regular = 8.00m;
                            timecardService.UpdateTimecardEntry(entries[i]);
                        }
                        weekEnding = timecard.WeekEnding;
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAllTimecards()
        {
            ViewBag.WeekEndingSort = null;
            ViewBag.ApprovalStatusSort = null;
            ViewBag.CustomerCodeSort = null;
            ViewBag.JobCodeSort = null;
            ViewBag.TotalHoursSort = null;
            ViewBag.TotalMilesSort = null;
            ViewBag.AdjustmentHoursSort = null;
            ViewBag.RegularHoursSort = null;
            if (Session["sort"] != null)
                Session["sort"] = null;
            GetApplicationUserId();
            Applicant employee = timecardService.GetEmployee(UserId);
            if (employee == null)
            {
                RedirectToAction("Index", "Applicant");
            }
            List<Timecard> timecards = timecardService.GetTimecards(employee.Id).ToList();
            foreach (var timecard in timecards)
            {
                timecardService.DeleteTimecard(timecard);
            }
            employee.TimecardCount = 0;
            ApplicantService applicantService = new ApplicantService();
            applicantService.UpdateApplicant(employee);
            ViewBag.CopyTimecardEligible = false;
            ViewBag.HasTimecards = false;
            return RedirectToAction("Index");
        }
        #endregion

        #region CreateMethods
        public ActionResult Create()
        {
            GetApplicationUserId();
            Applicant employee = timecardService.GetEmployee(UserId);
            if (employee == null)
            {
                RedirectToAction("Index", "Applicant");
            }
            ViewBag.TimecardCount = employee.TimecardCount;
            Timecard timecard = new Timecard
            {
                RegisterId = UserId,
                EmployeeID = employee.Id,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                CreatedByIPAddress = GetLocalIPAddress().ToString(),
                CreatedByComputerName = Environment.MachineName,
                CreatedByDeviceType = GetDeviceType(),
                CreatedDate = DateTime.Now,
                JobCode = "Select Job Code",
            };
            return View(timecard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RegisterId,Approved,CustomerCode,JobCode,JobCodeId,File,Comments,WeekEnding,TotalHours,TotalMiles,LunchIn,LunchOut,StartTime,EndTime,Break1,Break2,TotalRegular,TotalOvertime,TotalVacation,EmployeeID,CreatedByIPAddress,CreatedByComputerName,CreatedByDeviceType,CreatedDate, EmployeeName")] Timecard timecard)
        {
            IList<string> currentWeek = timecardService.GetCurrentWeek();
            timecard.WeekEnding = currentWeek[6];
            timecard.JobCode = JobCode;
            timecard.JobCodeId = timecardService.GetJobCodeId(timecard.JobCode);
            var jobcode = timecardService.GetJobCode(timecard.JobCode);
            timecard.CustomerCode = timecardService.GetCustomerCode(jobcode.CustomerCode);
            timecard.ApprovalStatus = "Pending ...";

            // THIS LINE ALLOWS YOU TO SEE THE MODEL ERRORS - look at common/ModelErrorChecker.cs class
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicantService applicantService = new ApplicantService();
                    timecardService.AddTimecard(timecard);
                    CreateTimecardEntries(timecard, currentWeek);
                    Applicant employee = timecardService.GetEmployeeByTimecardId(timecard);
                    employee.TimecardCount++;
                    applicantService.UpdateApplicant(employee);
                    ViewBag.TimecardCount = employee.TimecardCount;
                    return RedirectToAction("Edit", timecard);
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            return View(timecard);
        }
        #endregion

        #region EditMethods
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timecard timecard = timecardService.GetById((int)id);
            TimecardViewModel vm = new TimecardViewModel();
            vm.ApprovalStatus = timecard.ApprovalStatus;
            vm.WeekEnding = timecard.WeekEnding;
            vm.JobCode = timecard.JobCode;
            vm.CustomerCode = timecard.CustomerCode;
            vm.TotalHours = timecard.TotalHours;
            vm.TotalMiles = timecard.TotalMiles;
            vm.EmployeeName = timecard.EmployeeName;
            vm.TimecardId = timecard.Id;
            vm.TotalRegular = timecard.TotalRegular;
            vm.TimecardSubmitted = timecard.TimecardSubmitted;
            vm.EmployeeId = timecard.EmployeeID;
            vm.TimeEntries = timecardService.GetTimecardEntries(timecard.Id).OrderBy(w => w.WorkDate).ToList();
            vm.Adjustments = timecardService.GetTimecardAdjustments(timecard.Id).OrderByDescending(a => a.AdjustmentDate).ToList();
            vm.HasAdjustments = vm.Adjustments.Count > 0 ? true : false;
            for (int i = 0; i < vm.TimeEntries.Count; i++)
            {
                vm.StartMiles = vm.TimeEntries[i].StartMiles;
                vm.EndMiles = vm.TimeEntries[i].EndMiles;
            }
            if (timecard == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TimecardViewModel vm, string submit)
        {
            var timecard = timecardService.GetById(vm.TimecardId);
            var timecardEntries = timecardService.GetTimecardEntries(vm.TimecardId).OrderBy(w => w.WorkDate).ToList();
            for (int i = 0; i < vm.TimeEntries.Count; i++)
            {
                timecardEntries[i].StartMiles = Math.Round(vm.TimeEntries[i].StartMiles, 2, MidpointRounding.AwayFromZero);
                timecardEntries[i].Break1 = vm.TimeEntries[i].Break1;
                timecardEntries[i].Break2 = vm.TimeEntries[i].Break2;
                timecardEntries[i].DayOfWeek = vm.TimeEntries[i].DayOfWeek;
                timecardEntries[i].WorkDate = vm.TimeEntries[i].WorkDate;
                timecardEntries[i].EndMiles = Math.Round(vm.TimeEntries[i].EndMiles, 2, MidpointRounding.AwayFromZero);
                timecardEntries[i].StartTime = vm.TimeEntries[i].StartTime;
                timecardEntries[i].EndTime = vm.TimeEntries[i].EndTime;
                timecardEntries[i].LunchOut = vm.TimeEntries[i].LunchOut;
                timecardEntries[i].LunchIn = vm.TimeEntries[i].LunchIn;
            }
            if (submit == "Approve")
                timecard.ApprovalStatus = "Approved";
            if (submit == "Reject")
                timecard.ApprovalStatus = "Rejected";
            decimal totalRegular = 0.0m;
            foreach (var entry in timecardEntries)
            {
                if (entry.Regular > 0)
                {
                    timecard.TotalHours -= entry.Regular;
                    entry.TotalHours -= entry.Regular;
                    entry.Regular = 0.0m;
                }
                TimeSpan starttime = new TimeSpan();
                TimeSpan endtime = new TimeSpan();
                TimeSpan startLunch = new TimeSpan();
                TimeSpan endLunch = new TimeSpan();
                DateTime endOfDay = new DateTime(entry.WorkDate.Year, entry.WorkDate.Month, entry.WorkDate.Day, 23, 59, 59);
                DateTime startOfDay = new DateTime(entry.WorkDate.Year, entry.WorkDate.Month, entry.WorkDate.Day, 00, 00, 01);
                int minutes = 0;
                if (!string.IsNullOrWhiteSpace(entry.StartTime))
                    starttime = DateTime.Parse(entry.StartTime).TimeOfDay;
                if (!string.IsNullOrWhiteSpace(entry.EndTime))
                    endtime = DateTime.Parse(entry.EndTime).TimeOfDay;
                if (!string.IsNullOrWhiteSpace(entry.LunchOut))
                    startLunch = DateTime.Parse(entry.LunchOut).TimeOfDay;
                if (!string.IsNullOrWhiteSpace(entry.LunchIn))
                    endLunch = DateTime.Parse(entry.LunchIn).TimeOfDay;
                if (entry.EndTime != null && endtime.Hours == 0 && endtime.Minutes == 0 && endtime.Seconds == 0 && DateTime.Parse(entry.StartTime) != DateTime.Parse(entry.EndTime))
                {
                    endtime = endOfDay.TimeOfDay;
                }
                if (entry.StartTime != null && starttime.Hours == 0 && starttime.Minutes == 0 && starttime.Seconds == 0 && DateTime.Parse(entry.StartTime) != DateTime.Parse(entry.EndTime))
                {
                    starttime = startOfDay.TimeOfDay;
                }
                int starttimeMinutes = starttime == startOfDay.TimeOfDay ? 0 : (int)starttime.TotalMinutes;
                int endtimeMinutes = endtime == endOfDay.TimeOfDay ? 1440 : (int)endtime.TotalMinutes;
                int startlunchMinutes = (int)startLunch.TotalMinutes;
                int endlunchMinutes = (int)endLunch.TotalMinutes;
                if (starttimeMinutes > 0 || starttime == startOfDay.TimeOfDay)
                {
                    if (endtimeMinutes == 0)
                    {
                        if (startlunchMinutes > 0)
                            minutes = startlunchMinutes - starttimeMinutes;
                    }
                    else
                    {
                        if (startlunchMinutes > 0 && endlunchMinutes > 0)
                            minutes = endtimeMinutes - (endlunchMinutes - startlunchMinutes) - starttimeMinutes;
                        else if (startlunchMinutes == 0)
                            minutes = endtimeMinutes - starttimeMinutes;
                        else if (startlunchMinutes > 0 && endlunchMinutes == 0)
                            minutes = startlunchMinutes - starttimeMinutes;
                    }
                }
                else
                {
                    minutes = 0;
                }
                if (minutes > 0)
                    entry.Regular = Convert.ToDecimal(((decimal)minutes / 60).ToString("#.##"));
                totalRegular += entry.Regular;
                entry.TotalHours += entry.Regular;
                timecardService.UpdateTimecardEntry(entry);
            }
            timecard.TotalRegular = totalRegular;
            timecard.TotalHours += totalRegular;
            timecard.LastModifiedByComputerName = Environment.MachineName;
            timecard.LastModifiedByDeviceType = GetDeviceType();
            timecard.LastModifiedByIPAddress = GetLocalIPAddress().ToString();
            timecard.LastModifiedDate = DateTime.Now;
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                timecardService.Update(timecard);
            }
            if (submit == "Submit Timecard")
            {
                timecard.ApprovalStatus = "Submitted";
                timecard.TimecardSubmitted = true;
                timecard.SubmittedCount++;
                timecard.LastSubmittedDate = DateTime.Now;
                timecardService.Update(timecard);
                return RedirectToAction("Index", "Timecards");
            }
            vm.ApprovalStatus = timecard.ApprovalStatus;
            vm.WeekEnding = timecard.WeekEnding;
            vm.JobCode = timecard.JobCode;
            vm.CustomerCode = timecard.CustomerCode;
            vm.TotalHours = timecard.TotalHours;
            vm.TotalRegular = timecard.TotalRegular;
            vm.TotalOvertime = timecard.TotalOvertime;
            vm.DoubleTime = timecard.TotalDoubletime;
            vm.SickTime = timecard.TotalSickTime;
            vm.TotalVacation = timecard.TotalVacation;
            vm.TotalMiles = timecard.TotalMiles;
            vm.EmployeeName = timecard.EmployeeName;
            vm.EmployeeId = timecard.EmployeeID;
            vm.Adjustments = timecardService.GetTimecardAdjustments(timecard.Id).OrderByDescending(a => a.AdjustmentDate).ToList();
            vm.TimeEntries = timecardService.GetTimecardEntries(vm.TimecardId).OrderBy(w => w.WorkDate).ToList();
            return View(vm);
        }

        public ActionResult AddTimecardEntry(int timecardId)
        {
            Timecard timecard = timecardService.GetById(timecardId);
            TimecardViewModel vm = new TimecardViewModel
            {
                WeekEnding = timecard.WeekEnding,
                TimecardId = timecardId,
            };
            return PartialView("_addEntry", vm);
        }

        //[HttpPost]
        //public ActionResult AddTimecardEntry(TimecardViewModel vm)
        //{
        //    TimecardEntry parentEntry = timecardService.GetParentEntry(vm.TimecardId, vm.WorkDate);
        //    TimecardEntry entry = new TimecardEntry
        //    {
        //        ParentId = parentEntry.Id,
        //        WorkDate = vm.WorkDate,
        //        TimecardId = vm.TimecardId,
        //        CreatedDate = DateTime.Now,
        //        CreatedByComputerName = Environment.MachineName,
        //        CreatedByDeviceType = GetDeviceType(),
        //        CreatedByIPAddress = GetLocalIPAddress().ToString()
        //    };
        //    timecardService.AddTimeCardEntry(entry);
        //    return RedirectToAction("Edit", new { id = entry.TimecardId });
        //}

        [HttpPost]
        public ActionResult AddTimecardEntry(TimecardViewModel vm)
        {
            TimecardEntry parentEntry = timecardService.GetParentEntry(vm.TimecardId, vm.WorkDate);
            TimecardEntry entry = new TimecardEntry
            {
                ParentId = parentEntry.Id,
                WorkDate = parentEntry.WorkDate,
                DayOfWeek = parentEntry.DayOfWeek,
                TimecardId = parentEntry.TimecardId,
                CreatedDate = DateTime.Now,
                CreatedByComputerName = Environment.MachineName,
                CreatedByDeviceType = GetDeviceType(),
                CreatedByIPAddress = GetLocalIPAddress().ToString(),
                IsChild = true
            };
            parentEntry.HasChildren = true;
            // get siblings
            var siblings = timecardService.GetSiblings(entry.Id, entry.ParentId).ToList();
            if (siblings.Count > 0)
            {
                entry.HasSiblings = true;
            }
            timecardService.AddTimeCardEntry(entry);
            return RedirectToAction("Edit", new { id = entry.TimecardId });
        }

        public ActionResult CreateAdjustment(int timecardId, int employeeId)
        {
            Timecard timecard = timecardService.GetById(timecardId);
            IList<TimecardEntry> timecardEntries = timecardService.GetTimecardEntries(timecard.Id).ToList();
            Applicant employee = timecardService.GetEmployeeByTimecardId(timecard);
            TimecardViewModel vm = new TimecardViewModel
            {
                EmployeeName = employee.FirstName + " " + employee.LastName,
                JobCode = timecard.JobCode,
                TimecardId = timecard.Id,
                WeekEnding = timecard.WeekEnding,
                EmployeeId = employee.Id,
                TotalHours = timecard.TotalHours,
            };
            return PartialView("_addAdjustment", vm);
        }

        [HttpPost]
        public ActionResult CreateAdjustment(TimecardViewModel vm)
        {
            vm.Type = timecardService.SetAdjustmentType((int)vm.TimecardTypeId);
            Timecard timecard = timecardService.GetById(vm.TimecardId);
            TimecardEntry entry = timecardService.GetTimecardEntry(vm.WorkDate, vm.TimecardId);
            Adjustment adjustment = new Adjustment();
            decimal adjustmentHours = Math.Round(vm.AdjustmentHours, 2, MidpointRounding.AwayFromZero);
            decimal adjustmentMiles = Math.Round(vm.EndMiles - vm.StartMiles, 2, MidpointRounding.AwayFromZero);
            switch (vm.Type)
            {
                case "Overtime":
                    timecard.TotalOvertime += adjustmentHours;
                    entry.Overtime += adjustmentHours;
                    adjustment.AdjustmentType = "Overtime";
                    adjustment.AdjustmentQuantity = adjustmentHours;
                    break;
                case "Double Time":
                    timecard.TotalDoubletime += adjustmentHours;
                    entry.Doubletime += adjustmentHours;
                    adjustment.AdjustmentType = "Double Time";
                    adjustment.AdjustmentQuantity = adjustmentHours;
                    break;
                case "Vacation":
                    timecard.TotalVacation += adjustmentHours;
                    entry.Vacation += adjustmentHours;
                    adjustment.AdjustmentType = "Vacation";
                    adjustment.AdjustmentQuantity = adjustmentHours;
                    break;
                case "Sick":
                    timecard.TotalSickTime += adjustmentHours;
                    entry.SickTime += adjustmentHours;
                    adjustment.AdjustmentType = "Sick";
                    adjustment.AdjustmentQuantity = adjustmentHours;
                    break;
                case "Holiday":
                    timecard.TotalHoliday += adjustmentHours;
                    entry.Holiday += adjustmentHours;
                    adjustment.AdjustmentType = "Holiday";
                    adjustment.AdjustmentQuantity = adjustmentHours;
                    break;
                case "Mileage Entry":
                    timecard.TotalMiles += adjustmentMiles;
                    entry.TotalMiles += adjustmentMiles;
                    entry.StartMiles = vm.StartMiles;
                    entry.EndMiles = vm.EndMiles;
                    vm.TotalMiles = entry.TotalMiles;
                    adjustment.AdjustmentType = "Mileage Entry";
                    adjustment.AdjustmentQuantity = adjustmentMiles;
                    break;
            }
            adjustment.AdjustmentDate = DateTime.Now;
            adjustment.TimecardId = timecard.Id;
            adjustment.TimecardEntryId = entry.Id;
            adjustment.WorkDate = entry.WorkDate;
            timecard.TotalHours += adjustmentHours;
            entry.TotalHours += adjustmentHours;
            entry.HasAdjustments = true;
            vm.HasAdjustments = true;
            timecardService.Update(timecard);
            timecardService.UpdateTimecardEntry(entry);
            timecardService.AddTimecardAdjustment(adjustment);
            return RedirectToAction("Edit", new { id = timecard.Id });
        }

        #endregion
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timecard timecard = timecardService.GetById((int)id);
            if (timecard == null)
            {
                return HttpNotFound();
            }
            return View(timecard);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timecard timecard = timecardService.GetById((int)id);
            if (timecard == null)
            {
                return HttpNotFound();
            }
            return View(timecard);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Timecard timecard = timecardService.GetById(id);
            timecardService.DeleteTimecard(timecard);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timecardService.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helper Methods
        private void GetApplicationUserId()
        {
            ApplicationUser user = new ApplicationUser();
            UserManager<ApplicationUser> uManager = userService.GetUserManager();
            user = uManager.FindById(User.Identity.GetUserId());
            Guid userId = new Guid(User.Identity.GetUserId());
            UserId = userId;
        }

        public void SetJobCode(int jobcode)
        {
            JobCode = timecardService.SetJobCode(jobcode);
        }

        private void CreateTimecardEntries(Timecard timecard, IList<string> currentWeek)
        {
            for (int i = 0; i < currentWeek.Count; i++)
            {
                TimecardEntry entry = new TimecardEntry
                {
                    WorkDate = Convert.ToDateTime(currentWeek[i]),
                    TimecardId = timecard.Id,
                    DayOfWeek = ApplicationConstants.Days[i],
                    CreatedByIPAddress = GetLocalIPAddress().ToString(),
                    CreatedByComputerName = Environment.MachineName,
                    CreatedDate = DateTime.Now,
                    CreatedByDeviceType = GetDeviceType()
                };
                timecardService.AddTimeCardEntry(entry);
            }
        }



        private static string GetDeviceType()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                return "Windows Desktop";
            }
            else if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                return "Windows Mobile";
            }
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return "Mac";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return "Unix";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Xbox)
            {
                return "XBox";
            }
            return "";
        }

        private static IPAddress GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public void AddSortingAndPaging(int? page, ref string sortOrder, ref List<Timecard> timecards, out int pageNumber, out int pagesize)
        {
            if (Request.UrlReferrer != null)
            {
                if (!Request.UrlReferrer.ToString().ToLower().Contains("timecards"))
                {
                    page = 1;
                }
            }
            pageNumber = (page ?? 1);
            if (page != null)
            {
                TempData["page"] = page;
                _page = (int)TempData["page"];
            }
            else
            {
                if (TempData["page"] != null)
                {
                    pageNumber = (int)TempData["page"];
                }
                else
                {
                    pageNumber = _page;
                }
            }
            if (sortOrder == null)
                if (Session["sort"] != null)
                    sortOrder = Session["sort"].ToString();
            pagesize = 10;
            ViewBag.WeekEndingSort = "WeekEndingDesc";
            ViewBag.ApprovalStatusSort = "ApprovalStatusDesc";
            ViewBag.CustomerCodeSort = "CustomerCodeDesc";
            ViewBag.JobCodeSort = "JobCodeDesc";
            ViewBag.TotalHoursSort = "TotalHoursDesc";
            ViewBag.TotalMilesSort = "TotalMilesDesc";
            ViewBag.AdjustmentHoursSort = "AdjustmentHoursDesc";
            ViewBag.RegularHoursSort = "RegularHoursDesc";
            switch (sortOrder)
            {
                case "WeekEndingAsc":
                    timecards = timecards.OrderByDescending(a => a.WeekEnding).ToList();
                    ViewBag.WeekEndingSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "WeekEndingDesc":
                    timecards = timecards.OrderBy(a => a.WeekEnding).ToList();
                    ViewBag.WeekEndingSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "ApprovalStatusAsc":
                    timecards = timecards.OrderByDescending(a => a.ApprovalStatus).ToList();
                    ViewBag.ApprovalStatusSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "ApprovalStatusDesc":
                    timecards = timecards.OrderBy(a => a.ApprovalStatus).ToList();
                    ViewBag.ApprovalStatusSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "CustomerCodeAsc":
                    timecards = timecards.OrderByDescending(c => c.CustomerCode).ToList();
                    ViewBag.CustomerCodeSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "CustomerCodeDesc":
                    timecards = timecards.OrderBy(c => c.CustomerCode).ToList();
                    ViewBag.CustomerCodeSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "JobCodeAsc":
                    timecards = timecards.OrderByDescending(j => j.JobCode).ToList();
                    ViewBag.JobCodeSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "JobCodeDesc":
                    timecards = timecards.OrderBy(j => j.JobCode).ToList();
                    ViewBag.JobCodeSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "TotalHoursAsc":
                    timecards = timecards.OrderByDescending(th => th.TotalHours).ToList();
                    ViewBag.TotalHoursSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "TotalHoursDesc":
                    timecards = timecards.OrderBy(th => th.TotalHours).ToList();
                    ViewBag.TotalHoursSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "RegularHoursAsc":
                    timecards = timecards.OrderByDescending(tr => tr.TotalRegular).ToList();
                    ViewBag.RegularHoursSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "RegularHoursDesc":
                    timecards = timecards.OrderBy(tr => tr.TotalRegular).ToList();
                    ViewBag.RegularHoursSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "TotalMilesAsc":
                    timecards = timecards.OrderByDescending(tm => tm.TotalMiles).ToList();
                    ViewBag.TotalMilesSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                case "TotalMilesDesc":
                    timecards = timecards.OrderBy(tm => tm.TotalMiles).ToList();
                    ViewBag.TotalMilesSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
                default:
                    timecards = timecards.OrderByDescending(w => w.WeekEnding).ToList();
                    ViewBag.WeekEndingSort = sortOrder;
                    Session["sort"] = sortOrder;
                    break;
            }
        }
        #endregion
    }
}
