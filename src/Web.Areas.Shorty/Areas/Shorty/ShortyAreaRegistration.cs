using System.Web.Mvc;
using MvcContrib.PortableAreas;

namespace MvcPlugin.Shorty.Areas.Shorty
{
    /*
     *
     *  SEE http://elegantcode.com/2012/04/06/mvc-portable-areas/ for more information on this
     * 
     */

    public class ShortyAreaRegistration : PortableAreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Shorty";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RegisterRoutes(context);

            RegisterAreaEmbeddedResources();
        }

        private void RegisterRoutes(AreaRegistrationContext context)
        {
            #region Portable area stuff

            context.MapRoute(
                name: AreaName + "_scripts",
                url: base.AreaRoutePrefix + "/Scripts/{resourceName}",
                defaults: new { controller = "EmbeddedResource", action = "Index", resourcePath = "scripts" },
                namespaces: new[] { "MvcContrib.PortableAreas" }
            );
     
           context.MapRoute(
               name: AreaName + "_images",
               url: base.AreaRoutePrefix + "/images/{resourceName}",
               defaults: new { controller = "EmbeddedResource", action = "Index", resourcePath = "images" },
               namespaces: new[] { "MvcContrib.PortableAreas" }
           );

           context.MapRoute(
               name: AreaName + "_default",
               url: base.AreaRoutePrefix + "/{controller}/{action}",
               defaults: new { controller = "Api", action = "List" },
               namespaces: new[] { "MvcPlugin.Shorty.Areas.Shorty.Controllers", "MvcContrib" }
            );

            #endregion

            context.MapRoute(
                name: AreaName + "_create",
                url: base.AreaRoutePrefix + "/create/{*url}",
                defaults: new { controller = "Api", action = "Create" },
                namespaces: new[] { "MvcPlugin.Shorty.Areas.Shorty.Controllers", "MvcContrib" }
            );
        }
    }
}