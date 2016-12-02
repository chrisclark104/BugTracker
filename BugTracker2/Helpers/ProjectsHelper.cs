using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Helpers
{
    public class ProjectsHelper
    {
        //private ApplicationDbContext db;

        //public ProjectsHelper(ApplicationDbContext context)
        //{
        //    this.db = context;
        //}

        //public void AssignUser(string userId, int projectId)
        //{
        //    if (!db.Projects.Any(p => p.Id == projectId && p.ProjectUser.Any(u => u.Id == userId)))
        //    {
        //        var user = db.Users.Find(userId);
        //        var project = db.Projects.Find(projectId);
        //        project.ProjectUser.Add(user);
        //    }
        //}
        //public void RemoveUser(string userId, int projectId)
        //{
        //    if (db.Projects.Any(p => p.Id == projectId && p.ProjectUser.Any(u => u.Id == userId)))
        //    {
        //        var user = db.Users.Find(userId);
        //        var project = db.Projects.Find(projectId);
        //        project.ProjectUser.Add(user);
        //    }
        //}
        private ApplicationDbContext db = new ApplicationDbContext();


        }
    }
