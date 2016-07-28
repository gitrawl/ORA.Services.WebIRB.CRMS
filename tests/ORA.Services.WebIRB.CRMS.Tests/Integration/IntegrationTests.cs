using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORA.Services.WebIRBCRMS;
using ORA.Services.WebIRBCRMS.Controllers;
using ORA.Services.WebIRBCRMS.Models;
using Moq;
using Newtonsoft.Json;
using System.Web.Http.Results;
using System.Web.Http;
using ORA.Domain.Model.HumanSubjects;
using ORA.Domain.Model;
using ORA.Data.WebIrb.Staging;
using ORA.Data.Warehouse;
using ORA.Data;

namespace ORA.Services.Tests.IrbCrmsApi
{
    [TestClass]
    public class IntegrationTests
    {
        protected static IrbRecordUpdatesForCrmsController _testRecordController;
        protected static IrbStudyStatusHistoryUpdatesForCrmsController _testStatusController;
        protected static IrbAmendmentUpdatesForCrmsController _testAmendmentController;

        private DateTime _startDate;
        private DateTime _endDate;
        private IDataManagerWebIrbStaging _dataManager;

        public class ExceptionalDataManagerIrbCrms : Mock<IDataManagerWebIrbStaging>
        {
            public ExceptionalDataManagerIrbCrms()
            {
                this.Setup(foo => foo.GetStudyByUniqueId(It.IsAny<int>())).Throws(new Exception("GetStudyByUniqueId exception"));
                this.Setup(foo => foo.GetStudyChangesByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Throws(new Exception("GetStudyChangesByDateRange exception"));
                this.Setup(foo => foo.GetStudyStatusHistoryByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Throws(new Exception("GetStudyStatusHistoryByDateRange exception"));
                this.Setup(foo => foo.GetAmendmentsInProcessByDateRange()).Throws(new Exception("GetAmendmentsInProcessByDateRange exception"));
            }
        }

        public class ExceptionalIrbCgDataManager : Mock<IProjectDao>
        {
            public ExceptionalIrbCgDataManager()
            {
                this.Setup(foo => foo.GetStudiesSponsoredFunding(It.IsAny<ICollection<ORA.Domain.Model.HumanSubjects.Study>>())).Throws(new Exception("GetStudiesSponsoredFunding exception"));
                this.Setup(foo => foo.GetProjectByIrbNumbers(It.IsAny<ICollection<ORA.Domain.Model.HumanSubjects.Study>>())).Throws(new Exception("GetProjectByIrbNumbers exception"));
            }
        }

        public class ExceptionalFundDataManager : Mock<IFundsDao>
        {
            public ExceptionalFundDataManager()
            {
                this.Setup(foo => foo.GetBaseFaus(It.IsAny<Project>())).Throws(new Exception("GetBaseFaus exception"));
                this.Setup(foo => foo.GetBaseFundAccountingUnitKeyByProjects(It.IsAny<List<Project>>())).Throws(new Exception("GetBaseFundAccountingUnitKeyByProjects exception"));
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            var moqDm1 = new ExceptionalDataManagerIrbCrms();
            var moqDm2 = new ExceptionalIrbCgDataManager();
            var moqDm3 = new ExceptionalFundDataManager();
            _testRecordController = new IrbRecordUpdatesForCrmsController(moqDm1.Object, moqDm2.Object, moqDm3.Object);
            _testStatusController = new IrbStudyStatusHistoryUpdatesForCrmsController(moqDm1.Object);
            _testAmendmentController = new IrbAmendmentUpdatesForCrmsController(moqDm1.Object, moqDm2.Object, moqDm3.Object);

            _dataManager = new WebIrbStagingDataManager();
            _startDate = new DateTime(1900, 1, 1);
            _endDate = DateTime.Today;
        }

        /// <summary>
        /// Test exception handling for SC1, expecting internal Server Error
        /// </summary>
        [TestMethod]
        public void TestExceptionHandlingForSC1()
        {
            IHttpActionResult result = _testRecordController.Get(DateTime.Today);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        /// <summary>
        /// Test exception handling for SC2, expecting internal Server Error
        /// </summary>
        [TestMethod]
        public void TestExceptionHandlingForSC2()
        {
            IHttpActionResult result = _testStatusController.Get(DateTime.Today);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        /// <summary>
        /// Test exception handling for SC4, expecting internal Server Error
        /// </summary>
        [TestMethod]
        public void TestExceptionHandlingForSC4()
        {
            IHttpActionResult result = _testAmendmentController.Get();
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));

        }

        [TestMethod]
        public void GetStudyChangesByDateRange_AllDates_ResultContainsHistoricalAndNewPhases()
        {
            IEnumerable<ORA.Domain.Model.HumanSubjects.Study> studies =
                _dataManager.GetStudyChangesByDateRange(_startDate, _endDate);

            var phases = studies.Select(s => s.StudyProtocol.PhaseList);

            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_phase_0_fl)),
                "At least 1 project has historical phase 0 flag.");
            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_phase_i_fl)),
                "At least 1 project has historical phase 1 flag.");
            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_phase_ii_fl)),
                "At least 1 project has historical phase 2 flag.");
            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_phase_iii_fl)),
                "At least 1 project has historical phase 3 flag.");
            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_phase_iv_fl)),
                "At least 1 project has historical phase 4 flag.");
            Assert.IsTrue(
                phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_open_label_extension_fl)),
                "At least 1 project has historical open label extension flag.");
            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_expanded_access_fl)),
                "At least 1 project has historical expanded access flag.");
            Assert.IsTrue(phases.Any(p => p.HasFlag(Phase.historical_phase_of_the_clinical_trial_behavioral_fl)),
                "At least 1 project has historical behavioral flag.");
        }
        [TestMethod]
        public void GetStudyChangesByDateRange_AllDates_ResultContainsAssignedReviewType_AllProjectType()
        {
            IEnumerable<ORA.Domain.Model.HumanSubjects.Study> studies =
               _dataManager.GetStudyChangesByDateRange(_startDate, _endDate);

            var amendments = studies.Select(s => s.Amendments);
            Assert.IsTrue(amendments.Any(p => p.Any(p1 => p1.AssignedReviewType != string.Empty)), "At least 1 Amendment has AssignedReviewType.");

            var continueReviews = studies.Select(s => s.ContinuingReviews);
            Assert.IsTrue(continueReviews.Any(p => p.Any(p1 => p1.AssignedReviewType != string.Empty)), "At least 1 ContinueReview has AssignedReviewType.");

            var postApprovalReports = studies.Select(s => s.PostApprovalReports);
            Assert.IsTrue(postApprovalReports.Any(p => p.Any(p1 => p1.AssignedReviewType != string.Empty)), "At least 1 PostApprovalReport has AssignedReviewType.");


        }

    }
}
