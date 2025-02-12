using AccelokaAPI.Models;
using AccelokaAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AccelokaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // ✅ Define DbSet for each model
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Category> Categories { get; set; } // Assuming you have a Category model
        public DbSet<BookedTicket> BookedTickets { get; set; }
        public DbSet<BookedTicketDetail> BookedTicketDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Configure Relationships
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            // Configure Ticket Foreign Key
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
            
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.Code)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<BookedTicketDetail>()
                .HasOne(btd => btd.Ticket)
                .WithMany()
                .HasForeignKey(btd => btd.TicketId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
