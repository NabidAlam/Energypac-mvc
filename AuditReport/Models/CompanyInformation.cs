﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
//-- ok
namespace AuditReport.Models
{
    public class CompanyInformation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
        [StringLength(100)]
        [Display(Name="Contact Person")]
        public string ContactPerson { get; set; }
        [StringLength(100)]
        public string Address { get; set; }

    }
}