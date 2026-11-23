using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECHTGROUP.Models
{//start of namespace

    public static class TeamGroups
    { //start of class
        public const string Leadership = "Leadership";
        public const string Consultants = "Consultants";
        public const string Agents = "Agents";

        public static readonly string[] All = { Leadership, Consultants, Agents };
    } //end of class

    public class TeamMember
    { //start of class
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string Group { get; set; } = TeamGroups.Consultants;
        public string? Department { get; set; }
        public string? Bio { get; set; }
        public string? PhotoPath { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
    } //end of class

    public class TeamMemberForm
    { //start of class
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        [StringLength(100, ErrorMessage = "Please keep this under 100 characters")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Please enter a job title")]
        [StringLength(100, ErrorMessage = "Please keep this under 100 characters")]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; } = "";

        [Required(ErrorMessage = "Please choose a group")]
        public string Group { get; set; } = TeamGroups.Consultants;

        public string? Department { get; set; }

        [StringLength(600, ErrorMessage = "Please keep the bio under 600 characters")]
        public string? Bio { get; set; }

        [Range(0, 999, ErrorMessage = "Please use a number from 0 to 999")]
        [Display(Name = "Display order")]
        public int SortOrder { get; set; }

        [Display(Name = "Show on the Team page")]
        public bool IsVisible { get; set; } = true;

        [Display(Name = "Photo")]
        public IFormFile? Photo { get; set; }

        public bool RemovePhoto { get; set; }
        public string? PhotoPath { get; set; }
    } //end of class
} //end of namespace