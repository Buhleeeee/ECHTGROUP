using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{  //start of namespace

    public static class BookingOptions
    { //start of class
        public static readonly string[] Slots = { "09:00", "10:30", "13:00", "14:30", "16:00" };
    } //end of class

    public class BookingViewModel
    { //start of model class
        [Required(ErrorMessage = "Please choose a service")]
        public string Service { get; set; } = "";

        [Required(ErrorMessage = "Please choose a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Please choose a time")]
        [Display(Name = "Time")]
        public string TimeSlot { get; set; } = "";

        [StringLength(500, ErrorMessage = "Please keep notes under 500 characters")]
        public string? Notes { get; set; }
    } //end of model class
} //end of namespace
