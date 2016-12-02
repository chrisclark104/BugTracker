using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketPriority
    {
        public TicketPriority()
        {
            this.Ticket = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Priority { get; set; }

        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}