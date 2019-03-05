using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditReport.ViewModel
{
    public class VMNearTask
    {
        public string ProjectName { get; set; }
        public string SiteName { get; set; }
        public string TaskName { get; set; }
        public int PlanId { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}