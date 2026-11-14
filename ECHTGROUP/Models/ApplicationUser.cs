using Microsoft.AspNetCore.Identity;

namespace ECHTGROUP.Models
{ //start of namespace

public class ApplicationUser : IdentityUser
{ //start of class
    public string FullName { get; set; } = "";
    public DateTime? ConsentGivenUtc { get; set; }
} //end of class
    } //end of namespace
