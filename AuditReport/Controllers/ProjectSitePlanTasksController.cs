using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AuditReport.Models;
using AuditReport.ViewModel;
using PagedList;
using AuditReport.Helper;

namespace AuditReport.Controllers
{
    [Authorize]
    public class ProjectSitePlanTasksController : Controller
    {
        private EPARSDbContext db = new EPARSDbContext();
        private Dictionary<int, int> _dictIndent =
                    new Dictionary<int, int>();
        private int _pageSize = 50;

        #region user defined code

        public JsonResult editTask(int id, string name) 
        {
            var data = db.ProjectTask.Where(x => x.Id == id).FirstOrDefault();
            ProjectTask pt = new ProjectTask();
            pt.Name = name;
            pt.Id = id;
            var dataentry = db.Entry(data);
            dataentry.CurrentValues.SetValues(pt);
            bool flag = db.SaveChanges() > 0;

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult EditPlanTask(int PId, int ProjectId, int SiteId)
        {

            //var data = db.ProjectSitePlanTask.Where(x => x.ProjectId == ProjectId && x.ProjectSiteId == SiteId && x.SitePlanDate == PlanDate).FirstOrDefault();
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ProjectId);
            ViewBag.SiteId = new SelectList(db.ProjectSite, "Id", "Name", SiteId);
            //ViewBag.PlanCode = db.ProjectSitePlan.Where(x => x.Id == Pid).Select(y => y.PlanCode).FirstOrDefault();
            ViewBag.PId = PId;
            var sitePlan = db.ProjectSitePlan.FirstOrDefault(x => x.Id == PId);
            ViewBag.PlanCode = sitePlan.PlanCode;
            ViewBag.PlanDate = NullHelper.DateToString(sitePlan.SitePlanDate);

            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text", sitePlan.Active??"I");
            ViewBag.Status = status;

            return View();
        }



        private int GetIndentStringLevel(int id)
        {
            if (id == 0)
            {
                return 0;
            }

            if (!_dictIndent.ContainsKey(id))
            {
                return 0;
            }

            return GetIndentStringLevel(_dictIndent[id]) + 1;
        }

        //public JsonResult getEditPlanTask(int PId, int ProjectId, int SiteId)
        public JsonResult getEditPlanTask(int PId)
        {
            bool flag = false;
            //var checkStatus = db.ProjectSiteStatus.Where(x => x.ProjectId == ProjectId && x.ProjectSiteId == SiteId && x.SitePlanDate == PlanDate).Count();
            //if (checkStatus > 0)
            //{
            //    flag = true;
            //}
            //var data = db.ProjectSitePlanTask.Where(x => x.ProjectId == ProjectId && x.ProjectSiteId == SiteId && x.SitePlanDate == PlanDate).Select(y => new { TaskId = y.ProjectTaskId, Name = db.ProjectTask.Where(t => t.Id == y.ProjectTaskId).Select(tc => tc.Name).FirstOrDefault(), WBS = y.WBSCode, StartDate = y.StartDate, EndDate = y.EndDate, Remarks = y.Remarks, OrderId = y.OrderId }).OrderBy(o => o.OrderId).ToList();
            var data = db.ProjectSitePlanTask.Where(x => x.ProjectSitePlanId  == PId).Select(y => new { PlanTaskId=y.Id, ConId = y.ControlId, TaskId = y.ProjectTaskId, Name = db.ProjectTask.Where(t => t.Id == y.ProjectTaskId).Select(tc => tc.Name).FirstOrDefault(), WBS = y.WBSCode, StartDate = y.StartDate, EndDate = y.EndDate, Remarks = y.Remarks, OrderId = y.OrderId, WeightedAvg = y.WeightedAvg, Milestone = y.MileStone}).OrderBy(o => o.OrderId).ToList();

            _dictIndent = new Dictionary<int, int>();

            Dictionary<int,int> dictControl = new Dictionary<int, int>();

            int i = 0;
            dictControl.Add(0, 0);
            foreach (var item in data)
            {
                i++;

                _dictIndent.Add(item.PlanTaskId, item.ConId);

                dictControl.Add(item.PlanTaskId, i);                                            

            }                  

            var refinedata = data.AsEnumerable().Select(x => new { PlanTaskId = x.PlanTaskId, ConId = dictControl[x.ConId], IndLevel = GetIndentStringLevel(x.ConId), TaskId = x.TaskId, Name = x.Name, WBS = x.WBS ?? "", StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : "", EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("dd/MM/yyyy") : "", Remarks = x.Remarks ?? "", OrderId = x.OrderId, WeightedAvg = x.WeightedAvg, Milestone = x.Milestone ?? "N", flag = flag });

            return Json(refinedata, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteStatus(int PId, DateTime SiteStatusDate)
        {
            
            bool flag = false;
            try
            {
                var deletedItem = from siteTask in db.ProjectSitePlanTask
                                  join siteStatus in db.ProjectSiteStatus on siteTask.Id equals siteStatus.ProjectSitePlanTaskId 
                                  where siteTask.ProjectSitePlanId == PId && siteStatus.SiteStatusDate == SiteStatusDate 
                                  select siteStatus;

                //var deletedItem = db.ProjectSiteStatus.Where(x => x.ProjectSiteId == SiteId && x.ProjectId == ProjectId && x.PlanCode == PlanCode && x.SiteStatusDate == SiteStatusDate).ToList();
                
                db.ProjectSiteStatus.RemoveRange(deletedItem);
                flag = db.SaveChanges() > 0;             

            }
            catch (Exception ex)
            {

            }

            if (flag)
            {
                var result = new
                {
                    flag = true,
                    message = "Task status deletion successful."
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Task status deletion failed!\nError Occured."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }



        public JsonResult AuthorizeStatus(int PId, DateTime SiteStatusDate)
        {

            if (! User.IsInRole("Management"))
            {
                var result = new
                {
                    flag = false,
                    message = "You are not authorized do such operation."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            bool flag = false;
            try
            {
                var userInfo = db.User.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                var authItem = (from siteTask in db.ProjectSitePlanTask
                                join siteStatus in db.ProjectSiteStatus on siteTask.Id equals siteStatus.ProjectSitePlanTaskId
                                where siteTask.ProjectSitePlanId == PId
                                && siteStatus.SiteStatusDate == SiteStatusDate && siteStatus.IsAuth != true
                                select siteStatus).ToList();


                //var deletedItem = db.ProjectSiteStatus.Where(x => x.ProjectSiteId == SiteId && x.ProjectId == ProjectId && x.PlanCode == PlanCode && x.SiteStatusDate == SiteStatusDate).ToList();

                //db.ProjectSiteStatus.RemoveRange(authItem);
                //flag = db.SaveChanges() > 0;
                authItem.ForEach(x =>
                {
                    x.IsAuth = true;
                    x.AuthBy = userInfo.Id;
                    x.AuthOn = DateTime.Now;
                });

                flag = db.SaveChanges() > 0;

            }
            catch (Exception ex)
            {

            }

            if (flag)
            {
                var result = new
                {
                    flag = true,
                    message = "Authorization successful."
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Authorization failed!\nError Occured."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult DeletePlan(int SitePlandId)
        {
            
            var StatusCount = (from status in db.ProjectSiteStatus
                               join siteTask in db.ProjectSitePlanTask on status.ProjectSitePlanTaskId equals siteTask.Id
                               where siteTask.ProjectSitePlanId == SitePlandId
                               select status.SiteStatusDate).Distinct().Count();

            if (StatusCount == 0)
            {
                bool flag = false;
                try
                {
                    var itemsToDeleteTask = db.ProjectSitePlanTask.Where(x => x.ProjectSitePlanId == SitePlandId);
                    db.ProjectSitePlanTask.RemoveRange(itemsToDeleteTask);

                    var itemsToDeletePlan = db.ProjectSitePlan.Where(x => x.Id == SitePlandId);
                    db.ProjectSitePlan.RemoveRange(itemsToDeletePlan);

                    flag = db.SaveChanges() > 0;
                }
                catch
                {

                }

                if (flag)
                {
                    var result = new
                    {
                        flag = true,
                        message = "Plan deletion successful."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new
                    {
                        flag = false,
                        message = "Plan deletion failed!\nError Occured."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                
            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Plan deletion failed!\nDelete status first."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            
        }

        // public ActionResult StatusList(int PId, DateTime SitePlanDate, decimal ProjectId, decimal SiteId, string PlanCode)
        public ActionResult StatusList(int PId)
        {
            //var data = db.ProjectSiteStatus.Where(y => y.ProjectSitePlanTaskId == PId).ToList();

            var data = (from status in db.ProjectSiteStatus
                               join siteTask in db.ProjectSitePlanTask on status.ProjectSitePlanTaskId equals siteTask.Id
                               where siteTask.ProjectSitePlanId == PId
                               orderby status.SiteStatusDate descending 
                               select status).ToList();

            
            //if (data.Count == 0)
            //{
            //    return RedirectToAction("ProjectSitePlanTaskInfo", new { PId = PId });
            //    //return RedirectToAction("ProjectSitePlanTaskInfo", new { ProjectId = ProjectId, SiteId = SiteId, PlanDate = SitePlanDate, SiteStatusDate = DateTime.Now.ToShortDateString(), PlanCode = PlanCode });
            //}
            //else
            //{
            //    return View(data);
            //}

            if (data.Count == 0)
            {
                return RedirectToAction("ProjectSitePlanTaskInfo", new { PId = PId });
                //return RedirectToAction("ProjectSitePlanTaskInfo", new { ProjectId = ProjectId, SiteId = SiteId, PlanDate = SitePlanDate, SiteStatusDate = DateTime.Now.ToShortDateString(), PlanCode = PlanCode });
            }
            else
            {
                
                var SitePlan = db.ProjectSitePlan.Include(p => p.ProjectSite).Include(p => p.ProjectSite.Project).FirstOrDefault(x => x.Id == PId);

                ViewBag.ProjectSiteName = SitePlan.ProjectSite.Name;
                ViewBag.ProjectName = SitePlan.ProjectSite.Project.Name;

                ViewBag.PId = PId;
                return View(data);
            }


        }



        //public ActionResult ProjectSitePlanTaskInfoExisting(decimal ProjectId, decimal SiteId, DateTime PlanDate, DateTime SiteStatusDate, string PlanCode)
        public ActionResult ProjectSitePlanTaskInfoExisting(int PId, DateTime SiteStatusDate)
        {

            var SitePlan = db.ProjectSitePlan.Include(p => p.ProjectSite).Include(p => p.ProjectSite.Project).FirstOrDefault(x => x.Id == PId);

            if (SitePlan.Active == "A")
            {
                ViewBag.PlanActive = true;
            }
            else
            {
                ViewBag.PlanActive = false;
            }

            var prevStatus = from stat in db.ProjectSiteStatus
                             join planTask in db.ProjectSitePlanTask on stat.ProjectSitePlanTaskId equals planTask.Id
                             where planTask.ProjectSitePlanId == PId && stat.SiteStatusDate < SiteStatusDate
                             orderby stat.SiteStatusDate descending
                             select stat
                    ;

            var prevDate = DateTime.Now;
            var flagLoop = false;
            var prevStatusList = new List<VMPlanTaskStatus>();

            foreach (var item in prevStatus)
            {
                if (prevDate != item.SiteStatusDate)
                {
                    if (flagLoop)
                    {
                        break;
                    }
                    else
                    {
                        flagLoop = true;
                    }
                }

                var statInfo = new VMPlanTaskStatus();

                statInfo.PlanTaskId = item.ProjectSitePlanTaskId;
                statInfo.PresentStatus = item.PresentStatus;
                statInfo.StartDate = item.StartDate;
                statInfo.EndDate = item.EndDate;
                statInfo.PerComp = item.Percentage;
                statInfo.TaskStatusDate = item.SiteStatusDate;

                prevStatusList.Add(statInfo);

                prevDate = item.SiteStatusDate;
            }

            ViewBag.ProjectSiteName = SitePlan.ProjectSite.Name;
            ViewBag.ProjectName = SitePlan.ProjectSite.Project.Name;
            ViewBag.StatusDate = NullHelper.DateToString(SiteStatusDate);
            ViewBag.PlanCode = SitePlan.PlanCode;
            ViewBag.PlanDate = NullHelper.DateToString(SitePlan.SitePlanDate);
            ViewBag.SiteStatusDate = NullHelper.DateToString(SiteStatusDate);
            ViewBag.PId = PId;
            
            var data = from plan in db.ProjectSitePlan
                       join planTask in db.ProjectSitePlanTask on plan.Id equals planTask.ProjectSitePlanId
                       join tStatus in db.ProjectSiteStatus on planTask.Id equals tStatus.ProjectSitePlanTaskId into ts
                       from taskStatus in ts.Where(f=>f.SiteStatusDate == SiteStatusDate).DefaultIfEmpty()
                       where plan.Id == PId
                       select new { plan, planTask, taskStatus };

            var statusData = data.Select(y => new { PlanTaskId = y.planTask.Id , ConId = y.planTask.ControlId,
                TaskId = y.planTask.ProjectTaskId,
                Name = db.ProjectTask.Where(t => t.Id == y.planTask.ProjectTaskId).Select(tc => tc.Name).FirstOrDefault(),
                WBS = y.planTask.WBSCode, PlanStartDate = y.planTask.StartDate, PlanEndDate = y.planTask.EndDate,
                OrderId = y.planTask.OrderId, WeightedAvg = y.planTask.WeightedAvg, MileStone = y.planTask.MileStone,
                StartDate =y.taskStatus.StartDate, EndDate = y.taskStatus.EndDate, CompletionDate = y.taskStatus.ActualCompletionDate,
                Deviation = y.taskStatus.Deviation, ResDeviation = y.taskStatus.ResDeviation,
                PerComp = y.taskStatus.Percentage,  PresentStatus = y.taskStatus.PresentStatus, Remarks = y.taskStatus.Remarks,
                PlanTaskStatusId = y.taskStatus==null?0:y.taskStatus.Id, IsAuth = y.taskStatus.IsAuth??false, InsertedBy = y.taskStatus.InsertedBy??0
                }).OrderBy(o => o.OrderId).ToList();

            _dictIndent = new Dictionary<int, int>();

            Dictionary<int, int> dictControl = new Dictionary<int, int>();

            int i = 0;
            dictControl.Add(0, 0);

            var intEditedBy = 0;

            foreach (var item in statusData)
            {
                i++;

                _dictIndent.Add(item.PlanTaskId, item.ConId);
                dictControl.Add(item.PlanTaskId, i);

                ViewBag.flagApproval = item.IsAuth;
                intEditedBy = item.InsertedBy;
            }

            if (intEditedBy==0)
            {
                ViewBag.EditedBy = "";
            }
            else
            {
                ViewBag.EditedBy = db.User.Where(x => x.Id == intEditedBy).FirstOrDefault().UserName; 
            }

            var planList = new List<VMPlanTaskStatusFull>();

            planList = statusData.Select(x => new VMPlanTaskStatusFull
            {
                ConId = x.ConId,
                IndLevel = GetIndentStringLevel(x.ConId),
                TaskId = x.TaskId,
                PlanTaskId = x.PlanTaskId,
                Name = x.Name,
                WBS = x.WBS ?? "",
                PlanStartDate = x.PlanStartDate,
                PlanEndDate = x.PlanEndDate,                
                WeightedAvg = x.WeightedAvg,
                MileStone = x.MileStone,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CompletionDate = x.CompletionDate,
                PerComp = x.PerComp,
                Deviation = x.Deviation,
                ResDeviation = x.ResDeviation,
                PresentStatus = x.PresentStatus,
                Remarks = x.Remarks ,
                PlanTaskStatusId = x.PlanTaskStatusId,
                IsAuth = x.IsAuth,
                PrevStatus = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.PresentStatus).FirstOrDefault(),
                PrevStartDate = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.StartDate).FirstOrDefault(),
                PrevEndDate = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.EndDate).FirstOrDefault(),
                PrevPerComp = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.PerComp).FirstOrDefault()
            }).ToList();            
            
            if (User.IsInRole("Management"))
            {
                return View(planList);
            }
            else if (User.IsInRole("Operator"))
            {
                return View("ProjectSitePlanTaskInfoExisting_Op", planList);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }


        //public ActionResult ProjectSitePlanTaskInfo(decimal ProjectId, decimal SiteId, DateTime PlanDate, string PlanCode)
        public ActionResult ProjectSitePlanTaskInfo(int PId)
        {
            var SitePlan = db.ProjectSitePlan.Include(p => p.ProjectSite).Include(p=>p.ProjectSite.Project).FirstOrDefault(x => x.Id == PId);

            if (SitePlan.Active!="A")
            {
                TempData["messageClass"] = "alert alert-warning";
                TempData["messageCRUD"] = "You cannot set status for inactive plan.";

                return RedirectToAction("Index");
            }

            //var prevStatus = from stat in db.ProjectSiteStatus
            //        join planTask in db.ProjectSitePlanTask on stat.ProjectSitePlanTaskId equals planTask.Id
            //        where planTask.ProjectSitePlanId == PId && stat.SiteStatusDate < DateTime.Now 
            //        orderby stat.SiteStatusDate descending 
            //        select stat
            //        ;

            var prevStatus = from stat in db.ProjectSiteStatus
                             join planTask in db.ProjectSitePlanTask on stat.ProjectSitePlanTaskId equals planTask.Id
                             where planTask.ProjectSitePlanId == PId 
                             orderby stat.SiteStatusDate descending
                             select stat
                    ;

            var prevDate = DateTime.Now;
            var flagLoop = false;
            var prevStatusList = new List<VMPlanTaskStatus>();

            foreach (var item in prevStatus)
            {
                if (prevDate != item.SiteStatusDate)
                {
                    if (flagLoop)
                    {
                        break;
                    }
                    else
                    {
                        flagLoop = true;
                    }
                }

                var statInfo = new VMPlanTaskStatus();

                statInfo.PlanTaskId = item.ProjectSitePlanTaskId;
                statInfo.PresentStatus = item.PresentStatus;
                statInfo.StartDate = item.StartDate;
                statInfo.EndDate = item.EndDate;
                statInfo.PerComp = item.Percentage;
                statInfo.TaskStatusDate = item.SiteStatusDate;

                prevStatusList.Add(statInfo);

                prevDate = item.SiteStatusDate;
            }
                     

            ViewBag.ProjectSiteName = SitePlan.ProjectSite.Name;
            ViewBag.ProjectName = SitePlan.ProjectSite.Project.Name;
            ViewBag.StatusDate = NullHelper.DateToString(DateTime.Now);
            ViewBag.PlanCode = SitePlan.PlanCode;
            ViewBag.PlanDate = NullHelper.DateToString(SitePlan.SitePlanDate);
            ViewBag.PId = PId;

            //var data = db.ProjectSitePlanTask.Where(x => x.ProjectSitePlanId == PId).OrderBy(o => o.OrderId).ToList();
            
            //var query = db.ProjectSiteStatus.Join(db.ProjectSitePlanTask,

            //  s => new { s.ProjectId, s.ProjectSiteId, s.SitePlanDate, s.ProjectTaskId },
            //  p => new { p.ProjectId, p.ProjectSiteId, p.SitePlanDate, p.ProjectTaskId },
            //  (s, p) => new { s, p }).OrderBy(o => o.p.OrderId).ToList();


            //var data = query.Where(x => x.s.ProjectId == ProjectId && x.s.ProjectSiteId == SiteId && x.s.SitePlanDate == PlanDate).Select(k => k.p).Distinct().ToList();

            //-----------------
            var data = db.ProjectSitePlanTask.Where(x => x.ProjectSitePlanId  == PId).Select(y => new { PlanTaskId=y.Id, ConId = y.ControlId, TaskId = y.ProjectTaskId, Name = db.ProjectTask.Where(t => t.Id == y.ProjectTaskId).Select(tc => tc.Name).FirstOrDefault(), WBS = y.WBSCode, StartDate = y.StartDate, EndDate = y.EndDate, OrderId = y.OrderId, Milestone = y.MileStone, WeightedAvg = y.WeightedAvg }).OrderBy(o => o.OrderId).ToList();

            _dictIndent = new Dictionary<int, int>();

            Dictionary<int,int> dictControl = new Dictionary<int, int>();

            int i = 0;
            dictControl.Add(0, 0);
            foreach (var item in data)
            {
                i++;

                _dictIndent.Add(item.PlanTaskId, item.ConId);

                dictControl.Add(item.PlanTaskId, i);                                            

            }

            var planList = new List<VMPlanTaskDispPrevStat>();

            planList = data.Select(x => new VMPlanTaskDispPrevStat
            {
                ConId = x.ConId,
                IndLevel = GetIndentStringLevel(x.ConId),
                TaskId = x.TaskId,
                PlanTaskId = x.PlanTaskId,
                Name = x.Name,
                WBS = x.WBS ?? "",
                StartDate = x.StartDate,
                EndDate =x.EndDate,
                //StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : "",
                //EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("dd/MM/yyyy") : "",
                OrderId = x.OrderId,
                MileStone = x.Milestone,
                WeightedAvg = x.WeightedAvg,
                PrevStatus = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.PresentStatus).FirstOrDefault(),
                PrevStartDate = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.StartDate).FirstOrDefault(),
                PrevEndDate = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.EndDate).FirstOrDefault(),
                PrevPerComp = prevStatusList.Where(t => t.PlanTaskId == x.PlanTaskId).Select(tc => tc.PerComp).FirstOrDefault()
            }).ToList();


            //var refinedata = data.AsEnumerable().Select(x => new { PlanTaskId = x.PlanTaskId, ConId = dictControl[x.ConId], IndLevel = GetIndentStringLevel(x.ConId), TaskId = x.TaskId, Name = x.Name, WBS = x.WBS ?? "", StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : "", EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("dd/MM/yyyy") : "", OrderId = x.OrderId, WeightedAvg = x.WeightedAvg });



            //foreach (var item in refinedata)
            //{

            //}

            //-----------------
            if (User.IsInRole("Management"))
            {
                return View(planList);
            }
            else if (User.IsInRole("Operator"))
            {
                return View("ProjectSitePlanTaskInfo_Op",planList);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Error");
            }
        }


        //public JsonResult SavePlanTaskStatus(IEnumerable<VMPlanTaskStutas> TaskDetails, decimal ProjectId, decimal SiteId, DateTime PlanDate, string PlanCode, DateTime StatusDate)
        public JsonResult SavePlanTaskStatus(IEnumerable<VMPlanTaskStatus> TaskDetails, DateTime StatusDate)
        {

            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            if (!User.IsInRole("Management"))
            {
                result = new
                {
                    flag = false,
                    message = "You are not authorized to perform this operation !"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            foreach (var item in TaskDetails)
            {
                var taskStat = db.ProjectSiteStatus.Where(x => x.Id != item.PlanTaskStatusId && 
                    x.SiteStatusDate == StatusDate && x.ProjectSitePlanTaskId == item.PlanTaskId).ToList();

                if (taskStat.Count > 0)
                {
                    result = new
                    {
                        flag = false,
                        message = "Saving Failed!!\nNo multiple status for a single date !"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            var flag = false;
            var flagSiteComplete = true;
            var planTaskId = 0;
            try
            {

                var userInfo = db.User.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                foreach (var item in TaskDetails)
                {
                    ProjectSiteStatus siteplanTask = new ProjectSiteStatus();
                    siteplanTask.Id = item.PlanTaskStatusId;
                    siteplanTask.EndDate = item.EndDate; // tentative end date
                    siteplanTask.Percentage = item.PerComp;//
                    //siteplanTask.PlanCode = PlanCode;
                    siteplanTask.PresentStatus = item.PresentStatus;//
                    //siteplanTask.ProjectId = Convert.ToInt32(ProjectId);
                    siteplanTask.Remarks = item.Remarks;//
                    //siteplanTask.ProjectSiteId = Convert.ToInt32(SiteId);
                    //siteplanTask.SitePlanDate = PlanDate;
                    siteplanTask.ActualCompletionDate = item.CompletionDate;// actual completion date
                    siteplanTask.SiteStatusDate = StatusDate; // 
                    siteplanTask.StartDate = item.StartDate;// status start date
                    //siteplanTask.ProjectTaskId = item.TaskId;
                    //siteplanTask.WBSCode = item.WBS;
                    siteplanTask.Deviation = ""; //item.Deviation;//
                    siteplanTask.ResDeviation = item.ResDeviation;//
                    siteplanTask.ProjectSitePlanTaskId = item.PlanTaskId;
                    //db.ProjectSiteStatus.Add(siteplanTask);
                    

                    if (item.CompletionDate != null)
                    {
                        siteplanTask.Percentage = "100";
                    }
                    else if (NullHelper.ToIntNum(item.PerComp) == 100)
                    {
                        siteplanTask.Percentage = "100";
                        siteplanTask.ActualCompletionDate = StatusDate;
                    }

                    if (item.ConId == 0)
                    {
                        if (NullHelper.ObjectToString(item.PerComp).Trim()!="100" && item.CompletionDate == null)
                        {
                            flagSiteComplete = false;
                        }
                        
                    }

                    siteplanTask.InsertedBy = userInfo.Id;
                    siteplanTask.InsertedOn = DateTime.Now;
                    siteplanTask.IsAuth = false;

                    db.Entry(siteplanTask).State = siteplanTask.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                    db.SaveChanges();

                    planTaskId = item.PlanTaskId;
                }

                flag = true;

                

            }
            catch (Exception ex)
            {

            }


            try
            {
                if (TaskDetails != null && flag==true)
                {
                    var sitePlanId = db.ProjectSitePlanTask.Where(x => x.Id == planTaskId).FirstOrDefault().ProjectSitePlanId;

                    var projectSiteId = db.ProjectSitePlan.Where(x => x.Id == sitePlanId).FirstOrDefault().ProjectSiteId;

                    var projectSite = db.ProjectSite.Find(projectSiteId);

                    var projectId = projectSite.ProjectId;

                    if (flagSiteComplete == true)
                    {
                        projectSite.SiteStatus = Status.Complete;
                    }
                    else
                    {
                        projectSite.SiteStatus = Status.Running;
                    }

                    db.Entry(projectSite).State = EntityState.Modified;
                    db.SaveChanges();

                    var listProjectSite = db.ProjectSite.Where(x =>x.ProjectId==projectId && x.SiteStatus != Status.Complete).ToList();

                    var project = db.Project.Find(projectId);

                    if (listProjectSite.Count==0)
                    {
                        // all site are complete
                        project.Status = ProjectStatus.Complete;
                    }
                    else
                    {
                        // not all sites are complete
                        project.Status = ProjectStatus.Running;
                    }

                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            catch
            {

            }


            if (flag==true)
            {
                result = new
                {
                    flag = true,
                    message = "Saving successful !!"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            result = new
            {
                flag = false,
                message = "Saving Failed!!"
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePlanTaskStatusOp(IEnumerable<VMPlanTaskStatus> TaskDetails, DateTime StatusDate)
        {

            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };
            
            if (! User.IsInRole("Operator"))
            {
                result = new
                {
                    flag = false,
                    message = "You are not authorized to perform this operation !"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            

            foreach (var item in TaskDetails)
            {
                var taskStat = db.ProjectSiteStatus.Where(x => x.Id != item.PlanTaskStatusId &&
                    x.SiteStatusDate == StatusDate && x.ProjectSitePlanTaskId == item.PlanTaskId).ToList();

                if (taskStat.Count > 0)
                {
                    result = new
                    {
                        flag = false,
                        message = "Saving Failed!!\nNo multiple status for a single date !"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            var flag = false;
            var flagSiteComplete = true;
            var planTaskId = 0;
            //var userId = 0;
            try
            {
                var userInfo = db.User.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();           


                foreach (var item in TaskDetails)
                {
                    ProjectSiteStatus siteplanTask = new ProjectSiteStatus();
                    siteplanTask.Id = item.PlanTaskStatusId;
                    siteplanTask.EndDate = item.EndDate;// tentative end date
                    siteplanTask.Percentage = item.PerComp;//
                    //siteplanTask.PlanCode = PlanCode;
                    siteplanTask.PresentStatus = item.PresentStatus;//
                    //siteplanTask.ProjectId = Convert.ToInt32(ProjectId);
                    siteplanTask.Remarks = item.Remarks;//
                    //siteplanTask.ProjectSiteId = Convert.ToInt32(SiteId);
                    //siteplanTask.SitePlanDate = PlanDate;
                    siteplanTask.ActualCompletionDate = item.CompletionDate;// actual completion date
                    siteplanTask.SiteStatusDate = StatusDate; //
                    siteplanTask.StartDate = item.StartDate;// status start date
                    //siteplanTask.ProjectTaskId = item.TaskId;
                    //siteplanTask.WBSCode = item.WBS;

                    //siteplanTask.Deviation = item.Deviation;
                    siteplanTask.Deviation = "";
                    //siteplanTask.ResDeviation = item.ResDeviation;
                    siteplanTask.ResDeviation = "";//

                    siteplanTask.ProjectSitePlanTaskId = item.PlanTaskId;
                    //db.ProjectSiteStatus.Add(siteplanTask);


                    if (item.CompletionDate != null)
                    {
                        siteplanTask.Percentage = "100";
                    }
                    else if (NullHelper.ToIntNum(item.PerComp) == 100)
                    {
                        siteplanTask.Percentage = "100";
                        siteplanTask.ActualCompletionDate = StatusDate;
                    }

                    if (item.ConId == 0)
                    {
                        if (NullHelper.ObjectToString(item.PerComp).Trim() != "100" && item.CompletionDate == null)
                        {
                            flagSiteComplete = false;
                        }

                    }

                    siteplanTask.InsertedBy = userInfo.Id;
                    siteplanTask.InsertedOn = DateTime.Now;
                    siteplanTask.IsAuth = false;            
                    
                    db.Entry(siteplanTask).State = siteplanTask.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                    db.SaveChanges();

                    planTaskId = item.PlanTaskId;
                }

                flag = true;



            }
            catch (Exception ex)
            {

            }


            try
            {
                if (TaskDetails != null && flag == true)
                {
                    var sitePlanId = db.ProjectSitePlanTask.Where(x => x.Id == planTaskId).FirstOrDefault().ProjectSitePlanId;

                    var projectSiteId = db.ProjectSitePlan.Where(x => x.Id == sitePlanId).FirstOrDefault().ProjectSiteId;

                    var projectSite = db.ProjectSite.Find(projectSiteId);

                    var projectId = projectSite.ProjectId;

                    if (flagSiteComplete == true)
                    {
                        projectSite.SiteStatus = Status.Complete;
                    }
                    else
                    {
                        projectSite.SiteStatus = Status.Running;
                    }

                    db.Entry(projectSite).State = EntityState.Modified;
                    db.SaveChanges();

                    var listProjectSite = db.ProjectSite.Where(x => x.ProjectId == projectId && x.SiteStatus != Status.Complete).ToList();

                    var project = db.Project.Find(projectId);

                    if (listProjectSite.Count == 0)
                    {
                        // all site are complete
                        project.Status = ProjectStatus.Complete;
                    }
                    else
                    {
                        // not all sites are complete
                        project.Status = ProjectStatus.Running;
                    }

                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            catch
            {

            }


            if (flag == true)
            {
                result = new
                {
                    flag = true,
                    message = "Saving successful !!"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            result = new
            {
                flag = false,
                message = "Saving Failed!!"
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EditProjectTask(int id, string Name)
        {
            var data = db.ProjectTask.Where(x => x.Id == id).FirstOrDefault();
            ProjectTask p = new ProjectTask();
            p.Name = Name;
            var TaskEntry = db.Entry(data);
            TaskEntry.CurrentValues.SetValues(p);
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveProjectTask(IEnumerable<int> TaskId)
        {
            List<string> retList = new List<string>();
            foreach (var id in TaskId)
            {
                try
                {

                    var planTaskList = db.ProjectSitePlanTask.Where(x => x.ProjectTaskId == id).ToList();
                    if (planTaskList.Count != 0)
                    {
                        var task = db.ProjectTask.Single(s => s.Id == id);

                        retList.Add(task.Name);
                    }
                    else
                    {
                        var task = db.ProjectTask.Single(s => s.Id == id);

                        db.ProjectTask.Remove(task);
                        db.SaveChanges();
                    }

                    
                }
                catch (Exception ex)
                {

                }
            }
                        
            //ViewBag.NotDeletedItem = retList;
            TempData["NotDeletedItem"] = retList;

            return RedirectToAction("Index", "ProjectTask");

        }

        //public ActionResult ProjectTaskList()
        //{

        //    return View(db.ProjectTask.ToList());
        //}

        //[HttpPost]
        //public ActionResult ProjectTaskList(IEnumerable<int> TaskRecordDeletebyId)
        //{
        //    foreach (var id in TaskRecordDeletebyId)
        //    {
        //        try
        //        {
        //            var task = db.ProjectTask.Single(s => s.Id == id);

        //            db.ProjectTask.Remove(task);
        //            db.SaveChanges();
        //        }
        //        catch(Exception ex)
        //        {

        //        }
        //    }

        //    return RedirectToAction("Index","ProjectTask");

        //}  

        //public JsonResult MakeSubTask(List<int> Child, int parent)
        //{
        //    foreach(var item in Child)
        //    {
        //        db.Database.ExecuteSqlCommand("update Projectsiteplantasks set controlid='"+parent+"' where ProjectTaskId='"+item+"'");
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult UndoSubTask(List<int> Child)
        //{
        //    foreach (var item in Child)
        //    {
        //        db.Database.ExecuteSqlCommand("update Projectsiteplantasks set controlid=0 where ProjectTaskId='" + item + "'");
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        public JsonResult MakeMileStone(List<int> Child)
        {
            foreach (var item in Child)
            {
                db.Database.ExecuteSqlCommand("update Projectsiteplantasks set MileStone='Y' where TaskId='" + item + "'");
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult UndoMileStone(List<int> Child)
        {
            foreach (var item in Child)
            {
                db.Database.ExecuteSqlCommand("update Projectsiteplantasks set MileStone='N' where TaskId='" + item + "'");
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        //
        //public JsonResult deleteTask(List<int> Child)
        //{
        //    foreach (var item in Child)
        //    {
        //        db.Database.ExecuteSqlCommand("delete from Projectsiteplantasks  where TaskId='" + item + "'");
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult DeletePlanTask(int id)
        {
            var result = new
            {
                flag = false,
                message = "Error occured !!"
            };

            try
            {

                var statList = db.ProjectSiteStatus.Where(x => x.ProjectSitePlanTaskId == id).ToList();
                if (statList.Count == 0)
                {
                    ProjectSitePlanTask plan = db.ProjectSitePlanTask.Find(id);
                    db.ProjectSitePlanTask.Remove(plan);
                    db.SaveChanges();

                    result = new
                    {
                        flag = true,
                        message = "Delete successful !!"
                    };
                }
                else
                {
                    result = new
                    {
                        flag = false,
                        message = "Delete failed !!\nTask having status cannot be deleted."
                    };
                }

            }
            catch (Exception ex)
            {
                result = new
                {
                    flag = false,
                    message = "Delete failed !!\nError Occured."
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveTask(string TaskName)
        {
            ProjectTask projectTask = new ProjectTask();
            projectTask.Name = TaskName;
            var check = db.ProjectTask.Where(x=>x.Name==TaskName).Count();
            bool flag = false;
           if(check==0)
           {
               db.ProjectTask.Add(projectTask);

               flag = db.SaveChanges() > 0;
           }

           return Json(new { flag=flag,Id=projectTask.Id}, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetTask(string term)
        {
            var result = (from r in db.ProjectTask
                          where r.Name.Contains(term)
                          select new { r.Name,r.Id }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion




        //public JsonResult UpdatePlanTask(IEnumerable<VMPlanTask> TaskDetails, int ProjectId, int SiteId, DateTime PlanDate, string PlanCode)
        //{
        //    var result = false;
        //    try
        //    {

        //        var checkdata = db.ProjectSitePlan.Where(x => x.ProjectId == ProjectId && x.ProjectSiteId == SiteId && x.SitePlanDate == PlanDate).SingleOrDefault();
        //        var existingPlanTask = checkdata.ProjectSitePlanTasks.Select(x => new {  TaskId=x.ProjectTaskId,WBS=x.WBSCode,StartDate=x.StartDate,EndDate=x.EndDate,Remarks=x.Remarks}).ToList();
        //        if(checkdata!=null)
        //        {

        //            ProjectSitePlan sp = new ProjectSitePlan();
        //            sp.Active = "A";
        //            sp.PlanCode = PlanCode;
        //            sp.ProjectId = ProjectId;
        //            sp.ProjectSiteId = SiteId;
        //            sp.SitePlanDate = PlanDate;
        //            var siteplanEntry = db.Entry(checkdata);
        //            siteplanEntry.CurrentValues.SetValues(sp);
        //            db.SaveChanges();
        //            if(existingPlanTask!=null)
        //            {
        //                var currentItem = TaskDetails.Select(x => new {TaskId = x.TaskId, WBS = x.WBS, StartDate = x.StartDate, EndDate = x.EndDate, Remarks = x.Remarks }).ToList();
        //                var AddedPlanTaskList = currentItem.Except(existingPlanTask).ToList();
        //                var CommonTask = currentItem.Intersect(existingPlanTask).ToList();
        //                var NewPlanTask = CommonTask.Union(AddedPlanTaskList).Select(x => new VMPlanTask { TaskId = x.TaskId, WBS = x.WBS, StartDate = x.StartDate, EndDate = x.EndDate, Remarks = x.Remarks }).ToList();
        //                var deletedPlanTaskList = existingPlanTask.Except(currentItem).ToList();

        //                foreach(var deleteditem in deletedPlanTaskList)
        //                {
        //                    var data = db.ProjectSitePlanTask.Where(x => x.ProjectTaskId == deleteditem.TaskId && x.ProjectId == ProjectId && x.ProjectSiteId == SiteId && x.SitePlanDate == PlanDate).SingleOrDefault();
        //                    db.ProjectSitePlanTask.Remove(data);
        //                    db.SaveChanges();
        //                }
        //                var sortList =new  List<VMPlanTask>();
        //                foreach(var item in currentItem)
        //                {
        //                    sortList.Add(NewPlanTask.Where(x => x.TaskId == item.TaskId).SingleOrDefault());
        //                }

        //                for(int i=0; i<sortList.Count;i++)
        //                {
        //                    ProjectSitePlanTask projectplantask = new ProjectSitePlanTask();
        //                    var TaskId= sortList[i].TaskId;

        //                    var checkifexistingPlanTask = db.ProjectSitePlanTask.Where(x => x.ProjectTaskId == TaskId && x.ProjectId==ProjectId && x.ProjectSiteId==SiteId && x.SitePlanDate == PlanDate).SingleOrDefault();
        //                    if(checkifexistingPlanTask !=null)
        //                    {

        //                        projectplantask.EndDate = sortList[i].EndDate;
        //                        projectplantask.MileStone = checkifexistingPlanTask.MileStone;
        //                        projectplantask.ControlId = checkifexistingPlanTask.ControlId;
        //                        projectplantask.OrderId = i+1;
        //                        projectplantask.PlanCode = PlanCode;
        //                        projectplantask.ProjectId = ProjectId;
        //                        projectplantask.ProjectSiteId = SiteId;
        //                        projectplantask.ProjectTaskId = checkifexistingPlanTask.ProjectTaskId;
        //                        projectplantask.Remarks = sortList[i].Remarks;
        //                        projectplantask.SitePlanDate = PlanDate;
        //                        projectplantask.StartDate = sortList[i].StartDate;
        //                        projectplantask.WBSCode = sortList[i].WBS;
        //                        var siteplanTaskEntry = db.Entry(checkifexistingPlanTask);
        //                        siteplanTaskEntry.CurrentValues.SetValues(projectplantask);
        //                        db.SaveChanges();

        //                    }
        //                    else
        //                    {
        //                        projectplantask.EndDate = sortList[i].EndDate;
        //                        projectplantask.MileStone = "";
        //                        projectplantask.ControlId = 0;
        //                        projectplantask.OrderId = i + 1;
        //                        projectplantask.PlanCode = PlanCode;
        //                        projectplantask.ProjectId = ProjectId;
        //                        projectplantask.ProjectSiteId = SiteId;
        //                        projectplantask.ProjectTaskId = sortList[i].TaskId;
        //                        projectplantask.Remarks = sortList[i].Remarks;
        //                        projectplantask.SitePlanDate = PlanDate;
        //                        projectplantask.StartDate = sortList[i].StartDate;
        //                        projectplantask.WBSCode = sortList[i].WBS;
        //                        db.ProjectSitePlanTask.Add(projectplantask);
        //                        db.SaveChanges();
        //                    }
        //                }
        //                result = true;
        //            }
        //        }






        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //    //if(result==true)
        //    //{
        //    //    foreach (var item in TaskDetails)
        //    //    {
        //    //        ProjectSitePlanTask siteplanTask = new ProjectSitePlanTask();
        //    //        siteplanTask.EndDate = item.EndDate;
        //    //        siteplanTask.ProjectId = ProjectId;
        //    //        siteplanTask.ProjectSiteId = SiteId;
        //    //        siteplanTask.SitePlanDate = PlanDate;
        //    //        siteplanTask.StartDate = item.StartDate;
        //    //        siteplanTask.ProjectTaskId = item.TaskId;
        //    //        siteplanTask.WBSCode = item.WBS;
        //    //        siteplanTask.ControlId = 0;
        //    //        siteplanTask.PlanCode = PlanCode;
        //    //        siteplanTask.Id = db.ProjectSitePlanTask.Select(x=>x.Id).DefaultIfEmpty().Max() + 1 ;
        //    //       // db.Entry(siteplanTask).State = EntityState.Modified;
        //    //       // db.ProjectSitePlanTask.Add(siteplanTask);
        //    //        db.SaveChanges();


        //    //    }
        //    //}

        //    return Json(result,JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult UpdatePlanTask(IEnumerable<VMPlanTaskNew> TaskDetails, int PId, int ProjectId, int SiteId, DateTime PlanDate, string PlanCode)
        public JsonResult UpdatePlanTask(IEnumerable<VMPlanTaskNew> TaskDetails, int PId, int SiteId, DateTime PlanDate, string PlanCode, string Status)
        {
            var result = new
            {
                flag = false,
                message = "Plan saving Error !"
            };

            if (Status == "A")
            {
                var aPlanList = db.ProjectSitePlan.Where(x => x.ProjectSiteId == SiteId && x.Active == "A" && x.Id != PId).ToList();
                if (aPlanList.Count > 0)
                {
                    result = new
                    {
                        flag = false,
                        message = "Saving Failed!!\nActive plan exists under this site.\nYou cannot make active more than one plan under a site. !"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }


            var planList = db.ProjectSitePlan.Where(x => x.PlanCode.Trim() == PlanCode.Trim() && x.Id!=PId).ToList();

            

            if (planList.Count == 0)
            {
                try
                {
                    var flag = false;

                    var checkdata = db.ProjectSitePlan.Where(x => x.Id == PId).SingleOrDefault();

                    //var existingPlanTask = checkdata.ProjectSitePlanTasks.Select(x => new { TaskId = x.ProjectTaskId, WBS = x.WBSCode, StartDate = x.StartDate, EndDate = x.EndDate, Remarks = x.Remarks }).ToList();

                    if (checkdata != null)
                    {

                        checkdata.ProjectSiteId = SiteId;
                        checkdata.SitePlanDate = PlanDate;
                        checkdata.PlanCode = PlanCode;
                        checkdata.Active = Status;

                        db.Entry(checkdata).State = EntityState.Modified;


                        flag = db.SaveChanges() > 0;

                        if (flag == true)
                        {
                            int i = 1;

                            Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();

                            foreach (var item in TaskDetails)
                            {
                                ProjectSitePlanTask siteplanTask = new ProjectSitePlanTask();

                                siteplanTask.Id = item.PlanTaskId;
                                siteplanTask.ProjectSitePlanId = checkdata.Id;
                                siteplanTask.EndDate = item.EndDate;
                                siteplanTask.StartDate = item.StartDate;
                                siteplanTask.ProjectTaskId = item.TaskId;
                                siteplanTask.WBSCode = item.WBS;
                                siteplanTask.WeightedAvg = item.WeightedAvg;
                                siteplanTask.MileStone = item.Milestone;

                                if (item.ConId > 0)
                                {
                                    siteplanTask.ControlId = dictionary[item.ConId];
                                }
                                else
                                {
                                    siteplanTask.ControlId = 0;
                                }

                                siteplanTask.OrderId = i;

                                //db.ProjectSitePlanTask.Add(siteplanTask);

                                db.Entry(siteplanTask).State = siteplanTask.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                db.SaveChanges();

                                dictionary.Add(item.TempTaskId, siteplanTask.Id);
                                i++;

                            }

                            result = new
                            {
                                flag = true,
                                message = "Plan saving successful !"
                            };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        //----------------------------------------------------


                    }

                }
                catch (Exception ex)
                {

                }

                
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            else
            {
                result = new
                {
                    flag = false,
                    message = "Site plancode already exists on other plan!"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CreatePlanTask(IEnumerable<VMPlanTaskNew> TaskDetails, int SiteId, DateTime PlanDate, string PlanCode, string Status)
        {
            if (Status=="A")
            {
                var aPlanList = db.ProjectSitePlan.Where(x => x.ProjectSiteId == SiteId && x.Active=="A").ToList();
                if (aPlanList.Count>0)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Saving Failed!!\nActive plan exists under this site.\nYou cannot make active more than one plan under a site. !"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            


            var planList = db.ProjectSitePlan.Where(x => x.PlanCode.Trim() == PlanCode.Trim()).ToList();
            
            if (planList.Count == 0)
            {
                var flag = false;

                ProjectSitePlan sp = new ProjectSitePlan();

                try
                {
                    sp.Active = "A";
                    sp.PlanCode = PlanCode;
                    //sp.ProjectId = ProjectId;
                    sp.ProjectSiteId = SiteId;
                    sp.SitePlanDate = PlanDate;
                    sp.Active = Status;
                    db.ProjectSitePlan.Add(sp);
                    flag = db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {

                }

                if (flag == true)
                {
                    int i = 1;
                    //int prevSitePlanTaskId = 0;
                    Dictionary<int, int> dictionary =
                        new Dictionary<int, int>();

                    foreach (var item in TaskDetails)
                    {
                        ProjectSitePlanTask siteplanTask = new ProjectSitePlanTask();
                        siteplanTask.ProjectSitePlanId = sp.Id;
                        siteplanTask.EndDate = item.EndDate;
                        //siteplanTask.ProjectId = ProjectId;
                        //siteplanTask.ProjectSiteId = SiteId;
                        //siteplanTask.SitePlanDate = PlanDate;
                        siteplanTask.StartDate = item.StartDate;
                        siteplanTask.ProjectTaskId = item.TaskId;
                        siteplanTask.WBSCode = item.WBS;
                        siteplanTask.WeightedAvg = item.WeightedAvg;
                        siteplanTask.MileStone = item.Milestone;

                        if (item.ConId > 0)
                        {
                            siteplanTask.ControlId = dictionary[item.ConId];
                        }
                        else
                        {
                            siteplanTask.ControlId = 0;
                        }

                        //siteplanTask.PlanCode = PlanCode;
                        siteplanTask.OrderId = i;
                        //   siteplanTask.Id = db.ProjectSitePlanTask.Select(x=>x.Id).DefaultIfEmpty().Max() + 1 ;
                        db.ProjectSitePlanTask.Add(siteplanTask);
                        db.SaveChanges();
                        //prevSitePlanTaskId = siteplanTask.Id;
                        dictionary.Add(item.TempTaskId, siteplanTask.Id);
                        i++;

                    }

                    var result = new
                    {
                        flag = true,
                        message = "Plan saving successful !"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new
                    {
                        flag = false,
                        message = "Plan saving error !"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Site plancode already exists !"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: ProjectSitePlanTasks
        public ActionResult Index(int? page, int? ProjectId, int? SiteId)
        {
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            // var projectSitePlanTask = db.ProjectSitePlanTask.Include(p => p.Project).Include(p => p.ProjectSite);
            var data = db.ProjectSitePlan.OrderByDescending(y=>y.SitePlanDate).ToList();

            if (SiteId != null)
            { 
                //data = data.Where(x => x.ProjectId == ProjectId && x.ProjectSiteId == SiteId).ToList();
                data = data.Where(x => x.ProjectSiteId == SiteId).ToList();
                ViewBag.SiteId = SiteId;
            }
            //int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(data.ToPagedList(pageNumber, _pageSize));
        }

        // GET: ProjectSitePlanTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectSitePlanTask projectSitePlanTask = db.ProjectSitePlanTask.Find(id);
            if (projectSitePlanTask == null)
            {
                return HttpNotFound();
            }
            return View(projectSitePlanTask);
        }

        // GET: ProjectSitePlanTasks/Create
        public ActionResult Create()
        {

            if (!User.IsInRole("Management"))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            //  ViewBag.SiteId = new SelectList(db.ProjectSite, "Id", "Name");

            ViewBag.StartDate = DateTime.Now.Date;
            ViewBag.EndDate = DateTime.Now.Date;

            ViewBag.PlanDate = DateTime.Now.ToShortDateString();

            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text");
            ViewBag.Status = status;


            return View();
        }

        // POST: ProjectSitePlanTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]

        //public ActionResult Create([Bind(Include = "ProjectId,SiteId,SitePlanDate,TaskId,WBSCode,StartDate,EndDate,MileStone,CompletionPer,CompletionDate,Remarks")] ProjectSitePlanTask projectSitePlanTask)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ProjectSitePlanTask.Add(projectSitePlanTask);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectSitePlanTask.ProjectId);
        //    ViewBag.ProjectId = new SelectList(db.ProjectSite, "ProjectId", "Name", projectSitePlanTask.ProjectId);
        //    return View(projectSitePlanTask);
        //}

        //// GET: ProjectSitePlanTasks/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProjectSitePlanTask projectSitePlanTask = db.ProjectSitePlanTask.Find(id);
        //    if (projectSitePlanTask == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectSitePlanTask.ProjectId);
        //    ViewBag.ProjectId = new SelectList(db.ProjectSite, "ProjectId", "Name", projectSitePlanTask.ProjectId);
        //    return View(projectSitePlanTask);
        //}

        //// POST: ProjectSitePlanTasks/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ProjectId,SiteId,SitePlanDate,TaskId,WBSCode,StartDate,EndDate,MileStone,CompletionPer,CompletionDate,Remarks")] ProjectSitePlanTask projectSitePlanTask)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(projectSitePlanTask).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", projectSitePlanTask.ProjectId);
        //    ViewBag.ProjectId = new SelectList(db.ProjectSite, "ProjectId", "Name", projectSitePlanTask.ProjectId);
        //    return View(projectSitePlanTask);
        //}

        // GET: ProjectSitePlanTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectSitePlanTask projectSitePlanTask = db.ProjectSitePlanTask.Find(id);
            if (projectSitePlanTask == null)
            {
                return HttpNotFound();
            }
            return View(projectSitePlanTask);
        }

        // POST: ProjectSitePlanTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectSitePlanTask projectSitePlanTask = db.ProjectSitePlanTask.Find(id);
            db.ProjectSitePlanTask.Remove(projectSitePlanTask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PendingStatus()
        {
            //var data = db.ProjectSiteStatus.Where(y => y.ProjectSitePlanTaskId == PId).ToList();

            var data = (from status in db.ProjectSiteStatus
                        join siteTask in db.ProjectSitePlanTask on status.ProjectSitePlanTaskId equals siteTask.Id
                        join plan in db.ProjectSitePlan on siteTask.ProjectSitePlanId equals plan.Id
                        join site in db.ProjectSite on plan.ProjectSiteId equals site.Id
                        join project in db.Project on site.ProjectId equals project.Id
                        where status.IsAuth != true                        
                        select (new VMPendingStatus{
                            ProjectName = project.Name,
                            SiteName = site.Name,
                            InputBy = db.User.Where(x=>x.Id==status.InsertedBy).FirstOrDefault().UserName,
                            PlanId = plan.Id,
                            StatusDate = status.SiteStatusDate
                        })).Distinct();
                        
                        //new VMPlanTaskDispPrevStat
                        //{
                        //    ConId = x.
                        //});            
            
            
            return View(data);
           


        }


        public ActionResult NearTask()
        {
            //var data = db.ProjectSiteStatus.Where(y => y.ProjectSitePlanTaskId == PId).ToList();

            var dateComp = DateTime.Now.AddDays(7);            
            //var data1 = db.ProjectSiteStatus.Where(x => x.ActualCompletionDate == null && 
            //    x.EndDate != null)
            //    .GroupBy(x=>x.ProjectSitePlanTaskId)
            //    .Select(g=>g.OrderByDescending(p=>p.SiteStatusDate).FirstOrDefault());


            var data1 = db.ProjectSiteStatus.Where(x => x.ActualCompletionDate == null &&
                x.EndDate < dateComp)
                .GroupBy(x => x.ProjectSitePlanTaskId)
                .Select(g => g.OrderByDescending(p => p.SiteStatusDate).FirstOrDefault());

            var data = from status in data1
                        join siteTask in db.ProjectSitePlanTask on status.ProjectSitePlanTaskId equals siteTask.Id
                        join task in db.ProjectTask on siteTask.ProjectTaskId equals task.Id
                        join plan in db.ProjectSitePlan on siteTask.ProjectSitePlanId equals plan.Id
                        join site in db.ProjectSite on plan.ProjectSiteId equals site.Id
                        join project in db.Project on site.ProjectId equals project.Id
                        where status.IsAuth != true
                        select (new VMNearTask
                        {
                            ProjectName = project.Name,
                            SiteName = site.Name, 
                            TaskName = task.Name,                           
                            PlanId = plan.Id,
                            StatusDate = status.SiteStatusDate,
                            EndDate = status.EndDate                            
                        });

            // ProjectName, SiteName, 

            return View(data);
        }



        //Added by Nabid 26-9-17
        public ActionResult Search(string term)
        {
            if (string.IsNullOrEmpty(term)) return Json(new { }, JsonRequestBehavior.AllowGet);

            var projectNames = db.Project.ToList();

            var filteredList = projectNames
                .Where(p => p.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .Select(p => new { p.Id, p.Name })
                .ToList();

            return Json(filteredList, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
