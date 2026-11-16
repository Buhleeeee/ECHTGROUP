namespace ECHTGROUP.Models
{ //start of namespace

    public class Booking
    { //start of class
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string Service { get; set; } = "";
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Status { get; set; } = "Requested";
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    } //end of class
} //end of namespace