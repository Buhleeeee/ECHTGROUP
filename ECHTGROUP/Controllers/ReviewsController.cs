using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers;

public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var canModerate = IsStaff();

        var query = _db.Reviews.AsQueryable();
        if (!canModerate)
            query = query.Where(r => !r.IsHidden);

        var reviews = await query.OrderByDescending(r => r.CreatedUtc).ToListAsync();
        var visible = reviews.Where(r => !r.IsHidden).ToList();

        return View(new ReviewsPageViewModel
        {
            Reviews = reviews,
            Count = visible.Count,
            Average = visible.Count == 0 ? 0 : Math.Round(visible.Average(r => r.Rating), 1),
            CanModerate = canModerate
        });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Write()
    {
        if (IsStaff()) return RedirectToAction(nameof(Index));

        var userId = _userManager.GetUserId(User);
        var existing = await _db.Reviews.FirstOrDefaultAsync(r => r.UserId == userId);

        ViewData["Editing"] = existing != null;
        return View(new ReviewFormViewModel
        {
            Rating = existing?.Rating,
            Comment = existing?.Comment ?? ""
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Write(ReviewFormViewModel model)
    {
        if (IsStaff()) return RedirectToAction(nameof(Index));

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var existing = await _db.Reviews.FirstOrDefaultAsync(r => r.UserId == user.Id);

        if (!ModelState.IsValid)
        {
            ViewData["Editing"] = existing != null;
            return View(model);
        }

        if (existing == null)
        {
            _db.Reviews.Add(new Review
            {
                UserId = user.Id,
                AuthorName = DisplayName(user),
                Rating = model.Rating!.Value,
                Comment = model.Comment.Trim()
            });
        }
        else
        {
            existing.AuthorName = DisplayName(user);
            existing.Rating = model.Rating!.Value;
            existing.Comment = model.Comment.Trim();
            existing.UpdatedUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        TempData["Thanks"] = "Thank you, your review is now on the site.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Consultant,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleHidden(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        review.IsHidden = !review.IsHidden;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool IsStaff() => User.IsInRole("Consultant") || User.IsInRole("Admin");

    private static string DisplayName(ApplicationUser user)
    {
        var source = string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.Surname)
            ? user.FullName
            : $"{user.FirstName} {user.Surname}";

        var parts = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "Client";
        return parts.Length == 1 ? parts[0] : $"{parts[0]} {parts[^1][0]}.";
    }
}
