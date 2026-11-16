using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers
{ //startof namespace

    [Authorize]
    public class DashboardController : Controller
    {  //start of controller class
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var upcoming = _db.Bookings.Where(b =>
                b.UserId == user.Id && b.Status != "Cancelled" && b.Date >= DateTime.Today);

            var model = new DashboardViewModel
            {
                Name = string.IsNullOrWhiteSpace(user.FullName) ? (user.Email ?? "") : user.FullName,
                UpcomingCount = await upcoming.CountAsync(),
                Next = await upcoming.OrderBy(b => b.Date).ThenBy(b => b.TimeSlot).Take(3).ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            return View(new ProfileViewModel { FullName = user.FullName, Email = user.Email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                return View(model);
            }

            user.FullName = model.FullName.Trim();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                model.Email = user.Email;
                return View(model);
            }

            TempData["Thanks"] = "Your profile was updated.";
            return RedirectToAction(nameof(Profile));
        }
    } //end of class
} //end of naemespace