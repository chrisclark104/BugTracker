using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Where do you want your project to start??? what controller/viewyeah Home/Index..ok where is your Home/Index .cshtml page? Its not setup..no model etc...
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}