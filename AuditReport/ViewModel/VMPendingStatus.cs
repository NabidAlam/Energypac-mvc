using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditReport.ViewModel
{
    public class VMPendingStatus
    {
        public string ProjectName { get; set; }
        public string SiteName { get; set; }
        public DateTime StatusDate { get; set; }
        public string InputBy { get; set; }
        public int PlanId { get; set; }
    }
}