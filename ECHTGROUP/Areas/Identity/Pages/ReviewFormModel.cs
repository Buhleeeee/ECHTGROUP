using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models;

public class ReviewFormViewModel
{
    [Required(ErrorMessage = "Please choose a rating")]
    [Range(1, 5, ErrorMessage = "Please choose a rating")]
    public int? Rating { get; set; }

    [Required(ErrorMessage = "Please write a few words")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Please write between 10 and 1000 characters")]
    [Display(Name = "Your review")]
    public string Comment { get; set; } = "";
}

public class ReviewsPageViewModel
{
    public List<Review> Reviews { get; set; } = new();
    public int Count { get; set; }
    public double Average { get; set; }
    public bool CanModerate { get; set; }
}