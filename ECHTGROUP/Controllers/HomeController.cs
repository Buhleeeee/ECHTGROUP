using ECHTGROUP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ECHTGROUP.Data;

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
        { //start of method
            return View();
        } //end of method

        public IActionResult Privacy()
        { //startof privacy method
            return View();
        } //end of privacy method

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        { //start of error method
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } //end of error method

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
                ConsentGiven = model.Consent
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
                Message = model.Message
            });
            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Thanks, we’ll reply as soon as we can.";
            return RedirectToAction(nameof(Contact));
        } //end of contact post

    } //end of class
} //end of namespace