using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeePortal.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels
{
    public class TimecardApprovalViewModel
    {
        public int Id { get; set; }
        public Applicant Employee { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Customer Code")]
        public string CustomerCode { get; set; }

        [Display(Name = "Job Code")]
        public string JobCode { get; set; }

        [Display(Name = "Total Hours")]
        public decimal Hours { get; set; }

        [Display(Name = "Total Miles")]
        public decimal Miles { get; set; }

        [Display(Name = "Approve")]
        public bool Approve { get; set; }

        [Display(Name = "Reject")]
        public bool Disapprove { get; set; }

        [Display(Name = "Approved")]
        public bool Approved { get; set; }

        [Display(Name = "Status")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Week Ending")]
        public string WeekEnding { get; set; }

        public Guid RegisterId { get; set; }

        public int[] TimecardIds { get; set; }

        public int PageSize { get; set; }
        public List<Timecard> Timecards { get; set; }
    }
}