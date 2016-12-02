using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.Attachments = new HashSet<TicketAttachment>();
            this.History = new HashSet<TicketHistory>();
            this.Notifications = new HashSet<TicketNotification>();
        }
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Assigned to")]
        public string AssignedUserId { get; set; }
        [Display(Name = "Created by")]
        public string CreatedUserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        [Display(Name = "Type")]
        public int? TypeId { get; set; }
        [Display(Name ="Status")]
        public int? StatusId { get; set; }
        [Display(Name = "Priority Level")]
        public int? PriorityId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }
        public virtual ICollection<TicketNotification> Notifications { get; set; }
    }
}