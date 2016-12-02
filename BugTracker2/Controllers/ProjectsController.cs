using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker2.Models;
using BugTracker2.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker2
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //ProjectsHelper projectUser = new ProjectsHelper();

        // GET: Projects
        //[Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Index()
        {
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
                projects = db.Projects.ToList();
            else {
                string userId = User.Identity.GetUserId();
                projects = db.Projects.Where(p => p.Users.Any(u => u.Id == userId)).ToList();
            }
            return View(projects);
        }

        //// GET: Projects/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Project project = db.Projects.Find(id);
        //    List<Ticket> tickets = new List<Ticket>();
        //    tickets = db.Tickets.ToList();
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(project);
        //}

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: /Admin/EditProject
        // **** See Handout for Completed Example ****
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditProjectUser(int? id)
        {
            Project project = db.Projects.Find(id);
            ProjectUserViewModel ProjectUserModel = new ProjectUserViewModel();
            var selected = project.Users.Select(m => m.Id).ToList();
            List<SelectListItem> selectListUsers = db.Users.Select(u => new SelectListItem() { Value = u.Id, Text = u.Fname + " " + u.Lname }).ToList();
            ProjectUserModel.Users = new MultiSelectList(selectListUsers, "Value", "Text", selected);
            ProjectUserModel.Project = project;

            return View(ProjectUserModel);
        }

        //
        // POST:
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        public ActionResult EditProjectUser(ProjectUserViewModel model)
        {
            var project = db.Projects.Find(model.Project.Id);

            project.Users.Clear();

            foreach (var users in model.SelectedUsers)
                project.Users.Add(db.Users.Find(users));

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}