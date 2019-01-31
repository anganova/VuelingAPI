using System.Web.Mvc;
using System.Web.Routing;

namespace VuelingAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{sku}",
                defaults: new { controller = "Transactions", action = "Get", sku = UrlParameter.Optional }
            );
        }
    }
}

