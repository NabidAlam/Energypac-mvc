using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using AuditReport.Models;
using AuditReport.ViewModel;
namespace AuditReport.Controllers
{
    [Authorize]
    public class ToolsController : Controller
    {

        private EPARSDbContext db = new EPARSDbContext();
        // GET: Tools
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportData(HttpPostedFileBase excelfile)
        {

            // //for staff
            // string path_staff = Server.MapPath("~/Content/staff"  + DateTime.Now.Millisecond+".xlsx");
            // Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            // Excel.Workbook xlWorkBook_staff;
            // Excel.Worksheet xlWorkSheet_staff;
            // object misValue = System.Reflection.Missing.Value;

            // xlWorkBook_staff = xlApp.Workbooks.Add(misValue);
            // xlWorkSheet_staff = (Excel.Worksheet)xlWorkBook_staff.Worksheets.get_Item(1);
            // xlWorkSheet_staff.Cells[1, 1] = "Designation";
            // xlWorkSheet_staff.Cells[1, 2] = "Name";
            // xlWorkSheet_staff.Cells[1, 3] = "Phone";
            //// xlWorkBook_staff.SaveAs("~/Content/Staff" + DateTime.Now.Millisecond + ".xlsx", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            // xlWorkBook_staff.SaveAs(path_staff);
            // xlWorkBook_staff.Close();
            // xlApp.Quit();



            // List<VMStaff> StaffList = new List<VMStaff>();


            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please select a excel file !";
                return View();
            }
            if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
            {
                string path = Server.MapPath("~/upload/" + DateTime.Now.Millisecond + excelfile.FileName);
                excelfile.SaveAs(path);
                Excel.Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Open(path);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                Excel.Range range = worksheet.UsedRange;

                //parameters for insertion

                int clientId = 0;
                int projectId = 0;
                int siteId = 0;
                int CompanyResourceId = 0;


                for (var row = 10; row <= range.Rows.Count; row++)
                {

                    // parameter from excel
                    string clientParameter = ((Excel.Range)range.Cells[row, 3]).Text;
                    if (clientParameter == "")
                    {
                        clientParameter = "-";
                    }

                    int projectGroupId = db.ProjectGroup.Select(x => x.Id).FirstOrDefault();
                    if (projectGroupId == 0)
                    {
                        var p = new ProjectGroup { Name = "-" };
                        db.ProjectGroup.Add(p);
                        db.SaveChanges();
                        projectGroupId = p.Id;
                    }
                    string projectParameter = ((Excel.Range)range.Cells[row, 2]).Text;


                    string siteParameter = ((Excel.Range)range.Cells[row, 6]).Text;

                    string staffNameParameter = ((Excel.Range)range.Cells[row, 8]).Text;
                    string staffDesignationParameter = ((Excel.Range)range.Cells[row, 9]).Text;
                    string staffPhoneParameter = ((Excel.Range)range.Cells[row, 10]).Text;

                    //check client
                    if (clientParameter != "")
                    {
                        var clientdata = db.Client.FirstOrDefault(x => x.Name == clientParameter.Trim());

                        //if clientdata is not exists in db
                        if (clientdata == null)
                        {
                            var client = new Client { Name = clientParameter.Trim() };
                            db.Client.Add(client);
                            db.SaveChanges();
                            clientId = client.Id;

                        }
                        else // if data found
                        {
                            clientId = clientdata.Id;
                        }
                    }



                    //check project
                    if (projectParameter != "")
                    {
                        var projectdata = db.Project.FirstOrDefault(x => x.Name == projectParameter.Trim());

                        //if project data is not exists in db
                        if (projectdata == null)
                        {
                            var project = new Project
                            {
                                Name = projectParameter.Trim(),
                                ProjectGroupId = projectGroupId,
                                ClientId = clientId,
                                Status = 0
                            };
                            db.Project.Add(project);
                            db.SaveChanges();
                            projectId = project.Id;
                        }
                        else
                        {
                            projectId = projectdata.Id;
                        }
                    }



                    //check projectSite
                    if (siteParameter != "")
                    {
                        var sitedata = db.ProjectSite.FirstOrDefault(x => x.Name == siteParameter.Trim());

                        //if projectsite data is not exists in db
                        if (sitedata == null)
                        {
                            var site = new ProjectSite
                            {
                                Location = siteParameter.Trim(),
                                Name = siteParameter.Trim(),
                                SiteStatus = 0,
                                ProjectId = projectId
                            };
                            db.ProjectSite.Add(site);
                            db.SaveChanges();
                            siteId = site.Id;
                        }
                        else
                        {
                            siteId = sitedata.Id;
                        }
                    }


                    //add staff to list


                    if (staffNameParameter == "") continue;
                    var companyResource = db.CompanyResource.FirstOrDefault(x => x.Name == staffNameParameter);

                    //if companyResource data is not exists in db
                    if (companyResource == null)
                    {
                        var c = new CompanyResource
                        {
                            Name = staffNameParameter,
                            Phone = staffPhoneParameter,
                            Position = staffDesignationParameter,
                            Status = "A"
                        };
                        db.CompanyResource.Add(c);
                        db.SaveChanges();
                        CompanyResourceId = c.Id;
                    }
                    else
                    {
                        CompanyResourceId = companyResource.Id;
                    }

                    var checkProjectResource = db.ProjectResource.FirstOrDefault(x => x.CompanyResourceId == CompanyResourceId && x.ProjectId == projectId);
                    if (checkProjectResource == null)
                    {
                        var pr = new ProjectResource
                        {
                            CompanyResourceId = CompanyResourceId,
                            ProjectId = projectId
                        };
                        db.ProjectResource.Add(pr);
                        db.SaveChanges();
                    }


                    var checkSiteResource = db.ProjectSiteResource.FirstOrDefault(x => x.CompanyResourceId == CompanyResourceId && x.ProjectSiteId == siteId);

                    if (checkSiteResource != null) continue;
                    var psr = new ProjectSiteResource
                    {
                        CompanyResourceId = CompanyResourceId,
                        ProjectSiteId = siteId
                    };
                    //psr.ProjectId = projectId;
                    db.ProjectSiteResource.Add(psr);
                    db.SaveChanges();
                }
                workbook.Close();
                application.Quit();
                System.IO.File.Delete(path);
                return View();
            }
            ViewBag.Error = "File Type is incorrect !";
            return View();
        }

        //[HttpPost]
        //public ActionResult ImportData(HttpPostedFileBase excelfile)
        //{

        //   // //for staff
        //   // string path_staff = Server.MapPath("~/Content/staff"  + DateTime.Now.Millisecond+".xlsx");
        //   // Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        //   // Excel.Workbook xlWorkBook_staff;
        //   // Excel.Worksheet xlWorkSheet_staff;
        //   // object misValue = System.Reflection.Missing.Value;

        //   // xlWorkBook_staff = xlApp.Workbooks.Add(misValue);
        //   // xlWorkSheet_staff = (Excel.Worksheet)xlWorkBook_staff.Worksheets.get_Item(1);
        //   // xlWorkSheet_staff.Cells[1, 1] = "Designation";
        //   // xlWorkSheet_staff.Cells[1, 2] = "Name";
        //   // xlWorkSheet_staff.Cells[1, 3] = "Phone";
        //   //// xlWorkBook_staff.SaveAs("~/Content/Staff" + DateTime.Now.Millisecond + ".xlsx", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //   // xlWorkBook_staff.SaveAs(path_staff);
        //   // xlWorkBook_staff.Close();
        //   // xlApp.Quit();



        //   // List<VMStaff> StaffList = new List<VMStaff>();


        //    if (excelfile == null || excelfile.ContentLength == 0)
        //    {
        //        ViewBag.Error = "Please select a excel file !";
        //        return View();
        //    }
        //    else
        //    {
        //        if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
        //        {
        //            string path = Server.MapPath("~/upload/"+DateTime.Now.Millisecond + excelfile.FileName);
        //            excelfile.SaveAs(path);
        //            Excel.Application application = new Excel.Application();
        //            Excel.Workbook workbook = application.Workbooks.Open(path);
        //            Excel.Worksheet worksheet = workbook.ActiveSheet;
        //            Excel.Range range = worksheet.UsedRange;

        //            //parameters for insertion

        //            int clientId = 0;
        //            int projectId = 0;
        //            int siteId = 0;
        //            int CompanyResourceId = 0;


        //            for (int row = 10; row <= range.Rows.Count; row++)
        //            {




        //                // parameter from excel
        //                string clientParameter = ((Excel.Range)range.Cells[row, 3]).Text;
        //                if(clientParameter=="")
        //                {
        //                    clientParameter = "-";
        //                }

        //                int projectGroupId = db.ProjectGroup.Select(x => x.Id).FirstOrDefault();
        //                if(projectGroupId==0)
        //                {
        //                    ProjectGroup p = new ProjectGroup();
        //                    p.Name = "-";
        //                    db.ProjectGroup.Add(p);
        //                    db.SaveChanges();
        //                    projectGroupId = p.Id;
        //                }
        //                string projectParameter = ((Excel.Range)range.Cells[row, 2]).Text;


        //                string siteParameter = ((Excel.Range)range.Cells[row, 6]).Text;

        //                string staffNameParameter = ((Excel.Range)range.Cells[row, 8]).Text;
        //                 string staffDesignationParameter = ((Excel.Range)range.Cells[row, 9]).Text;
        //                 string staffPhoneParameter = ((Excel.Range)range.Cells[row, 10]).Text;
        //                //check client
        //                if(clientParameter !="")
        //                {
        //                    var clientdata = db.Client.Where(x => x.Name == clientParameter.Trim()).FirstOrDefault();

        //                    //if clientdata is not exists in db
        //                    if (clientdata== null)
        //                    {
        //                        Client client = new Client();
        //                        client.Name = clientParameter.Trim();
        //                        db.Client.Add(client);
        //                        db.SaveChanges();
        //                        clientId = client.Id;

        //                    }
        //                    else // if data found
        //                    {
        //                        clientId = clientdata.Id;
        //                    }
        //                }



        //                //check project
        //                if(projectParameter!="")
        //                {
        //                    var projectdata = db.Project.Where(x => x.Name == projectParameter.Trim()).FirstOrDefault();

        //                    //if project data is not exists in db
        //                    if(projectdata == null)
        //                    {
        //                        Project project = new Project();
        //                        project.Name = projectParameter.Trim();
        //                        project.ProjectGroupId = projectGroupId;
        //                        project.ClientId = clientId;
        //                        project.Status = 0;
        //                        db.Project.Add(project);
        //                        db.SaveChanges();
        //                        projectId = project.Id;
        //                    }
        //                    else
        //                    {
        //                        projectId = projectdata.Id;
        //                    }
        //                }



        //                //check projectSite
        //                if (siteParameter != "")
        //                {
        //                    var sitedata = db.ProjectSite.Where(x => x.Name == siteParameter.Trim()).FirstOrDefault();

        //                    //if projectsite data is not exists in db
        //                    if (sitedata == null)
        //                    {
        //                        ProjectSite site = new ProjectSite();
        //                        site.Location = siteParameter.Trim();
        //                        site.Name = siteParameter.Trim();
        //                        site.SiteStatus = 0;
        //                        site.ProjectId = projectId;
        //                        db.ProjectSite.Add(site);
        //                        db.SaveChanges();
        //                        siteId = site.Id;
        //                    }
        //                    else
        //                    {
        //                        siteId = sitedata.Id;
        //                    }
        //                }


        //                //add staff to list


        //                if (staffNameParameter != "")
        //                {
        //                    var companyResource = db.ComapanyResouce.Where(x => x.Name == staffNameParameter).FirstOrDefault();

        //                    //if companyResource data is not exists in db
        //                    if(companyResource == null)
        //                    {
        //                        CompanyResource c = new CompanyResource();
        //                        c.Name = staffNameParameter;
        //                        c.Phone = staffPhoneParameter;
        //                        c.Position = staffDesignationParameter;
        //                        c.Status = "A";
        //                        db.ComapanyResouce.Add(c);
        //                        db.SaveChanges();
        //                        CompanyResourceId = c.Id;
        //                    }
        //                    else
        //                    {
        //                        CompanyResourceId = companyResource.Id;
        //                    }

        //                    var checkProjectResource = db.ProjectResource.Where(x => x.ComapanyResouceId == CompanyResourceId && x.ProjectId == projectId).FirstOrDefault();
        //                    if(checkProjectResource ==null)
        //                    {
        //                        ProjectResource pr = new ProjectResource();
        //                        pr.ComapanyResouceId = CompanyResourceId;
        //                        pr.ProjectId = projectId;
        //                        db.ProjectResource.Add(pr);
        //                        db.SaveChanges();
        //                    }


        //                    var checkSiteResource = db.ProjectSiteResource.Where(x => x.ComapanyResouceId == CompanyResourceId && x.ProjectId == projectId && x.ProjectSiteId == siteId).FirstOrDefault();

        //                    if(checkSiteResource==null)
        //                    {
        //                        ProjectSiteResource psr = new ProjectSiteResource();
        //                        psr.ComapanyResouceId = CompanyResourceId;
        //                        psr.ProjectId = projectId;
        //                        psr.ProjectSiteId = siteId;
        //                        db.ProjectSiteResource.Add(psr);
        //                        db.SaveChanges();
        //                    }




        //                }






        //            }
        //            workbook.Close();
        //            application.Quit();
        //            System.IO.File.Delete(path);
        //            return View();
        //        }
        //        else
        //        {
        //            ViewBag.Error = "File Type is incorrect !";
        //            return View();
        //        }
        //    }
        //}

    }
}