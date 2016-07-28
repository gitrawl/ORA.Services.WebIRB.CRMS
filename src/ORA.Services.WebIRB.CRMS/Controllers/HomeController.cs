using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ORA.Services.WebIRBCRMS.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>Home View</returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}