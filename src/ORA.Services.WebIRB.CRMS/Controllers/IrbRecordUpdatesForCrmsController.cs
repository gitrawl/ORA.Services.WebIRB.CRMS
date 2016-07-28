using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ORA.Services.WebIRBCRMS;
using ORA.Services.WebIRBCRMS.Models;
using System.Web.OData;
using ORA.Services.WebIRBCRMS.Filters;
using ORA.Data.WebIrb.Staging;
using ORA.Data.Warehouse;
using ORA.Data;
using Elmah;

namespace ORA.Services.WebIRBCRMS.Controllers
{
    /// <summary>
    /// APIController for SC1 (GetIrbRecordUpdatesForCrms)
    /// </summary>
    public class IrbRecordUpdatesForCrmsController : ApiController
    {
        // Dependency Injection   
        private Interfaces.IIrbCrmsControllerService _service;
        private Interfaces.IIrbCrmsConverter _converter;

        /// <summary>
        /// Default constructor which takes the injection of 3 data managers.
        /// Unity prefers/requires? dependency injection in CTORs.
        /// The data managers are injected primarily to support unit testing.
        /// </summary>
        /// <param name="DataManagerIrbCrms">IDataManagerWebIrbStaging</param>
        /// <param name="ProjectDataManager">IProjectDao</param>
        /// <param name="FundDataManager">IFundsDao</param>
        public IrbRecordUpdatesForCrmsController( IDataManagerWebIrbStaging DataManagerIrbCrms, IProjectDao ProjectDataManager, IFundsDao FundDataManager ) // Unity likes ctor injection
        {
            // not injected yet but can be if we update Unity and the ctor
            _service = new IrbCrmsService();
            _converter = new IrbCrmsConverter();

            // pass injections
            _service.setDataManager(DataManagerIrbCrms); // inject DataManager for IrbCrms
            _service.SetFundDataManager(FundDataManager); // inject DataManager for LocalQDB
            _service.setIrbCrmsConverter(_converter); // inject our converter
            _service.SetProjectDataManager(ProjectDataManager); // pass injection downstream
        }

        /// <summary>
        /// This contract will retrieve the latest version of all agreed data for each record which has changed, including new records which meet the agreed criteria, within a specified date window.
        /// </summary>
        /// <param name="startdate">This date will be the date to retrieve data “since,” i.e. the minimum date in the date window.</param>
        /// <returns>One “row” (specific format: Study) for each protocol for whom any of the data field defined below have changed since the start date.</returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,CRMS")]
        [EnableQuery(MaxExpansionDepth=10)]
        public IHttpActionResult Get( DateTime startdate )
        {
            return GetChangedStudies(startdate, null);
        }

        /// <summary>
        /// This contract will retrieve the latest version of all agreed data for each record which has changed, including new records which meet the agreed criteria, within a specified date window.
        /// </summary>
        /// <param name="startdate">This date will be the date to retrieve data “since,” i.e. the minimum date in the date window.</param>
        /// <param name="enddate">This date will be the date to retrieve data “until,” i.e. the maximum date in the date window. If not specified it will assume today is the maximum date. This date will also be the “as-of” date for the purpose of determining the “latest version”.</param>
        /// <returns>One “row” (specific format: Study) for each protocol for whom any of the data field defined below have changed in the date window specified.</returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,CRMS")]
        [EnableQuery(MaxExpansionDepth = 10)]
        public IHttpActionResult Get( DateTime startdate, DateTime enddate )
        {
            return GetChangedStudies(startdate, enddate);
        }

        #region private methods
        private IHttpActionResult GetChangedStudies( DateTime startdate, DateTime? enddate )
        {
            try
            {
                return Ok(_service.GetChangedStudies(startdate, enddate).AsQueryable<Study>());
            }
            catch(Exception e)
            {
                try
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
                catch { }
                return InternalServerError();
            }
        }

        #endregion
    }
}