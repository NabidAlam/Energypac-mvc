namespace AuditReport.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;



    internal sealed class Configuration : DbMigrationsConfiguration<AuditReport.Models.EPARSDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AuditReport.Models.EPARSDbContext context)
        {
           // context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_Category_Title ON Categories (Title)");


            //context.falseModel.AddOrUpdate(new AuditReport.Models.FalseModel { FalseData="SS", Id=1 });
           // context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('FalseModels', RESEED, 3000);");
        }




    }
}
