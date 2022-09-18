using CRMProject.Service.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMProject.Web.Controllers
{
    public class CalendarController : Controller
    {
        public ProblemService _problemService;
        public CalendarController(ProblemService problemService)
        {
            _problemService = problemService;
        }
        // GET: Calendar
        public ActionResult Index()
        {
            var model = _problemService.Get();
            return View(model);
        }
    }
}