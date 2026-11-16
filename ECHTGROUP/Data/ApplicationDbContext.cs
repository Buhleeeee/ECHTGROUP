using ECHTGROUP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{ //start of namespace
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { //start of method
    } //end of method

    public DbSet<Enquiry> Enquiries { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    public DbSet<Booking> Bookings { get; set; }
} //end of namespace