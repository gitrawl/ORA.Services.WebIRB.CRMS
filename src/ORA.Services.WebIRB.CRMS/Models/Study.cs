using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;
using System.ComponentModel.DataAnnotations;

namespace ORA.Services.WebIRBCRMS.Models
{
    /// <summary>
    /// The CRMS representation of an IRB Study/Protocol. The destination object
    /// for our IRB data which will be presented directly for RESTful serialization 
    /// to the consumer. The bulk of the contract effort currently centers around this
    /// object.
    /// </summary>
    public class Study
    {
        // Study Data
        [Key]
        public int IrbSystemUniqueId { get; set; } // our unique key
        public IEnumerable<Amendment> Amendments { get; set; }
        public IEnumerable<ContinuingReview> ContinuingReviews { get; set; }
        public IEnumerable<PostApprovalReport> PostApprovalReports { get; set; }
        public string Department { get; set; }
        public string ProtocolUCLAHomeDepartment { get; set; }
        public string PreIrbNumber { get; set; }
        public string IrbNumber { get; set; }
        public string ReferenceIrbNumber { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Objectives { get; set; }
        public IEnumerable<string> Phases { get; set; }
        public string TypeOfSubmission { get; set; }
        public IEnumerable<string> AgeGroups { get; set; }
        public IEnumerable<string> InvestigationalMethods { get; set; }
        public IEnumerable<string> InvestigatorInitiated { get; set; }
        public string PrmcReviewRequired { get; set; }
        public string Keywords { get; set; }
        public ProtocolPI ProtocolPI { get; set; }
        public Person StudyContact { get; set; }
        public Person FacultySponsor { get; set; }
        public string ProtocolType { get; set; }
        public string ProtocolTargetAccrual { get; set; }
        public string UpperAccrualGoal { get; set; }
        public string AffiliateAccrualGoal { get; set; }
        public string ProtocolStatus { get; set; }
        public DateTimeOffset? DateProtocolStatusChanged { get; set; } 
        public DateTimeOffset? IrbReviewDate { get; set; }
        public DateTimeOffset? IrbSubmissionDate { get; set; }
        public DateTimeOffset? IrbInitialApprovedDate { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public string AssignedIrbCommittee { get; set; }
        public IEnumerable<Person> KeyPersonnel { get; set; } 
        public string AssignedReviewType { get; set; }
        public string InvolvesTherapy { get; set; }
        public IEnumerable<string> FDARegulatedList { get; set; }
        public string MultiSiteTrial { get; set; }
        public string ReviewForReliance { get; set; }
        public IEnumerable<string> IRBReviewsTypeList { get; set; }
        public string SupportedByHHS { get; set; }
        public string Risk { get; set; }
        public IEnumerable<string> ExpeditedReviewTypeList { get; set; }
        public string OverallDirection { get; set; }
        public IEnumerable<string> ContactWithSubjectsTypeList { get; set; }
        public IEnumerable<string> TypeOfTrialList { get; set; }
        public string ClincialTrialRegistrationStatus { get; set; }
        public string TrialRegistrationNumber { get; set; }
        public IEnumerable<string> SourceUseOfDataList { get; set; }
        public IEnumerable<string> DataCollectionWithoutParticipantContactList { get; set; }
        public string ObservationalEthnographic { get; set; }
        public string ScreenRecruitTarget { get; set; }
        public IEnumerable<string> PopulationTypeList { get; set; }
        public string NonEnglishSpeakers { get; set; }
        public string PotentialBenefitsToSociety { get; set; }
        public string DSMBRequired { get; set; }
        public IEnumerable<string> OverseeingStudyList { get; set; }
        public IEnumerable<string> ProceduresMinimalRiskList { get; set; }
        public IEnumerable<string> AncillaryCommitteeReviewList { get; set; }
        public string DataCoordinatingCenter { get; set; }
        public string CoverageAnalysisRequired { get; set; }
        public IEnumerable<string> HipaaAuthorizationList { get; set; }
        public string RequiresHealthSystemResources { get; set; }
        public string NonInterventional { get; set; }
        public string BillingDesignation { get; set; }
        public string ClinicalPreReviewInvestigatorInitiated { get; set; }

        //Institution
        public IEnumerable<string> InstitutionName { get; set; }
        public IEnumerable<string> OtherSites { get; set; }

        public IEnumerable<Drug> InvestigationalDrugs { get; set; }
        public IEnumerable<Device> InvestigationalDevices { get; set; }

        public IEnumerable<string> UpdatedDocuments { get; set; }
        public string HumanitarianUseOfDevice { get; set; }
        public IEnumerable<string> DeviceExpandedAccessStatus { get; set; }
        public string CompassionateUseOfDrugBiologic { get; set; }
        public IEnumerable<string> DrugBiologicExpandedAccessStatus { get; set; }

        // Sponsored Projects
        public IEnumerable<Award> Sponsorship { get; set; }
        // Protocol Activities
        public IEnumerable<ProtocolActivity> ProtocolActivities { get; set; }
    }
}