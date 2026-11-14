using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //start of namespace

    public class RegisterViewModel
    { //start of class
        [Required(ErrorMessage = "Please enter your full name")]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "That email doesn’t look right")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please choose a password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword { get; set; } = "";

        public bool Consent { get; set; }
    } //end of class
} //end of namespace