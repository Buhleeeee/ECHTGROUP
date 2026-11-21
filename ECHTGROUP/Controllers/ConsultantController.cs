using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers
{ //start of namespace

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

        public async Task<IActionResult> Index(string? type, string? status, string? dept)
        { //start of inbox
            var me = await _userManager.GetUserAsync(User);
            var allowed = AllowedDepartments(me);

            string? deptFilter = null;
            if (!string.IsNullOrEmpty(dept) && Departments.Topics.Contains(dept)
                && (allowed == null || allowed.Contains(dept)))
                deptFilter = dept;

            var items = new List<InboxItem>();

            if (string.IsNullOrEmpty(type) || type == "chat")
                items.AddRange(await LoadChats(allowed, deptFilter));

            if (string.IsNullOrEmpty(type) || type == "contact")
            {
                var query = _db.ContactMessages.AsQueryable();
                if (allowed != null) query = query.Where(m => allowed.Contains(m.Department));
                if (deptFilter != null) query = query.Where(m => m.Department == deptFilter);

                var contacts = await query.OrderByDescending(m => m.CreatedUtc).Take(200).ToListAsync();
                items.AddRange(contacts.Select(m => new InboxItem
                {
                    Type = "Contact",
                    Name = m.Name,
                    Preview = m.Message,
                    Department = m.Department,
                    LastAt = m.CreatedUtc,
                    NeedsAttention = !m.IsHandled,
                    Id = m.Id
                }));
            }

            if (string.IsNullOrEmpty(type) || type == "enquiry")
            {
                var query = _db.Enquiries.AsQueryable();
                if (allowed != null) query = query.Where(e => allowed.Contains(e.Department));
                if (deptFilter != null) query = query.Where(e => e.Department == deptFilter);

                var enquiries = await query.OrderByDescending(e => e.CreatedUtc).Take(200).ToListAsync();
                items.AddRange(enquiries.Select(e => new InboxItem
                {
                    Type = "Enquiry",
                    Name = $"{e.FirstName} {e.LastName}",
                    Preview = $"{e.Email} · {e.Phone}",
                    Department = e.Department,
                    LastAt = e.CreatedUtc,
                    NeedsAttention = !e.IsHandled,
                    Id = e.Id
                }));
            }

            var openCount = items.Count(i => i.NeedsAttention);

            if (status == "open") items = items.Where(i => i.NeedsAttention).ToList();
            else if (status == "done") items = items.Where(i => !i.NeedsAttention).ToList();

            return View(new InboxViewModel
            {
                Items = items.OrderByDescending(i => i.NeedsAttention).ThenByDescending(i => i.LastAt).ToList(),
                Type = type,
                Status = status,
                Dept = deptFilter,
                SeesAll = allowed == null,
                DeptChoices = allowed ?? Departments.Topics,
                OpenCount = openCount
            });
        } //end of inbox

        [HttpGet]
        public async Task<IActionResult> Thread(string id, string? dept)
        {
            var topic = CleanTopic(dept);
            if (!await CanSeeAsync(topic)) return Forbid();

            var client = await _userManager.FindByIdAsync(id);
            if (client == null) return NotFound();

            return View(await BuildThread(client, topic, new ConsultantThreadViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Thread(string id, string? dept, ConsultantThreadViewModel model)
        {
            var topic = CleanTopic(dept);
            if (!await CanSeeAsync(topic)) return Forbid();

            var client = await _userManager.FindByIdAsync(id);
            if (client == null) return NotFound();

            if (!ModelState.IsValid)
                return View(await BuildThread(client, topic, model));

            _db.ChatMessages.Add(new ChatMessage
            {
                ClientId = client.Id,
                SenderId = _userManager.GetUserId(User)!,
                FromConsultant = true,
                Body = model.Body.Trim(),
                Department = topic
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id, dept = topic });
        }

        public async Task<IActionResult> Detail(string type, int id)
        { //start of detail
            InboxDetailViewModel? model = null;

            if (type == "contact")
            {
                var m = await _db.ContactMessages.FindAsync(id);
                if (m != null)
                    model = new InboxDetailViewModel
                    {
                        Type = "contact",
                        Id = m.Id,
                        Name = m.Name,
                        Email = m.Email,
                        Body = m.Message,
                        Department = m.Department,
                        CreatedUtc = m.CreatedUtc,
                        IsHandled = m.IsHandled
                    };
            }
            else if (type == "enquiry")
            {
                var e = await _db.Enquiries.FindAsync(id);
                if (e != null)
                    model = new InboxDetailViewModel
                    {
                        Type = "enquiry",
                        Id = e.Id,
                        Name = $"{e.FirstName} {e.LastName}",
                        Email = e.Email,
                        Phone = e.Phone,
                        Department = e.Department,
                        CreatedUtc = e.CreatedUtc,
                        IsHandled = e.IsHandled
                    };
            }

            if (model == null) return NotFound();
            if (!await CanSeeAsync(model.Department)) return Forbid();

            return View(model);
        } //end of detail

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHandled(string type, int id)
        { //start of toggle
            if (type == "contact")
            {
                var m = await _db.ContactMessages.FindAsync(id);
                if (m == null) return NotFound();
                if (!await CanSeeAsync(m.Department)) return Forbid();
                m.IsHandled = !m.IsHandled;
            }
            else if (type == "enquiry")
            {
                var e = await _db.Enquiries.FindAsync(id);
                if (e == null) return NotFound();
                if (!await CanSeeAsync(e.Department)) return Forbid();
                e.IsHandled = !e.IsHandled;
            }
            else
            {
                return NotFound();
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { type, id });
        } //end of toggle

        private async Task<List<InboxItem>> LoadChats(string[]? allowed, string? deptFilter)
        { //start of load chats
            var query = _db.ChatMessages.AsQueryable();
            if (allowed != null) query = query.Where(m => allowed.Contains(m.Department));
            if (deptFilter != null) query = query.Where(m => m.Department == deptFilter);

            var groups = await query
                .GroupBy(m => new { m.ClientId, m.Department })
                .Select(g => new
                {
                    g.Key.ClientId,
                    g.Key.Department,
                    LastAt = g.Max(m => m.CreatedUtc),
                    Unread = g.Count(m => !m.FromConsultant && !m.IsRead)
                })
                .OrderByDescending(g => g.LastAt)
                .Take(200)
                .ToListAsync();

            var ids = groups.Select(g => g.ClientId).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var items = new List<InboxItem>();
            foreach (var g in groups)
            {
                var last = await _db.ChatMessages
                    .Where(m => m.ClientId == g.ClientId && m.Department == g.Department)
                    .OrderByDescending(m => m.CreatedUtc)
                    .FirstAsync();

                users.TryGetValue(g.ClientId, out var user);

                items.Add(new InboxItem
                {
                    Type = "Chat",
                    Name = NameOf(user),
                    Preview = last.Body,
                    Department = g.Department,
                    LastAt = g.LastAt,
                    NeedsAttention = !last.FromConsultant,
                    Unread = g.Unread,
                    ClientId = g.ClientId
                });
            }

            return items;
        } //end of load chats

        private async Task<ConsultantThreadViewModel> BuildThread(ApplicationUser client, string topic, ConsultantThreadViewModel model)
        {
            model.ClientId = client.Id;
            model.ClientName = NameOf(client);
            model.Department = topic;
            model.Messages = await _db.ChatMessages
                .Where(m => m.ClientId == client.Id && m.Department == topic)
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

        private string[]? AllowedDepartments(ApplicationUser? me)
        {
            if (me == null || User.IsInRole("Admin") || string.IsNullOrEmpty(me.Department) || me.Department == Departments.Management)
                return null;

            return new[] { me.Department, Departments.General };
        }

        private async Task<bool> CanSeeAsync(string department)
        {
            var me = await _userManager.GetUserAsync(User);
            var allowed = AllowedDepartments(me);
            return allowed == null || allowed.Contains(department);
        }

        private static string CleanTopic(string? topic) =>
            topic != null && Departments.Topics.Contains(topic) ? topic : Departments.General;

        private static string NameOf(ApplicationUser? user)
        {
            if (user == null) return "Unknown client";

            var full = $"{user.FirstName} {user.Surname}".Trim();
            if (full.Length > 0) return full;
            return !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : (user.Email ?? "Client");
        }
    } //end of class
} //end of namespace