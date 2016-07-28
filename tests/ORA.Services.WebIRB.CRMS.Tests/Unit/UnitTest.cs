using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using ORA.Services.WebIRBCRMS;
using ORA.Services.WebIRBCRMS.Controllers;
using ORA.Services.WebIRBCRMS.Models;
using ORA.Data.WebIrb.Staging;
using ORA.FakeIRBData;
using ORA.Data.FakeWarehouse;
using System.Web.Http.Results;
using System.Web.Http;

namespace ORA.Services.WebIRBCRMS.Tests.Unit
{
    [TestClass]
    public class UnitTest
    {
        //setup
        protected static IrbRecordUpdatesForCrmsController _testRecordController;
        protected static IrbStudyStatusHistoryUpdatesForCrmsController _testStatusController;
        protected static IrbAmendmentUpdatesForCrmsController _testAmendmentController;
        protected static IrbPreSubmissionUpdatesForCrmsController _testPreSubmissionController;
        private static int _numberToFake = 500;

        [AssemblyInitialize()]
        public static void AssemblyInit( TestContext context )
        {
            // Mock up a data manager
            var moqDm = new MoqDataManagerIrbCrms();
            var moqDmQDB = new MoqDataManagerQDB();
            _testRecordController = new IrbRecordUpdatesForCrmsController(moqDm.Object, new FakeProjectDao(), moqDmQDB.Object);
            _testStatusController = new IrbStudyStatusHistoryUpdatesForCrmsController(moqDm.Object);
            _testPreSubmissionController = new IrbPreSubmissionUpdatesForCrmsController(moqDm.Object, new FakeProjectDao(), moqDmQDB.Object);
            _testAmendmentController = new IrbAmendmentUpdatesForCrmsController(moqDm.Object, new FakeProjectDao(), moqDmQDB.Object);
            moqDm.SetNumberOfFakes(_numberToFake);
            moqDm.AddStudyHistories();

            string appDataDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataDirectory);

        }

        /// <summary>
        /// Test SC1, verify the count of studies returns is correct
        /// </summary>
        [TestMethod]
        public void GetStudyByStartDate_ShouldReturnStudies()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<Study>>));
            Assert.AreEqual(((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.Count(), _numberToFake);
        }

        /// <summary>
        /// Test SC1 for date range, verify the count of studies returns is correct
        /// </summary>
        [TestMethod]
        public void GetStudyByDates_ShouldReturnStudies()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2), DateTime.Today);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<Study>>));
            Assert.AreEqual(((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.Count(), _numberToFake);
        }

        /// <summary>
        /// Verify Study data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyStudyForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Assert.AreEqual(study.IrbSystemUniqueId, 1);
            Assert.AreEqual(study.Department, "SomeDepartment");
            Assert.AreEqual(study.PreIrbNumber, "SomePreIrbNumber");
            Assert.AreEqual(study.IrbNumber, "IRB#11-000397");
            Assert.AreEqual(study.ReferenceIrbNumber, "MS#14_IRB#11-000397");
            Assert.AreEqual(study.Title, "SomeTitle");
            Assert.AreEqual(study.ShortTitle, "SomeShortTitle");
            Assert.AreEqual(study.Objectives, "SomeObjectives");
            Assert.AreEqual(study.Phases.Count(), 3);
            Assert.IsTrue(study.Phases.Contains("phase_of_the_clinical_trial_behavioral_fl") && study.Phases.Contains("phase_of_the_clinical_trial_expanded_access_fl"));
            Assert.AreEqual(study.TypeOfSubmission, "SomeTypeOfSubmission");
            Assert.AreEqual(study.AgeGroups.Count(), 2);
            Assert.IsTrue(study.AgeGroups.Contains("study_population_age_range_0_to_6_years_fl") && study.AgeGroups.Contains("study_population_age_range_12_to_17_years_fl"));
            Assert.AreEqual(study.InvestigationalMethods.Count(), 1);
            Assert.IsTrue(study.InvestigationalMethods.Contains("methods_and_procedures_audio_visual_or_digital_recordings_fl"));
            Assert.AreEqual(study.InvestigatorInitiated.Count(), 2);
            Assert.IsTrue(study.InvestigatorInitiated.Contains("who_developed_this_study_another_institution_fl") && study.InvestigatorInitiated.Contains("who_developed_this_study_cooperative_group_fl"));
            Assert.AreEqual(study.PrmcReviewRequired, "Yes");
            Assert.AreEqual(study.Keywords, "Keyword1, Keyword2");
            Assert.AreEqual(study.ProtocolType, "SomeProtocolType");
            Assert.AreEqual(study.ProtocolTargetAccrual, "SomeProtocolTargetAccrual");
            Assert.AreEqual(study.UpperAccrualGoal, "SomeUpperAccrualGoal");
            Assert.AreEqual(study.AffiliateAccrualGoal, "SomeAffiliateAccrualGoal");
            Assert.AreEqual(study.ProtocolStatus, "Approved");
            Assert.IsNotNull(study.DateProtocolStatusChanged);
            Assert.IsNotNull(study.IrbReviewDate);
            Assert.IsNotNull(study.IrbSubmissionDate);
            Assert.IsNotNull(study.IrbInitialApprovedDate);
            Assert.IsNotNull(study.ExpirationDate);
            Assert.AreEqual(study.AssignedIrbCommittee, "SomeCommitteeName");
            Assert.AreEqual(study.AssignedReviewType, "SomeAssignedReviewType");
            Assert.AreEqual(study.InvolvesTherapy, "Yes");
            Assert.AreEqual(study.FDARegulatedList.Count(), 1);
            Assert.IsTrue(study.FDARegulatedList.Contains("regulated_by_fda_biological_products_fl"));
            Assert.AreEqual(study.MultiSiteTrial, "Yes");
            Assert.AreEqual(study.ReviewForReliance, "Yes");
            Assert.AreEqual(study.IRBReviewsTypeList.Count(), 2);
            Assert.IsTrue(study.IRBReviewsTypeList.Contains("submission_type_review_sources_faculty_sponsor_fl") && study.IRBReviewsTypeList.Contains("submission_type_review_sources_isprc_fl"));
            Assert.AreEqual(study.SupportedByHHS, "Yes");
            Assert.AreEqual(study.Risk, "SomeRisk");
            Assert.AreEqual(study.ExpeditedReviewTypeList.Count(), 2);
            Assert.IsTrue(study.ExpeditedReviewTypeList.Contains("applicable_categories_for_exempt_studies_category_1_fl") && study.ExpeditedReviewTypeList.Contains("applicable_categories_for_exempt_studies_category_4_fl"));
            Assert.AreEqual(study.OverallDirection, "Yes");
            Assert.AreEqual(study.ContactWithSubjectsTypeList.Count(), 1);
            Assert.IsTrue(study.ContactWithSubjectsTypeList.Contains("study_design_both_fl"));
            Assert.AreEqual(study.TypeOfTrialList.Count(), 2);
            Assert.IsTrue(study.TypeOfTrialList.Contains("type_of_clinical_trial_active_treatment_control_fl") && study.TypeOfTrialList.Contains("type_of_clinical_trial_crossover_fl"));
            Assert.AreEqual(study.ClincialTrialRegistrationStatus, "SomeClinicalTrialsRegistrationStatus");
            Assert.AreEqual(study.TrialRegistrationNumber, "SomeTrialRegistrationNumber");
            Assert.AreEqual(study.SourceUseOfDataList.Count(), 1);
            Assert.IsTrue(study.SourceUseOfDataList.Contains("study_data_source_health_care_fl"));
            Assert.AreEqual(study.DataCollectionWithoutParticipantContactList.Count(), 1);
            Assert.IsTrue(study.DataCollectionWithoutParticipantContactList.Contains("study_data_types_human_biological_specimens_fl"));
            Assert.AreEqual(study.ObservationalEthnographic, "Yes");
            Assert.AreEqual(study.ScreenRecruitTarget, "1");
            Assert.AreEqual(study.PopulationTypeList.Count(), 1);
            Assert.IsTrue(study.PopulationTypeList.Contains("none"));
            Assert.AreEqual(study.NonEnglishSpeakers, "Yes");
            Assert.AreEqual(study.PotentialBenefitsToSociety, "SomePotentialBenefitsToSociety");
            Assert.AreEqual(study.DSMBRequired, "Yes");
            Assert.AreEqual(study.OverseeingStudyList.Count(), 1);
            Assert.IsTrue(study.OverseeingStudyList.Contains("be_responsible_for_overseeing_the_study_safety_formally_constituted_dsmb_fl"));
            Assert.AreEqual(study.AncillaryCommitteeReviewList.Count(), 1);
            Assert.IsTrue(study.AncillaryCommitteeReviewList.Contains("requires_ancillary_committee_review_escro_fl"));
            Assert.AreEqual(study.DataCoordinatingCenter, "SomeDataCoordinatingCenter");
            Assert.AreEqual(study.CoverageAnalysisRequired, "Yes");
            Assert.AreEqual(study.ProceduresMinimalRiskList.Count(), 1);
            Assert.IsTrue(study.ProceduresMinimalRiskList.Contains("procedures_greater_than_minimal_risk_be_conducted_ctrc_fl"));
            Assert.AreEqual(study.HipaaAuthorizationList.Count(), 1);
            Assert.IsTrue(study.HipaaAuthorizationList.Contains("hipaa_a_total_waiver_of_hipaa_research_authorization_fl"));
            Assert.AreEqual(study.InstitutionName.Count(), 1);
            Assert.IsTrue(study.InstitutionName.Contains("study_location_ucla_sites_fl"));
            Assert.AreEqual(study.OtherSites.Count(), 2);
            Assert.IsTrue(study.OtherSites.Contains("Site1") && study.OtherSites.Contains("Site2"));
            Assert.AreEqual(study.UpdatedDocuments.Count(), 1);
            Assert.IsTrue(study.UpdatedDocuments.Contains("UpdatedDocument_protocol_fl"));
            Assert.AreEqual(study.HumanitarianUseOfDevice, "Yes");
            Assert.AreEqual(study.DeviceExpandedAccessStatus.Count(), 1);
            Assert.IsTrue(study.DeviceExpandedAccessStatus.Contains("none"));
            Assert.AreEqual(study.CompassionateUseOfDrugBiologic, "Yes");
            Assert.AreEqual(study.DrugBiologicExpandedAccessStatus.Count(), 1);
            Assert.IsTrue(study.DrugBiologicExpandedAccessStatus.Contains("none"));
            Assert.AreEqual(study.BillingDesignation, "No");
            Assert.AreEqual(study.NonInterventional, "No");
            Assert.AreEqual(study.RequiresHealthSystemResources, "Yes");
            Assert.AreEqual(study.ClinicalPreReviewInvestigatorInitiated, "Yes");
            Assert.AreEqual(study.ProtocolUCLAHomeDepartment, "SomeUclaHomeDepartment");
        }

        /// <summary>
        /// Verify ProtocolPI for SC1
        /// </summary>
        [TestMethod]
        public void VerifyProtocolPIForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Assert.AreEqual(study.ProtocolPI.UID, "123456789");
            Assert.AreEqual(study.ProtocolPI.FirstName, "Bob");
            Assert.AreEqual(study.ProtocolPI.MiddleName, "B");
            Assert.AreEqual(study.ProtocolPI.LastName, "FakeInvestigator");
            Assert.AreEqual(study.ProtocolPI.EmailAddress, "SomeEmailAddress");
            Assert.AreEqual(study.ProtocolPI.IrbSystemUniqueId, 1);
        }


        /// <summary>
        /// Verify StudyContact for SC1
        /// </summary>
        [TestMethod]
        public void VerifyStudyContactForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Assert.AreEqual(study.StudyContact.UID, null);
            Assert.AreEqual(study.StudyContact.FirstName, "John");
            Assert.AreEqual(study.StudyContact.MiddleName, "M");
            Assert.AreEqual(study.StudyContact.LastName, "Smith");
            Assert.AreEqual(study.StudyContact.EmailAddress, "SomeEmailAddress");
            Assert.AreEqual(study.StudyContact.IrbSystemUniqueId, 1);
        }

        /// <summary>
        /// Verify FacultySponsor For SC1
        /// </summary>
        [TestMethod]
        public void VerifyFacultySponsorForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Assert.AreEqual(study.FacultySponsor.UID, null);
            Assert.AreEqual(study.FacultySponsor.FirstName, "SomeFirstName");
            Assert.AreEqual(study.FacultySponsor.LastName, "SomeLastName");
            Assert.AreEqual(study.FacultySponsor.IrbSystemUniqueId, 1);

        }

        /// <summary>
        /// Verify KeyPersonnel data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyKeyPersonnelForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Services.WebIRBCRMS.Models.Person person = study.KeyPersonnel.FirstOrDefault();
            Assert.AreEqual(study.KeyPersonnel.Count(), 2);
            Assert.AreEqual(person.FirstName, "John");
            Assert.AreEqual(person.MiddleName, "M");
            Assert.AreEqual(person.LastName, "Smith");
            Assert.AreEqual(person.UID, null);
            Assert.IsNotNull(person.Institution);
            Assert.AreEqual(person.HomeDepartmentTitle, "SomeInstitution");
            Assert.AreEqual(person.HomeLocationCode, '4');
            Assert.AreEqual(person.EmailAddress, "SomeEmailAddress");
            Assert.AreEqual(person.AccessRoles.Count(), 2);
            Assert.IsTrue(person.AccessRoles.Contains("role_co_investigator_fl") && person.AccessRoles.Contains("role_other_fl"));
            Assert.AreEqual(person.OtherRole, "Yes");
            Assert.AreEqual(person.WillObtainConsent, "Yes");
            Assert.AreEqual(person.ManageDeviceAccountability, "Yes");
            Assert.AreEqual(person.AccessToPersonallyIndentifiableInfo, "Yes");
            Assert.AreEqual(person.AccessToCodeKey, "Yes");
            Assert.AreEqual(person.IrbSystemUniqueId, 1);
        }

        /// <summary>
        /// Verify InvestigationalDrugs data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyInvestigationalDrugsForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Services.WebIRBCRMS.Models.Drug drug = study.InvestigationalDrugs.FirstOrDefault();
            Assert.AreEqual(study.InvestigationalDrugs.Count(), 2);
            Assert.AreEqual(drug.Exempt, "SomeExempt");
            Assert.AreEqual(drug.HolderName, "SomeHolderName");
            Assert.AreEqual(drug.ID, "SomeId");
            Assert.IsNotNull(drug.SubmitToFDADate);
            Assert.IsNotNull(drug.FDAApprovalDate);
            Assert.AreEqual(drug.IrbSystemUniqueId, 1);
        }

        /// <summary>
        /// Verify InvestigationalDevices data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyInvestigationalDevicesForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Services.WebIRBCRMS.Models.Device device = study.InvestigationalDevices.FirstOrDefault();
            Assert.AreEqual(study.InvestigationalDevices.Count(), 2);
            Assert.AreEqual(device.Exempt, "SomeExempt");
            Assert.AreEqual(device.HolderName, "SomeHolderName");
            Assert.AreEqual(device.ID, "SomeId");
            Assert.AreEqual(device.Risk, "SomeRisk");
            Assert.IsNotNull(device.SubmitToFDADate);
            Assert.IsNotNull(device.FDAApprovalDate);
            Assert.AreEqual(device.IrbSystemUniqueId, 1);
        }

        /// <summary>
        /// Verify ProtocolActivities data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyProtocolActivitiesForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.FirstOrDefault();
            Services.WebIRBCRMS.Models.ProtocolActivity protocolactivity = study.ProtocolActivities.First();
            Assert.AreEqual(study.ProtocolActivities.Count(), 4);
            Assert.AreEqual(protocolactivity.ID, 1);
            Assert.AreEqual(protocolactivity.ActionStatus, "SomeActionStatus");
            Assert.IsNotNull(protocolactivity.ActionStatusDate);
            Assert.AreEqual(protocolactivity.IrbSystemUniqueId, 1);
        }

        /// <summary>
        /// Verify Sponsorship data for SC1
        /// </summary>
        [TestMethod]
        public void VerifySponsorShipForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.First();
            Services.WebIRBCRMS.Models.Award award = study.Sponsorship.FirstOrDefault();
            Assert.IsTrue(study.Sponsorship.Count() > 0);
            Assert.IsNotNull(award.SponsorUclaCode);
            Assert.IsNotNull(award.SponsoredProjectTitle);
            Assert.IsNotNull(award.SponsorName);
            Assert.IsNotNull(award.PrincipalInvestigator.EmailAddress);
            Assert.IsNotNull(award.PrincipalInvestigator.FirstName);
            Assert.IsNotNull(award.PrincipalInvestigator.MiddleName);
            Assert.IsNotNull(award.PrincipalInvestigator.LastName);
            Assert.IsNotNull(award.PrincipalInvestigator.SponsorUclaCode);
            Assert.IsNotNull(award.PrincipalInvestigator.HomeDepartmentCode);
            Assert.IsNotNull(award.PrincipalInvestigator.HomeLocationCode);
            Assert.IsNotNull(award.PrincipalInvestigator.Institution);
            Assert.IsNotNull(award.PrincipalInvestigator.HomeDepartmentTitle);
            Assert.IsNotNull(award.Fund);
        }

        /// <summary>
        /// Verify Amendment data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyAmendmentForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.First();
            Services.WebIRBCRMS.Models.Amendment amendment = study.Amendments.FirstOrDefault();
            Assert.AreEqual(study.Amendments.Count(), 2);
            Assert.AreEqual(amendment.AmendmentKey, 1);
            Assert.AreEqual(amendment.AmendmentNumber, "a1");
            Assert.AreEqual(amendment.AmendmentShortTitle, "SomeShortTitle");
            Assert.IsTrue(amendment.ChangeInStaffList.Contains("none"));
            Assert.AreEqual(amendment.PIChangeReason, "SomePIChangeReason");
            Assert.IsTrue(amendment.AmendmentDetailMinorList.Contains("minor_changes_clarification_or_technical_change_fl") && amendment.AmendmentDetailMinorList.Contains("minor_changes_minor_increas_decrease_in_number_of_study_participants_fl"));
            Assert.IsTrue(amendment.AmendmentDetailMajorList.Contains("major_changes_change_in_study_design_fl") && amendment.AmendmentDetailMajorList.Contains("major_changes_change_in_status_of_study_participants_fl"));
            Assert.AreEqual(amendment.AmendmentDetailOther, "SomeAmendmentDetailOther");
            Assert.AreEqual(amendment.Modifications, "SomeModifications");
            Assert.AreEqual(amendment.ParticipantsEnrolled, "Yes");
            Assert.AreEqual(amendment.Reconsent, "Yes");
            Assert.AreEqual(amendment.CompletedNotification, "Yes");
            Assert.AreEqual(amendment.AssignedReviewType, "SomeAssignedReviewType");
            Assert.AreEqual(amendment.RadiationProcedures, "SomeRadiationProcedures");
            Assert.IsTrue(amendment.UpdatedDocuments.Contains("study_in_crms_assent_form_fl") && amendment.UpdatedDocuments.Contains("study_in_crms_informed_consent_form_fl"));
            Assert.AreEqual(amendment.AssignedCommittee, "SomeCommitteeName");
            Assert.AreEqual(amendment.ProtocolRequiresCalendarRevisions, "Yes");
            Assert.AreEqual(amendment.IrbSystemUniqueId, study.IrbSystemUniqueId);
        }


        /// <summary>
        /// Verify Continuing Review data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyContinuingReviewForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.First();
            Services.WebIRBCRMS.Models.ContinuingReview cr = study.ContinuingReviews.FirstOrDefault();
            Assert.AreEqual(study.ContinuingReviews.Count(), 2);
            Assert.AreEqual(cr.ContinueReviewKey, 1);
            Assert.AreEqual(cr.ContinueReviewReportType, "SomeReportType");
            Assert.AreEqual(cr.CurrentStatusOfTheStudyEnrollment,"SomeCurrentStatusOfTheStudyEnrollment");
            Assert.AreEqual(cr.ReasonForClosingStudyAtUCLA, "ReasonForClosingStudyAtUCLA");
            Assert.AreEqual(cr.AssignedReviewType, "SomeAssignedReviewType");
            Assert.AreEqual(cr.IrbSystemUniqueId, study.IrbSystemUniqueId);
        }

        /// <summary>
        /// Verify PAR data for SC1
        /// </summary>
        [TestMethod]
        public void VerifyPARForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today.AddDays(-2));
            List<Study> studies = ((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.ToList();
            Study study = studies.First();
            Services.WebIRBCRMS.Models.PostApprovalReport par = study.PostApprovalReports.FirstOrDefault();
            Assert.AreEqual(study.PostApprovalReports.Count(), 2);
            Assert.AreEqual(par.PostApprovalReportKey, 1);
            Assert.AreEqual(par.PostApprovalReportType, "SomeReportType");
            Assert.AreEqual(par.InvestigatorBrochureUpdate, "Yes");
            Assert.AreEqual(par.AssignedReviewType, "SomeAssignedReviewType");
            Assert.AreEqual(par.IrbSystemUniqueId, study.IrbSystemUniqueId);
        }

        [TestMethod]
        public void GetStudyStateHistorybyStartDate_ShouldReturnStudyStateHistory()
        {
            IHttpActionResult result = _testStatusController.Get(DateTime.Today.AddDays(-11));
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<StudyStatusHistory>>));
            List<StudyStatusHistory> StudyStatusHistories = ((OkNegotiatedContentResult<IQueryable<StudyStatusHistory>>)(result)).Content.ToList();
            Assert.AreEqual(((OkNegotiatedContentResult<IQueryable<StudyStatusHistory>>)(result)).Content.Count(), _numberToFake);
        }

        [TestMethod]
        public void GetStudyStateHistorybyDates_ShouldReturnStudyStateHistory()
        {
            IHttpActionResult result = _testStatusController.Get(DateTime.Today.AddDays(-2), DateTime.Today);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<StudyStatusHistory>>));
            Assert.AreEqual(((OkNegotiatedContentResult<IQueryable<StudyStatusHistory>>)(result)).Content.Count(), _numberToFake);
        }

        [TestMethod]
        public void GetPreSubmissions_ShouldReturnsStudies()
        {
            IHttpActionResult result = _testPreSubmissionController.Get();
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<Study>>));
            Assert.AreEqual(((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.Count(), _numberToFake);
        }

        [TestMethod]
        public void GetAmendments_ShouldReturnsStudies()
        {
            IHttpActionResult result = _testAmendmentController.Get();
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<Study>>));
            Assert.AreEqual(((OkNegotiatedContentResult<IQueryable<Study>>)(result)).Content.Count(), _numberToFake);
        }

        [TestMethod]
        public void TestGetValidUID()
        {
            string UID;
            UID = Shared.SharedFunctions.GetValidUID(null);
            Assert.IsNull(UID, "UID is not returned as null if input is null");
            UID = Shared.SharedFunctions.GetValidUID("34343");
            Assert.IsNull(UID, "UID is not returned as null if input is not a valid UID");
            UID = Shared.SharedFunctions.GetValidUID("A23");
            Assert.IsNull(UID, "UID is not returned as null if input is not a valid UID");
            UID = Shared.SharedFunctions.GetValidUID("123456789");
            Assert.IsTrue(UID.Equals("123456789"));
        }

    }
}
