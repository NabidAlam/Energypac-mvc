using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

// ok
namespace AuditReport.Models
{
    public class ProjectSitePlanTask
    {
        //[Key, Column(Order =0 )]
        [Key]
        public int Id { get; set; } 
        [Required]
        public int ProjectSitePlanId { get; set; }
        public int ProjectTaskId { get; set; }
        public int OrderId { get; set; } 
        public string WBSCode { get; set; } 
        public int ControlId { get; set; } 
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        [StringLength(1)]
        public string MileStone { get; set; } 
        [StringLength(100)]
        public string Remarks { get; set; }
        public int WeightedAvg { get; set; }
        
        public virtual ProjectSitePlan ProjectSitePlan { get; set; }
        public virtual ProjectTask ProjectTask { get; set; }

        //public virtual ICollection<ProjectSiteStatus> ProjectSiteStatus { get; set; }

        //[Column(Order = 1)]
        //public int ProjectId { get; set; }
        //public virtual Project Project { get; set; }

        //[Key, Column(Order = 2)]
        //public int ProjectSiteId { get; set; }

        //public virtual ProjectSite ProjectSite { get; set; }

        //[Key, Column(Order = 3)]
        //public DateTime SitePlanDate { get; set; }

        //public string PlanCode { get; set; }

        //[Key, Column(Order = 4)]


    }
}