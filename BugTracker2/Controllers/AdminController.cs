using BugTracker2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRoleAssignHelper userRole = new UserRoleAssignHelper();

        [Authorize(Roles = "Admin")]
        // GET: Admin
        public ActionResult Index()
        {
            //List<AdminUserViewModel> userList = new List<AdminUserViewModel>();
            //foreach (var users in db.Users.ToList())
            //{
            //    var userCollection = new AdminUserViewModel();
            //    userCollection.User = users;
            //    userCollection.Role = userRole.ListUserRoles(users.Id).ToList();
            //    userList.Add(userCollection);
            //}
            return View(db.Users.ToList());
        }

        [Authorize(Roles = "Admin")]
        //Get: Admin/SelecRole/5
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new Models.AdminUserViewModel();
            var selected = userRole.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

                return View(AdminModel);
        }

        [Authorize(Roles = "Admin")]
        //POST: Admin/SelectRoles/5
        [HttpPost]
        public ActionResult Edit(AdminUserViewModel model)
        {
            var user = db.Users.Find(model.User.Id);
            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList())
            //foreach (var rolermv in user.Roles.ToList())
            {
                userRole.RemoveUserFromRole(user.Id, rolermv);
            }

            if (model.SelectedRoles != null)
            {
                foreach (var roleadd in model.SelectedRoles)
                {
                    userRole.AddUserToRole(user.Id, roleadd);
                }
            }
            return RedirectToAction("Index");

        }
    }
}