using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditReport.ViewModel
{
    public class VMStaff
    {
        public int projectId { get; set; }
        public int siteId { get; set; }
        public string Designation { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}