using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //start of namespace

    public class ContactViewModel
    { //start of class
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "That email doesn’t look right")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please write a message")]
        [StringLength(2000, ErrorMessage = "Please keep your message under 2000 characters")]
        public string Message { get; set; } = "";
    } //end of class
} //end of namespace