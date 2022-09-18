using System;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.Mvc;
using CRMProject.Service.Admin;
using CRMProject.ViewModels.Admin;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;
using CRMProject.Data;
using CRMProject.Web.Controllers.Abstract;
using Microsoft.Web.Mvc;
using MvcPaging;


namespace CRMProject.Web.Controllers
{

    
    public class TaskController : AdminBaseController
    {
        // GET: Task
        private readonly TaskService _taskService;
        private readonly UserService _userService;

        public TaskController(TaskService taskService, UserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }
       
        public ActionResult Index(int? pagedNo)
        { 
            var id= CurrentUser.UserId;
            var pie = _taskService.piechart(id);

            ViewBag.ChartData = pie;

            if (CurrentUser.AuthCodes == new List<string> { "Manager" })
            {
                TempData["manager"] = "Manager";
            }
            int pageListNo = pagedNo ?? 1;
            var model = new TaskSearchViewModel();
            ViewBag.Page = pageListNo;
            return View(model);
        }
        [HttpPost]

        public ActionResult TaskForm(TaskSearchViewModel searchmodel, int? page)
        {

            
            var currentPageIndex = page ?? 1;

            var model = _taskService.GetTasksListIQueryable(searchmodel, CurrentUser).OrderBy(x => x.Id);
            if (searchmodel.SortList == ListEnum.IsSelected)
            {
                model = model.OrderBy(x => x.IsSelected);
            }

            if (searchmodel.SortList==ListEnum.Deadline)
            {
                model = model.OrderBy(x => x.Deadline);
            }
            if (searchmodel.SortList == ListEnum.problem)
            {
                model = model.OrderBy(x => x.ProblemName);
            }
            var result=model.ToPagedList(currentPageIndex - 1, 4);
            ViewBag.searchModel = searchmodel;

            ViewBag.username = CurrentUser.UserName;
            ViewBag.Sum = _taskService.SumAsync(CurrentUser.UserName);
            return new ContentResult
            {
                ContentType = "application/json",
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/Task/TaskForm.cshtml", result)
                })
            };
        }


        public ActionResult Index2()
        {
            
            var role = CurrentUser.Role;
            var name = CurrentUser.UserName;
            var model = _taskService.GetEmptyTasks(role);
            ViewBag.asd = "asd";
            ViewBag.role = role;
            return View(model);
        }

        [HttpGet]
        public ActionResult TaskSelect()
        {
            return PartialView("~/Views/Task/_TaskSelect.cshtml");
        }


        [HttpPost]
        public async Task<ActionResult> TaskSelect(UserViewModel model2, int id)
        {
            

            ViewBag.username = CurrentUser.UserName;
            var name = CurrentUser.UserName;
            /*var model = *//*await _taskService.SelectUser(id, ViewBag.username);*/
            var model = await _taskService.GetCurrentUserTasks();
            var callResult = _taskService.SelectUser(id, model2, name.ToString());
            if (callResult.Success)
            {

                ModelState.Clear();
                var Id = (int)callResult.Item;
                var viewModel = await _taskService.GetTaskListViewAsync(Id);
                ViewBag.Sum = _taskService.SumAsync(name);

                var jsonResult = Json(
                    new
                    {
                        success = true,
                        responseText = RenderPartialViewToString("~/Views/Task/DisplayTemplates/TaskListViewModel.cshtml", viewModel),
                        item = viewModel

                    }, JsonRequestBehavior.AllowGet);

                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            //foreach (var error in callResult.ErrorMessages)
            //{
            //    ModelState.AddModelError("", error);
            //}

            return Json(
                new
                {
                    success = true,
                    responseText = RenderPartialViewToString("~/Views/Task/DisplayTemplates/TaskListViewModel.cshtml", model),
                });

            //  return RedirectToAction("Index");
        }

        public ActionResult AddStep()
        {


            var model = new StepViewModel();
            return PartialView("AddStepPartial", model);

        }
        public ActionResult EditStep(int taskId)
        {
            var model = new StepViewModel()
            {
                taskId  = taskId,
                
            };
            return PartialView("EditStepPartial", model);
        }

        //public ActionResult Yeni()
        //{
        //    //FormsAuthenticationTicket authTicket;
        //    //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    ////ViewBag.SelectedOrderType = typeId;
        //    ////ViewBag.CurrentPage = page;
        //    //authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //    //ViewBag.role = authTicket.UserData;
        //    //var model = _taskService.Get4();
        //    return View();
        //}



        public async Task<ActionResult> AddTask()
        {
            var problems = await _taskService.GetProblemViewModelsAsync();
       //     var users = await _taskService.GetUserViewModelsAsync();
       //    var roles = await _taskService.GetRoleViewModelsAsync();
            ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "AddTask" };
            var M = _taskService.GetEmptyStep();
            ViewData["Problems"] = problems;
         //   ViewData["Users"] = users;
         //   ViewData["Roles"] = roles;

            return PartialView("~/Views/Task/_TaskAdd.cshtml", M);
        }




        [HttpPost]
        public async Task<ActionResult> AddTask([Bind(Prefix = "AddTask")] TaskAddViewModel model)
        {
            
            ViewBag.username = CurrentUser.UserName;
            ViewBag.role = CurrentUser.Role;
            ViewBag.asd = "asd";

            if (ModelState.IsValid)
            {
                var callResult = await _taskService.AddTaskAsync(model,CurrentUser.Role);
                if (callResult.Success)
                {
                    ModelState.Clear();
                    var taskId = (int)callResult.Item;
                    var viewModel = await _taskService.GetTaskListViewAsync2(taskId);
                    var jsonResult = Json(
                        new
                        {
                            success = true,
                            responseText = RenderPartialViewToString("~/Views/Task/DisplayTemplates/TaskListViewModel.cshtml", viewModel),
                            item = viewModel
                        });
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                foreach (var error in callResult.ErrorMessages)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return Json(
                new
                {
                    success = false,
                    responseText = RenderPartialViewToString("~/Areas/Admin/Views/FaqSetting/_FaqAdd.cshtml", model)
                });

        }




        //public ActionResult Kaydet(TaskSet taskSet)
        //{
        //    FormsAuthenticationTicket authTicket;
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

        //    authTicket = FormsAuthentication.Decrypt(authCookie.Value);

        //    var model = _taskService.Get5(taskSet, authTicket.UserData.ToString());

        //    return RedirectToAction("Index", model);
        //}
        //public ActionResult Sil(int id)
        //{
        //     await _taskService.SilAsync(id);
        //    return RedirectToAction("Index");
        //}


        public ActionResult Delete(int taskId)
        {

            var model = new TaskDeleteViewModel() { TaskId = taskId };
            // return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml", "İşlem için yetkiniz yoktur.!");
            return PartialView("~/Views/Task/_TaskDelete.cshtml", model);

        }

        public ActionResult StepDelete(int id)
        {

           var callresult=_taskService.StepDelete(id);
           if (callresult.Success)
           {
               return Json(
                   new
                   {
                       success = true,
                       warningMessages = callresult.WarningMessages,
                       successMessages = callresult.SuccessMessages
                   });
           }
           return Json(
               new
               {
                   success = false,
                   errorMessages = callresult.ErrorMessages,
               });


        }
        [HttpPost]
        public async Task<ActionResult> Delete(TaskDeleteViewModel model)
        {
            
           var asd= CurrentUser.UserName;
            var callResult = await _taskService.SilAsync(model.TaskId, CurrentUser);
            var m = CurrentUser.UserName;

            // callResult.ErrorMessages.Add("Yetkiniz yok");
            if (callResult.Success)
            {
                var jsonResult = Json(new { });
                ModelState.Clear();
                ViewBag.Sum = _taskService.SumAsync(CurrentUser.UserName);

                return Json(

                new
                {
                    success = true,
                    warningMessages = callResult.WarningMessages,
                    successMessages = callResult.SuccessMessages,
                }
                );

            }

            return Json(
       new
       {
           success = false,
           errorMessages = callResult.ErrorMessages,
       });




            return View();
        }


        public async Task<ActionResult> Edit(int taskId)
        {
           

            var model = await _taskService.GetProblemViewModelAsync(taskId);
            var problems = await _taskService.GetProblemViewModelsAsync();
            var users = await _taskService.GetUserViewModelsAsync();
            var roles = await _taskService.GetRoleViewModelsAsync();
            var step = await _taskService.GetStepViewModel(taskId);
            ViewData["Problems"] = problems;
            ViewData["Users"] = users;
            ViewData["Roles"] = roles;
           ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "Edit" };

            ViewData["Steps"] = step;
            var atayankullanici = _taskService._Atayankullanicialma(taskId);
            if (CurrentUser.UserName == "admin" || CurrentUser.UserName== atayankullanici)
            {

                //   ViewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = "BlogEdit" };
                return PartialView("~/Views/Task/_TaskEdit.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Shared/_ItemNotFoundPartial.cshtml" );
            }
           
            return PartialView("~/Views/Task/_TaskEdit.cshtml", model);
              
        }
        [HttpPost]



        public async Task<ActionResult> Edit([Bind(Prefix = "Edit")] TaskEditViewModel taskSet)
        {
            //await _taskService.Edit(taskSet);

            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}
           
            if (ModelState.IsValid)
            {
                var callResult = await _taskService.Edit(taskSet);
                if (callResult.Success)
                {
                    //var jsonResult = Json(new { });
                    ModelState.Clear();
                    var viewModel = (TaskListViewModel)callResult.Item;

                  var  jsonResult = Json(
                       new
                       {
                           success = true,
                           responseText = RenderPartialViewToString("~/Views/Task/DisplayTemplates/TaskListViewModel.cshtml", viewModel),
                           item = viewModel
                       });


                    



                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;


                    //   return View("_TaskEdit", taskSet);

                }

            }
            return View();

        }

        
        public ActionResult ChangeStatus(int id)
        {
           var callresult= _taskService.Change(id);
           ViewBag.Sum = _taskService.SumAsync(CurrentUser.UserName);
           if (callresult.Success == true)
           {
               return Json(new
           {
               success = false,
               errorMessages = callresult.ErrorMessages
           },JsonRequestBehavior.AllowGet);
           }
           else
           {
               return Json(new
               {
                   success = true
               },JsonRequestBehavior.AllowGet);
           }
           


        }

        public int Percentage()
        {
            ViewBag.Sum = _taskService.SumAsync(CurrentUser.UserName);
            return _taskService.SumAsync(CurrentUser.UserName);


        }


        public ActionResult OpenIndex2()
        {
            return RedirectToAction("Index2");
        }

        public ActionResult KimeEklenecek(int id)
        {
           
            ViewBag.users = _userService.GetUsers(id, CurrentUser.Role.ToString());


            var model = new UserViewModel();
            return View(model);
        }

        public ActionResult Check(int id)
        {
            var callresult = _taskService.IsDone(id);
            if (callresult.Success)
            {
                ModelState.Clear();
                return Json(new
                {
                    success = true,
                    warningMessages=callresult.WarningMessages,
                    successMessages=callresult.SuccessMessages
                });
            }

            return Json(new
            {
                success = false,
                errorMessages = callresult.ErrorMessages
            });
        }

        //public ActionResult Yapabilirmi()
        //{
        //    int id=0;
        //    var callresult = _taskService.Change(id);
        //    if (callresult.Success==true)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            errorMessages = callresult.ErrorMessages

        //        });
        //    }
        //    return Json(new
        //    {
        //        success = false,
        //        errorMessages = callresult.ErrorMessages

        //    });

        //}
        
    

        //[HttpPost]
        //public async Task<ActionResult> KimeEklenecekAsync(List<UserViewModel> model,int id)
        //{FormsAuthenticationTicket authTicket;
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //    _taskService.KimeEklenecek(model,id,authTicket.UserData.ToString());



        //    return RedirectToAction("TaskSelect");
        // }

    }
}