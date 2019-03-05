namespace AuditReport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ini : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserName = c.String(nullable: false, maxLength: 50),
                    Password = c.String(nullable: false, maxLength: 155),
                    Salt = c.String(nullable: true, maxLength: 100),
                    FirstName = c.String(nullable: true, maxLength: 50),
                    LastName = c.String(nullable: true, maxLength: 50),
                    Email = c.String(nullable: true, maxLength: 100),
                    Phone = c.String(nullable: true, maxLength: 100),
                    Address = c.String(nullable:true, maxLength: 255),                    
                    IsActive = c.Boolean(nullable:false),
                    LastLogin = c.DateTime(nullable: true)
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    RoleName = c.String(nullable: false, maxLength: 50)
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UsersRoles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    RoleId = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users",t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 100),
                        Phone = c.String(maxLength: 50),
                        ContactPerson = c.String(maxLength: 100),
                        Note = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        ClientId = c.Int(),
                        ProjectGroupId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        Remarks = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.ProjectGroups", t => t.ProjectGroupId)
                .Index(t => t.ClientId)
                .Index(t => t.ProjectGroupId);
            
            CreateTable(
                "dbo.CompanyResources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Position = c.String(maxLength: 100),
                        DOJ = c.DateTime(),
                        Phone = c.String(maxLength: 50),
                        Address = c.String(maxLength: 100),
                        Status = c.String(nullable: false, maxLength: 1),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ProjectSiteResources",
                c => new
                {
                    //ProjectId = c.Int(nullable: false, identity: true),
                    ProjectSiteId = c.Int(nullable: false),
                    CompanyResourceId = c.Int(nullable: false),
                    //Project_Id = c.Int(),
                })
                .PrimaryKey(t => new { t.ProjectSiteId, t.CompanyResourceId })
                .ForeignKey("dbo.ProjectSites", t => t.ProjectSiteId)
                .ForeignKey("dbo.CompanyResources", t => t.CompanyResourceId)
                .Index(t => t.ProjectSiteId)
                .Index(t => t.CompanyResourceId);                
            
            CreateTable(
                "dbo.ProjectSites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Location = c.String(nullable: false, maxLength: 100),
                        SiteStatus = c.Int(nullable: false),
                        Remarks = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.ProjectSiteImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        ProjectSiteId = c.Int(nullable: false),
                        SitePlanDate = c.DateTime(),
                        ImageDate = c.DateTime(nullable: false),
                        ImageURL = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.ProjectSites", t => t.ProjectSiteId)
                .Index(t => t.ProjectId)
                .Index(t => t.ProjectSiteId);
            
            CreateTable(
                "dbo.ProjectSitePlans",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    //ProjectId = c.Int(nullable: false),
                    ProjectSiteId = c.Int(nullable: false),
                    SitePlanDate = c.DateTime(nullable: false),
                    PlanCode = c.String(maxLength: 20),
                    Active = c.String(maxLength: 1),
                    Remarks = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("dbo.ProjectSites", t => t.ProjectSiteId)              
                .Index(t => t.ProjectSiteId)
                .Index(t => t.PlanCode, unique: true, name: "PlanCodeIndex");
            
            CreateTable(
                "dbo.ProjectSitePlanTasks",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    //ProjectId = c.Int(nullable: false),
                    ProjectSitePlanId = c.Int(nullable: false),
                    SitePlanDate = c.DateTime(nullable: false),
                    ProjectTaskId = c.Int(nullable: false),
                    OrderId = c.Int(nullable: false),
                    //PlanCode = c.String(),
                    WBSCode = c.String(),
                    ControlId = c.Int(nullable: false),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
                    MileStone = c.String(maxLength: 1),
                    Remarks = c.String(maxLength: 100),
                    WeightedAvg = c.Int(nullable: true)
                })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("dbo.ProjectSitePlans", t => t.ProjectSitePlanId)
                .ForeignKey("dbo.ProjectTasks", t => t.ProjectTaskId)                
                .Index(t => t.ProjectSitePlanId)
                .Index(t => t.ProjectTaskId);
            
            CreateTable(
                "dbo.ProjectTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyInformations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(maxLength: 50),
                        ContactPerson = c.String(maxLength: 100),
                        Address = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PreStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectResources",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        CompanyResourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.CompanyResourceId })
                .ForeignKey("dbo.CompanyResources", t => t.CompanyResourceId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.CompanyResourceId);

            CreateTable(
                "dbo.ProjectSiteStatus",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ProjectSitePlanTaskId = c.Int(nullable: false),
                    //ProjectId = c.Int(nullable: false),
                    //ProjectSiteId = c.Int(nullable: false),
                    //SitePlanDate = c.DateTime(nullable: false),
                    //ProjectTaskId = c.Int(nullable: false),
                    SiteStatusDate = c.DateTime(nullable: false),
                    //PlanCode = c.String(),
                    //WBSCode = c.String(),
                    //ControlId = c.Int(nullable: false),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
                    Percentage = c.String(),
                    CompletionDate = c.DateTime(),
                    Remarks = c.String(maxLength: 100),
                    PresentStatus = c.String(),
                    Deviation = c.String(),
                    ResDeviation = c.String(),
                    ActualCompletionDate = c.DateTime(),
                    IsAuth = c.Boolean(),
                    InsertedBy = c.Int(),
                    InsertedOn = c.DateTime(),
                    AuthBy = c.Int(),
                    AuthOn = c.DateTime()
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectSitePlanTasks", t => t.ProjectSitePlanTaskId)
                .Index(t => t.ProjectSitePlanTaskId);                
            
            //CreateTable(
            //    "dbo.CompanyResourceProjects",
            //    c => new
            //        {
            //            CompanyResource_Id = c.Int(nullable: false),
            //            Project_Id = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => new { t.CompanyResource_Id, t.Project_Id })
            //    .ForeignKey("dbo.CompanyResources", t => t.CompanyResource_Id, cascadeDelete: true)
            //    .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
            //    .Index(t => t.CompanyResource_Id)
            //    .Index(t => t.Project_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectSiteStatus", "ProjectSitePlanTaskId", "dbo.ProjectSitePlanTasks"); //-- ok
            //DropForeignKey("dbo.ProjectSiteStatus", "ProjectTaskId", "dbo.ProjectTasks"); //--  need to remove
            DropForeignKey("dbo.ProjectSiteStatus", new[] { "ProjectId", "ProjectSiteId", "SitePlanDate" }, "dbo.ProjectSitePlans"); //-- need to remove
            //DropForeignKey("dbo.ProjectSiteStatus", "ProjectSiteId", "dbo.ProjectSites"); //-- need to remove
            //DropForeignKey("dbo.ProjectSiteStatus", "ProjectId", "dbo.Projects"); //-- need to remove

            DropForeignKey("dbo.ProjectResources", "ProjectId", "dbo.Projects"); //--ok
            //DropForeignKey("dbo.ProjectResources", "ComapanyResouceId", "dbo.CompanyResources"); //--need to remove
            DropForeignKey("dbo.ProjectResources", "CompanyResourceId", "dbo.CompanyResources"); //--ok

            DropForeignKey("dbo.ProjectSitePlanTasks", "ProjectSitePlanId", "dbo.ProjectSitePlans"); //-- ok
            DropForeignKey("dbo.ProjectSitePlanTasks", "ProjectTaskId", "dbo.ProjectTasks"); //-- ok
            //DropForeignKey("dbo.ProjectSitePlanTasks", new[] { "ProjectId", "ProjectSiteId", "SitePlanDate" }, "dbo.ProjectSitePlans"); //-- need to remove
            //DropForeignKey("dbo.ProjectSitePlanTasks", "ProjectSiteId", "dbo.ProjectSites"); //-- need to remove
            //DropForeignKey("dbo.ProjectSitePlanTasks", "ProjectId", "dbo.Projects"); //-- need to remove

            DropForeignKey("dbo.ProjectSitePlans", "ProjectSiteId", "dbo.ProjectSites"); //-- ok
            //DropForeignKey("dbo.ProjectSitePlans", "ProjectId", "dbo.Projects"); //-- need to remove

            DropForeignKey("dbo.ProjectSiteImages", "ProjectSiteId", "dbo.ProjectSites"); //-- ok
            DropForeignKey("dbo.ProjectSiteImages", "ProjectId", "dbo.Projects"); //-- ok

            DropForeignKey("dbo.ProjectSites", "ProjectId", "dbo.Projects"); //-- ok

            DropForeignKey("dbo.ProjectSiteResources", "ProjectSiteId", "dbo.ProjectSites"); //--ok
            //DropForeignKey("dbo.ProjectSiteResources", "Project_Id", "dbo.Projects"); //-- need to delete
            //DropForeignKey("dbo.ProjectSiteResources", "ComapanyResouceId", "dbo.CompanyResources"); //-- need to remove
            DropForeignKey("dbo.ProjectSiteResources", "CompanyResourceId", "dbo.CompanyResources"); //-- ok

            //DropForeignKey("dbo.CompanyResourceProjects", "Project_Id", "dbo.Projects"); // -- need to remove
            //DropForeignKey("dbo.CompanyResourceProjects", "CompanyResource_Id", "dbo.CompanyResources"); // -- need to remove

            DropForeignKey("dbo.Projects", "ProjectGroupId", "dbo.ProjectGroups"); //-- ok
            DropForeignKey("dbo.Projects", "ClientId", "dbo.Clients"); //-- ok

            DropForeignKey("dbo.UsersRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersRoles", "RoleId", "dbo.Roles");

            //DropIndex("dbo.CompanyResourceProjects", new[] { "Project_Id" }); //-- need to remove
            //DropIndex("dbo.CompanyResourceProjects", new[] { "CompanyResource_Id" }); //-- need to remove

            DropIndex("dbo.ProjectSiteStatus", new[] { "ProjectSitePlanTaskId" }); //-- ok
            //DropIndex("dbo.ProjectSiteStatus", new[] { "ProjectTaskId" }); //-- need to delete
            //DropIndex("dbo.ProjectSiteStatus", new[] { "ProjectSiteId" }); //-- need to delete
            //DropIndex("dbo.ProjectSiteStatus", new[] { "ProjectId", "ProjectSiteId", "SitePlanDate" }); //-- need to delete
            //DropIndex("dbo.ProjectSiteStatus", new[] { "ProjectId" }); //-- need to delete

            //DropIndex("dbo.ProjectResources", new[] { "ComapanyResouceId" }); //-- need to remove
            DropIndex("dbo.ProjectResources", new[] { "CompanyResourceId" }); //-- ok
            DropIndex("dbo.ProjectResources", new[] { "ProjectId" }); //-- ok

            DropIndex("dbo.ProjectSitePlanTasks", new[] { "ProjectSitePlanId" }); //--ok
            DropIndex("dbo.ProjectSitePlanTasks", new[] { "ProjectTaskId" }); //--ok
            //DropIndex("dbo.ProjectSitePlanTasks", new[] { "ProjectSiteId" }); //-- need to delete
            //DropIndex("dbo.ProjectSitePlanTasks", new[] { "ProjectId", "ProjectSiteId", "SitePlanDate" }); //-- need to delete
            //DropIndex("dbo.ProjectSitePlanTasks", new[] { "ProjectId" }); //-- need to delete

            DropIndex("dbo.ProjectSitePlans", "PlanCodeIndex"); //-- ok
            DropIndex("dbo.ProjectSitePlans", new[] { "ProjectSiteId" }); //-- ok
            //DropIndex("dbo.ProjectSitePlans", new[] { "ProjectId" }); // -- need to delete

            DropIndex("dbo.ProjectSiteImages", new[] { "ProjectSiteId" }); //-- ok
            DropIndex("dbo.ProjectSiteImages", new[] { "ProjectId" }); //-- ok

            DropIndex("dbo.ProjectSites", new[] { "ProjectId" }); //--ok

            //DropIndex("dbo.ProjectSiteResources", new[] { "Project_Id" }); //-- need to delete
            //DropIndex("dbo.ProjectSiteResources", new[] { "ComapanyResouceId" }); //-- need to delete
            DropIndex("dbo.ProjectSiteResources", new[] { "CompanyResourceId" }); //-- ok
            DropIndex("dbo.ProjectSiteResources", new[] { "ProjectSiteId" }); //-- ok

            DropIndex("dbo.UsersRoles", new[] { "RoleId" }); //--ok 
            DropIndex("dbo.UsersRoles", new[] { "UserId" }); //--- ok

            DropIndex("dbo.Projects", new[] { "ProjectGroupId" }); // ok
            DropIndex("dbo.Projects", new[] { "ClientId" });// ok

            //DropTable("dbo.CompanyResourceProjects"); //need to remove
            DropTable("dbo.ProjectSiteStatus");
            DropTable("dbo.ProjectResources");
            DropTable("dbo.PreStatus");
            DropTable("dbo.CompanyInformations");
            DropTable("dbo.ProjectGroups");
            DropTable("dbo.ProjectTasks");
            DropTable("dbo.ProjectSitePlanTasks");
            DropTable("dbo.ProjectSitePlans");
            DropTable("dbo.ProjectSiteImages");
            DropTable("dbo.ProjectSites");
            DropTable("dbo.ProjectSiteResources");
            DropTable("dbo.CompanyResources");
            DropTable("dbo.Projects");
            DropTable("dbo.Clients");

            
            DropTable("dbo.UsersRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
        }
    }
}
