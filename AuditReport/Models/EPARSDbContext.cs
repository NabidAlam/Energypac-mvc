using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace AuditReport.Models
{
    public class EPARSDbContext:DbContext
    {
        public EPARSDbContext() : base("EPARS") 
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UsersRole> UsersRole { get; set; }
        public DbSet<Client> Client {get;set;}
        public DbSet<ProjectGroup> ProjectGroup {get;set;}
        public DbSet<CompanyInformation> CompanyInformation {get;set;}
        public DbSet<CompanyResource> CompanyResource {get;set;}
        public DbSet<Project> Project {get;set;}
        public DbSet<ProjectTask> ProjectTask {get;set;}
        public DbSet<ProjectResource> ProjectResource {get;set;}
        public DbSet<ProjectSite>  ProjectSite {get;set;}
        public DbSet<ProjectSiteResource> ProjectSiteResource {get;set;}
        public DbSet<ProjectSitePlanTask> ProjectSitePlanTask {get;set;}
        //public DbSet<ProjectSiteStatusMaster> ProjectSiteStatusMaster {get;set;}
        //public DbSet<ProjectSiteStatusDetails> ProjectSiteStatusDetails {get;set;}
        public DbSet<ProjectSiteImage> ProjectSiteImage {get;set;}
        public DbSet<ProjectSitePlan> ProjectSitePlan  { get; set; }
        public DbSet<ProjectSiteStatus> ProjectSiteStatus { get; set; }
        public DbSet<PreStatus> PreStatus { get; set; }
      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           // modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

       
        }



    }
}