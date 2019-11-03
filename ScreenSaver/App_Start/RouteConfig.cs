using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ScreenSaver
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Images upload page
            routes.MapRoute(
                name: "Upload Images",
                url: "images-upload",
                defaults: new { controller = "Images", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //Default images page
            routes.MapRoute(
                name: "Default Images",
                url: "default-images",
                defaults: new { controller = "Images", action = "DefaultImages", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
