using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;
using Unity.WebApi;
using Unity.Mvc5;
using ORA.Data.WebIrb.Staging;
using ORA.Data.Warehouse;
using ORA.Services.WebIRBCRMS.Interfaces;
using ORA.FakeIRBData;
using ORA.Data;
using ORA.Data.FakeWarehouse;
using ORA.Data.LocalQdb;
using System.Web.Configuration;

namespace ORA.Services.WebIRBCRMS
{
    public static class UnityConfig
    {
        private static UnityContainer container;

        /// <summary>
        /// register all your components with the container here
        /// </summary>
        public static void RegisterComponents()
        {
            container = new UnityContainer();

            if (WebConfigurationManager.AppSettings["UseFakeWebIrbDataManager"].ToString() == "true")
            {
                //Register FakeDataManager
                container.RegisterType<IDataManagerWebIrbStaging, FakeIRBDataManager>();
            }
            else
            {
                //Register DataManagerIrbCrmsStaging
                container.RegisterType<IDataManagerWebIrbStaging, WebIrbStagingDataManager>();
            }

            if (WebConfigurationManager.AppSettings["UseMockedProjectDao"].ToString() == "true")
            {
                container.RegisterType<IProjectDao, MockedProjectDao>();
            }
            else
            {   
                //Register WarehouseDaoFactory
                container.RegisterType<IDaoFactory, WarehouseDaoFactory>("Warehouse");
                //Register IProjectDao instance
                container.RegisterInstance<IProjectDao>(container.Resolve<IDaoFactory>("Warehouse").CreateProjectDao());
            }
          
            if (WebConfigurationManager.AppSettings["UseFakeFundDao"].ToString() == "true")
            {
                container.RegisterType<IFundsDao, MockedFundsDao>();
            }
            else
            {
                //Register LocalQdbDaoFactory
                container.RegisterType<IDaoFactory, LocalQdbDaoFactory>("LocalQdb");
                //Register IFundsDao instance
                container.RegisterInstance<IFundsDao>(container.Resolve<IDaoFactory>("LocalQdb").CreateFundsDao());
            }
            //Register JSONAPIKeyVerifier
            container.RegisterType<IApiKeyProvider, ORA.Services.WebIRBCRMS.Security.JSONAPIKeyVerifier>();

            //Unity dependency resolver for MVC5
            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(container));
            //Unity dependency resolver for WebApi
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

        }

        public static UnityContainer GetContainer()
        {
            return container;
        }
    }
}