using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ORA.Services.WebIRB.CRMS.Tests
{
    [TestClass]
    public class EndToEndTests
    {
        protected static HttpClient client = new HttpClient();
        protected static List<MediaTypeFormatter> formatters = new List<MediaTypeFormatter>();
        protected static bool httpSetup = false;
        [TestInitialize]
        public void Initialize()
        {
            if (!httpSetup)
            {
            client.Timeout = new TimeSpan(0, 30, 0);
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseAddress"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpSetup = true;
            }
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

         /// <summary>
        /// Test the StudyContact returned by SC1 and make sure they have valid UID
        /// </summary>
        [TestMethod]
        public void TestStudyContactHasValidUIDForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=StudyContact ne null&$top=100").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            foreach (Study s in studies)
            {
                Assert.IsTrue(string.IsNullOrEmpty(s.StudyContact.UID) || Regex.IsMatch(s.StudyContact.UID, @"^[0-9]{9}$"));
            }
        }

        /// <summary>
        /// Test the FacultySponsor returned by SC1 and make sure they have UID
        /// </summary>
        [TestMethod]
        public void TestFacultySponsorHasUIDForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=FacultySponsor ne null&$top=10").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            foreach (Study s in studies)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(s.FacultySponsor.UID));
            }
        }

        /// <summary>
        /// Test InvestigationDrugs returned by SC1, expected some data
        /// </summary>
        [TestMethod]
        public void TestInvestigationalDrugsForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=4/13/2015&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=InvestigationalDrugs/any(d:d/ID ne null)&$top=10").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() > 0);
        }

        /// <summary>
        /// Test FAU for SC1
        /// </summary>
        [TestMethod]
        public void TestFAUsForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&$filter=Sponsorship/any(s:s/SponsorUclaCode ne null)&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$top=100").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() > 0);
            foreach (Study s in studies)
            {
                foreach (Award a in s.Sponsorship)
                {
                    Assert.IsNotNull(a.Fund.LocationCode, string.Format("Missing Fund.LocationCode for Study:{0}", s.IrbSystemUniqueId));
                    if (a.Fund.FundNumber != null && a.Fund.FundNumber.Length > 0)
                    {
                        Assert.IsTrue(a.Fund.FundNumber.Length == 5, string.Format("Invalid Fund Number Length for Study:{0}", s.IrbSystemUniqueId));
                        Assert.IsNotNull(a.Fund.FundBeginDate, "Missing Fund.FundBeginDate");
                        Assert.IsNotNull(a.Fund.FundEndDate, "Missing Fund.FundEndDate");
                        if (a.Fund.FundNumber != "99999")
                        {
                            Assert.IsTrue(a.Fund.FAUs != null, string.Format("Fund Number:{0} is missing Fund.FAUs", a.Fund.FundNumber));
                            foreach (FullAccountingUnit f in a.Fund.FAUs)
                            {
                                Assert.AreEqual(a.Fund.FundNumber, f.FundNumber, "Incorrect FAU Merging");
                                Assert.AreEqual(f.Account.Length, 6);
                                Assert.IsTrue(f.CostCenter.Trim() == "" || f.CostCenter.Length == 2);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test Invalid UID in KeyPersonnel for SC1, expecting UID is either a valid one or null
        /// </summary>
        [TestMethod]
        public void TestValidateUIDForSC1()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbRecordUpdatesForCrms?startdate=1/1/1900&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=KeyPersonnel/any(p:p/UID ne null and length(p/UID) ne 9)").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
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
        /// <summary>
        /// Testing SC3 with missing apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestMissingAPIKeyForSC3()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbPreSubmissionUpdatesForCrms").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC3 with an invalid apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestInvalidAPIKeyForSC3()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbPreSubmissionUpdatesForCrms?apikey=123").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC3 with a valid apikey but with no role assigned, expecting a Unauthorized response
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyButWithoutRoleForSC3()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbPreSubmissionUpdatesForCrms?apikey=B35191E3-4B05-4F6F-9A34-F251E7351F2B").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Testing SC3 with a valid apikey, expecting an OK reponse
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyForSC3()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbPreSubmissionUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Test the Pre Submission Filter for SC3, expects only Pre Submission study returns
        /// </summary>
        [TestMethod]
        public void TestPreSubmissionFilterForSC3()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbPreSubmissionUpdatesForCrms?&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=not(startswith(IrbNumber,'PRE'))").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }
        
        /// <summary>
        /// Test ClinicalPreReviewInvestigatorInitiated field for SC#, expects equal to Yes
        /// </summary>
        [TestMethod]
        public void TestClinicalPreReviewInvestigatorInitiatedForSC3()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbPreSubmissionUpdatesForCrms?&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=ClinicalPreReviewInvestigatorInitiated ne 'Yes'").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }

        #endregion SC3

        #region SC4
        /// <summary>
        /// Testing SC4 with missing apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestMissingAPIKeyForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC4 with an invalid apikey, expecting a forbidden response
        /// </summary>
        [TestMethod]
        public void TestInvalidAPIKeyForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=123").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Testing SC4 with a valid apikey but with no role assigned, expecting a Unauthorized response
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyButWithoutRoleForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=B35191E3-4B05-4F6F-9A34-F251E7351F2B").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == false);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Testing SC4 with a valid apikey, expecting an OK reponse
        /// </summary>
        [TestMethod]
        public void TestValidAPIKeyForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Test the Pre Submission Filter for SC4, expects no Pre Submission study returns
        /// </summary>
        [TestMethod]
        public void TestPreSubmissionFilterForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?&apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=startswith(IrbNumber,'PRE')").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }


        /// <summary>
        /// Test the StudyContact returned by SC4 and make sure they have valid UID
        /// </summary>
        [TestMethod]
        public void TestStudyContactHasValidUIDForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=StudyContact ne null&$top=10").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            foreach (Study s in studies)
            {
                Assert.IsTrue(string.IsNullOrEmpty(s.StudyContact.UID) || Regex.IsMatch(s.StudyContact.UID, @"^[0-9]{9}$"));
            }
        }

        /// <summary>
        /// Test InvestigationDrugs returned by SC4, expected some data
        /// </summary>
        [TestMethod]
        public void TestInvestigationalDrugsForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=InvestigationalDrugs/any(d:d/ID ne null)&$top=10").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() > 0);
        }

        /// <summary>
        /// Test FAU returned by SC4 
        /// </summary>
        [TestMethod]
        public void TestFAUsForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=Sponsorship/any(s:s/SponsorUclaCode ne null)&$top=100").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() > 0);
            foreach (Study s in studies)
            {
                foreach (Award a in s.Sponsorship)
                {
                    Assert.IsNotNull(a.Fund.LocationCode, string.Format("Missing Fund.LocationCode for Study:{0}", s.IrbSystemUniqueId));
                    if (a.Fund.FundNumber != null && a.Fund.FundNumber.Length > 0)
                    {
                        Assert.IsTrue(a.Fund.FundNumber.Length == 5, string.Format("Invalid Fund Number Length for Study:{0}", s.IrbSystemUniqueId));
                        Assert.IsNotNull(a.Fund.FundBeginDate, "Missing Fund.FundBeginDate");
                        if (a.Fund.FundNumber != "99999")
                        {
                            Assert.IsTrue(a.Fund.FAUs != null, string.Format("Fund Number:{0} is missing Fund.FAUs", a.Fund.FundNumber));
                            foreach (FullAccountingUnit f in a.Fund.FAUs)
                            {
                                Assert.AreEqual(a.Fund.FundNumber, f.FundNumber, "Incorrect FAU Merging");
                                Assert.AreEqual(f.Account.Length, 6);
                                Assert.IsTrue(f.CostCenter.Trim() == "" || f.CostCenter.Length == 2);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test Invalid UID in KeyPersonnel for SC4, expecting UID is either a valid one or null
        /// </summary>
        [TestMethod]
        public void TestValidateUIDForSC4()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=KeyPersonnel/any(p:p/UID ne null and length(p/UID) ne 9)").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }

        /// <summary>
        /// Test SC4 Returns At least One Project Type of Amendment or CR or PAR
        /// </summary>
        [TestMethod]
        public void TestSC4ReturnsAtleastOneProjectType()
        {
            HttpResponseMessage response = client.GetAsync("api/IrbAmendmentUpdatesForCrms?apikey=E8592165-F13E-4EE6-88E1-677BE21BF5F8&$filter=not (Amendments/any(a:a/IrbSystemUniqueId ne null)) and not (ContinuingReviews/any(c:c/IrbSystemUniqueId ne null)) and not (PostApprovalReports/any(p:p/IrbSystemUniqueId ne null))").Result;
            Assert.IsTrue(response.IsSuccessStatusCode == true);
            IEnumerable<Study> studies = response.Content.ReadAsAsync<IEnumerable<Study>>(formatters).Result;
            Assert.IsTrue(studies.Count() == 0);
        }

        #endregion SC4

    }
}

