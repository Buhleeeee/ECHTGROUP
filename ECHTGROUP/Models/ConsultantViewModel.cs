using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //start of namespace

    public class InboxItemViewModel
     {//start of class
        public string ClientId { get; set; } = "";
        public string ClientName { get; set; } = "";
        public string LastMessage { get; set; } = "";
        public DateTime LastAt { get; set; }
        public int Unread { get; set; }
    } //end of class

    public class ConsultantThreadViewModel
    { //start of class
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public List<ChatMessage> Messages { get; set; } = new();

        [Required(ErrorMessage = "Please write a reply")]
        [StringLength(1000, ErrorMessage = "Please keep your reply under 1000 characters")]
        public string Body { get; set; } = "";
    } //end of class
    public class EnquiriesViewModel
    {
        public List<ContactMessage> ContactMessages { get; set; } = new();
        public List<Enquiry> Enquiries { get; set; } = new();
    }
} //end of namespace