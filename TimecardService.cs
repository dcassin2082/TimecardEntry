using EmployeePortal.common;
using EmployeePortal.Models;
using EmployeePortal.Repository;
using EmployeePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeePortal.Services
{
    public class TimecardService : Controller
    {
        private IUnitOfWork unitOfWork = new UnitOfWork();

        #region CRUD
        public IEnumerable<Timecard> GetAll(int id)
        {
            return unitOfWork.TimecardRepository.GetAll();
        }

        public Timecard GetById(int id)
        {
            return unitOfWork.TimecardRepository.Get(id);
        }

        public Timecard GetLastTimecard(int employeeId)
        {
            return unitOfWork.TimecardRepository.GetEntities(e => e.EmployeeID == employeeId).OrderByDescending(i => i.Id).FirstOrDefault();
        }

        public void AddTimecard(Timecard timecard)
        {
            unitOfWork.TimecardRepository.Add(timecard);
            unitOfWork.Commit();
        }

        public void Update(Timecard timecard)
        {
            unitOfWork.TimecardRepository.Update(timecard);
            unitOfWork.Commit();
        }

        public JobCodes GetJobCode(string jobCode)
        {
            return unitOfWork.JobCodeRepository.GetEntity(c => c.Code == jobCode);
        }

        public int GetJobCodeId(string jobCode)
        {
            return unitOfWork.JobCodeRepository.GetEntity(c => c.Code == jobCode).Id;
        }

        public string GetCustomerCode(int customerCode)
        {
            return unitOfWork.CustomerCodeRepository.GetEntity(i => i.Id == customerCode).CustomerName;
        }

        public IEnumerable<TimecardEntry> GetTimecardEntries(int timecardId)
        {
            return unitOfWork.TimecardEntryRepository.GetEntities(t => t.TimecardId == timecardId);
        }
        
        public TimecardEntry GetTimecardEntry(DateTime workDate, int timecardId)
        {
            return unitOfWork.TimecardEntryRepository.GetEntity(e => e.WorkDate == workDate && e.TimecardId == timecardId);
        }
        
        public TimecardEntry GetParentEntry(int timecardId, DateTime workDate)
        {
            return unitOfWork.TimecardEntryRepository.GetEntities(p => p.TimecardId == timecardId && p.WorkDate == workDate && p.ParentId == 0).FirstOrDefault();
        }

        public IEnumerable<TimecardEntry> GetSiblings(int timecardEntryId, int parentId)
        {
            return unitOfWork.TimecardEntryRepository.GetEntities(t => t.Id != timecardEntryId && t.ParentId == parentId);
        }

        public void AddTimeCardEntry(TimecardEntry entry)
        {
            unitOfWork.TimecardEntryRepository.Add(entry);
            unitOfWork.Commit();
        }

        public void UpdateTimecardEntry(TimecardEntry entry)
        {
            unitOfWork.TimecardEntryRepository.Update(entry);
            unitOfWork.Commit();
        }

        public IEnumerable<Adjustment> GetTimecardAdjustments(int timecardId)
        {
            return unitOfWork.TimecardAdjustmentRepository.GetEntities(t => t.TimecardId == timecardId);
        }

        public void AddTimecardAdjustment(Adjustment adjustment)
        {
            unitOfWork.TimecardAdjustmentRepository.Add(adjustment);
            unitOfWork.Commit();
        }

        public void DeleteTimecard(Timecard timecard)
        {
            unitOfWork.TimecardRepository.Delete(timecard);
            var timecardEntries = unitOfWork.TimecardEntryRepository.GetEntities(t => t.TimecardId == timecard.Id);
            foreach (var item in timecardEntries)
            {
                unitOfWork.TimecardEntryRepository.Delete(item);
            }
            Applicant employee = unitOfWork.ApplicantRepository.Get(timecard.EmployeeID);
            if (employee.TimecardCount > 0)
                employee.TimecardCount--;
            unitOfWork.ApplicantRepository.Update(employee);
            unitOfWork.Commit();
        }
        
        public new void Dispose()
        {
            unitOfWork.Dispose();
        }
        #endregion

        #region Helper Methods
        public IEnumerable<Timecard> GetTimecards(int id)
        {
            return unitOfWork.TimecardRepository.GetEntities(i => i.EmployeeID == id);
        }

        public IEnumerable<Timecard> GetSubmittedTimecards()
        {
            return unitOfWork.TimecardRepository.GetEntities(t => t.TimecardSubmitted && t.ApprovalStatus == "Submitted");
        }

        public Applicant GetEmployee(Guid userId)
        {
            return unitOfWork.ApplicantRepository.GetEntity(r => r.RegisterId == userId);
        }

        public Applicant GetEmployeeByTimecardId(Timecard timecard)
        {
            return unitOfWork.ApplicantRepository.GetEntity(i => i.Id == timecard.EmployeeID);
        }
        #endregion

        #region Ajax helpers
        public string SetJobCode(int jobcode)
        {
            return unitOfWork.JobCodeRepository.GetEntity(i => i.Id == jobcode).Code;
        }

        public string SetAdjustmentType(int adjustmentType)
        {
            return unitOfWork.TimecardTypeRepository.GetEntity(i => i.Id == adjustmentType).Description;
        }
        #endregion

        #region Workdate Dropdowns
        public IList<string> GetCurrentWeek()
        {
            List<string> currentWeek = new List<string>();
            string sunday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).ToString("MM/dd/yyyy");
            string monday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).AddDays(1).ToString("MM/dd/yyyy");
            string tuesday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).AddDays(2).ToString("MM/dd/yyyy");
            string wednesday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).AddDays(3).ToString("MM/dd/yyyy");
            string thursday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).AddDays(4).ToString("MM/dd/yyyy");
            string friday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).AddDays(5).ToString("MM/dd/yyyy");
            string saturday = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).AddDays(6).ToString("MM/dd/yyyy");
            currentWeek.Add(sunday);
            currentWeek.Add(monday);
            currentWeek.Add(tuesday);
            currentWeek.Add(wednesday);
            currentWeek.Add(thursday);
            currentWeek.Add(friday);
            currentWeek.Add(saturday);
            return currentWeek;
        }

        public IList<string> GetNextWeek()
        {
            List<string> currentWeek = new List<string>();
            string sunday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).ToString("MM/dd/yyyy");
            string monday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).AddDays(1).ToString("MM/dd/yyyy");
            string tuesday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).AddDays(2).ToString("MM/dd/yyyy");
            string wednesday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).AddDays(3).ToString("MM/dd/yyyy");
            string thursday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).AddDays(4).ToString("MM/dd/yyyy");
            string friday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).AddDays(5).ToString("MM/dd/yyyy");
            string saturday = DateTime.Now.AddDays(7).StartOfWeek(DayOfWeek.Sunday).AddDays(6).ToString("MM/dd/yyyy");
            currentWeek.Add(sunday);
            currentWeek.Add(monday);
            currentWeek.Add(tuesday);
            currentWeek.Add(wednesday);
            currentWeek.Add(thursday);
            currentWeek.Add(friday);
            currentWeek.Add(saturday);
            return currentWeek;
        }
        #endregion
    }
}
