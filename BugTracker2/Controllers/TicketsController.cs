using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using BugTracker2.Helpers;

namespace BugTracker2
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            List<Ticket> tickets = new List<Ticket>();
            if (User.IsInRole("Admin"))
            { tickets = db.Tickets.ToList(); }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var assignedProjects = (from p in db.Projects
                                    where p.Users.Any(r => r.Id == user.Id)
                                    select p).ToList();

            if (User.IsInRole("Project Manager"))
            {
                List<Ticket> bugs = new List<Ticket>();
                foreach (var p in assignedProjects)
                {
                    foreach (var t in db.Tickets)
                    {
                        if (p.Id == t.ProjectId)
                        {
                            bugs.Add(t);
                        }
                    }

                }
                return View(bugs);
            }
            if (User.IsInRole("Submitter"))
            {
                var bugs = (from b in db.Tickets
                            where b.CreatedUserId == user.Id
                            select b).ToList();
                return View(bugs);
            }

            if (User.IsInRole("Developer"))
            {
                var bugs = (from b in db.Tickets
                            where b.AssignedUserId == user.Id
                            select b).ToList();
                return View(bugs);
            }
            //{
            //    string userId = User.Identity.GetUserId();
            //    tickets = db.Tickets.Where(u => u.AssignedUserId == userId).ToList();
            //}
            return View(tickets);
        }

        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (User.IsInRole("Admin"))
            {
                return View(ticket);
            }
            if (User.IsInRole("Project Manager"))
            {
                //View Ticket details for a ticket that is assigned to a project they are assigned with
                string userId = User.Identity.GetUserId();
                var project = ticket.Project;
                var users = project.Users.ToList();
                //Look up specific project for this ticketId
                //Pull up a list of users in that project
                //Linq statement to check to see if logged in user is in the same list of project users
                bool hasUsers = users.Any(u => u.Id == userId);
                {
                    return View(ticket);
                }
            }
            if (User.IsInRole("Developer"))
            {
                //View Ticket details for only a ticket they are assigned
                string userId = User.Identity.GetUserId();
                if (userId != ticket.AssignedUserId)
                {
                    return RedirectToAction("Index");
                }
            }
            if (User.IsInRole("Submitter"))
            {
                //View details of a ticket they created
                string userId = User.Identity.GetUserId();
                if (userId != ticket.CreatedUserId)
                {
                    return RedirectToAction("Index");
                }
            }
                      
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);
            ViewBag.ProjectId = new SelectList(user.Projects, "Id", "Title");
            //ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,StatusId,CreatedUserId,Title,Body,PriorityId,TypeId")] Ticket ticket)
        {

            if (ModelState.IsValid)
            {
                ticket.CreatedUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                TicketHistory history = new TicketHistory()
                {
                    TicketId = ticket.Id,
                    History = "Created",//If the History field is meant to store a description of what event took place regarding the ticket then you would assign to it something like "Created" in this case
                    Modified = DateTime.Now,//Modified may not be the best term to describe what this field represents because basically all this field represents is when this history record was created. The context that 
                    //the 'History' field sets can imply whether or not it was a modification event, creation event or whatever other events you may have for a ticket.
                    UserId = User.Identity.GetUserId()
                };


                db.TicketHistories.Add(history);
                ticket.Status = db.TicketStatus.FirstOrDefault(s => s.Status == "Open");
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedUserId = new SelectList(db.Users, "Id", "Fname", ticket.CreatedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            //ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.StatusId);           
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.TypeId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.PriorityId);

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var developerId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
            var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == developerId));
            ViewBag.AssignedUserId = new SelectList(developers, "Id", "Fname", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.StatusId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.TypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,AssignedUserId,CreatedUserId,Title,Body,Created,StatusId,PriorityId,TypeId")] Ticket ticket)
        {
            var editable = new List<string>() { "Title", "Body" };
            if (User.IsInRole("Admin"))
                editable.AddRange(new string[] { "AssignedUserId", "TypeId", "PriorityId", "StatusId", "Updated" });
            if (User.IsInRole("Project Manager"))
                editable.AddRange(new string[] { "AssignedUserId", "TypeId", "PriorityId", "StatusId", "Updated" });
            
            

            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                ticket.Updated = DateTimeOffset.Now;
                StringBuilder historyBody = new StringBuilder();
                db.Entry(ticket).State = EntityState.Modified;
                var newTicket = ticket;
                var history = new TicketHistory();

                if (oldTicket.Title != ticket.Title)
                historyBody.AppendFormat("<br>Old Title: {0} <br>New Title: {1}", oldTicket.Title, ticket.Title);
                if (oldTicket.Body != ticket.Body)
                    historyBody.AppendFormat("<br>Old Title: {0} <br>New Title: {1}", oldTicket.Body, ticket.Body);
                if (oldTicket.Priority != ticket.Priority)
                    historyBody.AppendFormat("<br>Old Title: {0} <br>New Title: {1}", oldTicket.Priority, ticket.Priority);
                if (oldTicket.Status != ticket.Status)
                    historyBody.AppendFormat("<br>Old Title: {0} <br>New Title: {1}", oldTicket.Status, ticket.Status);
                // append the changes to the history's body
                history.Body = historyBody.ToString();
                //    if (oldTicket.Status != newTicket.Status)
                //{ }
                db.Update(ticket, editable.ToArray());
                db.TicketHistories.Add(history);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { ticket.Id });
            }
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Fname", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.StatusId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.TypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ticket ticket = await db.Tickets.FindAsync(id);
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST Tickets/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include = "TicketId,Comment,Created")]Comment ticketComment, int ticketId)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                Ticket ticket = db.Tickets.Find(ticketId);

                bool notificationChecker = false;

                var commentHistory = new TicketHistory()
                {
                    TicketId = ticketId,
                    Property = "Comments",
                    PropertyDisplay = "Comment",
                    OldValue = ticket.Comments.Count().ToString(),
                    OldValueDisplay = ticket.Comments.Count().ToString(),
                    NewValue = (ticket.Comments.Count() + 1).ToString(),
                    NewValueDisplay = (ticket.Comments.Count() + 1).ToString(),
                    Modified = DateTimeOffset.Now,
                    UserId = user.Email,
                    Ticket = db.Tickets.Find(ticketId),
                    User = user
                };

                var assignedUser = ticket.AssignedUser;

                var notification = assignedUser != null ? new IdentityMessage()
                {
                    Subject = "A comment has been made on a ticket you're assigned to.",
                    Destination = assignedUser.Email,
                    Body = user.Email + " left the following comment on your ticket titled '" + ticket.Title + "': <br /><br />" + ticketComment.Body
                } : null;

                if (notification != null)
                {
                    var mailer = new EmailService();
                    mailer.SendAsync(notification);
                    notificationChecker = true;
                }

                ticketComment.UserId = User.Identity.GetUserId();
                ticketComment.Created = DateTimeOffset.Now;
                db.Comments.Add(ticketComment);
                db.TicketHistories.Add(commentHistory);

                if (notificationChecker)
                {
                    var notificationHistory = new TicketHistory()
                    {
                        TicketId = ticketId,
                        Property = "Notifications",
                        PropertyDisplay = "Notification",
                        OldValue = ticket.Notifications.Count().ToString(),
                        OldValueDisplay = ticket.Notifications.Count().ToString(),
                        NewValue = (ticket.Notifications.Count() + 1).ToString(),
                        NewValueDisplay = (ticket.Notifications.Count() + 1).ToString(),
                        Modified = DateTimeOffset.Now,
                        UserId = user.Email,
                        Ticket = db.Tickets.Find(ticketId),
                        User = user
                    };
                    db.TicketHistories.Add(notificationHistory);
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticketId });
            }
            return View(ticketComment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttachment([Bind(Include = "TicketId,Description")]TicketAttachment ticketAttachment, HttpPostedFileBase file, int ticketId)
        {
            if (file != null && file.ContentLength > 0)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (ext != ".png" && ext != ".jpg" && ext != ".gif" && ext != ".bmp" && ext != ".txt" && ext != ".pfd")
                {
                    ModelState.AddModelError("file", "Invalid Format.");
                }
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~" + filePath);
                    ticketAttachment.MediaURL = filePath + file.FileName;
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                }

                var user = db.Users.Find(User.Identity.GetUserId());

                Ticket ticket = db.Tickets.Find(ticketId);

                bool notificationChecker = false;

                var attachmentHistory = new TicketHistory()
                {
                    TicketId = ticketId,
                    Property = "Attachments",
                    PropertyDisplay = "Attachment",
                    OldValue = ticket.Attachments.Count().ToString(),
                    OldValueDisplay = ticket.Attachments.Count().ToString(),
                    NewValue = (ticket.Attachments.Count() + 1).ToString(),
                    NewValueDisplay = (ticket.Attachments.Count() + 1).ToString(),
                    Modified = DateTimeOffset.Now,
                    UserId = user.Email,
                    Ticket = db.Tickets.Find(ticketId),
                    User = user
                };

                var assignedUser = ticket.AssignedUser;

                var notification = assignedUser != null ? new IdentityMessage()
                {
                    Subject = "An attachment has been added to a ticket that you're assigned to.",
                    Destination = assignedUser.Email,
                    Body = user.Email + " has added an attachment to a ticket you're assigned to. The ticket title is '" + ticket.Title + "'.<br /><br />Following is informaton regarding the attachment:<br /><b>Created: </b>" + ticketAttachment.CreatedDate + "<br /><b>Description: </b>" + ticketAttachment.Body != null ? ticketAttachment.Body : null
                } : null;

                if (notification != null)
                {
                    var mailer = new EmailService();
                    mailer.SendAsync(notification);
                    notificationChecker = true;
                }

                ticketAttachment.UserId = User.Identity.GetUserId();
                ticketAttachment.CreatedDate = DateTimeOffset.Now;
                db.Attachments.Add(ticketAttachment);
                db.TicketHistories.Add(attachmentHistory);

                if (notificationChecker)
                {
                    var notificationHistory = new TicketHistory()
                    {
                        TicketId = ticketId,
                        Property = "Notifications",
                        PropertyDisplay = "Notification",
                        OldValue = ticket.Notifications.Count().ToString(),
                        OldValueDisplay = ticket.Notifications.Count().ToString(),
                        NewValue = (ticket.Notifications.Count() + 1).ToString(),
                        NewValueDisplay = (ticket.Notifications.Count() + 1).ToString(),
                        Modified = DateTimeOffset.Now,
                        UserId = user.Email,
                        Ticket = db.Tickets.Find(ticketId),
                        User = user
                    };
                    db.TicketHistories.Add(notificationHistory);
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticketId });
            }
            return View(ticketAttachment);
        }
    }
}
