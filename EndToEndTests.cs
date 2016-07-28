using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORA.Services.WebIRBCRMS;
using ORA.Services.WebIRBCRMS.Controllers;
using ORA.Services.WebIRBCRMS.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Configuration;

namespace ORA.Services.Tests
{
    [TestClass]
    public class EndToEndTests
    {
        protected static HttpClient client = new HttpClient();
        protected static List<MediaTypeFormatter> formatters = new List<MediaTypeFormatter>();
        [TestInitialize]
        public void Initialize()
        {
            client.Timeout = new TimeSpan(0, 30, 0);
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseAddress"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            formatters.Add(new JsonMediaTypeFormatter());
        }

        #region SC1
        /// <summary>
        /// Testing SC1 with missing apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestMissingAPIKeyForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/2015").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC1 with an invalid apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestInvalidAPIKeyForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/2015&apikey=123").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC1 with a valid apikey but with no role assigned, expecting a Unauthorized response
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyButWithoutRoleForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/2015&apikey=B35191E3-4B05-4F6F-9A34-F251E7351F2B").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Testing SC1 with a valid apikey, expecting an OK reponse
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyForSC1()
        {
            HttpResponseMessage response = client.GetAsync(string.Format("api/IrbRecordUpdatesForCrms?startdate={0}&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8", DateTime.Today.ToShortDateString())).Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Test the IrbSystemUniqueId returned by SC1 are unique
        /// </summary>
        [TestMethod]
        public void TestUniqueIrbSystemUniqueIdForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$select=IrbSystemUniqueId").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == studies.Select(x => x.IrbSystemUniqueId).Distinct().Count());

        }

        /// <summary>
        /// Test the Pre Submission Filter for SC1, expects no Pre Submission study returns
        /// </summary>
        [TestMethod]
        public void TestPreSubmissionFilterForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=startswith(IrbNumber,'PRE')").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }

        /// <summary>
        /// Test Malformed IRBNumber for SC1, expects no study returns
        /// </summary>
        [TestMethod]
        public void TestMalformedIRBNumberForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=contains(IrbNumber,'MS')").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }

        /// <summary>
        /// Test the IrbNumbers returned by SC1 are unique
        /// </summary>
        [TestMethod]
        public void TestUniqueIRBNumberForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$select=IrbNumber").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == studies.Select(x => x.IrbNumber).Distinct().Count());
        }

        #endregion SC1

        #region SC2
        /// <summary>
        /// Testing SC2 with missing apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestMissingAPIKeyForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2015").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC2 with an invalid apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestInvalidAPIKeyForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2015&apikey=123").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC2 with a valid apikey but with no role assigned, expecting a Unauthorized response
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyButWithoutRoleForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2015&apikey=B35191E3-4B05-4F6F-9A34-F251E7351F2B").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Testing SC2 with a valid apikey, expecting an OK reponse
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyForSC2()
        {
            HttpResponseMessage response = client.GetAsync(string.Format("api/IrbStudyStatusHistoryUpdatesForCrms?startdate={0}&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8", DateTime.Today.ToShortDateString())).Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Test the IrbSystemUniqueId returned by SC2 are unique
        /// </summary>
        [TestMethod]
        public void TestUniqueIrbSystemUniqueIdForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$select=IrbSystemUniqueId").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            int count1 = StudyStatusHistories.Count();
            int count2 = StudyStatusHistories.Select(x => x.IrbSystemUniqueId).Distinct().Count();
            var query = from s in StudyStatusHistories
                        group s by new { s.IrbSystemUniqueId } into g
                        where g.Count() > 1
                        select g.Key.ToString();
            List<string> d = query.ToList<string>();
            Assert.IsTrue(count1 == count2);
        }

        /// <summary>
        /// Test the IrbNumbers returned by SC2 are unique
        /// </summary>
        [TestMethod]
        public void TestUniqueIRBNumberForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$select=IrbNumber").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            int count1 = StudyStatusHistories.Count();
            int count2 = StudyStatusHistories.Select(x => x.IrbNumber).Distinct().Count();
            var query = from s in StudyStatusHistories
                        group s by new { s.IrbNumber } into g
                        where g.Count() > 1
                        select g.Key.ToString();
            List<string> d = query.ToList<string>();
            Assert.IsTrue(count1 == count2);
        }

        /// <summary>
        /// Test the Malformed IRBNumber for SC2, expects no StudyStatusHistory returns
        /// </summary>
        [TestMethod]
        public void TestMalformedIRBNumberForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=contains(IrbNumber,'MS')").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            Assert.IsTrue(StudyStatusHistories.Count() == 0);
        }

        /// <summary>
        /// Test AmendmentActivities should always have AmendmentKey for SC2
        /// </summary>
        [TestMethod]
        public void TestAmendmentActivitiesForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2014&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            var query = from s in StudyStatusHistories
                        from a in s.ExternalStatusChanges
                        where a.Type == "Amendment Activity"
                        select a;
            Assert.IsTrue(query.Count() > 0);
            var query2 = from a in query
                         where a.AmendmentKey == 0
                         select a;
            Assert.IsTrue(query2.Count() == 0);
        }

        /// <summary>
        /// Test ContinuingReviewActivities should always have ContinuingReviewKey for SC2
        /// </summary>
        [TestMethod]
        public void TestContinuingReviewActivitiesForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2014&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            var query = from s in StudyStatusHistories
                        from a in s.ExternalStatusChanges
                        where a.Type == "Continuing Review Activity"
                        select a;
            Assert.IsTrue(query.Count() > 0);
            var query2 = from a in query
                         where a.ContinueReviewKey == 0
                         select a;
            Assert.IsTrue(query2.Count() == 0);
        }

        /// <summary>
        /// Test PostApprovalReportActivities should always have PostApprovalReportKey for SC2
        /// </summary>
        [TestMethod]
        public void TestPARActivitiesForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2014&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            var query = from s in StudyStatusHistories
                        from a in s.ExternalStatusChanges
                        where a.Type == "Post Approval Report Activity"
                        select a;
            Assert.IsTrue(query.Count() > 0);
            var query2 = from a in query
                         where a.PostApprovalReportKey == 0
                         select a;
            Assert.IsTrue(query2.Count() == 0);
        }

        /// <summary>
        /// Test the External Status translation for 'Copying Original Version'
        /// </summary>
        [TestMethod]
        public void TestStatusTranslationForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=ExternalStatusChanges/any(e:e/ExternalStatus eq 'Copying Original Version')").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            Assert.AreEqual(StudyStatusHistories.Count(), 0);
        }

        /// <summary>
        /// Each StudyStatusHistory should have at least one External Status Changes
        /// </summary>
        [TestMethod]
        public void TestNumberOfExternalStatusChangesForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2014&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            Assert.IsTrue(StudyStatusHistories.Where(s => s.ExternalStatusChanges.Count() == 0).Count() == 0);
        }

        /// <summary>
        /// Test the ExternalStatusDate are within the date window specified
        /// </summary>
        [TestMethod]
        public void TestExternalStatusDateForSC2()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbStudyStatusHistoryUpdatesForCrms?startdate=1/1/2014&enddate=1/1/2015&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<StudyStatusHistory> StudyStatusHistories = response.Content.ReadAsAsync<IEnumerable<StudyStatusHistory>>(formatters).Result;
            var query = from s in StudyStatusHistories
                        from e in s.ExternalStatusChanges
                        where e.ExternalStatusDate < new DateTime(2014, 1, 1) || e.ExternalStatusDate > new DateTime(2015, 1, 1)
                        select e;
            Assert.AreEqual(query.Count(), 0);
        }
        #endregion SC2

        #region SC3

        #endregion SC3

        #region SC4
        /// <summary>
        /// Testing SC4 with missing apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestMissingAPIKeyForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?startdate=1/1/2015").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC4 with an invalid apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestInvalidAPIKeyForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?startdate=1/1/2015&apikey=123").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC4 with a valid apikey but with no role assigned, expecting a Unauthorized response
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyButWithoutRoleForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?startdate=1/1/2015&apikey=B35191E3-4B05-4F6F-9A34-F251E7351F2B").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Testing SC4 with a valid apikey, expecting an OK reponse
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyForSC4()
        {
            HttpResponseMessage response = client.GetAsync(string.Format("api/IrbAmendmentUpdatesForCrms?startdate={0}&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8", DateTime.Today.ToShortDateString())).Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Test the Pre Submission Filter for SC4, expects no Pre Submission study returns
        /// </summary>
        [TestMethod]
        public void TestPreSubmissionFilterForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=startswith(IrbNumber,'PRE')").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }

        /// <summary>
        /// Test the IrbNumbers returned by SC4 are unique
        /// </summary>
        [TestMethod]
        public void TestUniqueIRBNumberForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$select=IrbNumber").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == studies.Select(x => x.IrbNumber).Distinct().Count());
        }
        #endregion SC4
    }
}

