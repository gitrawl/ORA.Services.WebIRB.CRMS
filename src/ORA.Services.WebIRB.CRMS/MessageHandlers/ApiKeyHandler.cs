using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Security.Principal;
using ORA.Services.WebIRBCRMS.Interfaces;
using System.Web.Configuration;

namespace ORA.Services.WebIRBCRMS.MessageHandlers
{
    public class ApiKeyHandler : DelegatingHandler
    {
        private IApiKeyProvider APIKeyVerifier;

        //Allow unity to inject IAPIKeyProvider
        public ApiKeyHandler(IApiKeyProvider APIKeyProvider)
        {
            APIKeyVerifier = APIKeyProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            if (WebConfigurationManager.AppSettings["APIKeyRequired"].ToString() == "true")
            {
                string apikey = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");

                if (string.IsNullOrWhiteSpace(apikey))
                {
                    //Missing API Key
                    return SendError("You can't use the API without the key.", HttpStatusCode.Forbidden);
                }
                else
                {
                    if (APIKeyVerifier.IsAPIKeyValid(apikey))
                    {
                        //set the User Identity
                        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(apikey), APIKeyVerifier.GetRoles(apikey));
                        //Continue on the request
                        return base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        //API Key Not match
                        return SendError("Invalid API Key.", HttpStatusCode.Forbidden);
                    }
                }
            }
            else
            {
                //APIKey not required
                //set the User Identity
                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("Anonymous"), null);
                return base.SendAsync(request, cancellationToken);
            }
        }

        private Task<HttpResponseMessage> SendError(string error, HttpStatusCode code) 
        { 
            var response = new HttpResponseMessage(); 
            response.Content = new StringContent(error); 
            response.StatusCode = code; 
            return Task<HttpResponseMessage>.Factory.StartNew(() => response); 
        } 

    }
}