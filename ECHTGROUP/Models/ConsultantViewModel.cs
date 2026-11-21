using System.ComponentModel.DataAnnotations;

namespace ECHTGROUP.Models
{ //start of namespace

public class InboxItem
{ //start of class
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string Preview { get; set; } = "";
    public string Department { get; set; } = "";
    public DateTime LastAt { get; set; }
    public bool NeedsAttention { get; set; }
    public int Unread { get; set; }
    public string? ClientId { get; set; }
    public int Id { get; set; }
} //end of class

public class InboxViewModel
{ //start of class
    public List<InboxItem> Items { get; set; } = new();
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Dept { get; set; }
    public bool SeesAll { get; set; }
    public string[] DeptChoices { get; set; } = Array.Empty<string>();
    public int OpenCount { get; set; }
} //end of class

public class InboxDetailViewModel
{ //start of class
    public string Type { get; set; } = "";
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Body { get; set; }
    public string Department { get; set; } = "";
    public DateTime CreatedUtc { get; set; }
    public bool IsHandled { get; set; }
} //end of class

public class ConsultantThreadViewModel
{ //start of class
    public string? ClientId { get; set; }
    public string? ClientName { get; set; }
    public string? Department { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();

    [Required(ErrorMessage = "Please write a reply")]
    [StringLength(1000, ErrorMessage = "Please keep your reply under 1000 characters")]
    public string Body { get; set; } = "";
} //end of class
} //end of namespace