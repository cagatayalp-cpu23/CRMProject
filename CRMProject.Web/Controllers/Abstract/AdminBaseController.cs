﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMProject.ViewModels.Admin;
using CRMProject.Web.Extensions;
using CRMProject.Web.Filters;
using Microsoft.AspNet.Identity;


namespace CRMProject.Web.Controllers.Abstract
{
    [CustomAuthorizeFilter]
    public abstract class AdminBaseController : Controller
    {
        public CurrentUserModel CurrentUser { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                CurrentUser = filterContext.HttpContext.User.GetCurrentUserModelAsync().GetAwaiter().GetResult();
                if (CurrentUser == null)
                {
                    filterContext.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    filterContext.Result = new HttpUnauthorizedResult();
                }


            }
            base.OnActionExecuting(filterContext);
        }
        public ActionResult RedirectToLocalOr(string returnUrl, Func<ActionResult> action)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return action();

        }
        public string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult RedirectToPreviousOr(Func<ActionResult> action)
        {
            var httpContext = ControllerContext.HttpContext;

            var previousUrl = httpContext.Request.UrlReferrer?.ToString();

            return Url.IsLocalUrl(previousUrl) ? new RedirectResult(previousUrl) : action();
        }
    }
}