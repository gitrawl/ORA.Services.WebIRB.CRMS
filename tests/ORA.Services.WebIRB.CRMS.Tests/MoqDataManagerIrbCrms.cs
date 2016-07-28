using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Newtonsoft.Json;
using ORA.Services.WebIRBCRMS;
using ORA.Services.WebIRBCRMS.Controllers;
using ORA.Services.WebIRBCRMS.Models;
using ORA.Data.WebIrb.Staging;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;

namespace ORA.Services.WebIRBCRMS.Tests
{
    /// <summary>
    /// Mocking data manager of IDataManagerIrbCrms primarily for unit testing.
    /// </summary>
    public class MoqDataManagerIrbCrms : Mock<IDataManagerWebIrbStaging>
    {
          // setup
        private int _numberOfFakes = 1;
        private static List<ORA.Domain.Model.HumanSubjects.Study> fakeStudies = new List<ORA.Domain.Model.HumanSubjects.Study>();
        private static List<ORA.Domain.Model.HumanSubjects.Study> _fakeStudiesBig = new List<ORA.Domain.Model.HumanSubjects.Study>();
        private static List<ORA.Domain.Model.HumanSubjects.Study> _fakeStudiesHistory = new List<ORA.Domain.Model.HumanSubjects.Study>();

        #region KeyPersonnel Data
        private static List<Domain.Model.HumanSubjects.KeyPersonnel> _stubKeyPersonnel = new List<Domain.Model.HumanSubjects.KeyPersonnel>()
        {
            new Domain.Model.HumanSubjects.KeyPersonnel()
            {
                EmployeeId = "1",
                FirstName = "John",
                MiddleName = "M",
                LastName = "Smith",
                EmailAddress = "SomeEmailAddress",
                PhoneNumber = "SomePhoneNumber",
                Location =  new Location(){Code='4', Name=""},
                Titles = _stubTitles,
                CitizenStatus = Citizenship.Citizen,
                SmartFormOid = "SomeSmartFormOid",
                Institution = "SomeInstitution",
                WillObtainConsentFl = "Yes",
                ManageDeviceAccountabilityFl = "Yes",
                AccessToPiiFl = "Yes",
                AccessToCodeKey = "Yes",
                OtherRole = "Yes",
                AccessRoleList = AccessRole.role_co_investigator_fl | AccessRole.role_other_fl
            },
            new Domain.Model.HumanSubjects.KeyPersonnel()
            {
                EmployeeId = "2",
                FirstName = "Tom",
                MiddleName = "K",
                LastName = "Smith",
                EmailAddress = "SomeEmailAddress",
                PhoneNumber = "SomePhoneNumber",
                Location = new Location(){Code='4', Name=""},
                Titles = _stubTitles,
                CitizenStatus = Citizenship.Citizen,
                SmartFormOid = "SomeSmartFormOid",
                Institution = "SomeInstitution",
                WillObtainConsentFl = "Yes",
                ManageDeviceAccountabilityFl = "Yes",
                AccessToPiiFl = "Yes",
                AccessToCodeKey = "Yes",
                OtherRole = "Yes",
                AccessRoleList = AccessRole.role_co_investigator_fl | AccessRole.role_other_fl
            }
        };
        #endregion

        #region Title Data
        private static TitleKey _stubTitleKey = new TitleKey()
        {
            TitleCode = "SomeTitleCode",
            Title = "SomeTitle"
        };
        

        private static List<TitleKey> _stubTitles = new List<TitleKey>()
        {
            _stubTitleKey
        };
        #endregion

        #region Location
        private static Domain.Model.Location _stubLocation = new Domain.Model.Location()
        {
            Code = '4'
        };
        #endregion

        #region Person Data
        private static Domain.Model.Person _stubPerson = new Domain.Model.Person()
        {
            EmployeeId = "1",
            FirstName = "John",
            MiddleName = "M",
            LastName = "Smith",
            EmailAddress = "SomeEmailAddress",
            PhoneNumber = "SomePhoneNumber",
            Location = _stubLocation,
            Titles = _stubTitles,
            CitizenStatus = Citizenship.Citizen
        };

        private static Domain.Model.Person _stubPerson2 = new Domain.Model.Person()
        {
            EmployeeId = "2",
            FirstName = "Johnny",
            MiddleName = "M",
            LastName = "Smith",
            EmailAddress = "SomeEmailAddress",
            PhoneNumber = "SomePhoneNumber",
            Location = _stubLocation,
            Titles = _stubTitles,
            CitizenStatus = Citizenship.Citizen
        };

        private static List<Domain.Model.Person> _stubPeople = new List<Domain.Model.Person>()
        {
            _stubPerson,
            _stubPerson2
        };
        #endregion

        #region Institution Data
        private static Institution _stubInstitution = new Institution()
        {
            Id = "1",
            InstitutionName = StudyLocation.study_location_ucla_sites_fl,
            LeadInstitution = "SomeLeadInstitution",
            ProtocolExternalKey = "1"
        };
        #endregion

        #region Department Data
        private static Department _stubDepartment = new Department()
        {
            DepartmentTitle = "FakeDept",
            DepartmentCode = "1234",
            Location = _stubLocation,
        };
        #endregion

        #region Principal Investigator Data
        private static PrincipalInvestigator _stubInvestigator = new PrincipalInvestigator()
        {
            FirstName = "Bob",
            MiddleName= "B",
            LastName = "FakeInvestigator",
            EmployeeId = "123456789",
            EmailAddress = "SomeEmailAddress",
            Location = _stubLocation,
            HomeDepartment = _stubDepartment
        };
        #endregion

        #region Site Data
        private static List<string> _stubOtherSites = new List<string>()
        {
            "Site1",
            "Site2"
        };
        #endregion

        #region Protocol Activities Data
        private static ProtocolActivities _stubProtocolActivity = new ProtocolActivities()
        {
            ActivityKey = 1,
            ActivityOid = "SomeActivityOid",
            ActionStatus = "SomeActionStatus",
            Notes = "SomeNotes",
            ActionStatusDate = DateTime.Today.AddDays(-1),
            ProtocolOid = "SomeProtocolOid"
        };

        private static List<ProtocolActivities> _stubProtocolActivites1 = new List<ProtocolActivities>()
        {
            new ProtocolActivities()
            {
                ActivityKey = 1,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-30),
                ProtocolOid = "SomeProtocolOid"
            },
            new ProtocolActivities()
            {
                ActivityKey = 2,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                ProtocolOid = "SomeProtocolOid"
            },
            new ProtocolActivities()
            {
                ActivityKey = 3,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                ProtocolOid = "SomeProtocolOid"
            },
            new ProtocolActivities()
            {
                ActivityKey = 4,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                ProtocolOid = "SomeProtocolOid"
            },
        };

        private static List<ProtocolActivities> _stubProtocolActivites2 = new List<ProtocolActivities>()
        {
            new ProtocolActivities()
            {
                ActivityKey = 11,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-30),
                ProtocolOid = "SomeProtocolOid"
            },
            new ProtocolActivities()
            {
                ActivityKey = 21,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                ProtocolOid = "SomeProtocolOid"
            },
            new ProtocolActivities()
            {
                ActivityKey = 13,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                ProtocolOid = "SomeProtocolOid"
            },
            new ProtocolActivities()
            {
                ActivityKey = 14,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                ProtocolOid = "SomeProtocolOid"
            },
        };
        #endregion

        #region Amendment Activities Data
        private static List<AmendmentActivities> _stubAmendmentActivities1 = new List<AmendmentActivities>()
        {
           new AmendmentActivities(){
                ActivityKey = 2,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 5,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 7,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 8,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E3",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
        };

        private static List<AmendmentActivities> _stubAmendmentActivities2 = new List<AmendmentActivities>()
        {
           new AmendmentActivities(){
                ActivityKey = 10,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 11,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 12,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 13,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E3",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 14,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E3",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 15,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 16,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
           new AmendmentActivities(){
                ActivityKey = 17,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-10),
                AmendmentOid = "SomeAmendmentOid"
           },
        };
        #endregion

        #region Continuing Review Activities Data
        private static List<ContinuingReviewActivities> _stubContinuingReviewActivities1 = new List<ContinuingReviewActivities>()
        {
            new ContinuingReviewActivities()
            {
                ActivityKey = 1,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-30),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 2,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 3,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 4,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E3",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 5,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
        };

        private static List<ContinuingReviewActivities> _stubContinuingReviewActivities2 = new List<ContinuingReviewActivities>()
        {
            new ContinuingReviewActivities()
            {
                ActivityKey = 11,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-30),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 12,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 13,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 14,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
            new ContinuingReviewActivities()
            {
                ActivityKey = 15,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                Notes = "SomeNotes",
                ActionStatusDate = DateTime.Today.AddDays(-9),
                ContinuingReviewOid = "SomeContinuingReviewOid"
            },
        };
        #endregion

        #region Post Approval Report Activites Data
        private static List<PostApprovalReportActivities> _stubPostApprovalReportActivities1 = new List<PostApprovalReportActivities>()
        {
            new PostApprovalReportActivities()
            {
                ActivityKey = 1,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                Notes = "SomeNotes",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                ActionStatusDate = DateTime.Today.AddDays(-7),
                PostApprovalReportOid = "SomePostApprovalReportOid"
            },
            new PostApprovalReportActivities()
            {
                ActivityKey = 2,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                Notes = "SomeNotes",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E2",
                ActionStatusDate = DateTime.Today.AddDays(-7),
                PostApprovalReportOid = "SomePostApprovalReportOid"
            },
            new PostApprovalReportActivities()
            {
                ActivityKey = 3,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                Notes = "SomeNotes",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E3",
                ActionStatusDate = DateTime.Today.AddDays(-7),
                PostApprovalReportOid = "SomePostApprovalReportOid"
            },
        };

        private static List<PostApprovalReportActivities> _stubPostApprovalReportActivities2 = new List<PostApprovalReportActivities>()
        {
            new PostApprovalReportActivities()
            {
                ActivityKey = 11,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                Notes = "SomeNotes",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                ActionStatusDate = DateTime.Today.AddDays(-7),
                PostApprovalReportOid = "SomePostApprovalReportOid"
            },
            new PostApprovalReportActivities()
            {
                ActivityKey = 12,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                Notes = "SomeNotes",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E3",
                ActionStatusDate = DateTime.Today.AddDays(-7),
                PostApprovalReportOid = "SomePostApprovalReportOid"
            },
            new PostApprovalReportActivities()
            {
                ActivityKey = 13,
                ActivityOid = "SomeActivityOid",
                ActionStatus = "SomeActionStatus",
                Notes = "SomeNotes",
                InternalStatus = "SomeInternalStatus",
                ExternalStatus = "E1",
                ActionStatusDate = DateTime.Today.AddDays(-7),
                PostApprovalReportOid = "SomePostApprovalReportOid"
            },
        };
        #endregion

        #region InvestigationalDrugBiologic Data
        private static InvestigationalDrugBiologic _stubInvestigationalDrugBiologic = new InvestigationalDrugBiologic()
        {
            Id = "SomeId",
            SmartFormOid = "SomeSmartFormOid",
            HolderName = "SomeHolderName",
            Exempt = "SomeExempt",
            SubmitToFDADate = DateTime.Today.AddDays(-1),
            FDAApprovalDate = DateTime.Today.AddDays(-1)
        };

        private static List<InvestigationalDrugBiologic> _stubInvestigationalDrugBiologics = new List<InvestigationalDrugBiologic>()
        {
            _stubInvestigationalDrugBiologic,
            _stubInvestigationalDrugBiologic
        };
        #endregion

        #region InvestigationalDevice Data
        private static InvestigationalDevice _stubInvestigationalDevice = new InvestigationalDevice()
        {
            Id = "SomeId",
            SmartFormOid = "SomeSmartFormOid",
            HolderName = "SomeHolderName",
            Exempt = "SomeExempt",
            Risk = "SomeRisk",
            SubmitToFDADate = DateTime.Today.AddDays(-1),
            FDAApprovalDate = DateTime.Today.AddDays(-1)
        };

        private static List<InvestigationalDevice> _stubInvestigationalDevices = new List<InvestigationalDevice>()
        {
            _stubInvestigationalDevice,
            _stubInvestigationalDevice
        };
        #endregion

        #region FacultySponsor Data
        private static FacultySponsor _stubFacultySponsor = new FacultySponsor()
        {
            Uid = "SomeUid",
            FirstName = "SomeFirstName",
            LastName = "SomeLastName",
            SponsorPersonOid = "SomeSponsorPersonOid",
            StudyCoordinatorPersonOid = "SomeStudyCoordinatorPersonOid"
        };
        #endregion

        #region AssignedCommittee
        private static Committee _stubCommittee = new Committee()
        {
            CommitteeID = "SomeCommitteeID",
            CommitteeName = "SomeCommitteeName",
            CommitteeOid = "SomeCommitteeOid"
        };
        #endregion

        #region HumanSubjectProtocol Data
        private static HumanSubjectProtocol _stubProtocol = new HumanSubjectProtocol()
        {
            ExternalKey = 1,
            DepartmentRC = "SomeDepartment",
            PreIrbNumber = "SomePreIrbNumber",
            IrbNumber = "MS#14_IRB#11-000397",
            Title = "SomeTitle",
            ShortTitle = "SomeShortTitle",
            Objectives = "SomeObjectives",
            PhaseList = Phase.phase_of_the_clinical_trial_behavioral_fl | Phase.phase_of_the_clinical_trial_expanded_access_fl | Phase.historical_phase_of_the_clinical_trial_open_label_extension_fl,
            TypeOfSubmission = "SomeTypeOfSubmission",
            AgeGroupList = AgeGroup.study_population_age_range_0_to_6_years_fl | AgeGroup.study_population_age_range_12_to_17_years_fl,
            InvestigationalDrugList = InvestigationalDrug.methods_and_procedures_audio_visual_or_digital_recordings_fl,
            InvestigatorInitiatedList = InvestigatorInitiate.who_developed_this_study_another_institution_fl | InvestigatorInitiate.who_developed_this_study_cooperative_group_fl,
            PrmcReviewRequired = "Yes",
            Keywords = "Keyword1, Keyword2",
            ProtocolType = "SomeProtocolType",
            ProtocolTargetAccrual = "SomeProtocolTargetAccrual",
            UpperAccrualGoal = "SomeUpperAccrualGoal",
            AffiliateAccrualGoal = "SomeAffiliateAccrualGoal",
            ProtocolStatus = "Approved",
            StatusEffectiveDate = DateTime.Today.AddDays(-1),
            IrbReviewDate = DateTime.Today.AddDays(-1),
            IrbSubmissionDate = DateTime.Today.AddDays(-1),
            IrbInitialApprovedDate = DateTime.Today.AddDays(-1),
            ExpirationDate = DateTime.Today.AddDays(-1),
            AssignedIrbCommittee = _stubCommittee,
            AssignedReviewType = "SomeAssignedReviewType",
            InvolvesTherapy = "Yes",
            FdaRegulatedList = FdasRegulated.regulated_by_fda_biological_products_fl,
            OtherRegulatedOption = "Yes",
            MultiSiteTrial = "Yes",
            ReviewForReliance = "Yes",
            IRBReviewsTypeList = IRBReviewsType.submission_type_review_sources_faculty_sponsor_fl | IRBReviewsType.submission_type_review_sources_isprc_fl,
            SupportedbyHhs = "Yes",
            Risk = "SomeRisk",
            ExpeditedReviewTypeList = ExpeditedReviewType.applicable_categories_for_exempt_studies_category_1_fl | ExpeditedReviewType.applicable_categories_for_exempt_studies_category_4_fl,
            OverallDirection = "Yes",
            ProceduresMinimalRiskList = ProceduresMinimalRisk.procedures_greater_than_minimal_risk_be_conducted_ctrc_fl,
            ContactWithSubjectsTypeList = ContactWithSubjectsType.study_design_both_fl,
            TypeOfTrialList = TypeOfTrial.type_of_clinical_trial_active_treatment_control_fl | TypeOfTrial.type_of_clinical_trial_crossover_fl,
            ClinicalTrialsRegistrationStatus = "SomeClinicalTrialsRegistrationStatus",
            TrialRegistrationNumber = "SomeTrialRegistrationNumber",
            SourceUseOfDataList = SourceUseOfData.study_data_source_health_care_fl,
            DataCollectionWithoutParticipantContactList = DataCollectionWithoutParticipantContact.study_data_types_human_biological_specimens_fl,
            ObservationalEthnographic = "Yes",
            ScreenRecruitMeetTarget = "1",
            PopulationTypeList = PopulationType.none,
            NonEnglishSpeakers = "Yes",
            PotentialBenefitsToSociety = "SomePotentialBenefitsToSociety",
            DsmbRequired = "Yes",
            OverseeingStudyList = OverseeingStudy.be_responsible_for_overseeing_the_study_safety_formally_constituted_dsmb_fl,
            AncillaryCommitteeReviewList = AncillaryCommitteeReview.requires_ancillary_committee_review_escro_fl,
            DataCoordinatingCenter = "SomeDataCoordinatingCenter",
            CoverageAnalysisRequired = "Yes",
            HipaaAuthorizationList = HipaaAuthorization.hipaa_a_total_waiver_of_hipaa_research_authorization_fl,
            Institution = _stubInstitution,
            OtherSites = _stubOtherSites,
            ProtocolActivities = _stubProtocolActivites1,
            InvestigationalDrugBiologics = _stubInvestigationalDrugBiologics,
            InvestigationalDevices = _stubInvestigationalDevices,
            UpdatedDocumentList = UpdatedDocument.UpdatedDocument_protocol_fl,
            HumanitarianUseOfDevice = "Yes",
            DeviceExpandedAccessStatusList = Device_ExpandedAccessStatus.none,
            CompassionateUseOfDrugBiologic = "Yes",
            DrugBiologicExpandedAccessStatusList = DrugBiologic_ExpandedAccessStatus.none,
            StudyTeamMembers = _stubPeople,
            StudyCoordinator = _stubPerson,
            ProtocolPI = _stubInvestigator,
            FacultySponsor = _stubFacultySponsor,
            BillingDesignation = "No - we will bill one or more protocol-required items or services to participants or their insurers",
            RequiresHealthSystemResources = "Yes",
            ClinicalPreReviewInvestigatorInitiatedFlag = "Yes",
            UclaHomeDepartment = "SomeUclaHomeDepartment"
        };
        #endregion

        #region Amendment Data
        private static Domain.Model.HumanSubjects.Amendment _stubAmendment1 = new Domain.Model.HumanSubjects.Amendment()
        {
            ParentRecordKey = 1,
            AmendmentKey = 1,
            AmendmentNumber = "a1",
            ExternalKey = 1,
            AmendmentOid = "SomeAmendmentOid",
            ParentProtocolOid = "SomeParentmentOid",
            ModifiedStudyProtocolOid = "SomeModifiedStudyProtocolOid",
            ShortTitle = "SomeShortTitle",
            ChangeInStaffList = ChangeInStaff.none,
            PIChangeReason = "SomePIChangeReason",
            AmendmentDetailMinorList = AmendmentDetailMinor.minor_changes_clarification_or_technical_change_fl | AmendmentDetailMinor.minor_changes_minor_increas_decrease_in_number_of_study_participants_fl,
            AmendmentDetailMajorList = AmendmentDetailMajor.major_changes_change_in_study_design_fl | AmendmentDetailMajor.major_changes_change_in_status_of_study_participants_fl,
            AmendmentDetailOther = "SomeAmendmentDetailOther",
            Modifications = "SomeModifications",
            ParticipantsEnrolled = "Yes",
            Reconsent = "Yes",
            CompletedNotification = "Yes",
            AssignedReviewType = "SomeAssignedReviewType",
            RadiationProcedures = "SomeRadiationProcedures",
            UpdatedDocumentList = UpdatedDocuments.study_in_crms_assent_form_fl | UpdatedDocuments.study_in_crms_informed_consent_form_fl,
            AmendmentActivities = _stubAmendmentActivities1,
            AssignedIrbCommittee = _stubCommittee,
            crms_protocol_selected_fl = "Yes"
        };

        private static Domain.Model.HumanSubjects.Amendment _stubAmendment2 = new Domain.Model.HumanSubjects.Amendment()
        {
            ParentRecordKey = 1,
            AmendmentKey = 2,
            ExternalKey = 2,
            AmendmentOid = "SomeAmendmentOid",
            ParentProtocolOid = "SomeParentmentOid",
            ModifiedStudyProtocolOid = "SomeModifiedStudyProtocolOid",
            ShortTitle = "SomeShortTitle",
            ChangeInStaffList = ChangeInStaff.none,
            PIChangeReason = "SomePIChangeReason",
            AmendmentDetailMinorList = AmendmentDetailMinor.minor_changes_clarification_or_technical_change_fl | AmendmentDetailMinor.minor_changes_minor_increas_decrease_in_number_of_study_participants_fl,
            AmendmentDetailMajorList = AmendmentDetailMajor.major_changes_change_in_study_design_fl | AmendmentDetailMajor.major_changes_change_in_status_of_study_participants_fl,
            AmendmentDetailOther = "SomeAmendmentDetailOther",
            Modifications = "SomeModifications",
            ParticipantsEnrolled = "Yes",
            Reconsent = "Yes",
            CompletedNotification = "Yes",
            AssignedReviewType = "SomeAssignedReviewType",
            RadiationProcedures = "SomeRadiationProcedures",
            UpdatedDocumentList = UpdatedDocuments.study_in_crms_assent_form_fl | UpdatedDocuments.study_in_crms_informed_consent_form_fl,
            AmendmentActivities = _stubAmendmentActivities2,
            AssignedIrbCommittee = _stubCommittee,
        };

        private static List<Domain.Model.HumanSubjects.Amendment> _stubAmendments = new List<Domain.Model.HumanSubjects.Amendment>()
        {
            _stubAmendment1,
            _stubAmendment2
        };
        #endregion

        #region ContinuingReview Data
        private static Domain.Model.HumanSubjects.ContinuingReview _stubContinuingReview1 = new Domain.Model.HumanSubjects.ContinuingReview()
        {
            ContinuingReviewKey = 1,
            ExternalKey = 1,
            ContinuingReviewOid = "SomeContinuingReviewOid",
            ReportType = "SomeReportType",
            CurrentStatusOfTheStudyEnrollment = "SomeCurrentStatusOfTheStudyEnrollment",
            ReasonForClosingStudyAtUCLA = "ReasonForClosingStudyAtUCLA",
            AssignedReviewType = "SomeAssignedReviewType",
            ContinuingReviewActivities = _stubContinuingReviewActivities1,
            AssignedIrbCommittee = _stubCommittee,
        };

        private static Domain.Model.HumanSubjects.ContinuingReview _stubContinuingReview2 = new Domain.Model.HumanSubjects.ContinuingReview()
        {
            ContinuingReviewKey = 2,
            ExternalKey = 2,
            ContinuingReviewOid = "SomeContinuingReviewOid",
            ReportType = "SomeReportType",
            CurrentStatusOfTheStudyEnrollment = "SomeCurrentStatusOfTheStudyEnrollment",
            ReasonForClosingStudyAtUCLA = "ReasonForClosingStudyAtUCLA",
            AssignedReviewType = "SomeAssignedReviewType",
            ContinuingReviewActivities = _stubContinuingReviewActivities2,
            AssignedIrbCommittee = _stubCommittee,
        };

        private static List<Domain.Model.HumanSubjects.ContinuingReview> _stubContinuingReviews = new List<Domain.Model.HumanSubjects.ContinuingReview>()
        {
            _stubContinuingReview1,
            _stubContinuingReview2
        };
        #endregion

        #region PostApprovalReport Data
        private static Domain.Model.HumanSubjects.PostApprovalReport _stubPostApprovalReport1 = new Domain.Model.HumanSubjects.PostApprovalReport()
        {
            PostApprovalReportKey = 1,
            ExternalKey = 1,
            PostApprovalReportOid = "SomePostApprovalReportOid",
            Description = "SomeDescription",
            ReportType = "SomeReportType",
            InvestigatorBrochureUpdate = "Yes",
            AssignedReviewType = "SomeAssignedReviewType",
            PostApprovalReportActivities = _stubPostApprovalReportActivities1,
            AssignedIrbCommittee = _stubCommittee,
        };

        private static Domain.Model.HumanSubjects.PostApprovalReport _stubPostApprovalReport2 = new Domain.Model.HumanSubjects.PostApprovalReport()
        {
            PostApprovalReportKey = 2,
            ExternalKey = 2,
            PostApprovalReportOid = "SomePostApprovalReportOid",
            Description = "SomeDescription",
            ReportType = "SomeReportType",
            InvestigatorBrochureUpdate = "Yes",
            AssignedReviewType = "SomeAssignedReviewType",
            PostApprovalReportActivities = _stubPostApprovalReportActivities2,
            AssignedIrbCommittee = _stubCommittee,
        };

        private static List<Domain.Model.HumanSubjects.PostApprovalReport> _stubPostApprovalReports = new List<Domain.Model.HumanSubjects.PostApprovalReport>()
        {
            _stubPostApprovalReport1,
            _stubPostApprovalReport2
        };
        #endregion

        #region IrbApprovals Data
        private static IrbApproval _stubIrbApproval = new IrbApproval()
        {
            IrbNumber = "13-001240"
        };


        private static List<IrbApproval> _stubApprovals = new List<IrbApproval>()
        {
            _stubIrbApproval,
        };
        #endregion

        #region Study Data
        private static Domain.Model.HumanSubjects.Study _stubStudy = new Domain.Model.HumanSubjects.Study()
        {
            StudyProtocol = _stubProtocol,
            KeyPersonnels = _stubKeyPersonnel,
            Amendments = _stubAmendments,
            ContinuingReviews = _stubContinuingReviews,
            PostApprovalReports = _stubPostApprovalReports,
        };
        #endregion

        public void SetNumberOfFakes(int i)
        {
            _numberOfFakes = i;
            AddStudies();
        }

        public MoqDataManagerIrbCrms()
        {
            fakeStudies.Add(_stubStudy);
            this.Setup(foo => foo.GetStudyByUniqueId(It.IsAny<int>())).Returns(_stubStudy);
            this.Setup(foo => foo.GetStudyChangesByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(_fakeStudiesBig);
            this.Setup(foo => foo.GetStudyStatusHistoryByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(_fakeStudiesBig);
            this.Setup(foo => foo.GetAmendmentsInProcessByDateRange()).Returns(_fakeStudiesBig);
            this.Setup(foo => foo.GetPreReviewedStudies()).Returns(_fakeStudiesBig);
        }

        public void AddStudies()
        {
            for (int i = 1; i <= _numberOfFakes; i++)
            {
                ORA.Domain.Model.HumanSubjects.Study study = new ORA.Domain.Model.HumanSubjects.Study()
                {
                    UniqueId = i,
                    StudyProtocol = _stubProtocol,
                    KeyPersonnels = _stubKeyPersonnel,
                    Amendments = _stubAmendments,
                    ContinuingReviews = _stubContinuingReviews,
                    PostApprovalReports = _stubPostApprovalReports,
                    Approvals = _stubApprovals,
                };
                _fakeStudiesBig.Add(study);
            }
        }

        public void AddStudyHistories()
        {
            DateTime d1 = DateTime.Today.AddDays(-1);
            DateTime d2 = d1.AddHours(6);
            DateTime d3 = d1.AddHours(8);

            for (int i = 1; i <= _numberOfFakes; i++)
            {
                ORA.Domain.Model.HumanSubjects.Study studyState1 = new ORA.Domain.Model.HumanSubjects.Study()
                {
                    UniqueId = i,
                    StudyProtocol = new HumanSubjectProtocol()
                    {
                        ProtocolPI = _stubInvestigator,
                        ProtocolStatus = "Some Status",
                        StatusEffectiveDate = d1,
                        IrbReviewDate = d1,
                        ProtocolTargetAccrual = "",
                        ShortTitle = String.Concat("Study Number ", i.ToString()),
                        //PhaseList = GetPhases()
                    },
                    KeyPersonnels = _stubKeyPersonnel,
                };
                ORA.Domain.Model.HumanSubjects.Study studyState2 = new ORA.Domain.Model.HumanSubjects.Study()
                {
                    StudyProtocol = new HumanSubjectProtocol()
                    {
                        ProtocolPI = _stubInvestigator,
                        ProtocolStatus = "Some Other Status",
                        StatusEffectiveDate = d2,
                        IrbReviewDate = d2,
                        ProtocolTargetAccrual = "",
                        ShortTitle = String.Concat("Study Number ", i.ToString()),
                        //PhaseList = GetPhases()
                    },
                    KeyPersonnels = _stubKeyPersonnel,
                    //UniqueId = String.Concat("IRB", i.ToString()),
                };
                ORA.Domain.Model.HumanSubjects.Study studyState3 = new ORA.Domain.Model.HumanSubjects.Study()
                {
                    //Approval = _stubApproval,
                    StudyProtocol = new HumanSubjectProtocol()
                    {
                        ProtocolPI = _stubInvestigator,
                        ProtocolStatus = "Approved",
                        StatusEffectiveDate = d3,
                        IrbReviewDate = d3,
                        ProtocolTargetAccrual = "",
                        ShortTitle = String.Concat("Study Number ", i.ToString()),
                        //PhaseList = GetPhases()
                    },
                    KeyPersonnels = _stubKeyPersonnel,
                    //UniqueId = String.Concat("IRB", i.ToString()),
                };

                _fakeStudiesHistory.Add(studyState1);
                _fakeStudiesHistory.Add(studyState2);
                _fakeStudiesHistory.Add(studyState3);
            }
        }
    }
}
