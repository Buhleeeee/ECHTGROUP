using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //start of namespace

    public class GetStartedModel
    { //start of class
        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name = "Name")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Please enter your surname")]
        [Display(Name = "Surname")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "That email doesn’t look right")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please enter your contact number")]
        [Phone(ErrorMessage = "That number doesn’t look right")]
        [Display(Name = "Contact number")]
       
        //getters and setters
        public string Phone { get; set; } = "";

        public bool Consent { get; set; }
    } //end of class
} //end of namespace