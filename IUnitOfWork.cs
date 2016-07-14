using EmployeePortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeePortal.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Applicant> ApplicantRepository { get; }
        IRepository<Job> JobRepository { get; }
        //IRepository<TimecardDetails> TimecardRepository { get; }
        IRepository<TimecardType> TimecardTypeRepository { get; }
        IRepository<JobCodes> JobCodeRepository { get; }
        IRepository<CustomerCode> CustomerCodeRepository { get; }
        IRepository<Customer> CustomerRepository { get; }
        IRepository<EmailHistory> EmailHistoryRepository { get; }
        IRepository<EmailTemplate> EmailTemplateRepository { get; }
        IRepository<SMSHistory> SMSHistoryRepository { get; }
        IRepository<ApplicantJobs> ApplicantJobRepository { get; }
        IRepository<FormUpload> FormUploadRepository { get; }
        IRepository<FormType> FormTypeRepository { get; }
        IRepository<Form> FormRepository { get; }
        IRepository<Resume> ResumeRepository { get; }
        IRepository<WorkHistory> WorkHistoryRepository { get; }
        IRepository<Conviction> ConvictionRepository { get; }
        IRepository<Skill> SkillRepository { get; }
        IRepository<Education> EducationRepository { get; }
        IRepository<EEOC> EEOCRepository { get; }
        IRepository<Timecard> TimecardRepository { get; }
        IRepository<TimecardEntry> TimecardEntryRepository { get; }
        IRepository<Adjustment> TimecardAdjustmentRepository { get; }
        void Commit();
    }
}