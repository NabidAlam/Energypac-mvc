using AuditReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuditReport.Controllers
{
    [Authorize]
    public class ManualStatusController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();
        // GET: ManualStatus
        public ActionResult Index()
        {
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Index(int SitePlanCode)
        {
            
            return RedirectToAction("StatusList", "ProjectSitePlanTasks", new { PId = SitePlanCode });

            //ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            //var data = db.ProjectSiteStatus. 
            //return View();
        }

        [HttpPost]
        public ActionResult GetPlanCode(int SiteId)
        {

            List<SelectListItem> PlanCode = new List<SelectListItem>();

            var data = db.ProjectSitePlan.Where(x =>x.ProjectSiteId == SiteId).ToList();

            foreach (var x in data)
            {
                PlanCode.Add(new SelectListItem { Text = x.PlanCode, Value = x.Id.ToString() });
            }
            return Json(PlanCode, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GetPlanDate(int ProjectId, int SiteId)
        //{

        //    List<SelectListItem> PlanDate = new List<SelectListItem>();

        //    var data = db.ProjectSitePlanTask.Where(x => x.ProjectId == ProjectId && x.ProjectSiteId==SiteId).Select(y=>y.SitePlanDate).Distinct().ToList();

        //    foreach (var x in data)
        //    {
        //        PlanDate.Add(new SelectListItem { Text = x.ToShortDateString(), Value = x.ToShortDateString() });
        //    }
        //    return Json(PlanDate, JsonRequestBehavior.AllowGet);
        //}  

    }
}