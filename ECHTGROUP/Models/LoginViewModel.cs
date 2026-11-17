using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //end of namespace

    public class LoginViewModel
    { //start of class
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "That email doesn’t look right")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string AccountType { get; set; } = "Client";
    } //end of class
}//end of namespace