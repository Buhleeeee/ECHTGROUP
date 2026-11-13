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
        public IActionResult About()             // we added in 4e-3a
        {
            return View();
        }

        public IActionResult Team()              // we added in 4e-3a
        {
            return View();
        }

        [HttpGet]                                // <- the two actions you pasted
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
        
    } //end of class
} //end of namespace
