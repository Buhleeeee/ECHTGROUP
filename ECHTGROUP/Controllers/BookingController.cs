using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers
{ //start of namespace

    [Authorize]
    public class BookingController : Controller
    { //start of class
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create(string? service = null)
        {
            var model = new BookingViewModel
            {
                Service = ServiceData.Find(service ?? "")?.Title ?? ""
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var date = model.Date!.Value.Date;

                if (!ServiceData.All.Any(s => s.Title == model.Service))
                    ModelState.AddModelError(nameof(model.Service), "Please choose a service");
                else if (date <= DateTime.Today)
                    ModelState.AddModelError(nameof(model.Date), "Please choose a date after today");
                else if (!BookingOptions.Slots.Contains(model.TimeSlot))
                    ModelState.AddModelError(nameof(model.TimeSlot), "Please choose a time");
                else if (await _db.Bookings.AnyAsync(b => b.Date == date && b.TimeSlot == model.TimeSlot && b.Status != "Cancelled"))
                    ModelState.AddModelError(nameof(model.TimeSlot), "That time is already booked, please choose another");
            }

            if (!ModelState.IsValid)
                return View(model);

            _db.Bookings.Add(new Booking
            {
                UserId = _userManager.GetUserId(User)!,
                Service = model.Service,
                Date = model.Date!.Value.Date,
                TimeSlot = model.TimeSlot,
                Notes = model.Notes ?? ""
            });
            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Your booking request has been received.";
            return RedirectToAction(nameof(Index));
        } //end of method

        public async Task<IActionResult> Index()
        { //start of method
            var userId = _userManager.GetUserId(User);
            var bookings = await _db.Bookings
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Date).ThenBy(b => b.TimeSlot)
                .ToListAsync();
            return View(bookings);
        } //end of method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (booking == null) return NotFound();

            booking.Status = "Cancelled";
            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Your booking was cancelled.";
            return RedirectToAction(nameof(Index));
        } //end of method
    } //end of class
} //end of namespace
