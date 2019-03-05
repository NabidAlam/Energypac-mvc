using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AuditReport.Models;
using AuditReport.ViewModel;
using System.Data.Entity.Validation;
using PagedList;

namespace AuditReport.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();



        [HttpPost]  
        public ActionResult Sites (int ProjectId)  
        {  

            List<SelectListItem> sitetNames = new List<SelectListItem>();  

                var sites = db.ProjectSite.Where(x=>x.ProjectId==ProjectId).ToList();

            foreach(var x in sites)
            {
                sitetNames.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            }
            return Json(sitetNames, JsonRequestBehavior.AllowGet);  
        }  



        public JsonResult Addresource(int ProjectId, int Rid)
        {
           
            bool flag=false;
            try
            {
                ProjectResource sr = new ProjectResource();
                sr.ProjectId = ProjectId;
                sr.CompanyResourceId = Rid;
                db.ProjectResource.Add(sr);
                db.SaveChanges();
                flag = true;
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
          
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManualDeleteProjectResource(int projectId, int resourceId)
        {
            var data = db.ProjectResource.Where(x => x.CompanyResourceId == resourceId &&  x.ProjectId == projectId).FirstOrDefault();
            db.ProjectResource.Remove(data);
            bool flag = db.SaveChanges() > 0;
            return RedirectToAction("Edit", "Projects", new { id = projectId });
        }

        public JsonResult GetResources(int ? id)
        {
            var data = db.CompanyResource.Where(x => x.Id == id).Select(y => new { Name = y.Name, Position = y.Position, Id = y.Id }).FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);

        }




        // GET: Projects
        public ActionResult Index(int? page, int? ProjectId)
        {

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");

             var projects = db.Project.Include(p => p.Client).Include(p => p.ProjectGroup).ToList();


            

            if (ProjectId != null)
            {
                projects = projects.Where(x => x.Id == ProjectId).ToList();
            }


            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(projects.ToPagedList(pageNumber, pageSize));
            //var project = db.Project.Include(p => p.Client).Include(p => p.ProjectGroup).ToList();



            ////if (ProjectId != null)
            ////{
            ////    project = project.Where(x => x.Id == ProjectId).ToList();
            ////}


            //int pageSize = 100;
            //int pageNumber = (page ?? 1);
            //return View(project.ToPagedList(pageNumber, pageSize));
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name");
            ViewBag.ProjectGroupId = new SelectList(db.ProjectGroup, "Id", "Name");
            ViewBag.RName = new SelectList(db.CompanyResource, "Id", "Name");
            return View();
        }



        // POST: Projects/ProjectCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult ProjectCreate(List<VMResource> ResourceDetails, int ? ClientId, DateTime? EndDate, string Name, int ? ProjectGroupId, string Remarks, DateTime ? StartDate, string Status)
        {

            var result = new
            {
                flag = false,
                message = "Saving failed"
            };

            if (Name.Trim() != "" )
            {
                Project project = new Project();

                try
                {
                    project.ClientId = ClientId;
                    project.EndDate = EndDate;
                    project.Name = Name;
                    project.ProjectGroupId = ProjectGroupId;
                    project.Remarks = Remarks;
                    project.StartDate = StartDate;
                    project.Status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), Status);

                    db.Project.Add(project);
                    db.SaveChanges();

                    if (ResourceDetails != null)
                    {
                        foreach (var item in ResourceDetails)
                        {                            
                            ProjectResource Presource = new ProjectResource();
                            Presource.ProjectId = project.Id;
                            Presource.CompanyResourceId = item.Id;
                            db.ProjectResource.Add(Presource);
                            db.SaveChanges();
                        }
                    }

                    result = new
                    {
                        flag = true,
                        message = "Project saving successful."
                    };

                }
                catch
                {
                    result = new
                    {
                        flag = false,
                        message = "Saving failed! Error occured."
                    };

                } 
                 
            }
            else
            {
                result = new
                {
                    flag = false,
                    message = "Saving failed!\nProject name required."
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);

            //ViewBag.ClientId = new SelectList(db.Client, "Id", "Name", project.ClientId);
            //ViewBag.ProjectGroupId = new SelectList(db.ProjectGroup, "Id", "Name", project.ProjectGroupId);
            //ViewBag.RName = new SelectList(db.CompanyResource, "Id", "Name");
            //return View(project);

        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name", project.ClientId);
            ViewBag.ProjectGroupId = new SelectList(db.ProjectGroup, "Id", "Name", project.ProjectGroupId);
            ViewBag.RName = new SelectList(db.CompanyResource, "Id", "Name");
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ClientId,ProjectGroupId,StartDate,EndDate,Status,Remarks")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Client, "Id", "Name", project.ClientId);
            ViewBag.ProjectGroupId = new SelectList(db.ProjectGroup, "Id", "Name", project.ProjectGroupId);
            return View(project);
        }

        //// GET: Projects/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var siteList = db.ProjectSite.Where(x => x.ProjectId == id).ToList();
        //    if(siteList.Count != 0)
        //    {
        //        ViewBag.Exists = true;
        //        ViewBag.Count = siteList.Count;
        //    }
            
        //    Project project = db.Project.Find(id);
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(project);
        //}

        //// POST: Projects/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Project project = db.Project.Find(id);
        //    db.Project.Remove(project);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}




        public JsonResult DeleteProjects(int id)
        {
            //string result = "";
          
            var checkSites = db.ProjectSite.Where(x => x.ProjectId == id).ToList();
            if (checkSites.Count == 0)
            {
                bool flag = false;
                try
                {

                    //Project project = db.Project.Find(id);
                    //db.Project.Remove(project);

                    var itemsToDeleteResources = db.ProjectResource.Where(x => x.ProjectId == id);
                    db.ProjectResource.RemoveRange(itemsToDeleteResources);

                    var itemToDeleteProject = db.Project.Where(x => x.Id == id).FirstOrDefault();
                    db.Project.Remove(itemToDeleteProject);

                    flag = db.SaveChanges()>0;                  

                }
                catch (Exception ex)
                {

                }

                if (flag)
                {
                    var result = new
                    {
                        flag = true,
                        message = "Project deletion successful."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new
                    {
                        flag = false,
                        message = "Project deletion failed!\nError Occured."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Project deletion failed!\nDelete project site first."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
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
