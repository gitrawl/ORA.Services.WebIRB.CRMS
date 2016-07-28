using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace ORA.Services.WebIRBCRMS.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetClientIpAddress( this HttpRequestMessage request )
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                return ((RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name]).Address;

            return "IP Address Unavailable"; 
        }
    }
}