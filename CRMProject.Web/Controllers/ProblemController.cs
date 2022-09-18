using CRMProject.Data;
using CRMProject.Service.Admin;
using CRMProject.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CRMProject.Web.Controllers.Abstract;

namespace CRMProject.Web.Controllers
{

    [Authorize]
    public class ProblemController : AdminBaseController
    {
        // GET: Problem
        public ProblemService _problemService;
        public ProblemController(ProblemService problemService)
        {
            _problemService = problemService;
        }
        public ActionResult Index()
        {
          var model=  _problemService.Get();
            return View(model);
        }
        public ActionResult Yeni()
        {


            var model = _problemService.Get2();
            return View(model);



        }
        public async Task<ActionResult> Kaydet(ProblemViewModel problemViewModel)
        {
            var callResult=await  _problemService.Get3(problemViewModel);
            if (callResult.Success)
            {

                ModelState.Clear();
                var problemId = (int)callResult.Item;
                var viewModel = await _problemService.GetProblemListViewAsync(problemId);
                var jsonResult = Json(
                    new
                    {
                        success = true,
                        responseText = RenderPartialViewToString("~/Views/Problem/DisplayTemplates/ProblemViewModel.cshtml", viewModel),
                        item = viewModel
                    });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            foreach (var error in callResult.ErrorMessages)
            {
                ModelState.AddModelError("", error);
            }

            return RedirectToAction("Index");
        }

        public  ActionResult Delete(int problemId)
        {
            var model = new ProblemViewModel()
            {
                Id = problemId
            };
            return  PartialView("/Views/Problem/_problemDelete.cshtml",model);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(ProblemViewModel problemViewModel)
        {
            var callResult = await _problemService.DeleteAsync(problemViewModel.Id);
            if (callResult.Success)
            {

                ModelState.Clear();
                return Json(
                    new
                    {
                        success = true,
                        warningMessages = callResult.WarningMessages,
                        successMessages = callResult.SuccessMessages,
                    });
            }
            return Json(
                new
                {
                    success = false,
                    errorMessages = callResult.ErrorMessages,
                    responseText = RenderPartialViewToString("~/Views/Problem/_problemDelete.cshtml", problemViewModel)
                });
        }





        }

    }
