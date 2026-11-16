namespace ECHTGROUP.Models
{ //start of namespace

    public class ContactMessage
    { //end of class
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    } //end of class
} // end if namespace