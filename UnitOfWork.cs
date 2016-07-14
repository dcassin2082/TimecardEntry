using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeePortal.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace EmployeePortal.Repository
{
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        private IRepository<Customer> customerRepo;
        private IRepository<Applicant> applicantRepo;
        private IRepository<TimecardType> timecardTypeRepo;
        private IRepository<JobCodes> jobcodeRepo;
        private IRepository<CustomerCode> customercodeRepo;
        private IRepository<EmailHistory> emailHistoryRepo;
        private IRepository<EmailTemplate> emailTemplateRepo;
        private IRepository<SMSHistory> smsHistoryRepo;
        private IRepository<Job> jobRepo;
        private IRepository<ApplicantJobs> applicantJobsRepo;
        private IRepository<FormUpload> formUploadRepo;
        private IRepository<FormType> formTypeRepo;
        private IRepository<Form> formRepo;
        private IRepository<Resume> resumeRepo;
        private IRepository<WorkHistory> workHistoryRepo;
        private IRepository<Conviction> convictionRepo;
        private IRepository<Skill> skillRepo;
        private IRepository<Education> educationRepo;
        private IRepository<EEOC> eeocRepo;
        private IRepository<Timecard> timecardRepo;
        private IRepository<TimecardEntry> timecardEntryRepo;
        private IRepository<Adjustment> timecardAdjustmentRepo;

        public IRepository<Adjustment> TimecardAdjustmentRepository
        {
            get
            {
                return timecardAdjustmentRepo ?? (timecardAdjustmentRepo = new Repository<Adjustment>(dbContext));
            }
        }

        public IRepository<TimecardEntry> TimecardEntryRepository
        {
            get
            {
                return timecardEntryRepo ?? (timecardEntryRepo = new Repository<TimecardEntry>(dbContext));
            }
        }

        public IRepository<Timecard> TimecardRepository
        {
            get
            {
                return timecardRepo ?? (timecardRepo = new Repository<Timecard>(dbContext));
            }
        }

        public IRepository<Education> EducationRepository
        {
            get
            {
                return educationRepo ?? (educationRepo = new Repository<Education>(dbContext));
            }
        }

        public IRepository<Customer> CustomerRepository
        {
            get { return customerRepo ?? (customerRepo = new Repository<Customer>(dbContext)); }
        }

        public IRepository<Applicant> ApplicantRepository
        {
            get
            {
                return applicantRepo ?? (applicantRepo = new Repository<Applicant>(dbContext));
            }
        }

        public IRepository<Job> JobRepository
        {
            get
            {
                return jobRepo ?? (jobRepo = new Repository<Job>(dbContext));
            }
        }

        public IRepository<ApplicantJobs> ApplicantJobRepository
        {
            get
            {
                return applicantJobsRepo ?? (applicantJobsRepo = new Repository<ApplicantJobs>(dbContext));
            }
        }

        public IRepository<Resume> ResumeRepository
        {
            get
            {
                return resumeRepo ?? (resumeRepo = new Repository<Resume>(dbContext));
            }
        }

        public IRepository<TimecardType> TimecardTypeRepository
        {
            get
            {
                return timecardTypeRepo ?? (timecardTypeRepo = new Repository<TimecardType>(dbContext));
            }
        }

        public IRepository<JobCodes> JobCodeRepository
        {
            get
            {
                return jobcodeRepo ?? (jobcodeRepo = new Repository<JobCodes>(dbContext));
            }
        }

        public IRepository<CustomerCode> CustomerCodeRepository
        {
            get
            {
                return customercodeRepo ?? (customercodeRepo = new Repository<CustomerCode>(dbContext));
            }
        }

        public IRepository<EmailHistory> EmailHistoryRepository
        {
            get
            {
                return emailHistoryRepo ?? (emailHistoryRepo = new Repository<EmailHistory>(dbContext));
            }
        }

        public IRepository<EmailTemplate> EmailTemplateRepository
        {
            get
            {
                return emailTemplateRepo ?? (emailTemplateRepo = new Repository<EmailTemplate>(dbContext));
            }
        }

        public IRepository<SMSHistory> SMSHistoryRepository
        {
            get
            {
                return smsHistoryRepo ?? (smsHistoryRepo = new Repository<SMSHistory>(dbContext));
            }
        }

        public IRepository<FormUpload> FormUploadRepository
        {
            get
            {
                return formUploadRepo ?? (formUploadRepo = new Repository<FormUpload>(dbContext));
            }
        }

        public IRepository<FormType> FormTypeRepository
        {
            get
            {
                return formTypeRepo ?? (formTypeRepo = new Repository<FormType>(dbContext));
            }
        }

        public IRepository<Form> FormRepository
        {
            get
            {
                return formRepo ?? (formRepo = new Repository<Form>(dbContext));
            }
        }

        public IRepository<WorkHistory> WorkHistoryRepository
        {
            get
            {
                return workHistoryRepo ?? (workHistoryRepo = new Repository<WorkHistory>(dbContext));
            }
        }

        public IRepository<Conviction> ConvictionRepository
        {
            get
            {
                return convictionRepo ?? (convictionRepo = new Repository<Conviction>(dbContext));
            }
        }

        public IRepository<Skill> SkillRepository
        {
            get
            {
                return skillRepo ?? (skillRepo = new Repository<Skill>(dbContext));
            }
        }

        public IRepository<EEOC> EEOCRepository
        {
            get
            {
                return eeocRepo ?? (eeocRepo = new Repository<EEOC>(dbContext));
            }
        }

        public void Commit()
        {
            try
            {
                dbContext.SaveChanges();

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
}