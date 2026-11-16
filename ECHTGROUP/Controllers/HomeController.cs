using ECHTGROUP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECHTGROUP.Controllers
{ //start of namespace
    public class HomeController : Controller
    { //start of controller class
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
        public IActionResult GetStarted(GetStartedModel model)
        {
            if (!model.Consent)
                ModelState.AddModelError(nameof(model.Consent), "Please agree to the privacy policy");

            if (!ModelState.IsValid)
                return View(model);

            TempData["Thanks"] = $"Thanks {model.FirstName}, we’ll be in touch soon.";
            return RedirectToAction(nameof(GetStarted));
        }
        public IActionResult FAQ() => View();

        public IActionResult Insights() => View(PostData.All);

        public IActionResult Article(string id)
        {
            var post = PostData.Find(id);
            if (post == null) return NotFound();
            return View(post);
        }


    } //end of class
} //end of namespace
