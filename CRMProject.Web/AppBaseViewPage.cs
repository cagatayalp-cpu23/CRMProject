using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMProject.ViewModels.Admin;
using CRMProject.Web.Controllers.Abstract;

namespace CRMProject.Web
{
    public abstract class AppBaseViewPage<TModel> : WebViewPage<TModel>
    {
        protected CurrentUserModel CurrentUser
        {
            get
            {
                if (ViewContext.Controller is AdminBaseController baseController)
                    return baseController.CurrentUser ?? new CurrentUserModel();
                return null;
            }
        }
    }

    public abstract class AppBaseViewPage : AppBaseViewPage<dynamic>
    {
    }
}