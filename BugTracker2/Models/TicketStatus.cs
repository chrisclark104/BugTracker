using BugTracker2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketStatus
    {
        public TicketStatus()
        {
            this.Ticket = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Status { get; set; }
        

        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}