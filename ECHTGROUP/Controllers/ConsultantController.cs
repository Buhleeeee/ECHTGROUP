using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers;

[Authorize(Roles = "Consultant,Admin")]
public class ConsultantController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ConsultantController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _db.ChatMessages
            .GroupBy(m => m.ClientId)
            .Select(g => new
            {
                ClientId = g.Key,
                Unread = g.Count(m => !m.FromConsultant && !m.IsRead)
            })
            .ToListAsync();

        var ids = groups.Select(g => g.ClientId).ToList();
        var users = await _userManager.Users
            .Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        var items = new List<InboxItemViewModel>();
        foreach (var g in groups)
        {
            var last = await _db.ChatMessages
                .Where(m => m.ClientId == g.ClientId)
                .OrderByDescending(m => m.CreatedUtc)
                .FirstAsync();

            users.TryGetValue(g.ClientId, out var user);

            items.Add(new InboxItemViewModel
            {
                ClientId = g.ClientId,
                ClientName = NameOf(user),
                LastMessage = last.Body,
                LastAt = last.CreatedUtc,
                Unread = g.Unread
            });
        }

        return View(items.OrderByDescending(i => i.LastAt).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Thread(string id)
    {
        var client = await _userManager.FindByIdAsync(id);
        if (client == null) return NotFound();

        return View(await BuildThread(client, new ConsultantThreadViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Thread(string id, ConsultantThreadViewModel model)
    {
        var client = await _userManager.FindByIdAsync(id);
        if (client == null) return NotFound();

        if (!ModelState.IsValid)
            return View(await BuildThread(client, model));

        _db.ChatMessages.Add(new ChatMessage
        {
            ClientId = client.Id,
            SenderId = _userManager.GetUserId(User)!,
            FromConsultant = true,
            Body = model.Body.Trim()
        });
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Thread), new { id });
    }

    private async Task<ConsultantThreadViewModel> BuildThread(ApplicationUser client, ConsultantThreadViewModel model)
    {
        model.ClientId = client.Id;
        model.ClientName = NameOf(client);
        model.Messages = await _db.ChatMessages
            .Where(m => m.ClientId == client.Id)
            .OrderBy(m => m.CreatedUtc)
            .ToListAsync();

        var unread = model.Messages.Where(m => !m.FromConsultant && !m.IsRead).ToList();
        if (unread.Count > 0)
        {
            unread.ForEach(m => m.IsRead = true);
            await _db.SaveChangesAsync();
        }

        return model;
    }

    private static string NameOf(ApplicationUser? user)
    {
        if (user == null) return "Unknown client";
        return !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : (user.Email ?? "Client");
    }
}