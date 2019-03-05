using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditReport.ViewModel
{
    public class VMPlanTaskStatus
    {        
        public int TaskId { get; set; }
        //public string WBS { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ?  CompletionDate { get; set; }
        public string Remarks { get; set; }
        public string PerComp { get; set; }
        public string PresentStatus { get; set; }
        public string Deviation { get; set; }
        public string ResDeviation { get; set; }
        public int PlanTaskId { get; set; }
        public int PlanTaskStatusId { get; set; }
        public DateTime TaskStatusDate { get; set; }
        public int ConId { get; set; }
    }

    public class VMPlanTaskStatusFull
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string WBS { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }
        public int PlanTaskId { get; set; }
        public int ConId { get; set; }
        public int IndLevel { get; set; }
        public int WeightedAvg { get; set; }
        public string MileStone { get; set; }

        public int PlanTaskStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ?  CompletionDate { get; set; }
        public string Remarks { get; set; }
        public string PerComp { get; set; }
        public string PresentStatus { get; set; }
        public string Deviation { get; set; }
        public string ResDeviation { get; set; }

        public bool? IsAuth { get; set; }

        public string PrevStatus { get; set; }
        public DateTime? PrevStartDate { get; set; }
        public DateTime? PrevEndDate { get; set; }
        public string PrevPerComp { get; set; }

        
    }
}