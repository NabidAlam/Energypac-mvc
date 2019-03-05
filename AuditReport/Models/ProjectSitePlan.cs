using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
// ok
namespace AuditReport.Models
{
    public class ProjectSitePlan
    {
        [Key]        
        public int Id { get; set; }                
        //[Required]
        //[Column(Order = 0)]
        //public int ProjectId { get; set; }                        
        [Required]
        //[Column(Order = 1)]
        public int ProjectSiteId { get; set; }                
        [Required]
        //[Column(Order = 2)]
        public DateTime SitePlanDate { get; set; }
        [StringLength(20)]
        [Index("PlanCodeIndex", IsUnique = true)]
        public string PlanCode { get; set; }
        [StringLength(1)]
        public string Active { get; set; }
        [StringLength(100)]
        public string Remarks { get; set; }

        //public virtual Project Project { get; set; }
        public virtual ProjectSite ProjectSite { get; set; }

        public virtual ICollection<ProjectSitePlanTask> ProjectSitePlanTasks { get; set; }
    
    }
}