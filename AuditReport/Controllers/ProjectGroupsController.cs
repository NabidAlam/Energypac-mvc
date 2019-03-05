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
    public class ProjectGroupsController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();

        // GET: ProjectGroups
        public ActionResult Index()
        {
            return View(db.ProjectGroup.ToList());
        }

        // GET: ProjectGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectGroup projectGroup = db.ProjectGroup.Find(id);
            if (projectGroup == null)
            {
                return HttpNotFound();
            }
            return View(projectGroup);
        }

        // GET: ProjectGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ProjectGroup projectGroup)
        {
            if (ModelState.IsValid)
            {
                db.ProjectGroup.Add(projectGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projectGroup);
        }

        // GET: ProjectGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectGroup projectGroup = db.ProjectGroup.Find(id);
            if (projectGroup == null)
            {
                return HttpNotFound();
            }
            return View(projectGroup);
        }

        // POST: ProjectGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] ProjectGroup projectGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projectGroup);
        }

        // GET: ProjectGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var projectList = db.Project.Where(x => x.ProjectGroupId  == id).ToList();
            if (projectList.Count != 0)
            {
                ViewBag.Exists = true;
                ViewBag.Count = projectList.Count;
            }

            ProjectGroup projectGroup = db.ProjectGroup.Find(id);

            if (projectGroup == null)
            {
                return HttpNotFound();
            }

            return View(projectGroup);
        }

        // POST: ProjectGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var projectList = db.Project.Where(x => x.ProjectGroupId == id).ToList();
            if (projectList.Count != 0)
            {
                //ViewBag.Exists = true;
                //ViewBag.Count = projectList.Count;
                return RedirectToAction("Delete", new { id = id });
            }
            else
            {
                ProjectGroup projectGroup = db.ProjectGroup.Find(id);
                db.ProjectGroup.Remove(projectGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
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
