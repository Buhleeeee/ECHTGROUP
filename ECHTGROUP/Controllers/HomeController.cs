using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECHTGROUP.Controllers
{ //start of namespace
    public class HomeController : Controller
    { //start of controller class
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        { //start of constructor
            _db = db;
        } //end of constructor

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Team()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetStarted()
        {
            return View(new GetStartedModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetStarted(GetStartedModel model)
        { //start of get started post
            if (!model.Consent)
                ModelState.AddModelError(nameof(model.Consent), "Please agree to the privacy policy");

            if (!ModelState.IsValid)
                return View(model);

            _db.Enquiries.Add(new Enquiry
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                ConsentGiven = model.Consent,
                Department = CleanTopic(model.Department)
            });
            await _db.SaveChangesAsync();

            TempData["Thanks"] = $"Thanks {model.FirstName}, we’ll be in touch soon.";
            return RedirectToAction(nameof(GetStarted));
        } //end of get started post

        public IActionResult FAQ() => View();

        public IActionResult Insights() => View(PostData.All);

        public IActionResult Article(string id)
        {
            var post = PostData.Find(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpGet]
        public IActionResult Contact() => View(new ContactViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        { //start of contact post
            if (!ModelState.IsValid)
                return View(model);

            _db.ContactMessages.Add(new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                Message = model.Message,
                Department = CleanTopic(model.Department)
            });
            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Thanks, we’ll reply as soon as we can.";
            return RedirectToAction(nameof(Contact));
        } //end of contact post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickMessage(ContactViewModel model)
        { //start of quick message
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "Please check your details";
                return BadRequest(new { error = message });
            }

            _db.ContactMessages.Add(new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                Message = model.Message,
                Department = CleanTopic(model.Department)
            });
            await _db.SaveChangesAsync();

            return Ok(new { ok = true });
        } //end of quick message

        public IActionResult Search(string? q)
        { //start of search
            var model = new SearchViewModel { Query = (q ?? "").Trim() };

            if (model.Query.Length >= 2)
            {
                var term = model.Query;

                foreach (var s in ServiceData.All)
                {
                    if (Matches(term, s.Title, s.Summary, string.Join(" ", s.WhatWeDo), string.Join(" ", s.WhatYouGet)))
                        model.Results.Add(new SearchResult { Title = s.Title, Summary = s.Summary, Type = "Service", Controller = "Services", Action = "Details", Id = s.Slug });
                }

                foreach (var post in PostData.All)
                {
                    if (Matches(term, post.Title, post.Summary, post.Category, string.Join(" ", post.Paragraphs)))
                        model.Results.Add(new SearchResult { Title = post.Title, Summary = post.Summary, Type = "Article", Controller = "Home", Action = "Article", Id = post.Slug });
                }

                var pages = new[]
                {
                    new SearchResult { Title = "About", Summary = "Our story and how we work.", Type = "Page", Action = "About" },
                    new SearchResult { Title = "Team", Summary = "Meet the consultants.", Type = "Page", Action = "Team" },
                    new SearchResult { Title = "FAQ", Summary = "Answers to common questions.", Type = "Page", Action = "FAQ" },
                    new SearchResult { Title = "Contact", Summary = "Send us a message.", Type = "Page", Action = "Contact" },
                    new SearchResult { Title = "Privacy policy", Summary = "How we handle your information.", Type = "Page", Action = "Privacy" }
                };
                model.Results.AddRange(pages.Where(pg => Matches(term, pg.Title, pg.Summary)));
            }

            return View(model);
        } //end of search

        private static bool Matches(string term, params string[] fields) =>
            fields.Any(f => f.Contains(term, StringComparison.OrdinalIgnoreCase));

        private static string CleanTopic(string? topic) =>
            topic != null && Departments.Topics.Contains(topic) ? topic : Departments.General;

    } //end of class
} //end of namespace