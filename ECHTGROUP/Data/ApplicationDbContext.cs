using ECHTGROUP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Data
{ //sart of namespace

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    { //start of class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        { //start of method
            base.OnModelCreating(builder);

            builder.Entity<Review>(e =>
            {
                e.Property(r => r.UserId).HasMaxLength(450);
                e.HasIndex(r => r.UserId).IsUnique();
            });

            builder.Entity<ChatMessage>().Property(m => m.Department).HasDefaultValue(Departments.General);
            builder.Entity<ContactMessage>().Property(m => m.Department).HasDefaultValue(Departments.General);
            builder.Entity<Enquiry>().Property(e => e.Department).HasDefaultValue(Departments.General);
        } //end of method
    } //end of namespace
}