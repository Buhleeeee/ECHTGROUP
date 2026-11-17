using ECHTGROUP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.ViewComponents;

public class LatestReviewsViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _db;

    public LatestReviewsViewComponent(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var reviews = await _db.Reviews
            .Where(r => !r.IsHidden)
            .OrderByDescending(r => r.CreatedUtc)
            .Take(5)
            .ToListAsync();

        return View(reviews);
    }
}