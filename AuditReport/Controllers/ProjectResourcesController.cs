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
    public class ProjectResourcesController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();

        // GET: ProjectResources
        public ActionResult Index()
        {
            var projectResource = db.ProjectResource.Include(p => p.Project);
            return View(projectResource.ToList());
        }

        // GET: ProjectResources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectResource projectResource = db.ProjectResource.Find(id);
            if (projectResource == null)
            {
                return HttpNotFound();
            }
            return View(projectResource);
        }

        // GET: ProjectResources/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            return View();
        }

        // POST: ProjectResources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,ResourceId")] ProjectResource projectResource)
        {
            if (ModelState.IsValid)
            {
                db.ProjectResource.Add(projectResource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectResource.ProjectId);
            return View(projectResource);
        }

        // GET: ProjectResources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectResource projectResource = db.ProjectResource.Find(id);
            if (projectResource == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectResource.ProjectId);
            return View(projectResource);
        }

        // POST: ProjectResources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectId,ResourceId")] ProjectResource projectResource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectResource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectResource.ProjectId);
            return View(projectResource);
        }

        // GET: ProjectResources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectResource projectResource = db.ProjectResource.Find(id);
            if (projectResource == null)
            {
                return HttpNotFound();
            }
            return View(projectResource);
        }

        // POST: ProjectResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectResource projectResource = db.ProjectResource.Find(id);
            db.ProjectResource.Remove(projectResource);
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
