using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using ORA.Domain.Model.HumanSubjects;
using ORA.Domain.Model;
using ORA.Data;
using ORA.Data.WebIrb.Staging;

namespace ORA.Services.WebIRBCRMS.Tests.Unit
{
    [TestClass]
    public class QDBDataManagerUnitTest
    {
        private static IFundsDao moqQDBDm = new MoqDataManagerQDB().Object;
        private static IDataManagerWebIrbStaging moqDmWebIrbStaging = new MoqDataManagerIrbCrms().Object;

        [TestMethod]
        public void TestFAULookup()
        {
            ILookup<Tuple<string, string, string>, FullAccountingUnitKey> FAUKeysLookup = moqQDBDm.GetBaseFundAccountingUnitKeyByProjects(new List<Project> { new Project() });
            Assert.IsTrue(FAUKeysLookup.Count() > 0);
            Assert.IsTrue(FAUKeysLookup[new Tuple<string, string, string>("4", "12345", "abc")].Count() == 1);
            Assert.IsTrue(FAUKeysLookup[new Tuple<string, string, string>("4", "54321", "efg")].Count() == 0);
            Assert.IsTrue(FAUKeysLookup[new Tuple<string, string, string>("4", "54321", "def")].Count() == 1);
        }

        [TestMethod]
        public void TestConverterForMergingFAUs()
        {
            Study study = moqDmWebIrbStaging.GetStudyByUniqueId(0);
            study.Approvals.Add(new IrbApproval()
                {
                    AwardedHumanSubjectsApprovals = new List<HumanSubjectsApproval>() 
                    { 
                        new HumanSubjectsApproval() 
                        {
                            Project = new Project()
                            {
                                InstitutionNumber = "abc"
                            }
                        } 
                    }
                });
                                   
            Assert.IsTrue(study.Approvals != null);
        }
    }
}
