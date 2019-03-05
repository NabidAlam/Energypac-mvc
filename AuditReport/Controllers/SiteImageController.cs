using AuditReport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuditReport.Controllers
{
    [Authorize]
    public class SiteImageController : Controller
    {
        // GET: SiteImage
        private EPARSDbContext db = new EPARSDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ImageUpload(int ProjectId, int SiteId)
        {
            ViewBag.Project = db.Project.Where(x => x.Id == ProjectId).Select(y => y.Name).FirstOrDefault();
            ViewBag.Site = db.ProjectSite.Where(x => x.Id == SiteId).Select(y => y.Name).FirstOrDefault();

            return View();
        }
        [HttpPost]
        public ActionResult Upload(decimal ? ProjectId, decimal ? SiteId, DateTime ? PlanDate)
        {

            bool isSavedSuccessfully = true;
           // bool saveinfolder = false;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Path.Combine(Server.MapPath("~/MyImages"));
                        string pathString = System.IO.Path.Combine(path.ToString());
                        var fileName1 = Path.GetFileName(file.FileName);
                        bool isExists = System.IO.Directory.Exists(pathString);
                        if (!isExists) System.IO.Directory.CreateDirectory(pathString);
                        {
                            var uploadpath = string.Format("{0}\\{1}", pathString, file.FileName);
                            file.SaveAs(uploadpath);
                            //save in db
                            ProjectSiteImage upload = new ProjectSiteImage();
                            string imagepath = "~/MyImages/" + file.FileName;
                            upload.ImageURL = imagepath;
                            upload.ImageDate = DateTime.Now;
                            upload.ProjectId =Convert.ToInt32( ProjectId);
                            upload.ProjectSiteId = Convert.ToInt32( SiteId);
                            upload.SitePlanDate = PlanDate;
                            db.ProjectSiteImage.Add(upload);
                            db.SaveChanges();
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new
                {
                    Message = fName
                });
            }
            else
            {
                return Json(new
                {
                    Message = "Error in saving file"
                });
            }
        }



        public ActionResult Gallery()
        {
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Gallery(int ProjectId, int SiteId, string Type)
        {
            if(Type=="Image")
            {
                return RedirectToAction("ImageGallery", new { ProjectId = ProjectId, SiteId = SiteId });
            }
            else
            {
                return RedirectToAction("VideoGallery", new { ProjectId = ProjectId, SiteId = SiteId });
            }

           
        }


        public ActionResult ImageGallery(int ProjectId, int SiteId)
        {
            ViewBag.ProjectId = ProjectId;
            ViewBag.SiteId = SiteId;
            
            return View();
        }

        public ActionResult VideoGallery(int ProjectId, int SiteId)
        {
            ViewBag.ProjectId = ProjectId;
            ViewBag.SiteId = SiteId;

            return View();
        }



    }
}