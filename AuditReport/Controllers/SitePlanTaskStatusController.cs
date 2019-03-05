using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuditReport.Controllers
{
    public class SitePlanTaskStatusController : Controller
    {
        // GET: SitePlanTaskStatus
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StatusList()
        {
            return View();
        }


    }
}