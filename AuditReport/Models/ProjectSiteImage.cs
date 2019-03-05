using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditReport.Models
{
    public class ProjectSiteImage
    {
        public int Id { get; set; }
        public int  ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int ProjectSiteId { get; set; }
        public virtual ProjectSite ProjectSite { get; set; }
        public DateTime ? SitePlanDate { get; set; }
        public DateTime ImageDate { get; set; }
        public string ImageURL { get; set; }

    }
}