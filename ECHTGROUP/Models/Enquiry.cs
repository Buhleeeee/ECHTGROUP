namespace ECHTGROUP.Models
{ //start of namespace
    public class Enquiry
    { //start of class
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public bool ConsentGiven { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public bool IsHandled { get; set; }
        public string Department { get; set; } = Departments.General;

    } //end of class
} //end of namespace
