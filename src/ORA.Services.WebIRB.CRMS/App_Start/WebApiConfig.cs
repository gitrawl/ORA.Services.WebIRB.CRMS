using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Net.Http;
using Microsoft.OData.Edm;
using ORA.Services.WebIRBCRMS.Models;
using Microsoft.Practices.Unity;

namespace ORA.Services.WebIRBCRMS
{
    // Web API configuration and services
    public static class WebApiConfig
    { 
        public static void Register( HttpConfiguration config )
        {
            // Get Unity dependency container
            UnityContainer container = UnityConfig.GetContainer();

            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Web API routes and add APIKeyHandler
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints:null,
                handler: HttpClientFactory.CreatePipeline(
                         new System.Web.Http.Dispatcher.HttpControllerDispatcher(config),
                         new DelegatingHandler[] { container.Resolve<MessageHandlers.ApiKeyHandler>() })
            );

            // odata route
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: "odata",
                model: GetEdmModel());

            // global message handler: RequiredHttpsHandler
            config.MessageHandlers.Add(new MessageHandlers.LoggingHandler());
            config.MessageHandlers.Add(new MessageHandlers.RequireHttpsHandler());

            // makes json the default response for all web browsers 
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
        }

        // Build Edm Model for odata
        public static IEdmModel GetEdmModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Person>("Persons");
            builder.EntitySet<ProtocolPI>("ProtocolPIs");
            builder.EntitySet<SponsoredFund>("SponsoredFunds");
            builder.EntitySet<Award>("Awards");
            builder.EntitySet<Device>("Devices");
            builder.EntitySet<Drug>("Drugs");
            builder.EntitySet<Amendment>("Amendments");
            builder.EntitySet<ContinuingReview>("ContinuingReviews");
            builder.EntitySet<PostApprovalReport>("PostApprovalReports");
            builder.EntitySet<ProtocolActivity>("ProtocolActivities");
            builder.EntitySet<Study>("Studies");
            builder.EntitySet<ExternalStatusChange>("ExternalStatusChanges");
            builder.EntitySet<StudyStatusHistory>("StudyStatusHistories");
            return builder.GetEdmModel();
        }

    }
}