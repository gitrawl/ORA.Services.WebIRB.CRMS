using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ORA.Services.WebIRBCRMS.MessageHandlers
{
    public class RequireHttpsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            if (WebConfigurationManager.AppSettings["HttpsRequired"].ToString() == "true" && request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                HttpResponseMessage forbiddenResponse = request.CreateResponse(HttpStatusCode.Forbidden);
                forbiddenResponse.ReasonPhrase = "SSL Required";
                return Task.FromResult<HttpResponseMessage>(forbiddenResponse);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}