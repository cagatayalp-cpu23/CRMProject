using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMProject.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ModalPartial()
        {        
            return PartialView("~/Views/Home/ModalPartial.cshtml");
        }
        public ActionResult NewRequestPartial()
        {
            return PartialView("~/Views/Home/NewRequestPartial.cshtml");
        }


    }


}