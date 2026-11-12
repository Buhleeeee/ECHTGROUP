using ECHTGROUP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECHTGROUP.Controllers
{ //start of namespace
    public class ServicesController : Controller
    { //start of class
        public IActionResult Index()
        { //start of method
            return View(ServiceData.All);
        } //end of method
        public IActionResult Details(string id)
        { //start of method
            var service = ServiceData.Find(id);
            if (service == null) return NotFound();
            return View(service);
        } //end of method
        } //end of class
} //end of namespace
