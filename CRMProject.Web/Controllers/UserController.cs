using CRMProject.Service.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CRMProject.ViewModels.Admin;
using CRMProject.Web.Controllers.Abstract;
using CRMProject.Web.Filters;
using MvcPaging;

namespace CRMProject.Web.Controllers
{

    public class UserController : AdminBaseController
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        // GET: User
        //[CustomAuthorizeFilter(UserAuthCodes = "A,B")]
        [CustomAuthorizeFilter(UserAuthCodes = "Manager")]

        public ActionResult Index()
        {
            var searchModel = new UserSearchViewModel();
            
            return View(searchModel);
        }
        public  ActionResult UserForm(UserSearchViewModel searchModel,int? page)
        {
            
            ViewBag.searchmodel = searchModel;
            var currentPageIndex = page ?? 1;
            ViewBag.page = page;
            var model = _userService.GetUserListIQueryable(searchModel).OrderByDescending(x => x.Id);
            if (searchModel.SortList == UserListEnum.Role)
            {
                model = model.OrderBy(x => x.Role);
            }

            if (searchModel.SortList == UserListEnum.UserName)
            {
                model = model.OrderBy(x => x.Name);
            }
            var viewmodel=model.ToPagedList(currentPageIndex - 1, 4);
            return new ContentResult
            {
                ContentType = "application/json",
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/User/UserForm.cshtml", viewmodel)
                })
            };
        }

        public ActionResult AddUser()
        {
            var model = _userService.UserForm();
            //var model = new UserAddViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUser(UserAddViewModel model)
        {
         
           var callresult =_userService.AddUser(model);
          
           if (callresult.Success==true)
           {
               var id = (int)callresult.Item;
               var userlistviewmodel= _userService.GetOneUserListIQueryable(id);
               return Json(new
               {
                   success = true,
                  responseText= RenderPartialViewToString("/Views/User/DisplayTemplates/UserListViewModel.cshtml", userlistviewmodel)
               });
           }

           return Json(new
           {
               success = false,
               errorMessages=callresult.ErrorMessages
           });

           return View();
        }

        public ActionResult DeleteUser(int id)
        {
            UserDeleteViewModel model = new UserDeleteViewModel() { Id = id };
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteUser(UserDeleteViewModel model)
        {
           var callresult= _userService.DeleteUser(model.Id);
           if (callresult.Success)
           {
               return Json(new
               {
                   success = true,
                   warningMessages = callresult.WarningMessages,
                   successMessages = callresult.SuccessMessages,
               });
           }

           return Json(new
           {
               success = false,
               errorMessages=callresult.ErrorMessages
           });
           return View();
        }
        public ActionResult EditUser(int id)
        {

            var model = _userService.EditUserForm(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(UserEditViewModel model)
        {
            
            var callresult= _userService.EditUser(model);
           if (callresult.Success)
           {
               var id = (int)callresult.Item;
               var user = _userService.GetOneUserListIQueryable(id);
               return Json(new
               {
                   success=true,
                   responseText=RenderPartialViewToString("/Views/User/DisplayTemplates/UserListViewModel.cshtml",user)
               });

           }
           return Json(new
           {
               success = false,
               errorMessages=callresult.ErrorMessages
           });

            
        }
    }
}