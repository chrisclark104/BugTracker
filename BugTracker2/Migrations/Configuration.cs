namespace BugTracker2.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using BugTracker2.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker2.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker2.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>
            (new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            var userManager = new UserManager<ApplicationUser>(
        new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "chrisclark104@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "chrisclark104@gmail.com",
                    Email = "chrisclark104@gmail.com",
                    Fname = "Chris",
                    Lname = "Clark",
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "jtwichell@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jtwichell@coderfoundry.com",
                    Email = "jtwichell@coderfoundry.com",
                    Fname = "Jason",
                    Lname = "Twichell",
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "demodev@zmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demodev@zmail.com",
                    Email = "demodev@zmail.com",
                    Fname = "Demo",
                    Lname = "Developer",
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "elepalmer@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "elepalmer@coderfoundry.com",
                    Email = "elepalmer@coderfoundry.com",
                    Fname = "Ele",
                    Lname = "Palmer",
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "demosubmitter.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demosubmitter.com",
                    Email = "demosubmitter.com",
                    Fname = "Demo",
                    Lname = "Submitter",
                }, "Abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "sedison@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "sedison@coderfoundry.com",
                    Email = "sedison@coderfoundry.com",
                    Fname = "Steven",
                    Lname = "Edison",
                }, "Abc&123!");
            }
            if (!context.Users.Any(u => u.Email == "demoprojectmanager.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demoprojectmanager.com",
                    Email = "demoprojectmanager.com",
                    Fname = "Demo",
                    Lname = "PM",
                }, "Abc&123!");
            }

            var userId = userManager.FindByEmail("chrisclark104@gmail.com").Id; userManager.AddToRole(userId, "Admin");

            var juserId = userManager.FindByEmail("jtwichell@coderfoundry.com").Id; userManager.AddToRole(juserId, "Project Manager");
            var duserId = userManager.FindByEmail("demodev@zmail.com").Id; userManager.AddToRole(duserId, "Developer");

            var euserId = userManager.FindByEmail("elepalmer@coderfoundry.com").Id; userManager.AddToRole(euserId, "Developer");
            var subuserId = userManager.FindByEmail("demosubmitter.com").Id; userManager.AddToRole(subuserId, "Submitter");

            var suserId = userManager.FindByEmail("sedison@coderfoundry.com").Id; userManager.AddToRole(suserId, "Submitter");
            var puserId = userManager.FindByEmail("demoprojectmanager.com").Id; userManager.AddToRole(puserId, "Project Manager");

            context.TicketStatus.AddOrUpdate(s => s.Status,
           new TicketStatus() { Status = "Open" },
           new TicketStatus() { Status = "Pending" },
           new TicketStatus() { Status = "Closed" });

            context.TicketPriorities.AddOrUpdate(p =>p.Priority,
           new TicketPriority() { Priority = "High" },
           new TicketPriority() { Priority = "Medium" },
           new TicketPriority() { Priority = "Low" });

            context.TicketTypes.AddOrUpdate(t => t.Type,
           new TicketType() { Type = "Bug" },
           new TicketType() { Type = "Feature Request" },
           new TicketType() { Type = "Documentation" });
        }            
    }
}