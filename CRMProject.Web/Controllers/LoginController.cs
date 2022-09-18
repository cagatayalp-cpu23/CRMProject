using CRMProject.Service.Admin;
using CRMProject.Web.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CRMProject.Data;
using CRMProject.ViewModels.Admin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CRMProject.Web.Controllers
{
    public class LoginController : AdminBaseController
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        private readonly SettingService _settingService;
        public LoginController(SettingService settingService)
        {
            _settingService = settingService;

        }
        // GET: Admin/Login
        private void _signIn(long userId, bool rememberMe = false)
        {
            var identity = new ClaimsIdentity(
                new[] {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()/*UserId*/)

                },
                DefaultAuthenticationTypes.ApplicationCookie,
                ClaimTypes.NameIdentifier, ClaimTypes.Role);

            identity.AddClaim(
                new Claim(
                    "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider"/*schema url*/
                    , "ASP.NET Identity"
                    , "http://www.w3.org/2001/XMLSchema#string"
                )
            );
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe /*RememberMe*/}, identity);
        }
        [AllowAnonymous]
        
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = "Giriş";
            return View(new LoginViewModel());
        }
        //
        // POST: /User/Login

        [HttpPost]
        [AllowAnonymous]
     [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Users kullanici = _settingService.Login2(model);
                var loginResult = _settingService.Login(model);
                if (loginResult)
                {
                    
                    if (kullanici != null)
                    {
                        _signIn(kullanici.Id, true);

                        return RedirectToLocalOr(returnUrl, () => RedirectToAction("Index", "Task", new { Area = String.Empty }));
                    }
                    //    ;
                }

            }
            // If we got this far, something failed, redisplay form
            ViewBag.Title = "PraPazar | Giriş";
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var callResult =  _settingService.Register(model);
                if (callResult.Success)
                {
                    ModelState.Clear();
                    foreach (var success in callResult.SuccessMessages)
                    {
                        ModelState.AddModelError("", success);
                        ViewBag.successRegister = success;
                    }
                    return View(model);
                }
                foreach (var error in callResult.ErrorMessages)
                {
                    ModelState.AddModelError("", error);
                    ViewBag.errorRegister = error;
                }
            }
            return View(model);
        }
    }
}