using EmployeePortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeePortal.Repository
{
    public class AppHelperRepository
    {
        #region Lookups
        public static IEnumerable<SelectListItem> GetSourceTypeList()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.SourceLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.Description
               });
            return items;

        }

        public static IEnumerable<SelectListItem> GetPageSizes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Insert(0, new SelectListItem { Text = "10", Value = "10" });
            items.Insert(1, new SelectListItem { Text = "20", Value = "20" });
            items.Insert(2, new SelectListItem { Text = "50", Value = "50" });
            items.Insert(3, new SelectListItem { Text = "100", Value = "100" });
            return items;
        }
        public static IEnumerable<SelectListItem> GetTimeCardTypes()
        {
            var db = new ApplicationDbContext();

            IEnumerable<SelectListItem> items = db.TimecardTypes.Where(d => d.Description == "Mileage Entry" || d.Description == "Time Entry").OrderByDescending(d => d.Description)
                .Select(sk => new SelectListItem
                {
                    Value = sk.Id.ToString(),
                    Text = sk.Description
                });
            return items;
        }

        public static IEnumerable<SelectListItem> GetTimecardAdjustments()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = 
                db.TimecardTypes.Where(d => d.Description == "Overtime" || 
                d.Description == "Vacation" || 
                d.Description == "Double Time" || 
                d.Description == "Sick" || 
                d.Description == "Mileage Entry" ||
                d.Description == "Holiday")
                .Select(sk => new SelectListItem
                {
                    Value = sk.Id.ToString(),
                    Text = sk.Description
                }).OrderByDescending(i => i.Value);
            return items;
        }

        public static IEnumerable<SelectListItem> GetJobCodes()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.JobCodes
                .Select(sk => new SelectListItem
                {
                    Value = sk.Id.ToString(),
                    Text = sk.Code
                });
            return items;
        }

        public static IEnumerable<SelectListItem> GetCustomerCodes()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.CustomerCodes
                .Select(sk => new SelectListItem
                {
                    Value = sk.Id.ToString(),
                    Text = sk.CustomerName
                });
            return items;
        }

        public static IEnumerable<SelectListItem> GetGenderTypeList()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.GenderLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.Description
               });
            return items;

        }

        public static IEnumerable<SelectListItem> GetRaceTypeList()
        {

            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.RaceLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.Description
               });
            return items;
        }

        public static IEnumerable<SelectListItem> GetVeteranStatusTypeList()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.VeteranLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.Description
               });
            return items;

        }

        public static IEnumerable<SelectListItem> GetOfficeTypeList()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.OfficeLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.Name
               });
            return items;

        }
        public static IEnumerable<SelectListItem> GetFormTypes()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.FormTypes
                .Select(sk => new SelectListItem
                {
                    Value = sk.Id.ToString(),
                    Text = sk.Description
                });
            return items;
        }

        public static IEnumerable<SelectListItem> GetLanguageTypeList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "English", Value = "English"},
                new SelectListItem{Text = "Spanish", Value = "Spanish"},
                new SelectListItem{Text = "French", Value = "French"},
                new SelectListItem{Text = "German", Value = "German"},
                new SelectListItem{Text = "Italian", Value = "Italian"},
                new SelectListItem{Text = "Other", Value = "Other"},


            };
            return items;
        }

        public static IEnumerable<SelectListItem> GetConvictionTypeList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "DUI", Value = "DUI"},
                new SelectListItem{Text = "Felony", Value = "Felony"},
                new SelectListItem{Text = "Misdemeanor", Value = "Misdemeanor"},
                new SelectListItem{Text = "Other", Value = "Other"}
            };
            return items;
        }

        public static IEnumerable<SelectListItem> GetSkillLookupItems()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.SkillLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.Description
               });
            return items;

        }

        public static IEnumerable<SelectListItem> GetEmploymentTypeList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Perm", Value = "Perm"},
                new SelectListItem{Text = "Temp", Value = "Temp"},
                new SelectListItem{Text = "Contract", Value = "Contract"},
                new SelectListItem{Text = "Present", Value = "Present"}
            };
            return items;
        }

        public static IEnumerable<SelectListItem> GetSkillLengthTypelList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Years", Value = "Years"},
                new SelectListItem{Text = "Months", Value = "Months"}
            };
            return items;
        }

        public static IEnumerable<SelectListItem> GetSkillLevelList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Advanced", Value = "Advanced"},
                new SelectListItem{Text = "Novice", Value = "Novice"},
                new SelectListItem{Text = "Some Experience", Value = "Some Experience"},
                new SelectListItem{Text = "Intermediate", Value = "Intermediate"},
            };
            return items;
        }

        public static IEnumerable<SelectListItem> GetStateList()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.StateLookups
               .Select(sk => new SelectListItem
               {
                   Value = sk.Id.ToString(),
                   Text = sk.State
               });
            return items;
            //IList<SelectListItem> items = new List<SelectListItem>
            //{
            //    new SelectListItem{Text = "Alabama", Value = "AL"},
            //    new SelectListItem{Text = "Alaska", Value = "AK"},
            //    new SelectListItem{Text = "Arizona", Value = "AZ"},
            //    new SelectListItem{Text = "Arkansas", Value = "AR"},
            //    new SelectListItem{Text = "California", Value = "CA"},
            //    new SelectListItem{Text = "Colorado", Value = "CO"},
            //    new SelectListItem{Text = "Connecticut", Value = "CT"},
            //    new SelectListItem{Text = "Delaware", Value = "DE"},
            //    new SelectListItem{Text = "District Columbia", Value = "DC"},
            //    new SelectListItem{Text = "Florida", Value = "FL"},
            //    new SelectListItem{Text = "Georgia", Value = "GA"},
            //    new SelectListItem{Text = "Hawaii", Value = "HI"},
            //    new SelectListItem{Text = "Idaho", Value = "ID"},
            //    new SelectListItem{Text = "Illinois", Value = "IL"},
            //    new SelectListItem{Text = "Indiana", Value = "IN"},
            //    new SelectListItem{Text = "Iowa", Value = "IA"},
            //    new SelectListItem{Text = "Kansas", Value = "KS"},
            //    new SelectListItem{Text = "Kentucky", Value = "KY"},
            //    new SelectListItem{Text = "Louisiana", Value = "LA"},
            //    new SelectListItem{Text = "Maine", Value = "ME"},
            //    new SelectListItem{Text = "Maryland", Value = "MD"},
            //    new SelectListItem{Text = "Massachusetts", Value = "MA"},
            //    new SelectListItem{Text = "Michigan", Value = "MI"},
            //    new SelectListItem{Text = "Minnesota", Value = "MN"},
            //    new SelectListItem{Text = "Mississippi", Value = "MS"},
            //    new SelectListItem{Text = "Missouri", Value = "MO"},
            //    new SelectListItem{Text = "Montana", Value = "MT"},
            //    new SelectListItem{Text = "Nebraska", Value = "NE"},
            //    new SelectListItem{Text = "Nevada", Value = "NV"},
            //    new SelectListItem{Text = "New Hampshire", Value = "NH"},
            //    new SelectListItem{Text = "New Jersey", Value = "NJ"},
            //    new SelectListItem{Text = "New Mexico", Value = "NM"},
            //    new SelectListItem{Text = "New York", Value = "NY"},
            //    new SelectListItem{Text = "North Carolina", Value = "NC"},
            //    new SelectListItem{Text = "North Dakota", Value = "ND"},
            //    new SelectListItem{Text = "Ohio", Value = "OH"},
            //    new SelectListItem{Text = "Oklahoma", Value = "OK"},
            //    new SelectListItem{Text = "Oregon", Value = "OR"},
            //    new SelectListItem{Text = "Pennsylvania", Value = "PA"},
            //    new SelectListItem{Text = "Rhode Island", Value = "RI"},
            //    new SelectListItem{Text = "South Carolina", Value = "SC"},
            //    new SelectListItem{Text = "South Dakota", Value = "SD"},
            //    new SelectListItem{Text = "Tennesee", Value = "TN"},
            //    new SelectListItem{Text = "Texas", Value = "TX"},
            //    new SelectListItem{Text = "Utah", Value = "UT"},
            //    new SelectListItem{Text = "Vermont", Value = "VT"},
            //    new SelectListItem{Text = "Virginia", Value = "VA"},
            //    new SelectListItem{Text = "Washington", Value = "WA"},
            //    new SelectListItem{Text = "West Virginia", Value = "WV"},
            //    new SelectListItem{Text = "Wisconsin", Value = "WI"},
            //    new SelectListItem{Text = "Wyoming", Value = "WY"},
            //};
            /*  HERE IS THE SQL TO CREATE THE TABLE **************************************************************
              insert statelookups(state, abbreviation) values ('Alabama', 'AL')
              insert statelookups(state, abbreviation) values ('Alaska', 'AK')
              insert statelookups(state, abbreviation) values( 'Arizona',  'AZ')
              insert statelookups(state, abbreviation) values( 'Arkansas',  'AR')
              insert statelookups(state, abbreviation) values( 'California',  'CA')
              insert statelookups(state, abbreviation) values( 'Colorado',  'CO')
              insert statelookups(state, abbreviation) values( 'Connecticut',  'CT')
              insert statelookups(state, abbreviation) values( 'Delaware',  'DE')
              insert statelookups(state, abbreviation) values( 'District Columbia',  'DC')
              insert statelookups(state, abbreviation) values( 'Florida',  'FL')
              insert statelookups(state, abbreviation) values( 'Georgia',  'GA')
              insert statelookups(state, abbreviation) values( 'Hawaii',  'HI')
              insert statelookups(state, abbreviation) values( 'Idaho',  'ID')
              insert statelookups(state, abbreviation) values( 'Illinois',  'IL')
              insert statelookups(state, abbreviation) values( 'Indiana',  'IN')
              insert statelookups(state, abbreviation) values( 'Iowa',  'IA')
              insert statelookups(state, abbreviation) values( 'Kansas',  'KS')
              insert statelookups(state, abbreviation) values( 'Kentucky',  'KY')
              insert statelookups(state, abbreviation) values( 'Louisiana',  'LA')
              insert statelookups(state, abbreviation) values( 'Maine',  'ME')
              insert statelookups(state, abbreviation) values( 'Maryland',  'MD')
              insert statelookups(state, abbreviation) values( 'Massachusetts',  'MA')
              insert statelookups(state, abbreviation) values( 'Michigan',  'MI')
              insert statelookups(state, abbreviation) values( 'Minnesota',  'MN')
              insert statelookups(state, abbreviation) values( 'Mississippi',  'MS')
              insert statelookups(state, abbreviation) values( 'Missouri',  'MO')
              insert statelookups(state, abbreviation) values( 'Montana',  'MT')
              insert statelookups(state, abbreviation) values( 'Nebraska',  'NE')
              insert statelookups(state, abbreviation) values( 'Nevada',  'NV')
              insert statelookups(state, abbreviation) values( 'New Hampshire',  'NH')
              insert statelookups(state, abbreviation) values( 'New Jersey',  'NJ')
              insert statelookups(state, abbreviation) values( 'New Mexico',  'NM')
              insert statelookups(state, abbreviation) values ('New York',  'NY')
              insert statelookups(state, abbreviation) values( 'North Carolina',  'NC')
              insert statelookups(state, abbreviation) values( 'North Dakota',  'ND')
              insert statelookups(state, abbreviation) values( 'Ohio',  'OH')
             insert statelookups(state, abbreviation) values ( 'Oklahoma',  'OK')
              insert statelookups(state, abbreviation) values( 'Oregon',  'OR')
              insert statelookups(state, abbreviation) values( 'Pennsylvania',  'PA')
              insert statelookups(state, abbreviation) values( 'Rhode Island',  'RI')
              insert statelookups(state, abbreviation) values( 'South Carolina',  'SC')
              insert statelookups(state, abbreviation) values( 'South Dakota',  'SD')
              insert statelookups(state, abbreviation) values( 'Tennesee',  'TN')
              insert statelookups(state, abbreviation) values( 'Texas',  'TX')
              insert statelookups(state, abbreviation) values( 'Utah',  'UT')
              insert statelookups(state, abbreviation) values( 'Vermont',  'VT')
              insert statelookups(state, abbreviation) values( 'Virginia',  'VA')
              insert statelookups(state, abbreviation) values( 'Washington',  'WA')
              insert statelookups(state, abbreviation) values( 'West Virginia',  'WV')
             insert statelookups(state, abbreviation) values( 'Wisconsin',  'WI')
              insert statelookups(state, abbreviation) values( 'Wyoming',  'WY')
              */
        }

        public static IEnumerable<SelectListItem> GetDegreeList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {

                new SelectListItem{Text = "Bachelors", Value = "Bachelors"},
                new SelectListItem{Text = "Associates", Value = "Associates"},
                new SelectListItem{Text = "Masters", Value = "Masters"},
                new SelectListItem{Text = "PHD", Value = "PHD"},
                new SelectListItem{Text = "Diploma", Value = "Diploma"},
                new SelectListItem{Text = "GED", Value = "GED"},
                new SelectListItem{Text = "No Education", Value = "No Education"},
                new SelectListItem{Text = "Middle School", Value = "Middle School"},
                new SelectListItem{Text = "Elementary", Value = "Elementary"},
                new SelectListItem{Text = "Junior High", Value = "Junior High"},
                new SelectListItem{Text = "Trade School", Value = "Trade School"},
                new SelectListItem{Text = "On The Job Training", Value = "On The Job Training"},
                new SelectListItem{Text = "Self Taught", Value = "Self Taught"}
            };
            return items;


        }

        public static IEnumerable<SelectListItem> GetJobTypeList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {

                new SelectListItem{Text = "Full-time", Value = "Full-time"},
                new SelectListItem{Text = "Part-time", Value = "Part-time"},
                new SelectListItem{Text = "Temporary", Value = "Temporary"},
                new SelectListItem{Text = "Contract", Value = "Contract"},
                new SelectListItem{Text = "Internship", Value = "Internship"},
                new SelectListItem{Text = "Commission", Value = "Commission"},
            };
            return items;


        }

        public static IEnumerable<SelectListItem> GetSalaryTypeList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {

                new SelectListItem{Text = "per hour", Value = "per hour"},
                new SelectListItem{Text = "per day", Value = "per day"},
                new SelectListItem{Text = "per week", Value = "per week"},
                new SelectListItem{Text = "per month", Value = "per month"},
                new SelectListItem{Text = "per year", Value = "per year"},
            };
            return items;


        }

        public static IEnumerable<SelectListItem> GetEmailTemplateType()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "Employee Registration", Value = "Employee Registration"},
                new SelectListItem{Text = "Application Submit", Value = "Application Submit"},
                new SelectListItem{Text = "Rejection", Value = "Rejection"},
                new SelectListItem{Text = "Acceptance", Value = "Acceptance"},
                new SelectListItem{Text = "General", Value = "General"},
                new SelectListItem{Text = "Job Apply", Value = "Job Apply"},
                new SelectListItem{Text = "Register Notification", Value = "Register Notification"}
            };
            return items;
        }

        public static IEnumerable<SelectListItem> ApplicantStatusList()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.StatusLookups
               .Select(m => new SelectListItem
               {
                   Value = m.Id.ToString(),
                   Text = m.Description
               });
            return items;

        }

        public static IEnumerable<SelectListItem> GetTimeTypes()
        {
            var db = new ApplicationDbContext();
            IEnumerable<SelectListItem> items = db.TimeTypes
               .Select(m => new SelectListItem
               {
                   Value = m.Id.ToString(),
                   Text = m.Description
               });
            return items;
        }

        public static List<SkillLookup> GetSkillLookupToken()
        {
            var db = new ApplicationDbContext();
            return db.SkillLookups.ToList();
        }

        #endregion
    }
}