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
using System.IO;
using PagedList;
namespace AuditReport.Controllers
{
    [Authorize]
    public class ProjectSitesController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();



        public JsonResult DeleteImages(int id)
        {
            bool flag=false;
            try
            {
                var images = db.ProjectSiteImage.Where(x=>x.Id==id).FirstOrDefault();
                var filename = images.ImageURL;
                db.ProjectSiteImage.Remove(images);
                flag=db.SaveChanges()>0;

                if (flag)
                {
                    string fullPath =  Request.MapPath(filename);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);                        
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }




        public JsonResult Addresource(int SiteId, int Rid)
        {
            ProjectSiteResource sr = new ProjectSiteResource();
            bool flag = false;
            try
            {
                //sr.ProjectId = ProjectId;
                sr.ProjectSiteId = SiteId;
                sr.CompanyResourceId = Rid;
                db.ProjectSiteResource.Add(sr);
                flag = db.SaveChanges() > 0;
            }
            catch(Exception ex)
            {

            }
         
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManualDeleteSiteResource(int resourceId,int Siteid)
        {
            //var data = db.ProjectSiteResource.Where(x => x.ComapanyResouceId == resourceId && x.ProjectSiteId == Siteid && x.ProjectId == projectId).FirstOrDefault();
            var data = db.ProjectSiteResource.Where(x => x.CompanyResourceId == resourceId && x.ProjectSiteId == Siteid).FirstOrDefault();
            db.ProjectSiteResource.Remove(data);
            bool flag = db.SaveChanges() >0;
            return RedirectToAction("Edit","ProjectSites", new { id=Siteid });
        }


        public JsonResult DeleteSites(int id)
        {
            //string result = "";

            var checkPlans = db.ProjectSitePlan.Where(x => x.ProjectSiteId == id).ToList();
            if (checkPlans.Count == 0)
            {
                bool flag = false;
                try
                {

                    var itemsToDeleteResources = db.ProjectSiteResource.Where(x => x.ProjectSiteId == id);
                    db.ProjectSiteResource.RemoveRange(itemsToDeleteResources);
                
                    var itemToDeleteSite = db.ProjectSite.Where(x => x.Id == id).FirstOrDefault();
                    db.ProjectSite.Remove(itemToDeleteSite);
                    
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
                        message = "Site deletion successful."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new
                    {
                        flag = false,
                        message = "Site deletion failed!\nError Occured."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Site deletion failed!\nDelete plan first."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: ProjectSites
        public ActionResult Index(int? page, int? ProjectId, int? SiteId)
        {

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            // var projectSitePlanTask = db.ProjectSitePlanTask.Include(p => p.Project).Include(p => p.ProjectSite);
            // var data = db.ProjectSitePlan.ToList();
            int pageSize = 50;
            int pageNumber = (page ?? 1);

            var projectSite = db.ProjectSite.Include(p => p.Project);

            if (ProjectId != null && SiteId != null)
            {
                projectSite = projectSite.Where(x => x.ProjectId == ProjectId && x.Id == SiteId);
            }
            else if (ProjectId != null)
            {
                projectSite = projectSite.Where(x => x.ProjectId == ProjectId);
            }
            else
            {
                return View(projectSite.ToList().ToPagedList(pageNumber, pageSize));
            }

            return View(projectSite.ToList().ToPagedList(pageNumber, pageSize));



            //var projectSite = db.ProjectSite.Include(p => p.Project).ToList();
            //int pageSize = 100;
            //int pageNumber = (page ?? 1);
            //return View(projectSite.ToPagedList(pageNumber, pageSize));
        }

        // GET: ProjectSites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectSite projectSite = db.ProjectSite.Where(x=>x.Id==id).FirstOrDefault();
            if (projectSite == null)
            {
                return HttpNotFound();
            }
            return View(projectSite);
        }

        // GET: ProjectSites/Create
        public ActionResult Create(int? id)
        {

           if(id!=null)
           {
               ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name",id);
           }
           else
           {
               ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
           }
            ViewBag.RName = new SelectList(db.CompanyResource, "Id", "Name");
            return View();
        }

        // POST: ProjectSites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SiteCreate(List<VMResource> ResourceDetails, int ProjectId, string Name, string Location, string Remarks, string Status)
        {

            var result = new
            {
                flag = false,
                message = "Error occured!"
            };

            ProjectSite projectSite = new ProjectSite();

            if (Name != "" && Location !="")
            {

                try
                {
                    projectSite.Location = Location;
                    projectSite.Name = Name;
                    projectSite.ProjectId = ProjectId;
                    projectSite.Remarks = Remarks;
                    projectSite.SiteStatus = (Status)Enum.Parse(typeof(Status), Status);
                    //projectSite.Id = db.ProjectSite.Select(x=>x.Id).DefaultIfEmpty(0).Max() + 1;                    

                    db.ProjectSite.Add(projectSite);
                    db.SaveChanges();
                    if (ResourceDetails != null)
                    {
                        foreach (var item in ResourceDetails)
                        {
                            //var companyresource = db.ComapanyResouce.Where(x => x.Id == item.Id).FirstOrDefault();
                            ProjectSiteResource Presource = new ProjectSiteResource();
                            //Presource.ProjectId = projectSite.ProjectId;
                            Presource.ProjectSiteId = projectSite.Id;
                            Presource.CompanyResourceId = item.Id;
                            db.ProjectSiteResource.Add(Presource);
                            db.SaveChanges();
                        }
                    }
                    result = new
                    {
                        flag = true,
                        message = "Saving successful"
                    };

                    //return Json(result, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {

                    result = new
                    {
                        flag = false,
                        message = "Saving Failed!!\nError occured !"
                    };
                }


            }
            else
            {
                result = new
                {
                    flag = false,
                    message = "Saving Failed!!\nSite Name or Location missing !"
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);

            //ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectSite.ProjectId);
            //ViewBag.RName = new SelectList(db.CompanyResource, "Id", "Name");
            //return View(projectSite);
        }


        // GET: ProjectSites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectSite projectSite = db.ProjectSite.Where(x=>x.Id==id).FirstOrDefault();
            if (projectSite == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectSite.ProjectId);
            ViewBag.RName = new SelectList(db.CompanyResource, "Id", "Name");
            return View(projectSite);
        }

        // POST: ProjectSites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectSite projectSite)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectSite).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectSite.ProjectId);
            return View(projectSite);
        }

        //GET: ProjectSites/Delete/5
        public ActionResult Delete(int? id)
        {

            var planList = db.ProjectSitePlan.Where(x => x.ProjectSiteId == id).ToList();
            if (planList != null)
            {
                ViewBag.Exists = true;
                ViewBag.Count = planList.Count;
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProjectSite projectSite = db.ProjectSite.Where(x => x.Id == id).FirstOrDefault();
            if (projectSite == null)
            {
                return HttpNotFound();
            }
            return View(projectSite);
        }

        // POST: ProjectSites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var planList = db.ProjectSitePlan.Where(x => x.ProjectSiteId == id).ToList();
            if (planList != null)
            {
                ViewBag.Exists = true;
                ViewBag.Count = planList.Count;
                return RedirectToAction("Delete", new {id = id});
            }
            else
            {
                //ProjectSite projectSite = db.ProjectSite.Where(x => x.Id == id).FirstOrDefault();
                //db.ProjectSite.Remove(projectSite);
                //db.SaveChanges();

                var itemsToDeleteResources = db.ProjectSiteResource.Where(x => x.ProjectSiteId == id);
                db.ProjectSiteResource.RemoveRange(itemsToDeleteResources);

                var itemToDeleteSite = db.ProjectSite.Where(x => x.Id == id).FirstOrDefault();
                db.ProjectSite.Remove(itemToDeleteSite);

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
