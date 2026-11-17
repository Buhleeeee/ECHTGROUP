using Microsoft.AspNetCore.Identity;

namespace ECHTGROUP.Models
{

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public DateTime? ConsentGivenUtc { get; set; }
        public string? PhotoPath { get; set; }

        public string? Title { get; set; }
        public string FirstName { get; set; } = "";
        public string Surname { get; set; } = "";
        public string? Gender { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? AlternativeContact { get; set; }
    }
}