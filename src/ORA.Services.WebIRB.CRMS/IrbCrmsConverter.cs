using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using ORA.Domain.Model.HumanSubjects;
using ORA.Domain.Model;
using ORA.Services.WebIRBCRMS.Extensions;
using ORA.Services.WebIRBCRMS.Shared;

namespace ORA.Services.WebIRBCRMS
{
    public class IrbCrmsConverter : Interfaces.IIrbCrmsConverter
    {

        /// <summary>
        /// Implementation of IIrbCrmsConverter.GetStudyFacade() which uses Linq to Object.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="ProjectLookup"></param>
        /// <param name="FAUKeysLookup"></param>
        /// <returns>study in service model</returns>
        public Models.Study GetStudyFacade( Study study, ILookup<string, ORA.Domain.Model.Project> ProjectLookup, ILookup<Tuple<string, string, string>, ORA.Domain.Model.FullAccountingUnitKey> FAUKeysLookup, ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup )
        {
            Models.Study studyFacade = new Models.Study()
            {
                IrbSystemUniqueId = study.UniqueId,
                Amendments = study.Amendments(),
                ContinuingReviews = study.ContinuingReviews(),
                PostApprovalReports = study.PostApprovalReports(),
                Department = study.StudyProtocol.DepartmentRC,
                ProtocolUCLAHomeDepartment = study.StudyProtocol.UclaHomeDepartment,
                PreIrbNumber = study.StudyProtocol.PreIrbNumber,
                IrbNumber = study.RootIrbNumber(),
                ReferenceIrbNumber = study.StudyProtocol.IrbNumber,
                Title = study.StudyProtocol.Title.RemoveInvalidXmlChars(),
                ShortTitle = study.StudyProtocol.ShortTitle.RemoveInvalidXmlChars(),
                Objectives = study.StudyProtocol.Objectives.RemoveInvalidXmlChars(),
                Phases = study.StudyProtocol.PhaseList.ToStrings(),
                TypeOfSubmission = study.StudyProtocol.TypeOfSubmission,
                AgeGroups = study.StudyProtocol.AgeGroupList.ToStrings(),
                InvestigationalMethods = (study.StudyProtocol.InvestigationalDrugList.GetAttributeOfType<StringValue>())
                                            .Select(x => x.Value).ToList(),
                InvestigatorInitiated = study.StudyProtocol.InvestigatorInitiatedList.ToStrings(),
                PrmcReviewRequired = study.StudyProtocol.PrmcReviewRequired,
                Keywords = study.StudyProtocol.Keywords.RemoveInvalidXmlChars(),
                ProtocolPI = study.ProtocolPI(),
                StudyContact = study.StudyCoordinator(),
                FacultySponsor = study.FacultySponsor(),
                ProtocolType = study.StudyProtocol.ProtocolType,
                ProtocolTargetAccrual = study.StudyProtocol.ProtocolTargetAccrual,
                UpperAccrualGoal = study.StudyProtocol.UpperAccrualGoal,
                AffiliateAccrualGoal = study.StudyProtocol.AffiliateAccrualGoal,
                ProtocolStatus = study.StudyProtocol.ProtocolStatus,
                DateProtocolStatusChanged = study.StudyProtocol.StatusEffectiveDate,
                IrbReviewDate = study.StudyProtocol.IrbReviewDate,
                IrbSubmissionDate = study.StudyProtocol.IrbSubmissionDate,
                IrbInitialApprovedDate = study.StudyProtocol.IrbInitialApprovedDate,
                ExpirationDate = study.StudyProtocol.ExpirationDate,
                AssignedIrbCommittee = study.CommitteeName(),
                KeyPersonnel = study.KeyPersonnels(),
                AssignedReviewType = study.StudyProtocol.AssignedReviewType,
                InvolvesTherapy = study.StudyProtocol.InvolvesTherapy,
                FDARegulatedList = study.StudyProtocol.FdaRegulatedList.ToStrings(),
                MultiSiteTrial = study.StudyProtocol.MultiSiteTrial,
                ReviewForReliance = study.StudyProtocol.ReviewForReliance,
                IRBReviewsTypeList = study.StudyProtocol.IRBReviewsTypeList.ToStrings(),
                SupportedByHHS = study.StudyProtocol.SupportedbyHhs,
                Risk = study.StudyProtocol.Risk.RemoveInvalidXmlChars(),
                ExpeditedReviewTypeList = study.StudyProtocol.ExpeditedReviewTypeList.ToStrings(),
                OverallDirection = study.StudyProtocol.OverallDirection,
                ContactWithSubjectsTypeList = study.StudyProtocol.ContactWithSubjectsTypeList.ToStrings(),
                TypeOfTrialList = study.StudyProtocol.TypeOfTrialList.ToStrings(),
                ClincialTrialRegistrationStatus = study.StudyProtocol.ClinicalTrialsRegistrationStatus,
                TrialRegistrationNumber = study.StudyProtocol.TrialRegistrationNumber,
                SourceUseOfDataList = study.StudyProtocol.SourceUseOfDataList.ToStrings(),
                DataCollectionWithoutParticipantContactList = study.StudyProtocol.DataCollectionWithoutParticipantContactList.ToStrings(),
                ObservationalEthnographic = study.StudyProtocol.ObservationalEthnographic,
                ScreenRecruitTarget = study.StudyProtocol.ScreenRecruitMeetTarget,
                PopulationTypeList = study.StudyProtocol.PopulationTypeList.ToStrings(),
                NonEnglishSpeakers = study.StudyProtocol.NonEnglishSpeakers,
                PotentialBenefitsToSociety = study.StudyProtocol.PotentialBenefitsToSociety.RemoveInvalidXmlChars(),
                DSMBRequired = study.StudyProtocol.DsmbRequired,
                OverseeingStudyList = study.StudyProtocol.OverseeingStudyList.ToStrings(),
                AncillaryCommitteeReviewList = study.StudyProtocol.AncillaryCommitteeReviewList.ToStrings(),
                DataCoordinatingCenter = study.StudyProtocol.DataCoordinatingCenter,
                CoverageAnalysisRequired = study.StudyProtocol.CoverageAnalysisRequired,
                ProceduresMinimalRiskList = study.StudyProtocol.ProceduresMinimalRiskList.ToStrings(),
                HipaaAuthorizationList = study.StudyProtocol.HipaaAuthorizationList.ToStrings(),
                InstitutionName = study.InstitutionNames(),
                OtherSites = study.StudyProtocol.OtherSites,
                InvestigationalDrugs = study.InvestigationalDrugs(),
                InvestigationalDevices = study.InvestigationalDevices(),
                UpdatedDocuments = study.StudyProtocol.UpdatedDocumentList.ToStrings(),
                HumanitarianUseOfDevice = study.StudyProtocol.HumanitarianUseOfDevice,
                DeviceExpandedAccessStatus = study.StudyProtocol.DeviceExpandedAccessStatusList.ToStrings(),
                CompassionateUseOfDrugBiologic = study.StudyProtocol.CompassionateUseOfDrugBiologic,
                DrugBiologicExpandedAccessStatus = study.StudyProtocol.DrugBiologicExpandedAccessStatusList.ToStrings(),
                ProtocolActivities = study.ProtocolActivities(),
                Sponsorship = study.Awards(ProjectLookup,FAUKeysLookup,FundEndDatesLookup),
                RequiresHealthSystemResources = study.StudyProtocol.RequiresHealthSystemResources,
                NonInterventional = study.NonInterventional(),
                BillingDesignation = study.BillingDesignation(),
                ClinicalPreReviewInvestigatorInitiated = study.StudyProtocol.ClinicalPreReviewInvestigatorInitiatedFlag
            };
            study = null;
            return studyFacade;
        }

        /// <summary>
        /// Implementation of IIrbCrmsConverter.GetStudyFacades() which uses Linq to Object.
        /// </summary>
        /// <param name="studies"></param>
        /// <returns>List of Study in service model</returns>
        public IEnumerable<Models.Study> GetStudyFacades( IEnumerable<Study> studies, ILookup<string, ORA.Domain.Model.Project> ProjectLookup, ILookup<Tuple<string, string, string>, ORA.Domain.Model.FullAccountingUnitKey> FAUKeysLookup, ILookup<Tuple<string, string>, DateTime> FundEndDatesLookup )
        {
            // guard clause
            if (!studies.Any())
            {
                return new List<Models.Study>();
            }

            var studyFacades = studies.Select(study => GetStudyFacade(study, ProjectLookup, FAUKeysLookup, FundEndDatesLookup)).ToList();
            return studyFacades;
        }

        /// <summary>
        /// Implementation of IIrbCrmsConverter.GetStatusHistoryFacade() which uses Linq to Object.
        /// </summary>
        /// <param name="study"></param>
        /// <returns>StatusHistory in service model</returns>
        public Models.StudyStatusHistory GetStatusHistoryFacade( Study study)
        {
            return new Models.StudyStatusHistory()
            {
                IrbSystemUniqueId = study.UniqueId,
                IrbNumber = study.RootIrbNumber(),
                ReferenceIrbNumber = study.StudyProtocol.IrbNumber,
                AssignedReviewType = study.StudyProtocol.AssignedReviewType,
                PrmcReviewRequired = study.StudyProtocol.PrmcReviewRequired,
                TypeOfSubmission = study.StudyProtocol.TypeOfSubmission,
                ProtocolStatus = study.StudyProtocol.ProtocolStatus,
                //ProtocolUCLAHomeDepartment = study.StudyProtocol.UclaHomeDepartment,
                //RequiresHealthSystemResources = study.StudyProtocol.RequiresHealthSystemResources,
                //NonInterventional = study.NonInterventional(),
                //BillingDesignation = study.BillingDesignation(),
                //ClinicalPreReviewInvestigatorInitiated = study.StudyProtocol.ClinicalPreReviewInvestigatorInitiatedFlag,
                ContactWithSubjectsTypeList = study.StudyProtocol.ContactWithSubjectsTypeList.ToStrings(),
                ExternalStatusChanges = study.ExternalStatusChanges(),
            };
        }

        /// <summary>
        /// Implementation of IIrbCrmsConverter.GetStatusHistoryFacades() which uses Linq to Object.
        /// </summary>
        /// <param name="studies"></param>
        /// <returns>List of StatusHistory in service model</returns>
        public IEnumerable<Models.StudyStatusHistory> GetStatusHistoryFacades( IEnumerable<Study> studies )
        {
            // guard clause
            if (!studies.Any())
            {
                return new List<Models.StudyStatusHistory>();
            }

            var StatusHistoryFacades = studies.Select(study => GetStatusHistoryFacade(study)).ToList();
            return StatusHistoryFacades;
        }
    }
}