using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AuditReport.Models
{
    // -- ok
    public class ProjectSiteStatus
    {
        [Key]
        public int Id { get; set; } //--
        [Required]
        public int ProjectSitePlanTaskId { get; set; } //--

        //[Key, Column(Order =0 )]
        //public int ProjectId { get; set; }

        //public virtual Project Project { get; set; }

        //[Key, Column(Order = 1)]
        //public int ProjectSiteId { get; set; }

        //public virtual ProjectSite ProjectSite { get; set; }

        //[Key, Column(Order = 2)]
        //public DateTime SitePlanDate { get; set; }

        //public string PlanCode { get; set; }

        //public virtual ProjectSitePlan ProjectSitePlan { get; set; }

        //[Key, Column(Order=4)]
        public DateTime SiteStatusDate { get; set; } //--

        //public string WBSCode { get; set; }

        //[Key, Column(Order = 3)]
        //public int ProjectTaskId { get; set; }

        //public virtual ProjectTask ProjectTask { get; set; }

        //public int ControlId { get; set; }

        public DateTime? StartDate { get; set; } //--
        public DateTime? EndDate { get; set; } //--
        public string Percentage { get; set; } //--
        
        //[StringLength(25)]
        //public string CompletionPer { get; set; }
        public DateTime ? CompletionDate { get; set; } //--
        [StringLength(100)]
        public string Remarks { get; set; } //--
        public string PresentStatus { get; set; } //--
        public string Deviation { get; set; } //--
        [Display(Name="Reason for Deviation")] //--
        public string ResDeviation { get; set; } //--
        public DateTime ? ActualCompletionDate { get; set; } //--

        //IS_AUTH,INSERTED_BY,INSERTED_ON,AUTH_BY,AUTH_ON 
        public bool? IsAuth { get; set; }
        public int? InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public int? AuthBy { get; set; }
        public DateTime? AuthOn { get; set; }

        public virtual ProjectSitePlanTask ProjectSitePlanTask { get; set; }

    }
}