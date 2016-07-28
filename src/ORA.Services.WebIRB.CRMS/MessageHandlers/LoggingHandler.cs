using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web;
using System.Diagnostics;
using ORA.Services.WebIRBCRMS.Extensions;
using System.Text;
//using NLog;
using Elmah;

namespace ORA.Services.WebIRBCRMS.MessageHandlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private static Common.Logging.ILog commonLogging = Common.Logging.LogManager.GetCurrentClassLogger();
        
        protected async override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            //logging asynchronously
            Task.Run(() => Log(request, response, stopwatch.ElapsedMilliseconds), cancellationToken);
            response.Headers.Add("ExecutionTime", stopwatch.ElapsedMilliseconds.ToString());
            return response;
        }

        private void Log(HttpRequestMessage request, HttpResponseMessage response, long ElapsedMilliseconds)
        {
            try
            {
                var log = new StringBuilder("");
                string Uri = request.RequestUri.ToString();
                string apikey = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("apikey");
                //mask part of apikey
                if (!string.IsNullOrWhiteSpace(apikey) && apikey.Length > 8)
                    Uri = Uri.Replace(apikey, apikey.Substring(8).PadLeft(apikey.Length, '*'));

                //log request
                log.Append("IP:").Append(request.GetClientIpAddress()).Append(", ");
                log.Append("HttpMethod:").Append(request.Method).Append(", ");
                log.Append("Uri:").Append(Uri).Append(", ");
                
                //log response
                log.Append("StatusCode:").Append(response.StatusCode).Append(", ");
                log.Append("TimeElapsedInMilliseconds:").Append(ElapsedMilliseconds);
                commonLogging.Info(log.ToString());
            }
            catch(Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }
    }
}