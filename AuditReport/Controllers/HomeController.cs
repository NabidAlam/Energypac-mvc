using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;



namespace AuditReport.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:12351");
                var result = client.PostAsync("/api/contacts", new
                {
                   Name="sdfd",
                   Date = DateTime.Now
                }, new JsonMediaTypeFormatter()).Result;
               
            }

            return View();
        }
    }
}
