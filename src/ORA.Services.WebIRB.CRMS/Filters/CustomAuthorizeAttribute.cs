using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Configuration;

namespace ORA.Services.WebIRBCRMS.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if ( actionContext.RequestContext.Principal.Identity.Name == "Anonymous")
            {
                // bypass the filter if APIKey is not required
                return true;
            }

            return base.IsAuthorized(actionContext);
        }

    }
}