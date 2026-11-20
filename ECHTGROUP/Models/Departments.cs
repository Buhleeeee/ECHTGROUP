namespace ECHTGROUP.Models
{ //start of namespace

    public static class Departments
    { //start of class
        public const string Marketing = "Marketing";
        public const string MarketDevelopment = "Strategic Market Development";
        public const string Legal = "Legal and Compliance";
        public const string IT = "IT";
        public const string Management = "Management";
        public const string General = "General";

        public static readonly string[] Staff = { Marketing, MarketDevelopment, Legal, IT, Management };
        public static readonly string[] Topics = { Marketing, MarketDevelopment, Legal, IT, General };
        public static readonly string[] TopicChoices = { General, Marketing, MarketDevelopment, Legal, IT };

        public static string? CodeKey(string department) => department switch
        { //start of class
            Marketing => "StaffCodes:Marketing",
            MarketDevelopment => "StaffCodes:MarketDevelopment",
            Legal => "StaffCodes:Legal",
            IT => "StaffCodes:IT",
            Management => "StaffCodes:Management",
            _ => null
        };
    } //end of class
} //end of namespace