using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers
{ //start of namespace

    [Authorize]
    public class DashboardController : Controller
    { //start of class
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxPhotoBytes = 2 * 1024 * 1024;

        public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        } //end of class

        public async Task<IActionResult> Index()
        { //start of method
            if (IsStaff()) return RedirectToAction("Index", "Consultant");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var upcoming = _db.Bookings.Where(b =>
                b.UserId == user.Id && b.Status != "Cancelled" && b.Date >= DateTime.Today);

            var model = new DashboardViewModel
            {
                Name = NameOf(user),
                PhotoPath = user.PhotoPath,
                Email = user.Email,
                Phone = user.PhoneNumber,
                UpcomingCount = await upcoming.CountAsync(),
                Next = await upcoming.OrderBy(b => b.Date).ThenBy(b => b.TimeSlot).Take(3).ToListAsync()
            };

            return View(model);
        } //end of method

        [HttpGet]
        public async Task<IActionResult> Profile()
        { //start of profile method
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var first = user.FirstName;
            var last = user.Surname;
            if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(last) && !string.IsNullOrWhiteSpace(user.FullName))
            {
                var parts = user.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                first = parts[0];
                last = parts.Length > 1 ? parts[1] : "";
            } //end of method

            var model = new ProfileViewModel
            {
                Title = user.Title,
                FirstName = first,
                Surname = last,
                Gender = user.Gender,
                Phone = user.PhoneNumber,
                AlternativeContact = user.AlternativeContact,
                AlternativeEmail = user.AlternativeEmail,
                AddressLine = user.AddressLine,
                City = user.City,
                PostalCode = user.PostalCode
            };
            await FillDisplay(model, user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!string.IsNullOrEmpty(model.Title) && !ProfileOptions.Titles.Contains(model.Title))
                ModelState.AddModelError(nameof(model.Title), "Please choose a title from the list");

            if (!string.IsNullOrEmpty(model.Gender) && !ProfileOptions.Genders.Contains(model.Gender))
                ModelState.AddModelError(nameof(model.Gender), "Please choose an option from the list");

            if (!string.IsNullOrWhiteSpace(model.AlternativeEmail) &&
                string.Equals(model.AlternativeEmail.Trim(), user.Email, StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(model.AlternativeEmail), "Your alternative email must be different from your main email");

            if (!string.IsNullOrWhiteSpace(model.AlternativeContact) &&
                string.Equals(model.AlternativeContact.Trim(), model.Phone?.Trim(), StringComparison.Ordinal))
                ModelState.AddModelError(nameof(model.AlternativeContact), "Your alternative contact must be different from your main number");

            if (!ModelState.IsValid)
            {
                await FillDisplay(model, user);
                return View(model);
            }

            if (model.Photo != null && model.Photo.Length > 0)
            {
                var saved = await SavePhoto(model.Photo, user.Id);
                if (saved == null)
                {
                    ModelState.AddModelError(nameof(model.Photo), "Please choose a JPG, PNG or WebP image under 2 MB");
                    await FillDisplay(model, user);
                    return View(model);
                }

                DeletePhotoFile(user.PhotoPath);
                user.PhotoPath = saved;
            }

            user.Title = Clean(model.Title);
            user.FirstName = model.FirstName.Trim();
            user.Surname = model.Surname.Trim();
            user.FullName = $"{user.FirstName} {user.Surname}";
            user.Gender = Clean(model.Gender);
            user.PhoneNumber = Clean(model.Phone);
            user.AlternativeContact = Clean(model.AlternativeContact);
            user.AlternativeEmail = Clean(model.AlternativeEmail);
            user.AddressLine = Clean(model.AddressLine);
            user.City = Clean(model.City);
            user.PostalCode = Clean(model.PostalCode);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                await FillDisplay(model, user);
                return View(model);
            }

            TempData["Thanks"] = "Your profile was updated.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoto()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            DeletePhotoFile(user.PhotoPath);
            user.PhotoPath = null;
            await _userManager.UpdateAsync(user);

            TempData["Thanks"] = "Your photo was removed.";
            return RedirectToAction(nameof(Profile));
        }

        private bool IsStaff() => User.IsInRole("Consultant") || User.IsInRole("Admin");

        private async Task FillDisplay(ProfileViewModel model, ApplicationUser user)
        {
            model.Email = user.Email;
            model.PhotoPath = user.PhotoPath;
            model.DisplayName = NameOf(user);

            var roles = await _userManager.GetRolesAsync(user);
            model.RoleName = roles.Contains("Admin") ? "Admin"
                           : roles.Contains("Consultant") ? "Consultant"
                           : "Client";
            if (!string.IsNullOrEmpty(user.Department))
                model.RoleName += " · " + user.Department;
        }

        private static string NameOf(ApplicationUser user)
        {
            var full = $"{user.FirstName} {user.Surname}".Trim();
            if (full.Length > 0) return full;
            return !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : (user.Email ?? "");
        }

        private static string? Clean(string? text) =>
            string.IsNullOrWhiteSpace(text) ? null : text.Trim();

        private async Task<string?> SavePhoto(IFormFile file, string userId)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext) || file.Length < 12 || file.Length > MaxPhotoBytes)
                return null;

            var header = new byte[12];
            using (var stream = file.OpenReadStream())
            {
                await stream.ReadExactlyAsync(header, 0, 12);
            }

            var isJpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            var isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
            var isWebp = header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                      && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
            if (!isJpeg && !isPng && !isWebp) return null;

            var folder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
            Directory.CreateDirectory(folder);

            var name = $"{userId}-{Guid.NewGuid():N}{ext}";
            using (var target = new FileStream(Path.Combine(folder, name), FileMode.Create))
            {
                await file.CopyToAsync(target);
            }

            return "/uploads/profiles/" + name;
        } //end of method

        private void DeletePhotoFile(string? webPath)
        { //start of method
            if (string.IsNullOrEmpty(webPath) || !webPath.StartsWith("/uploads/profiles/")) return;

            var file = Path.Combine(_env.WebRootPath, "uploads", "profiles", Path.GetFileName(webPath));
            if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
        } //end of method
    } //end of class
} //end of namespace