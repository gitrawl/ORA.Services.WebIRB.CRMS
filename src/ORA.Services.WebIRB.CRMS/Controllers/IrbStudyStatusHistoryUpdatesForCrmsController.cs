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
using Elmah;

namespace ORA.Services.WebIRBCRMS.Controllers
{
    /// <summary>
    /// APIController for SC2 (GetIrbStudyStatusHistoryUpdatesForCrms).
    /// </summary>
    public class IrbStudyStatusHistoryUpdatesForCrmsController : ApiController
    {
        // Dependency Injection   
        private Interfaces.IIrbCrmsControllerService _service;
        private Interfaces.IIrbCrmsConverter _converter;

        /// <summary>
        /// The data manager is injected primarily to support unit testing.
        /// </summary>
        /// <param name="dataManager">IDataManagerIrbCrms</param>
        public IrbStudyStatusHistoryUpdatesForCrmsController (IDataManagerWebIrbStaging dataManager) // Unity likes ctor injection
        {
            // not injected yet but can be if we update Unity and the ctor
            _service = new IrbCrmsService();
            _converter = new IrbCrmsConverter();
            // pass injections
            _service.setDataManager(dataManager); // pass injection downstream
            _service.setIrbCrmsConverter(_converter); // inject our converter
        }

        /// <summary>
        /// This contract will retrieve all external status changes which have occurred for each record which has undergone one or more external status changes within a specified date window.
        /// </summary>
        /// <param name="startdate">This date will be the date to retrieve data “since,” i.e. the minimum date in the date window.</param>
        /// <returns>
        /// The contract will output one “row” (specific format: StudyStatusHistory) for each study that has at least one external status change in its protocol or any of its amendments, continuing reviews and post approval reports in the date window specified.
        /// The “row” of data will contain the data fields for filtering study and a list of all external status changes that have occurred in the date window specified.
        /// </returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,CRMS")]
        [EnableQuery(MaxExpansionDepth = 10)]
        public IHttpActionResult Get( DateTime startdate )
        {
            return GetStudyChangedHistory(startdate, null);
        }

        /// <summary>
        /// This contract will retrieve all external status changes which have occurred for each record which has undergone one or more external status changes within a specified date window.
        /// </summary>
        /// <param name="startdate">This date will be the date to retrieve data “since,” i.e. the minimum date in the date window.</param>
        /// <param name="enddate">This date will be the date to retrieve data “until,” i.e. the maximum date in the date window.</param>
        /// <returns>
        /// The contract will output one “row” (specific format: StudyStatusHistory) for each study that has at least one external status change in its protocol or any of its amendments, continuing reviews and post approval reports in the date window specified.
        /// The “row” of data will contain the data fields for filtering study and a list of all external status changes that have occurred in the date window specified.
        /// </returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,CRMS")]
        [EnableQuery(MaxExpansionDepth = 10)]
        public IHttpActionResult Get( DateTime startdate, DateTime enddate )
        {
            return GetStudyChangedHistory(startdate, enddate);
        }

        #region private methods
        private IHttpActionResult GetStudyChangedHistory( DateTime startdate, DateTime? enddate )
        {
            try
            {
                return Ok(_service.GetStudyChangedHistory(startdate, enddate).AsQueryable<StudyStatusHistory>());
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
