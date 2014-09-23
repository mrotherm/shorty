using System;
using System.Web.Mvc;
using Raven.Client;

namespace MvcPlugin.Shorty.Areas.Shorty.Controllers
{
    public class RavenDataController : Controller
    {
        public IDocumentSession DocumentSession { get; set; }

        protected readonly IDocumentStore DocumentStore;

        // Maybe create another base class to hold generic properties like BaseUrl,
        // but so far this is the only one so we'll ship it here
        protected readonly string BaseUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        public RavenDataController(IDocumentStore documentStore)
        {
            this.DocumentStore = documentStore;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DocumentSession = this.DocumentStore.OpenSession();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (DocumentSession)
            {
                if (DocumentSession != null && filterContext.Exception == null)
                {
                    DocumentSession.SaveChanges();
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
