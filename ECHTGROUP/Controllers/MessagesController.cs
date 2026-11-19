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

        public async Task<IActionResult> Index()
        {
            if (IsStaff()) return RedirectToAction("Index", "Consultant");

            var userId = _userManager.GetUserId(User)!;
            var messages = await LoadThread(userId);

            var unread = messages.Where(m => m.FromConsultant && !m.IsRead).ToList();
            if (unread.Count > 0)
            {
                unread.ForEach(m => m.IsRead = true);
                await _db.SaveChangesAsync();
            }

            return View(new ThreadViewModel { Messages = messages });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ThreadViewModel model)
        {
            if (IsStaff()) return RedirectToAction("Index", "Consultant");

            var userId = _userManager.GetUserId(User)!;

            if (!ModelState.IsValid)
            {
                model.Messages = await LoadThread(userId);
                return View(model);
            }

            _db.ChatMessages.Add(new ChatMessage
            {
                ClientId = userId,
                SenderId = userId,
                FromConsultant = false,
                Body = model.Body.Trim()
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool IsStaff() => User.IsInRole("Consultant") || User.IsInRole("Admin");

        private Task<List<ChatMessage>> LoadThread(string clientId) =>
            _db.ChatMessages
                .Where(m => m.ClientId == clientId)
                .OrderBy(m => m.CreatedUtc)
                .ToListAsync();
    } //end of class
} //end of namespace