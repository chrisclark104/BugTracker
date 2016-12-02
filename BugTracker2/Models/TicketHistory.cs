using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Body { get; set; }
        public string Property { get; set; }
        public string PropertyDisplay { get; set; }
        public string OldValue { get; set; }
        public string OldValueDisplay { get; set; }
        public string NewValue { get; set; }
        public string NewValueDisplay { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string History { get; set; }
        public string UserId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}