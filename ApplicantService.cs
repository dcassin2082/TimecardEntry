using EmployeePortal.common;
using EmployeePortal.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;
using System.Linq;
using EmployeePortal.Repository;
using System.Collections;
using System.Collections.Generic;

namespace EmployeePortal.Services
{
    public class ApplicantService : Controller
    {
        private IUnitOfWork unitOfWork = new UnitOfWork();

        public Applicant GetApplicant(Guid userId)
        {
            return unitOfWork.ApplicantRepository.GetEntity(r => r.RegisterId == userId);
        }

        //KEEP THIS METHOD EVEN THOUGH IT'S NOT BEING USED RIGHT NOW ************************************
        //public Applicant CreateApplicant(Guid userId, string emailAddress)
        //{
        //    // WE STILL NEED TO 'UN-HARDCODE' THIS ****************************
        //    Applicant applicant = new Applicant
        //    {
        //        RegisterId = userId,
        //        InternalDatabaseID = Guid.NewGuid(),
        //        SSN = "SSN is required",
        //        CellPhone = "Cell phone is required",
        //        BirthDate = DateTime.Now,
        //        HasCar = true,
        //        HasFelony = false,
        //        HasMisdeamonor = true,
        //        Race = "3",
        //        Gender = "Gender is required",
        //        Address = "Address is required",
        //        City = "City is required",
        //        State = "AZ is required",
        //        ZipCode = "Zip Code is required",
        //        HomePhone = "Home Phone is required",
        //        Email = emailAddress
        //    };
        //    unitOfWork.ApplicantRepository.Add(applicant);
        //    unitOfWork.Commit();
        //    return applicant;
        //}

        public void UpdateApplicant(Applicant applicant)
        {
            unitOfWork.ApplicantRepository.Update(applicant);
            unitOfWork.Commit();
            //SaveChanges(applicant);
        }

        public void PostResume(string fFile, Applicant applicant)
        {
            if (!string.IsNullOrWhiteSpace(fFile))
            {
                Match guid = Regex.Match(fFile, ApplicationConstants.ParseGuid);
                Resume resume = new Resume();
                resume.ApplicantId = applicant.Id;
                resume.ResumeName = fFile;
                resume.ResumeId = new Guid(guid.ToString());
                resume.SubmittedDate = DateTime.Now;
                applicant.ResumeCount++;
                unitOfWork.ResumeRepository.Add(resume);
                SaveChanges(applicant);
            }
        }

        public void SaveChanges(Applicant applicant)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (applicant.Id != 0)
                    {
                        unitOfWork.ApplicantRepository.Update(applicant);
                        unitOfWork.Commit();
                    }
                    else
                    {
                        unitOfWork.Commit();
                    }
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

        private static string GetChildNode(XmlNode node, string nodepath)
        {
            return node.SelectSingleNode(nodepath) == null ? "" : node.SelectSingleNode(nodepath).InnerText;
        }

        public static string ParseResume(Applicant applicant, string sFile)
        {
            string hrxml_profile;
            //DaxtraServices.CVXService CVX_Service = new DaxtraServices.CVXService("https://cvx-carvin.daxtra.com", "carvin_software");
            DaxtraServices.CVXService CVX_Service = new DaxtraServices.CVXService(ConfigurationManager.AppSettings["DaxtraServerUrl"], ConfigurationManager.AppSettings["DaxtraAccountName"]);
            hrxml_profile = CVX_Service.ProcessCVphase1(sFile);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(hrxml_profile);
            string xpath = @"Resume/StructuredXMLResume/ContactInfo";
            var contactNodes = doc.SelectNodes(xpath);
            foreach (XmlNode node in contactNodes)
            {
                applicant.FirstName = GetChildNode(node, ApplicationConstants.ChildNodeFirstName);
                applicant.LastName = GetChildNode(node, ApplicationConstants.ChildNodeLastName);
                applicant.HomePhone = GetChildNode(node, ApplicationConstants.ChildNodeHomePhone);
                applicant.Address = GetChildNode(node, ApplicationConstants.ChildNodeAddress);
                applicant.Address += " " + GetChildNode(node, ApplicationConstants.ChildNodeStreet);
                applicant.AddressContinued = GetChildNode(node, ApplicationConstants.ChildNodeAddressContinued);
                applicant.City = GetChildNode(node, ApplicationConstants.ChildNodeCity);
                applicant.State = GetChildNode(node, ApplicationConstants.ChildNodeState);
                applicant.ZipCode = GetChildNode(node, ApplicationConstants.ChildNodeZipCode);

            }
            return hrxml_profile;
        }

        public void SaveApplicant(Applicant applicant)
        {
            if (ModelState.IsValid)
            {
                
                if (applicant.Id == 0)
                {
                    unitOfWork.ApplicantRepository.Add(applicant);
                    unitOfWork.Commit();
                    //EEOC eeoc = new EEOC
                    //{
                    //    ApplicantId = applicant.Id,
                    //    Gender = applicant.Gender,
                    //    Race = applicant.Race,
                    //    Veteran = string.IsNullOrWhiteSpace(applicant.Veteran) ? "N/A" : applicant.Veteran
                    //};
                    //AddEEOC(eeoc);
                }
                else
                {
                    //EEOC eeoc = unitOfWork.EEOCRepository.GetEntity(a => a.ApplicantId == applicant.Id);
                    //eeoc.Gender = applicant.Gender;
                    //eeoc.Race = applicant.Race;
                    //eeoc.Veteran = string.IsNullOrWhiteSpace(applicant.Veteran) ? "N/A" : applicant.Veteran;
                    unitOfWork.ApplicantRepository.Update(applicant);
                    //unitOfWork.EEOCRepository.Update(eeoc);
                    unitOfWork.Commit();
                }
            }
        }

        #region WorkHistory
        public IEnumerable<WorkHistory> GetWorkHistories(int id)
        {
            return unitOfWork.WorkHistoryRepository.GetEntities(a => a.ApplicantId == id);
        }

        public WorkHistory GetWorkHistory(int id)
        {
            return unitOfWork.WorkHistoryRepository.Get(id);
        }

        public void AddWorkHistory(WorkHistory workHistory)
        {
            unitOfWork.WorkHistoryRepository.Add(workHistory);
            unitOfWork.Commit();
        }

        public void UpdateWorkHistory(WorkHistory workHistory)
        {
            unitOfWork.WorkHistoryRepository.Update(workHistory);
            unitOfWork.Commit();
        }

        public void DeleteWorkHistory(WorkHistory workHistory)
        {
            unitOfWork.WorkHistoryRepository.Delete(workHistory);
            unitOfWork.Commit();
        }
        #endregion

        #region Skills
        public IEnumerable<Skill> GetSkills(int id)
        {
            return unitOfWork.SkillRepository.GetEntities(a => a.ApplicantId == id);
        }

        public Skill GetSkill(int id)
        {
            return unitOfWork.SkillRepository.Get(id);
        }

        public void AddSkill(Skill skill)
        {
            unitOfWork.SkillRepository.Add(skill);
            unitOfWork.Commit();
        }

        public void UpdateSkill(Skill skill)
        {
            unitOfWork.SkillRepository.Update(skill);
            unitOfWork.Commit();
        }

        public void DeleteSkill(Skill skill)
        {
            unitOfWork.SkillRepository.Delete(skill);
            unitOfWork.Commit();
        }
        #endregion

        #region Convictions
        public IEnumerable<Conviction> GetConvictions(int id)
        {
            return unitOfWork.ConvictionRepository.GetEntities(a => a.ApplicantId == id);
        }

        public Conviction GetConviction(int id)
        {
            return unitOfWork.ConvictionRepository.Get(id);
        }

        public void AddConviction(Conviction conviction)
        {
            unitOfWork.ConvictionRepository.Add(conviction);
            unitOfWork.Commit();
        }

        public void UpdateConviction(Conviction conviction)
        {
            unitOfWork.ConvictionRepository.Update(conviction);
            unitOfWork.Commit();
        }

        public void DeleteConviction(Conviction conviction)
        {
            unitOfWork.ConvictionRepository.Delete(conviction);
            unitOfWork.Commit();
        }
        #endregion

        #region Education
        public IEnumerable<Education> GetEducations(int id)
        {
            return unitOfWork.EducationRepository.GetEntities(a => a.ApplicantId == id);
        }

        public Education GetEducation(int id)
        {
            return unitOfWork.EducationRepository.Get(id);
        }

        public void AddEducation(Education education)
        {
            unitOfWork.EducationRepository.Add(education);
            unitOfWork.Commit();
        }

        public void UpdateEducation(Education education)
        {
            unitOfWork.EducationRepository.Update(education);
            unitOfWork.Commit();
        }

        public void DeleteEducation(Education education)
        {
            unitOfWork.EducationRepository.Delete(education);
            unitOfWork.Commit();
        }
        #endregion

        #region EEOCs
        public IEnumerable<EEOC> GetEEOCs()
        {
            return unitOfWork.EEOCRepository.GetAll();
        }

        public EEOC GetEEOC(int id)
        {
            return unitOfWork.EEOCRepository.Get(id);
        }

        public void AddEEOC(EEOC eeoc)
        {
            unitOfWork.EEOCRepository.Add(eeoc);
            unitOfWork.Commit();
        }

        public void UpdateEEOC(EEOC eeoc)
        {
            unitOfWork.EEOCRepository.Update(eeoc);
            unitOfWork.Commit();
        }

        public void DeleteEEOC(EEOC eeoc)
        {
            unitOfWork.EEOCRepository.Delete(eeoc);
            unitOfWork.Commit();
        }
        #endregion
    }
}