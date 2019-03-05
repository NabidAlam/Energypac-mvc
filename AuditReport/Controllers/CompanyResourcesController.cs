using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AuditReport.Models;

namespace AuditReport.Controllers
{
    [Authorize]
    public class CompanyResourcesController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();

        // GET: CompanyResources
        public ActionResult Index()
        {
            return View(db.CompanyResource.ToList());
        }

        // GET: CompanyResources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyResource companyResource = db.CompanyResource.Find(id);
            if (companyResource == null)
            {
                return HttpNotFound();
            }
            return View(companyResource);
        }

        // GET: CompanyResources/Create
        public ActionResult Create()
        {

            var status = new SelectList(new List<SelectListItem>{new SelectListItem{Text="Active",Value="A"},new SelectListItem {Text="Inactive",Value="I"},},"Value","Text");
            ViewBag.Status = status;
            return View();
        }

        // POST: CompanyResources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Position,DOJ,Phone,Address,Status")] CompanyResource companyResource)
        {
            if (ModelState.IsValid)
            {
                db.CompanyResource.Add(companyResource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text");
            ViewBag.Status = status;
            return View(companyResource);
        }

        // GET: CompanyResources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyResource companyResource = db.CompanyResource.Find(id);
            if (companyResource == null)
            {
                return HttpNotFound();
            }
            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text",companyResource.Status);
            ViewBag.Status = status;
            return View(companyResource);
        }

        // POST: CompanyResources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Position,DOJ,Phone,Address,Status")] CompanyResource companyResource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyResource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text", companyResource.Status);
            ViewBag.Status = status;
            return View(companyResource);
        }

        // GET: CompanyResources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            

            CompanyResource companyResource = db.CompanyResource.Find(id);
            if (companyResource == null)
            {
                return HttpNotFound();
            }

            ViewBag.ExistsProjectResource = false;
            ViewBag.ExistsSiteResource = false;

            var projectResourceList = db.ProjectResource.Where(x => x.CompanyResourceId == id).ToList();
            if (projectResourceList.Count != 0)
            {
                ViewBag.ExistsProjectResource = true;
                //ViewBag.Count = projectList.Count;
            }

            var siteResourceList = db.ProjectSiteResource.Where(x => x.CompanyResourceId == id).ToList();
            if (siteResourceList.Count != 0)
            {
                ViewBag.ExistsSiteResource = true;
                //ViewBag.Count = projectList.Count;
            }

            return View(companyResource);
        }

        // POST: CompanyResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var projectResourceList = db.ProjectResource.Where(x => x.CompanyResourceId == id).ToList();
            if (projectResourceList.Count != 0)
            {
                return RedirectToAction("Delete", new { id = id });
            }

            var siteResourceList = db.ProjectSiteResource.Where(x => x.CompanyResourceId == id).ToList();
            if (siteResourceList.Count != 0)
            {
                return RedirectToAction("Delete", new { id = id });
            }

            CompanyResource companyResource = db.CompanyResource.Find(id);
            db.CompanyResource.Remove(companyResource);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
