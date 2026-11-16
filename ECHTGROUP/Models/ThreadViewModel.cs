using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //start of namespace

    public class ThreadViewModel
    { //start of class
        public List<ChatMessage> Messages { get; set; } = new();

        [Required(ErrorMessage = "Please write a message")]
        [StringLength(1000, ErrorMessage = "Please keep your message under 1000 characters")]
        public string Body { get; set; } = "";
    } //end of classs
} //end of namespace