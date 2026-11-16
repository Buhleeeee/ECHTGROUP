using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models 
{  //strat of namespace

public class DashboardViewModel
{ //start of class
    public string Name { get; set; } = "";
    public int UpcomingCount { get; set; }
    public List<Booking> Next { get; set; } = new();
} //end of class

public class ProfileViewModel
{ //start of class
    [Required(ErrorMessage = "Please enter your full name")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    public string? Email { get; set; }
}  //end of class
} //end of namespace
