using CRMProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LightInject;

namespace CRMProject.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
       
        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            //GlobalFilters.Filters.Add(new AuthorizeAttribute());
          
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = new LightInject.ServiceContainer();
            container.RegisterControllers();
            container.Register(typeof(CrmDbTestEntities), new PerRequestLifeTime());
            //container.Register<System.Data.Entity.Infrastructure.ICacheService, Infrastructure.Web.InMemoryCache>(new PerRequestLifeTime());
            //System.Net.ServicePointManager.SecurityProtocol |=
            //    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            container.Register(typeof(CRMProject.Service.Admin.SettingService), new PerRequestLifeTime());
            container.Register(typeof(CRMProject.Service.Admin.TaskService), new PerRequestLifeTime());
            container.Register(typeof(CRMProject.Service.Admin.ProblemService), new PerRequestLifeTime());
            container.Register(typeof(CRMProject.Service.Admin.UserService), new PerRequestLifeTime());
            container.Register(typeof(CRMProject.Service.Common.CommonService), new PerRequestLifeTime());



            container.EnableMvc();

        }
    }
}
