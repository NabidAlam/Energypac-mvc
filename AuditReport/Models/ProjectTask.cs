using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AuditReport.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<ProjectSitePlanTask> ProjectSitePlanTasks { get; set; }
    }
}