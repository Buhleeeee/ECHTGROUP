namespace ECHTGROUP.Models{ //start of namespace

public class AskRequest
{ //start of class
    public string? Text { get; set; }
    public string? Choice { get; set; }
    public string? Dept { get; set; }
} //end of class

public class AssistantOption
{ //start of class
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
} //end of class

public class AssistantReply
{ //start of class
    public string Message { get; set; } = "";
    public string? Department { get; set; }
    public bool Handoff { get; set; }
    public List<AssistantOption> Options { get; set; } = new();
    public string? LinkKey { get; set; }
    public string? LinkLabel { get; set; }
    public string? Url { get; set; }
} //end of class

public class HandoffRequest
{
    public string? Department { get; set; }
    public string? Text { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
} //end of class
} //end of namespace