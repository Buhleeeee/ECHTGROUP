namespace ECHTGROUP.Models
{ //start of namespace
    public class ServiceData
    { //start of class
        public static readonly List<ServiceInfo> All = new()
    {
        new ServiceInfo
        {
            Slug = "marketing",
            Title = "Marketing",
            Summary = "Positioning and campaigns that reach the right buyers and can be measured.",
            WhatWeDo = new() { "Brand and messaging", "Digital and content strategy", "Campaign planning and reporting" },
            WhatYouGet = new() { "Brand positioning workshop", "12-month campaign calendar", "Monthly performance report" }
        },
        new ServiceInfo
        {
            Slug = "market-development",
            Title = "Strategic Market Development",
            Summary = "Evidence-based plans for entering new markets and growing the ones you are in.",
            WhatWeDo = new() { "Market research and sizing", "Entry and expansion strategy", "Partnerships and channels" },
            WhatYouGet = new() { "Market sizing report", "Entry roadmap", "Partner shortlist" }
        },
        new ServiceInfo
        {
            Slug = "legal-compliance",
            Title = "Legal and Compliance",
            Summary = "Practical guidance that keeps your business protected as it grows.",
            WhatWeDo = new() { "Contracts and commercial terms", "Regulatory and data-protection compliance", "Policies and risk reviews" },
            WhatYouGet = new() { "Contract review", "Compliance checklist", "Risk register" }
        },
        new ServiceInfo
        {
            Slug = "it",
            Title = "IT",
            Summary = "Technology choices that fit your goals, budget and security needs.",
            WhatWeDo = new() { "IT strategy and systems selection", "Cybersecurity and data protection", "Digital tools and process automation" },
            WhatYouGet = new() { "Systems audit", "Security assessment", "Automation plan" }
        }
    };

        public static ServiceInfo? Find(string slug) =>
            All.FirstOrDefault(s => s.Slug == slug);
    } //end of class
} //end of namespace
