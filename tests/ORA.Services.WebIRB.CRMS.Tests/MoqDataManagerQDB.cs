using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ORA.Data;
using ORA.Domain.Model;
using ORA.Domain.Model.HumanSubjects;

namespace ORA.Services.WebIRBCRMS.Tests
{
    public class MoqDataManagerQDB : Mock<IFundsDao>
    {
        public MoqDataManagerQDB()
        {
            this.Setup(x => x.GetBaseFaus(It.IsAny<Project>())).Returns(fakeFAUKeys);
            this.Setup(x => x.GetBaseFundAccountingUnitKeyByProjects(It.IsAny<List<Project>>())).Returns(FakeFAUKeyLookup);
        }

        #region Setup_Fake_FAU_Data
        private static List<FullAccountingUnitKey> fakeFAUKeys = new List<FullAccountingUnitKey>()
        {
           fake_FAUKey_1(),
        };

        private static FullAccountingUnitKey fake_FAUKey_1()
        {
            return new FullAccountingUnitKey()
            {
                    LocationCode = "4",
                    FundNumber = "12345",
                    FundBeginDate = DateTime.Today,
                    CostCenterCode = "AB",
                    AccountNumber = "123456"
            };
        }

        private static FullAccountingUnitKey fake_FAUKey_2()
        {
            return new FullAccountingUnitKey()
            {
                    LocationCode = "4",
                    FundNumber = "54321",
                    FundBeginDate = DateTime.Today,
                    CostCenterCode = "AB",
                    AccountNumber = "654321"
            };
        }

        private static ILookup<Tuple<string,string,string>, Domain.Model.FullAccountingUnitKey> FakeFAUKeyLookup()
        {
            var dataset = new List<Tuple<string, string, string, Domain.Model.FullAccountingUnitKey>>();
            dataset.Add(new Tuple<string,string,string,FullAccountingUnitKey>("4","12345","abc",fake_FAUKey_1()));
            dataset.Add(new Tuple<string,string,string,FullAccountingUnitKey>("4","54321","def",fake_FAUKey_2()));
            return dataset.ToLookup(f => new Tuple<string,string,string>(f.Item1,f.Item2,f.Item3), f => f.Item4);
        }
        #endregion

    }
}
