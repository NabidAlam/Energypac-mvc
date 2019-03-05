using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuditReport.Models;
using AuditReport.ReportDataSet;
using AuditReport.ViewModel;
using CrystalDecisions.Shared;
using System.Data;


namespace AuditReport.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {



        private readonly EPARSDbContext _db;

        public ReportController()
        {
            _db = new EPARSDbContext();
        }
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }


        //public ActionResult ProjectStatusReport()
        //{

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult ProjectStatusReport(string s)
        //{

        //    ReportDocument rd = new ReportDocument(); //
        //    rd.Load(Path.Combine(Server.MapPath("~/Report/ProjectStatusReport.rpt"))); 


        // //   var query = _db.refill_request.Where(y => y.type == "out" && (y.create_date.Date >= F && y.create_date.Date <= T)).Select(x => new { amount = (decimal?)x.amount ?? 0, prebalance = (decimal?)x.prebalance ?? 0, create_date = (DateTime?)x.create_date ?? DateTime.Now, type = x.type, ucid = _db.client.Where(k => k.id == x.ucid).Select(z => z.loginname).FirstOrDefault(), update_date = (DateTime?)x.update_date ?? DateTime.Now, note = x.note }).ToList();



        //    var query = _db.ProjectSiteStatus.Select(x => new { ProjectId = x.Project.Name, ProjectSiteId = x.ProjectSite.Name, Client = x.Project.Client.Name  }).ToList();




        //    rd.SetDataSource(query.ToList());
        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    //rd.SetParameterValue("Company", companyName.ToString());
        //    //rd.SetParameterValue("From", From.ToString("dd-MM-yyyy"));
        //    //rd.SetParameterValue("To", To.ToString("dd-MM-yyyy"));
        //    //rd.SetParameterValue("ReportName", "Fund Transfer Report");
        //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return File(stream, "application/pdf", "Project Site Status Report " + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf");
        //}

        [HttpGet]
        public ActionResult ProjectStatusPeriodReport()
        {
            return View();
        }

        [HttpPost]
        public string ProjectStatusPeriodReport(DateTime? from, DateTime? to)
        {
            //var rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Report"), "ProjectStatusPeriod.rpt"));
            //var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //var memoryStream = new MemoryStream();
            //stream.CopyTo(memoryStream);
            //var fileBuffer = memoryStream.ToArray();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-length", fileBuffer.Length.ToString());
            //Response.BinaryWrite(fileBuffer);

            return "hello";
        }


        //Added by Nabid August 3, 2017

        public void ExportProjectLists()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "ProjectListReport.rpt"));
            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);

        }



        public void ExportProjectSites()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "ProjectSiteReport.rpt"));
            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);

        }

        public void ProjectStatusReport()
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "ProjectsStatusReport.rpt"));
            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
        }

        public void SitePlanTaskReport()
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "SitePlanTaskReport.rpt"));
            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
        }

        [HttpGet]
        public ActionResult ProjectStatusSummary()
        {
            var months = new[]
            {
                new{MonthName="January", Value=1},  new{MonthName="February", Value=2},  new{MonthName="March", Value=3},  new{MonthName="April", Value=4},
                new{MonthName="May", Value=5},  new{MonthName="June", Value=6},  new{MonthName="July", Value=7},  new{MonthName="August", Value=8},
                new{MonthName="September", Value=9},  new{MonthName="October", Value=10},  new{MonthName="November", Value=11},  new{MonthName="December", Value=12}
            };
            var years = new List<int> { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029 };
            ViewBag.Years = new SelectList(years.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }), "Value", "Text", DateTime.Now.Year);
            ViewBag.Months = new SelectList(months, "Value", "MonthName", DateTime.Now.Month);
            return View();
        }


        [HttpPost]
        public ActionResult ProjectStatusSummary(int? month, int? year)
        {
            if (year == null || month == null) return RedirectToAction("ProjectStatusSummary");

            var projectResources = _db.ProjectResource
                .Join(_db.CompanyResource, pr => pr.CompanyResourceId, cr => cr.Id, (pr, cr) => new { pr, cr })
                .Select(x => new VProjectResource { ProjectId = x.pr.ProjectId, Name = x.cr.Name, Phone = x.cr.Phone });
            var siteResources = _db.ProjectSiteResource
                .Join(_db.CompanyResource, psr => psr.CompanyResourceId, cr => cr.Id, (psr, cr) => new { psr, cr })
                .Select(x => new VSiteResource { ProjectSiteId = x.psr.ProjectSiteId, Name = x.cr.Name, Phone = x.cr.Phone });

            var data = _db.ProjectSiteStatus.Take(4).OrderByDescending(x => x.SiteStatusDate)
                .Join(_db.ProjectSitePlanTask, pss => pss.ProjectSitePlanTaskId, pspt => pspt.Id, (pss, pspt) => new { pss, pspt })
                .Join(_db.ProjectTask, x => x.pspt.ProjectTaskId, pt => pt.Id, (x, pt) => new { pt, x.pspt, x.pss })
                .Join(_db.ProjectSitePlan, x => x.pspt.ProjectSitePlanId, psp => psp.Id, (x, psp) => new { psp, x.pspt, x.pss, x.pt })
                .Join(_db.ProjectSite, x => x.psp.ProjectSiteId, ps => ps.Id, (x, ps) => new { ps, x.pspt, x.pss, x.pt, x.psp })
                .Join(siteResources, x => x.ps.Id, sr => sr.ProjectSiteId, (x, sr) => new { sr, x.ps, x.pspt, x.pss, x.pt, x.psp })
                .Join(_db.Project, x => x.ps.ProjectId, p => p.Id, (x, p) => new { p, x.sr, x.ps, x.pspt, x.pss, x.pt, x.psp })
                .Join(projectResources, x => x.p.Id, pr => pr.ProjectId, (x, pr) => new { pr, x.sr, x.p, x.ps, x.pspt, x.pss, x.pt, x.psp })
                .Join(_db.Client, x => x.p.ClientId, c => c.Id, (x, c) => new { c, x.p, x.pr, x.sr, x.ps, x.pspt, x.pss, x.pt, x.psp })
                .Where(x => x.pss.SiteStatusDate.Month == month && x.pss.SiteStatusDate.Year == year)
                //.Join(_db.PreStatus, x => x.pss.Id, prevs => prevs.Id, (x, prevs) => new { prevs, x.c, x.p, x.pr, x.sr, x.ps, x.pspt, x.pss, x.pt, x.psp })
                .Select(x => new
                {
                    ProjectName = x.p.Name,
                    ClientName = x.c.Name,
                    ProEngName = x.pr.Name,
                    ProEngPhone = x.pr.Phone,
                    x.ps.Location,
                    SiteEngName = x.sr.Name,
                    SiteEngPhone = x.sr.Phone,
                    TaskName = x.pt.Name,
                    Status = x.pss.PresentStatus,
                    x.pss.StartDate,
                    x.pss.EndDate,
                    x.pss.SiteStatusDate,
                    ProjectId = x.p.Id,
                    ProjectSiteId = x.ps.Id,
                    ClientId = x.c.Id
                });


            DataTable dataTable = new ReportDataSource.ProjectStatusSummaryDataTable();

            var groupedData = data.Select(x => new
            {
                x.ProjectName,
                x.ClientName,
                x.Location,
                x.TaskName,
                x.StartDate,
                x.EndDate,
                x.SiteStatusDate,
                x.ProjectId,
                x.ProjectSiteId,
                x.ClientId
            }).Distinct().ToList();

            if (groupedData.Count < 1)
            {
                var dataRow = dataTable.NewRow();
                dataRow["Month"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)month);
                dataRow["Year"] = year.ToString();
                dataTable.Rows.Add(dataRow);
            }

            foreach (var row in groupedData)
            {
                var dataRow = dataTable.NewRow();
                dataRow["Project"] = row.ProjectName;
                dataRow["Client"] = row.ClientName;

                var projectManger = string.Join(";", data
                    .Where(x => x.ProjectId == row.ProjectId && x.ProjectSiteId == row.ProjectSiteId && x.ClientId == row.ClientId)
                    .Select(x => x.ProEngName + (x.ProEngPhone.Trim() != "" ? ("-" + x.ProEngPhone) : "")).Distinct().ToArray());
                var siteManger = string.Join(";", data
                   .Where(x => x.ProjectId == row.ProjectId && x.ProjectSiteId == row.ProjectSiteId && x.ClientId == row.ClientId)
                   .Select(x => x.SiteEngName + (x.SiteEngPhone.Trim() != "" ? ("-" + x.SiteEngPhone) : "")).Distinct().ToArray());
                var statuses = data
                    .Where(x => x.ProjectId == row.ProjectId && x.ProjectSiteId == row.ProjectSiteId && x.ClientId == row.ClientId)
                    .OrderByDescending(x => x.SiteStatusDate).Select(x => new { x.SiteStatusDate, x.Status }).Distinct().ToArray();


                dataRow["ProjectManager"] = projectManger;
                dataRow["SiteLocation"] = row.Location;
                dataRow["SiteManager"] = siteManger;
                dataRow["TaskName"] = row.TaskName;
                dataRow["StartDate"] = row.StartDate;
                dataRow["EndDate"] = row.EndDate;
                dataRow["EndDate"] = row.EndDate;

                if (statuses.Length > 0) dataRow["PresentStatus"] = statuses[0].SiteStatusDate.ToString("dd/MM/yyyy") + (statuses[0].Status == "" ? "" : ": " + Environment.NewLine + statuses[0].Status);
                else dataRow["PresentStatus"] = "";

                if (statuses.Length > 1) dataRow["Previous3"] = statuses[1].SiteStatusDate.ToString("dd/MM/yyyy") + (statuses[1].Status == "" ? "" : ": " + Environment.NewLine + statuses[1].Status);
                else dataRow["Previous3"] = "";

                if (statuses.Length > 2) dataRow["Previous2"] = statuses[2].SiteStatusDate.ToString("dd/MM/yyyy") + (statuses[2].Status == "" ? "" : ": " + Environment.NewLine + statuses[2].Status);
                else dataRow["Previous2"] = "";

                if (statuses.Length > 3) dataRow["Previous1"] = statuses[3].SiteStatusDate.ToString("dd/MM/yyyy") + (statuses[3].Status == "" ? "" : ": " + Environment.NewLine + statuses[3].Status);
                else dataRow["Previous1"] = "";

                dataRow["Month"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)month);
                dataRow["Year"] = year.ToString();

                dataTable.Rows.Add(dataRow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "ProjectStatusSummarizedReport.rpt"));
            rd.SetDataSource(dataTable);
            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;
        }



    }
}