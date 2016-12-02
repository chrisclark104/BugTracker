using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTracker2.Models;
using System.Collections.Generic;

namespace BugTracker2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string UserRoles { get; set; }
        public string Timezone { get; set; }
        public ApplicationUser()
        {
            this.Projects = new HashSet<Project>();
        }       
        public virtual ICollection<Project> Projects { get; set; }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        // Add custom user claims here
        return userIdentity;
    }
}
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<TicketAttachment> Attachments { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TicketHistory> TicketHistories { get; set; }

        public DbSet<TicketPriority> TicketPriorities { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketStatus> TicketStatus { get; set; }

        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<TicketNotification> TicketNotifications { get; set; }

        // public System.Data.Entity.DbSet<BugTracker2.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}