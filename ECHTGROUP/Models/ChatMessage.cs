namespace ECHTGROUP.Models
{ //start of namespace
  
    public class ChatMessage
    { //start of class
        public int Id { get; set; }
        public string ClientId { get; set; } = "";
        public string SenderId { get; set; } = "";
        public bool FromConsultant { get; set; }
        public string Body { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    } //end of class
}//end of namespace