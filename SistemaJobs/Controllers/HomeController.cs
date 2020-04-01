using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaJobs.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Bem-vindo ao";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Bem-vindo ao";

            return View();
        }
    }
}