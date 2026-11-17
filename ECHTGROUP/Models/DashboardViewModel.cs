using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECHTGROUP.Models
{

    public static class ProfileOptions
    {
        public static readonly string[] Titles = { "Mr", "Mrs", "Ms", "Miss", "Mx", "Dr", "Prof" };
        public static readonly string[] Genders = { "Female", "Male", "Non-binary", "Prefer not to say" };
    }

    public class DashboardViewModel
    {
        public string Name { get; set; } = "";
        public string? PhotoPath { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int UpcomingCount { get; set; }
        public List<Booking> Next { get; set; } = new();
    }

    public class ProfileViewModel
    {
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        [StringLength(50, ErrorMessage = "Please keep this under 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Please enter your surname")]
        [StringLength(50, ErrorMessage = "Please keep this under 50 characters")]
        public string Surname { get; set; } = "";

        public string? Gender { get; set; }

        [Phone(ErrorMessage = "That number doesn’t look right")]
        [Display(Name = "Contact number")]
        public string? Phone { get; set; }

        [Phone(ErrorMessage = "That number doesn’t look right")]
        [Display(Name = "Alternative contact (optional)")]
        public string? AlternativeContact { get; set; }

        [EmailAddress(ErrorMessage = "That email doesn’t look right")]
        [Display(Name = "Alternative email (optional)")]
        public string? AlternativeEmail { get; set; }

        [StringLength(120, ErrorMessage = "Please keep this under 120 characters")]
        [Display(Name = "Street address")]
        public string? AddressLine { get; set; }

        [StringLength(60, ErrorMessage = "Please keep this under 60 characters")]
        public string? City { get; set; }

        [StringLength(10, ErrorMessage = "Please keep this under 10 characters")]
        [Display(Name = "Postal code")]
        public string? PostalCode { get; set; }

        [Display(Name = "Profile photo")]
        public IFormFile? Photo { get; set; }

        public string? Email { get; set; }
        public string? PhotoPath { get; set; }
        public string? RoleName { get; set; }
        public string? DisplayName { get; set; }
    }
}