using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditReport.ViewModel
{
    public class VMPlanTask
    {
        public int TaskId { get; set; }
        public string WBS { get; set; }
        public DateTime ? StartDate { get; set; }
        public DateTime ? EndDate  { get; set; }
        public string Remarks { get; set; }
    }

    public class VMPlanTaskNew
    {
        public int TaskId { get; set; }
        public string WBS { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }
        public int TempTaskId { get; set; }
        public int PlanTaskId { get; set; }
        public int ConId { get; set; }
        public int IndLevel { get; set; }
        public int WeightedAvg { get; set; }
        public string Milestone { get; set; }

    }

    public class VMPlanTaskDisp
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string WBS { get; set; }
        public DateTime?  StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }
        public int TempTaskId { get; set; }
        public int PlanTaskId { get; set; }
        public int ConId { get; set; }
        public int IndLevel { get; set; }
        public int WeightedAvg { get; set; }
        public int OrderId { get; set; }        
        public string MileStone { get; set; }
    }

    public class VMPlanTaskDispPrevStat
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string WBS { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }
        public int TempTaskId { get; set; }
        public int PlanTaskId { get; set; }
        public int ConId { get; set; }
        public int IndLevel { get; set; }
        public int WeightedAvg { get; set; }
        public int OrderId { get; set; }
        public string MileStone { get; set; }

        public string PrevStatus { get; set; }
        public DateTime? PrevStartDate { get; set; }
        public DateTime? PrevEndDate { get; set; }
        public string PrevPerComp { get; set; }        
    }


}