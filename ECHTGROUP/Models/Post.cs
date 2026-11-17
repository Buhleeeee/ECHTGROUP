namespace ECHTGROUP.Models
{

    public class Post
    { //start of class
        public string Slug { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Summary { get; set; } = "";
        public List<string> Paragraphs { get; set; } = new();
        public DateTime Published { get; set; }
    } //end of class

    public static class PostData
    { //start of class
        public static readonly List<Post> All = new()
    {
        new Post
        {
            Slug = "legal-check",
            Published = new DateTime(2026, 11, 12),   // legal-check
            Title = "Why every growth plan needs a legal check",
            Category = "Legal and Compliance",
            Summary = "A marketing plan that ignores contracts and data rules can cost more than it earns.",
            Paragraphs = new()
            {
                "Sample article. Growth plans usually start with marketing and sales, and the legal side is added last, if at all.",
                "Checking contracts, terms and data-protection duties early keeps the plan from being rewritten later.",
                "Replace this text with the real article."
            }
        },
        new Post
        {
            Slug = "first-market",
            Published = new DateTime(2026, 11, 09),   // legal-check
            Title = "Choosing your first new market",
            Category = "Strategic Market Development",
            Summary = "A simple way to compare markets before you commit money to one.",
            Paragraphs = new()
            {
                "Sample article. Start with the size of the market, then how easy it is to reach buyers there.",
                "Compare two or three options on the same few measures before choosing one.",
                "Replace this text with the real article."
            }
        },
        new Post
        {
            Slug = "it-decisions",
            Published = new DateTime(2026, 10, 29),   // legal-check
            Title = "Five IT decisions small teams get wrong",
            Category = "IT",
            Summary = "Common technology choices that look cheap now and cost more later.",
            Paragraphs = new()
            {
                "Sample article. Teams often pick tools one at a time, so nothing works together.",
                "A short plan for systems, security and backups avoids most of the pain.",
                "Replace this text with the real article."
            }
        }
    };

        public static Post? Find(string slug) =>
            All.FirstOrDefault(p => p.Slug == slug);
    } //end of class
} //end of namespace