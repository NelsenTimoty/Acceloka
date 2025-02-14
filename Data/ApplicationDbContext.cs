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

            // Configure Relationships
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            // Configure Ticket Foreign Key
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.Code)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<BookedTicketDetail>()
                .HasOne(btd => btd.Ticket)
                .WithMany()
                .HasForeignKey(btd => btd.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookedTicket>()
                .HasMany(bt => bt.BookedTicketDetails)
                .WithOne(btd => btd.BookedTicket)
                .HasForeignKey(btd => btd.BookedTicketId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Default Seed Data

            var concertId = Guid.Parse("d1b2c3d4-e5f6-789a-bcde-f01234567890");
            var sportsId = Guid.Parse("a1b2c3d4-e5f6-789a-bcde-f01234567891");
            var theaterId = Guid.Parse("a1b2c3d4-e5f6-789a-bcde-f01234567892");

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = concertId, CategoryName = "Concert" },
                new Category { CategoryId = sportsId, CategoryName = "Sports" },
                new Category { CategoryId = theaterId, CategoryName = "Theater" }
            );

            var fixedDateSucess = new DateTime(2026, 2, 14, 12, 0, 0, DateTimeKind.Utc);
            var fixedDateFail = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            // Seeding Tickets using the fixed GUIDs for categories
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = Guid.Parse("f1b2c3d4-e5f6-789a-bcde-f01234567893"),
                    Code = "RF001",
                    Name = "Queens Rock Festival",
                    CategoryId = concertId,  // Matches the Concert category
                    EventDate = fixedDateSucess.AddDays(30),
                    Price = 150.00m,
                    Quota = 100,
                    CreatedAt = fixedDateFail,
                    CreatedBy = "SYSTEM",
                    UpdatedAt = fixedDateFail,
                    UpdatedBy = "SYSTEM"
                },
                new Ticket
                {
                    Id = Guid.Parse("f1b2c3d4-e5f6-789a-bcde-f01234567894"),
                    Code = "BF002",
                    Name = "NBA Basketball Finals",
                    CategoryId = sportsId,  // Matches the Sports category
                    EventDate = fixedDateFail.AddDays(14),
                    Price = 200.00m,
                    Quota = 50,
                    CreatedAt = fixedDateFail,
                    CreatedBy = "SYSTEM",
                    UpdatedAt = fixedDateFail,
                    UpdatedBy = "SYSTEM"
                },
                new Ticket
                {
                    Id = Guid.Parse("f1b2c3d4-e5f6-789a-bcde-f01234567843"),
                    Code = "BM003",
                    Name = "Hollywood Broadway Musical",
                    CategoryId = theaterId,  // Matches the Theater category
                    EventDate = fixedDateSucess.AddDays(36),
                    Price = 180.00m,
                    Quota = 75,
                    CreatedAt = fixedDateFail,
                    CreatedBy = "SYSTEM",
                    UpdatedAt = fixedDateFail,
                    UpdatedBy = "SYSTEM"
                }
            );
        }
    }
}
