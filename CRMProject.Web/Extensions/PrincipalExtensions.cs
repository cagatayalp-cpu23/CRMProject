using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CRMProject.Service.Common;
using CRMProject.ViewModels.Admin;

namespace CRMProject.Web.Extensions
{
    public static class PrincipalExtensions
    {
        public static async Task<CurrentUserModel> GetCurrentUserModelAsync(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;

            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var commonService = DependencyResolver.Current.GetService<CommonService>();
                var userId = Convert.ToInt64(claim.Value);

                return await commonService.GetCurrentUserModelAsync(userId).ConfigureAwait(false);
            }

            return null;
        }
    }
}