using System.Text.RegularExpressions;

namespace ECHTGROUP.Models
{ //start of namespace

    public static class AssistantBrain
    { //start of class
      //  the referral sentence.
        public const string ReferralLead = "This needs a closer look, so I’ll be referring you to a relevent consultant.";

        private static readonly Dictionary<string, string[]> Stems = new()
        {
            [Departments.Marketing] = new[] { "marketing", "brand", "branding", "campaign", "social", "advertise", "content", "seo" },
            [Departments.MarketDevelopment] = new[] { "expansion", "expand", "entry", "enter", "growth", "partnership", "partner", "competitor", "research", "sizing" },
            [Departments.Legal] = new[] { "legal", "contract", "complian", "law", "lawyer", "popia", "gdpr", "polic", "agreement", "terms", "regulat", "privacy" },
            [Departments.IT] = new[] { "software", "system", "cyber", "secur", "data", "automat", "technolog", "network", "hosting", "computer" }
        };

        private static readonly HashSet<string> DetailWords = new()
    {
        "quote", "quotes", "price", "prices", "pricing", "cost", "costs", "fee", "fees", "rate", "rates",
        "invoice", "urgent", "urgently", "lawsuit", "court", "complaint", "audit", "breach", "hacked",
        "proposal", "tender", "advice"
    };

        private static readonly string[] DetailPhrases = { "how much", "my company", "my business", "our company", "our business" };

        private static readonly HashSet<string> Greetings = new() { "hi", "hello", "hey", "howzit", "hallo", "morning", "afternoon", "evening" };

        public static AssistantReply Reply(AskRequest req)
        {
            var context = req.Dept != null && Departments.Topics.Contains(req.Dept) ? req.Dept : null;
            var choice = req.Choice ?? "";

            if (choice == "start") return Start();
            if (choice == "book") return Book();
            if (choice == "other") return Other();
            if (choice.StartsWith("dept:")) return Summary(DeptFrom(choice));
            if (choice.StartsWith("get:")) return WhatYouGet(DeptFrom(choice));
            if (choice.StartsWith("handoff:")) return Handoff(DeptFrom(choice) ?? context ?? Departments.General);

            return FromText(req.Text ?? "", context);
        }

        private static AssistantReply FromText(string text, string? context)
        {
            var words = Tokens(text);
            if (words.Length == 0) return Start();

            var dept = Detect(words);
            var detailed = text.Length >= 140
                || words.Any(w => DetailWords.Contains(w))
                || DetailPhrases.Any(p => text.Contains(p, StringComparison.OrdinalIgnoreCase));

            if (words.Length <= 3 && words.Any(w => Greetings.Contains(w)) && dept == null)
                return Start();

            if (detailed)
                return Handoff(dept ?? context ?? Departments.General);

            if (words.Any(w => w == "book" || w == "booking" || w == "appointment" || w == "schedule"))
                return Book();

            if (words.Any(w => w == "review" || w == "reviews"))
                return new AssistantReply
                {
                    Message = "You can read what our clients say, and write your own review once you have an account.",
                    LinkKey = "reviews",
                    LinkLabel = "See reviews"
                };

            if (words.Any(w => w.StartsWith("regist") || w == "signup" || w == "login" || w == "account"))
                return new AssistantReply
                {
                    Message = "You can create an account or sign in from the icon at the top of the page. An account lets you book consultations and message our team.",
                    LinkKey = "register",
                    LinkLabel = "Create an account"
                };

            if (words.Any(w => w == "services" || w == "offer" || w == "do"))
                return new AssistantReply
                {
                    Message = "We work across four areas: Marketing, Strategic Market Development, Legal and Compliance, and IT. Which one are you interested in?",
                    Options = ServiceOptions()
                };

            if (dept != null)
                return Summary(dept);

            return Other();
        }

        private static AssistantReply Start() => new()
        {
            Message = "Hi, I’m the ECHT GROUP assistant. What do you need help with?",
            Options = ServiceOptions()
                .Concat(new[]
                {
                new AssistantOption { Label = "Book a consultation", Value = "book" },
                new AssistantOption { Label = "Something else", Value = "other" }
                })
                .ToList()
        };

        private static AssistantReply Summary(string? dept)
        {
            var service = ServiceData.All.FirstOrDefault(s => s.Title == dept);
            if (service == null) return Other();

            return new AssistantReply
            {
                Department = service.Title,
                Message = $"{service.Title}: {service.Summary} We help with {string.Join(", ", service.WhatWeDo)}.",
                Options = new List<AssistantOption>
            {
                new() { Label = "What is included?", Value = "get:" + service.Title },
                new() { Label = $"Talk to a {service.Title} consultant", Value = "handoff:" + service.Title },
                new() { Label = "Back", Value = "start" }
            }
            };
        }

        private static AssistantReply WhatYouGet(string? dept)
        {
            var service = ServiceData.All.FirstOrDefault(s => s.Title == dept);
            if (service == null) return Other();

            return new AssistantReply
            {
                Department = service.Title,
                Message = $"Our {service.Title} service includes: {string.Join(", ", service.WhatYouGet)}. Your consultant will confirm the details for your situation.",
                Options = new List<AssistantOption>
            {
                new() { Label = $"Talk to a {service.Title} consultant", Value = "handoff:" + service.Title },
                new() { Label = "Back", Value = "start" }
            }
            };
        }

        private static AssistantReply Book() => new()
        {
            Message = "You can book a consultation online: choose a service, then a date and a time slot. You’ll need to be signed in.",
            LinkKey = "book",
            LinkLabel = "Book a consultation",
            Options = new List<AssistantOption> { new() { Label = "Back", Value = "start" } }
        };

        private static AssistantReply Other() => new()
        {
            Message = "Tell me a little about what you need and I’ll point you to the right team, or I can refer you to a consultant straight away.",
            Options = new List<AssistantOption>
        {
            new() { Label = "Talk to a consultant", Value = "handoff:" + Departments.General },
            new() { Label = "Back", Value = "start" }
        }
        };

        private static AssistantReply Handoff(string dept)
        {
            var who = dept == Departments.General ? "team" : dept + " team";

            return new AssistantReply
            {
                Department = dept,
                Handoff = true,
                Message = $"{ReferralLead} {who}. A consultant there will answer you in detail. Please add your question below and I’ll pass it on."
            };
        }

        private static List<AssistantOption> ServiceOptions() =>
            ServiceData.All.Select(s => new AssistantOption { Label = s.Title, Value = "dept:" + s.Title }).ToList();

        private static string? DeptFrom(string choice)
        {
            var d = choice[(choice.IndexOf(':') + 1)..];
            return Departments.Topics.Contains(d) ? d : null;
        }

        private static string[] Tokens(string text) =>
            Regex.Matches(text.ToLowerInvariant(), "[a-z0-9]+").Select(m => m.Value).ToArray();

        private static string? Detect(string[] words)
        { //start of method 
            string? best = null;
            var bestScore = 0;

            foreach (var (dept, stems) in Stems)
            {
                var score = words.Count(w => stems.Any(s => s.Length <= 3 ? w == s : w.StartsWith(s)));
                if (score > bestScore)
                {
                    best = dept;
                    bestScore = score;
                }
            }

            return best;
        } //end of method
    } //end of class
} //end of namespace