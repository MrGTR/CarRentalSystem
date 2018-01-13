using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CarRentalSystem
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Add by GTR - log4net requirment
            log4net.Config.XmlConfigurator.Configure();
         
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
