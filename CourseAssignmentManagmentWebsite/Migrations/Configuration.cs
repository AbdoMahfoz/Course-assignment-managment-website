namespace CourseAssignmentManagmentWebsite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CourseAssignmentManagmentWebsite.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CourseAssignmentManagmentWebsite.Models.ApplicationDbContext";
        }

        protected override void Seed(CourseAssignmentManagmentWebsite.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            if (!(from role in context.Roles where role.Name == "professor" select role).Any())
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("professor"));
            }
            if (!(from role in context.Roles where role.Name == "student" select role).Any())
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("student"));
            }
            context.SaveChanges();
        }
    }
}
