using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers
{ //start of namespace

    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? topic)
        {
            if (IsStaff()) return RedirectToAction("Index", "Consultant");

            var userId = _userManager.GetUserId(User)!;
            return View(await BuildModel(userId, CleanTopic(topic), new ThreadViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ThreadViewModel model)
        {
            if (IsStaff()) return RedirectToAction("Index", "Consultant");

            var userId = _userManager.GetUserId(User)!;
            var topic = CleanTopic(model.Topic);

            if (!ModelState.IsValid)
                return View(await BuildModel(userId, topic, model));

            _db.ChatMessages.Add(new ChatMessage
            {
                ClientId = userId,
                SenderId = userId,
                FromConsultant = false,
                Body = model.Body.Trim(),
                Department = topic
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { topic });
        }

        private async Task<ThreadViewModel> BuildModel(string userId, string topic, ThreadViewModel model)
        {
            model.Topic = topic;
            model.Messages = await _db.ChatMessages
                .Where(m => m.ClientId == userId && m.Department == topic)
                .OrderBy(m => m.CreatedUtc)
                .ToListAsync();

            var unread = model.Messages.Where(m => m.FromConsultant && !m.IsRead).ToList();
            if (unread.Count > 0)
            {
                unread.ForEach(m => m.IsRead = true);
                await _db.SaveChangesAsync();
            }

            model.UnreadByTopic = await _db.ChatMessages
                .Where(m => m.ClientId == userId && m.FromConsultant && !m.IsRead)
                .GroupBy(m => m.Department)
                .Select(g => new { Topic = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Topic, x => x.Count);

            return model;
        }

        private bool IsStaff() => User.IsInRole("Consultant") || User.IsInRole("Admin");

        private static string CleanTopic(string? topic) =>
            topic != null && Departments.Topics.Contains(topic) ? topic : Departments.General;
    } //end of class
} //end of namespace