using ORA.Domain.Model.HumanSubjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ORA.Services.WebIRBCRMS.Shared;

namespace ORA.Services.WebIRBCRMS.Extensions
{
    /// <summary>
    /// Extensions to convert from Domain model to Service model
    /// </summary>
    public static class StudyExtensions
    {
        /// <summary>
        /// Obtains institution names from study and puts into facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<string> InstitutionNames(this Study study)
        {
            if (study.StudyProtocol == null)
            {
                return null;
            }
            if (study.StudyProtocol.Institution == null)
            {
                return null;
            }

            return study.StudyProtocol.Institution.InstitutionName.ToStrings();
        }

        /// <summary>
        /// Converts amendments from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Amendment> Amendments(this Study study)
        {
            return study.Amendments.Select(a => new Models.Amendment()
            {
                AmendmentKey = a.ExternalKey,
                AmendmentNumber = a.AmendmentNumber,
                AmendmentShortTitle = a.ShortTitle,
                ChangeInStaffList = a.ChangeInStaffList.ToStrings(),
                PIChangeReason = a.PIChangeReason,
                AmendmentDetailMinorList = a.AmendmentDetailMinorList.ToStrings(),
                AmendmentDetailMajorList = a.AmendmentDetailMajorList.ToStrings(),
                AmendmentDetailOther = a.AmendmentDetailOther,
                Modifications = a.Modifications,
                ParticipantsEnrolled = a.ParticipantsEnrolled,
                Reconsent = a.Reconsent,
                CompletedNotification = a.CompletedNotification,
                AssignedReviewType = a.AssignedReviewType,
                RadiationProcedures = a.RadiationProcedures,
                UpdatedDocuments = a.UpdatedDocumentList.ToStrings(),
                AssignedCommittee = a.CommitteeName(),
                ProtocolRequiresCalendarRevisions = a.crms_protocol_selected_fl,
                IrbSystemUniqueId = study.UniqueId
            });
        }

        /// <summary>
        /// Converts continuing reviews from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ContinuingReview> ContinuingReviews(this Study study)
        {
            return study.ContinuingReviews.Select(c => new Models.ContinuingReview()
            {
                ContinueReviewKey = c.ExternalKey,
                ContinueReviewReportType = c.ReportType,
                CurrentStatusOfTheStudyEnrollment = c.CurrentStatusOfTheStudyEnrollment,
                ReasonForClosingStudyAtUCLA = c.ReasonForClosingStudyAtUCLA,
                AssignedReviewType = c.AssignedReviewType,
                AssignedCommittee = c.CommitteeName(),
                IrbSystemUniqueId = study.UniqueId
            });
        }


        /// <summary>
        /// Converts post approval reports from domain model into facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.PostApprovalReport> PostApprovalReports(this Study study)
        {
            return study.PostApprovalReports.Select(p => new Models.PostApprovalReport()
            {
                PostApprovalReportKey = p.ExternalKey,
                PostApprovalReportType = p.ReportType,
                InvestigatorBrochureUpdate = p.InvestigatorBrochureUpdate,
                AssignedReviewType = p.AssignedReviewType,
                AssignedCommittee = p.CommitteeName(),
                IrbSystemUniqueId = study.UniqueId
            });
        }

        /// <summary>
        /// Converts investigational drugs from domain model into facade
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Drug> InvestigationalDrugs(this Study study)
        {
            return study.StudyProtocol.InvestigationalDrugBiologics
                .Where(d=> !string.IsNullOrEmpty(d.Id))
                .Select(d => new Models.Drug()
                {
                    Exempt = d.Exempt.RemoveInvalidXmlChars(),
                    HolderName = d.HolderName,
                    ID = d.Id,
                    SubmitToFDADate = d.SubmitToFDADate,
                    FDAApprovalDate = d.FDAApprovalDate,
                    IrbSystemUniqueId = study.UniqueId
                }
                );
        }

        /// <summary>
        /// Converts investigational devices from domain model into facade
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Device> InvestigationalDevices(this Study study)
        {
            return study.StudyProtocol.InvestigationalDevices
                .Where(d => !string.IsNullOrEmpty(d.Id))
                .Select(d => new Models.Device()
                {
                    Exempt = d.Exempt.RemoveInvalidXmlChars(),
                    HolderName = d.HolderName,
                    ID = d.Id,
                    Risk = d.Risk,
                    SubmitToFDADate = d.SubmitToFDADate,
                    FDAApprovalDate = d.FDAApprovalDate,
                    IrbSystemUniqueId = study.UniqueId
                }
                );
        }

        /// <summary>
        /// Converts ORA Domain tree into a list of awards pertaining to a study. 
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Award> Awards( this Study study, ILookup<Tuple<string, string, string>, ORA.Domain.Model.FullAccountingUnitKey> FAUKeysLookup )
        {
            // TODO: Build Unit Tests
            // Guard clauses
            if (!study.Approvals.Any())
            {
                return null;
            }

            var projects = study.Approvals
                .Where(a => a.Projects != null && a.Projects.Any())
                .SelectMany(a => a.Projects)
                .Distinct()
                ;

            if (!projects.Any())
            {
                return null;
            }

            var awardFacades = new Models.Award[projects.Count()];

            /*
            var sponsors = projects
                .Select(project => new Sponsor[] { project.Sponsor, project.PrimeSponsor })
                .SelectMany(s => s)
                .Distinct()
                ;
            */
            var awards = projects
                .Where(p => p.ProjectProposals.Any())
                .SelectMany(p => p.ProjectProposals)
                .Where(pp => pp.ProposalAwardSequences.Any())
                .SelectMany(pp => pp.ProposalAwardSequences)
                .Where(pas => pas.AwardCurrent)
                .ToLookup(pas => pas.Key.InstitutionNumber)
                ;

            //var funds = awards
            //  .Where(a => a.AwardedFund != null)
            // .Select(a => a.AwardedFund);

            var i = 0;
            foreach (var project in projects)
            {
                var fund = new Models.SponsoredFund();
                var award = awards[project.InstitutionNumber].FirstOrDefault();
                var af = new Models.Award()
                {
                    SponsoredProjectTitle = project.ProjectTitle.RemoveInvalidXmlChars(),
                    IrbSystemUniqueId = study.UniqueId,
                };

                if (project.Sponsor != null)
                {
                    af.SponsorName = project.Sponsor.Name;
                    af.SponsorUclaCode = project.Sponsor.UCLACode;
                }

                if (project.PrimeSponsor != null)
                {
                    af.PrimeSponsorName = project.PrimeSponsor.Name;
                    af.PrimeSponsorUclaCode = project.PrimeSponsor.UCLACode;
                }

                if (project.PrimePrincipalInvestigator != null)
                {
                    af.PrincipalInvestigator = new Models.Person()
                    {
                        UID = SharedFunctions.GetValidUID(project.PrimePrincipalInvestigator.EmployeeId),
                        FirstName = project.PrimePrincipalInvestigator.FirstName,
                        MiddleName = project.PrimePrincipalInvestigator.MiddleName,
                        LastName = project.PrimePrincipalInvestigator.LastName,
                        EmailAddress = project.PrimePrincipalInvestigator.EmailAddress,
                        Institution = GetCampus(GetLocationCode(project.PrimePrincipalInvestigator.Location)),
                        SponsorUclaCode = af.SponsorUclaCode
                    };
                    if (project.PrimePrincipalInvestigator.HomeDepartment != null)
                    {
                        af.PrincipalInvestigator.HomeDepartmentCode = project.PrimePrincipalInvestigator.HomeDepartment.DepartmentCode;
                        af.PrincipalInvestigator.HomeLocationCode = GetLocationCode(project.PrimePrincipalInvestigator.HomeDepartment.Location);
                        af.PrincipalInvestigator.Institution = GetCampus(GetLocationCode(project.PrimePrincipalInvestigator.HomeDepartment.Location));
                        af.PrincipalInvestigator.HomeDepartmentTitle = project.PrimePrincipalInvestigator.HomeDepartment.DepartmentTitle.TrimString();
                    }
                }

                // Set up fund if values are not null
                fund.LocationCode = '4';
                if (award != null && award.AwardedFund != null && award.AwardedFund.Key != null)
                {
                    fund.SponsorUclaCode = af.SponsorUclaCode;
                    fund.FundNumber = award.AwardedFund.Key.Number;
                    fund.MinimumLedgerYearMonth = award.AwardedFund.Key.KeyDate.ToString("yyyyMM");

                    if (FAUKeysLookup != null && FAUKeysLookup.Count() > 0)
                    {
                        //Merging the FAUs Data
                        List<Domain.Model.FullAccountingUnitKey> FAUKeys = FAUKeysLookup[new Tuple<string, string, string>("4", award.AwardedFund.Key.Number, project.InstitutionNumber)].ToList();
                        
                        if (FAUKeys.Any())
                        {
                            fund.FAUs = FAUKeys.Select(f => new Models.FullAccountingUnit() { LocationCode = '4', FundNumber = f.FundNumber, CostCenter = f.CostCenterCode, Account = f.AccountNumber });
                            fund.FundBeginDate = FAUKeys.First().FundBeginDate;
                        }
                    }
                }
                af.Fund = fund;
                af.IrbSystemUniqueId = study.UniqueId;
                awardFacades[i] = af;
                i++;
            }

            return awardFacades;
        }

        /// <summary>
        /// Converts ORA Domain tree into a list of awards pertaining to a study.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="ProjectLookup"></param>
        /// <param name="FAUKeysLookup"></param>
        /// <param name="FundEndDatesLookup"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Award> Awards( this Study study, ILookup<string, ORA.Domain.Model.Project> ProjectLookup, ILookup<Tuple<string, string, string>, ORA.Domain.Model.FullAccountingUnitKey> FAUKeysLookup, ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup )
        {
            // Check for any approvals exists in study
            if (!study.Approvals.Any())
            {
                return null;
            }
            
            // Look for match Projects by Approval IrbNumber 
            List<ORA.Domain.Model.Project> projects = ProjectLookup[SharedFunctions.RemoveIrbNumberPrefix(study.Approvals.First().IrbNumber)].ToList();
            if (projects != null && !projects.Any())
            {
                return null;
            }

            var awardFacades = new Models.Award[projects.Count()];

            var awards = projects
               .Where(p => p.ProjectProposals.Any())
               .SelectMany(p => p.ProjectProposals)
               .Where(pp => pp.ProposalAwardSequences.Any())
               .SelectMany(pp => pp.ProposalAwardSequences)
               .Where(pas => pas.AwardCurrent)
               .ToLookup(pas => pas.Key.InstitutionNumber);

            var i = 0;
            foreach (var project in projects)
            {
                var fund = new Models.SponsoredFund();
                var award = awards[project.InstitutionNumber].FirstOrDefault();
                var af = new Models.Award()
                {
                    SponsoredProjectTitle = project.ProjectTitle.RemoveInvalidXmlChars(),
                    IrbSystemUniqueId = study.UniqueId,
                };

                if (project.Sponsor != null)
                {
                    af.SponsorName = project.Sponsor.Name;
                    af.SponsorUclaCode = project.Sponsor.UCLACode;
                }

                if (project.PrimeSponsor != null)
                {
                    af.PrimeSponsorName = project.PrimeSponsor.Name;
                    af.PrimeSponsorUclaCode = project.PrimeSponsor.UCLACode;
                }

                if (project.PrimePrincipalInvestigator != null)
                {
                    af.PrincipalInvestigator = new Models.Person()
                    {
                        UID = SharedFunctions.GetValidUID(project.PrimePrincipalInvestigator.EmployeeId),
                        FirstName = project.PrimePrincipalInvestigator.FirstName,
                        MiddleName = project.PrimePrincipalInvestigator.MiddleName,
                        LastName = project.PrimePrincipalInvestigator.LastName,
                        EmailAddress = project.PrimePrincipalInvestigator.EmailAddress,
                        Institution = GetCampus(GetLocationCode(project.PrimePrincipalInvestigator.Location)),
                        SponsorUclaCode = af.SponsorUclaCode
                    };
                    if (project.PrimePrincipalInvestigator.HomeDepartment != null)
                    {
                        af.PrincipalInvestigator.HomeDepartmentCode = project.PrimePrincipalInvestigator.HomeDepartment.DepartmentCode;
                        af.PrincipalInvestigator.HomeLocationCode = GetLocationCode(project.PrimePrincipalInvestigator.HomeDepartment.Location);
                        af.PrincipalInvestigator.Institution = GetCampus(GetLocationCode(project.PrimePrincipalInvestigator.HomeDepartment.Location));
                        af.PrincipalInvestigator.HomeDepartmentTitle = project.PrimePrincipalInvestigator.HomeDepartment.DepartmentTitle.TrimString();
                    }
                }

                // Set up fund if values are not null
                fund.LocationCode = '4';
                if (award != null && award.AwardedFund != null && award.AwardedFund.Key != null)
                {
                    fund.SponsorUclaCode = af.SponsorUclaCode;
                    fund.FundNumber = award.AwardedFund.Key.Number;
                    fund.MinimumLedgerYearMonth = award.AwardedFund.Key.KeyDate.ToString("yyyyMM");
                    fund.FundEndDate = FundEndDatesLookup[new Tuple<string, string>("4", award.AwardedFund.Key.Number)].FirstOrDefault();
                    if (FAUKeysLookup != null && FAUKeysLookup.Count() > 0)
                    {
                        //Merging the FAUs Data
                        List<Domain.Model.FullAccountingUnitKey> FAUKeys = FAUKeysLookup[new Tuple<string, string, string>("4", award.AwardedFund.Key.Number, project.InstitutionNumber)].ToList();

                        if (FAUKeys.Any())
                        {
                            fund.FAUs = FAUKeys.Select(f => new Models.FullAccountingUnit() { LocationCode = '4', FundNumber = f.FundNumber, CostCenter = f.CostCenterCode, Account = f.AccountNumber });
                            fund.FundBeginDate = FAUKeys.First().FundBeginDate;
                        }
                    }
                }
                af.Fund = fund;
                af.IrbSystemUniqueId = study.UniqueId;
                awardFacades[i] = af;
                i++;
            }

            return awardFacades;
        }

        /// <summary>
        /// Converts ProtocolActivites from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ProtocolActivity> ProtocolActivities( this Study study )
        {
            return study.StudyProtocol.ProtocolActivities.Select(p => new Models.ProtocolActivity()
            {
                ID = p.ActivityKey,
                ActionStatus = p.ActionStatus,
                ActionStatusDate = p.ActionStatusDate.Value,
                IrbSystemUniqueId = study.UniqueId,
            });
        }

        /// <summary>
        /// Converts KeyPersonnels from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Person> KeyPersonnels( this Study study )
        {
            return study.KeyPersonnels.Select(i => new Models.Person()
            {
                FirstName = i.FirstName,
                MiddleName = i.MiddleName,
                LastName = i.LastName,
                UID = SharedFunctions.GetValidUID(i.EmployeeId),
                Institution = GetCampus(GetLocationCode(i.Location)),
                // UniversityAffiliation = person. //TODO: Implement
                //HomeDepartmentCode = i.DepartmentCode,  //Department Code is actually not in the requirement
                HomeDepartmentTitle = i.Institution,
                HomeLocationCode = GetLocationCode(i.Location),
                EmailAddress = i.EmailAddress,
                AccessRoles = i.AccessRoleList.ToStrings(),
                OtherRole = i.OtherRole,
                WillObtainConsent = i.WillObtainConsentFl,
                ManageDeviceAccountability = i.ManageDeviceAccountabilityFl,
                AccessToPersonallyIndentifiableInfo = i.AccessToPiiFl,
                AccessToCodeKey = i.AccessToCodeKey,
                IrbSystemUniqueId = study.UniqueId
            });
        }

        /// <summary>
        /// Converts ProtocolPI from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static Models.ProtocolPI ProtocolPI( this Study study )
        {
            if (study.StudyProtocol.ProtocolPI == null)
            {
                return null;
            }
            else
            {
                return new Models.ProtocolPI()
                {
                    UID = SharedFunctions.GetValidUID(study.StudyProtocol.ProtocolPI.EmployeeId),
                    FirstName = study.StudyProtocol.ProtocolPI.FirstName,
                    MiddleName = study.StudyProtocol.ProtocolPI.MiddleName,
                    LastName = study.StudyProtocol.ProtocolPI.LastName,
                    EmailAddress = study.StudyProtocol.ProtocolPI.EmailAddress,
                    IrbSystemUniqueId = study.UniqueId
                };
            }
        }

        /// <summary>
        /// Converts StudyContact from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static Models.Person StudyCoordinator( this Study study)
        {
            if (study.StudyProtocol.StudyCoordinator != null && !string.IsNullOrEmpty(study.StudyProtocol.StudyCoordinator.EmployeeId))
            {
                return new Models.Person()
                {
                    UID = SharedFunctions.GetValidUID(study.StudyProtocol.StudyCoordinator.EmployeeId),
                    FirstName = study.StudyProtocol.StudyCoordinator.FirstName,
                    MiddleName = study.StudyProtocol.StudyCoordinator.MiddleName,
                    LastName = study.StudyProtocol.StudyCoordinator.LastName,
                    EmailAddress = study.StudyProtocol.StudyCoordinator.EmailAddress,
                    IrbSystemUniqueId = study.UniqueId
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts FacultySponsor from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static Models.Person FacultySponsor( this Study study)
        {
            if (study.StudyProtocol.FacultySponsor != null && !string.IsNullOrEmpty(study.StudyProtocol.FacultySponsor.Uid))
            {
                return new Models.Person()
                {
                    UID = SharedFunctions.GetValidUID(study.StudyProtocol.FacultySponsor.Uid),
                    FirstName = study.StudyProtocol.FacultySponsor.FirstName,
                    LastName = study.StudyProtocol.FacultySponsor.LastName,
                    IrbSystemUniqueId = study.UniqueId
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get LocationCode from Location domain model
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static char GetLocationCode(Domain.Model.Location location)
        {
            return location != null ? location.Code : default(char);
        }

        /// <summary>
        /// Converts location_code to Campus
        /// </summary>
        /// <param name="location_code"></param>
        /// <returns></returns>
        public static string GetCampus( char location_code )
        {
            switch (location_code)
            {
                case '0':
                    return "UCM - UC Merced";
                case '4':
                    return "UCLA Los Angeles";
                case 'M':
                    return "UCOP UC President's Office";
                case 'S':
                    return "S UCM-Systemwide Programs";
                default:
                    return null;
            }
        }

        /// <summary>
        ///  Convert ProtocolTransitionStateActivities from Domain model into ExternalStatusChanges Service model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ExternalStatusChange> ProtocolExternalStatusChanges(this Study study)
        {
            if (study.StudyProtocol.ProtocolTransitionStateActivities.Any())
            {
                return study.StudyProtocol.ProtocolTransitionStateActivities.Select(t => new Models.ExternalStatusChange()
                {
                    StudyStateKey = t.ActivityKey,
                    ExternalStatus = t.NewExternalStatus,
                    ExternalStatusDate = t.ExitDate,
                    IrbSystemUniqueId = study.UniqueId,
                    Type = "Protocol Activity"
                });
            }
            else
            {
                return new List<Models.ExternalStatusChange>();
            }
        }

        /// <summary>
        /// Converts all activities from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static IEnumerable<Models.ExternalStatusChange> ExternalStatusChanges(this Study study)
        {
            List<Models.ExternalStatusChange> result = new List<Models.ExternalStatusChange>();
            if (study.StudyProtocol.ProtocolTransitionStateActivities != null && study.StudyProtocol.ProtocolTransitionStateActivities.Any())
            {
                result.AddRange(study.ProtocolExternalStatusChanges());
            }
            
            study.Amendments.ToList().ForEach(a => result.AddRange(a.ExternalStatusChanges(study.UniqueId)));
            study.ContinuingReviews.ToList().ForEach(c => result.AddRange(c.ExternalStatusChanges(study.UniqueId)));
            study.PostApprovalReports.ToList().ForEach(p => result.AddRange(p.ExternalStatusChanges(study.UniqueId)));
            return result.OrderBy(r => r.ExternalStatusDate.Value);
        }

        /// <summary>
        /// Convert AssignedIrbCommittee from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static string CommitteeName(this Study study)
        {
            if (study.StudyProtocol.AssignedIrbCommittee == null)
            {
                return null;
            }
            else
            {
                return study.StudyProtocol.AssignedIrbCommittee.CommitteeName;
            }
        }

        /// <summary>
        /// Convert BillingDesignation from the domain model into the facade model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static string BillingDesignation(this Study study)
        {
            if (study.StudyProtocol == null || string.IsNullOrEmpty(study.StudyProtocol.BillingDesignation))
            {
                return null;
            }
            else
            {
                switch (study.StudyProtocol.BillingDesignation.Substring(0,3).ToLower().Trim())
                {
                    case "yes":
                        return "Yes";
                    case "no":
                        return "No";
                    case "not":
                        return "Not Applicable";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// Derived NonInterventional from the domain model
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public static string NonInterventional(this Study study)
        {
            if (study.BillingDesignation() == "Not Applicable")
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        public static string RootIrbNumber(this Study study)
        {
            if (!string.IsNullOrEmpty(study.StudyProtocol.IrbNumber) && study.StudyProtocol.IrbNumber.ToUpper().StartsWith("MS"))
            {
                return study.StudyProtocol.IrbNumber.Substring(study.StudyProtocol.IrbNumber.IndexOf('_') + 1);
            }
            else
            {
                return study.StudyProtocol.IrbNumber;
            }
        }
    }
}