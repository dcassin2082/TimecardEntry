using EmployeePortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeePortal.ViewModels
{
    public class TimecardViewModel
    {
        public int TimecardId { get; set; }

        public int TimecardCount { get; set; }

        //[Display(Name = "Amount")]
        //[DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        //public decimal Amount { get; set; }

        //[Display(Name = "Rate")]
        //public decimal Rate { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Approval Status")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Customer Code")]
        public string CustomerCode { get; set; }

        [Required]
        [Display(Name = "Job Code")]
        public string JobCode { get; set; }

       
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Documents")]
        public byte[] File { get; set; }

        [Display(Name = "Comments")]
        [StringLength(300)]
        public string Comments { get; set; }

        [Display(Name = "Week Ending")]
        public string WeekEnding { get; set; }
       
        [Range(0.0, double.MaxValue, ErrorMessage ="Start miles cannot be negative")]
        [Display(Name ="Start Miles")]
        public decimal StartMiles { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "End miles cannot be negative")]
        [Display(Name = "End Miles")]
        public decimal EndMiles { get; set; }

        [Display(Name ="Miles")]
        public decimal DailyMiles { get; set; }

        [Display(Name = "Total Miles")]
        public decimal TotalMiles { get; set; }

        [Display(Name = "Total Regular")]
        public decimal TotalRegular { get; set; }

        public decimal Regular { get; set; }

        [Display(Name = "Total Overtime")]
        public decimal TotalOvertime { get; set; }

        [Display(Name = "Total Vacation")]
        public decimal TotalVacation { get; set; }

        [Display(Name = "Work Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage ="Please select a date")]
        public DateTime WorkDate { get; set; }

        [Display(Name = "Day")]
        public string DayOfWeek { get; set; }

        [Display(Name ="Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }

        //[Display(Name = "Total Amount")]
        //[DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        //public decimal TotalAmount { get; set; }

        [Display(Name = "Total Double Time")]
        public decimal DoubleTime { get; set; }

        [Display(Name = "Total Sick Time")]
        public decimal SickTime { get; set; }

        [Display(Name = "Total Holiday Time")]
        public decimal HolidayTime { get; set; }
        // this is used to populate the timecard adjustments dropdowns -- must be nullable or else the outer submit buttons won't work
        [Required(ErrorMessage = "Please select adjustment type")]
        public int TimecardTypeId { get; set; }
        public bool TimecardSubmitted { get; set; }
        public int JobCodeId { get; set; }
        public int TimeTypeId { get; set; }
        [Range(0.0, double.MaxValue, ErrorMessage ="Hours cannot be negative")]
        public decimal AdjustmentHours { get; set; }
        //public decimal AdjustmentQuantity { get; set; }
        //public string AdjustmentType { get; set; }
        // should we make this virtual ??
        public List<TimecardEntry> TimeEntries { get; set; }
        public bool HasAdjustments { get; set; }
        public int ParentId { get; set; }
        public List<Adjustment> Adjustments { get; set; }
    }
}