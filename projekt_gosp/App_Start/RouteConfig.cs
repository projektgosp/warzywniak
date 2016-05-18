using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace projekt_gosp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Items",
                url: "items/category/{category}",
                defaults: new { controller = "Items", action = "category", category = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AdminPanel",
                url: "adminpanel",
                defaults: new { controller = "AdminPanel", action = "page" }
            );

            routes.MapRoute(
                name: "SellerPanel",
                url: "sellerPanel",
                defaults: new { controller = "SellerPanel", action = "page" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Shop", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}