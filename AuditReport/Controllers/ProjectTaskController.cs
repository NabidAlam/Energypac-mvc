using AuditReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuditReport.Helper;
namespace AuditReport.Controllers
{
    [Authorize]
    public class ProjectTaskController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();
        // GET: ProjectTask
        public ActionResult Index()
        {
            
            ViewBag.ExistNotDeletedItem = false;

            if (NullHelper.ObjectToString(TempData["NotDeletedItem"]) != "")
            {
                ViewBag.NotDeletedItem = TempData["NotDeletedItem"];

                foreach (var item in ViewBag.NotDeletedItem)
                {
                    ViewBag.ExistNotDeletedItem = true;
                }

            }
                     

            TempData["NotDeletedItem"] = "";

            return View(db.ProjectTask.ToList());
        }
    }
}