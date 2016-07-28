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
    /// APIController for SC3 (GetPreSubmissionsForCrms)
    /// </summary>
    public class IrbPreSubmissionUpdatesForCrmsController : ApiController
    {
        // Dependency Injection   
        private Interfaces.IIrbCrmsControllerService _service;
        private Interfaces.IIrbCrmsConverter _converter;

         /// <summary>
        /// Default constructor for IrbPreSubmissionForCrmsController takes 3 data managers
        /// injection. Unity requires/prefers? constructor injection.
        /// The data manager is injected primarily to support unit testing.
        /// </summary>
        /// <param name="DataManagerIrbCrms">IDataManagerWebIrbStaging</param>
        /// <param name="ProjectDataManager">IProjectDao</param>
        /// <param name="FundDataManager">IFundsDao</param>
        public IrbPreSubmissionUpdatesForCrmsController( IDataManagerWebIrbStaging DataManagerIrbCrms, IProjectDao ProjectDataManager, IFundsDao FundDataManager ) // Unity likes ctor injection
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
        /// This contract will retrieve the latest version of all Pre-Submission protocols with ClinicalPreReviewInvestigatorInitiated field set to Yes.
        /// </summary>
        /// <returns>This contract will output one "row" (specific format: Study) for Pre-Submission protocol which has ClinicalPreReviewInvestigatorInitiated field set to Yes;</returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,CRMS")]
        [EnableQuery(MaxExpansionDepth = 10)]
        public IHttpActionResult Get()
        {
            return GetPreSubmissions();
        }

        #region private methods
        private IHttpActionResult GetPreSubmissions()
        {
            try
            {
                return Ok(_service.GetPreSubmissions().AsQueryable<Study>());
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
