namespace ECHTGROUP.Models
{

    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public bool IsHidden { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedUtc { get; set; }
    }
}