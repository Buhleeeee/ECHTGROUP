using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECHTGROUP.Controllers
{ //start of namespace

    [Authorize(Roles = "Admin")]
    public class ManageTeamController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ManageTeamController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _db.TeamMembers
                .OrderBy(m => m.Group).ThenBy(m => m.SortOrder).ThenBy(m => m.Name)
                .ToListAsync();
            return View(members);
        }

        [HttpGet]
        public IActionResult Create() => View("Edit", new TeamMemberForm());

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var m = await _db.TeamMembers.FindAsync(id);
            if (m == null) return NotFound();

            return View(new TeamMemberForm
            {
                Id = m.Id,
                Name = m.Name,
                JobTitle = m.JobTitle,
                Group = m.Group,
                Department = m.Department,
                Bio = m.Bio,
                SortOrder = m.SortOrder,
                IsVisible = m.IsVisible,
                PhotoPath = m.PhotoPath
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeamMemberForm model)
        { //start of save
            if (!TeamGroups.All.Contains(model.Group))
                ModelState.AddModelError(nameof(model.Group), "Please choose a group");

            if (!string.IsNullOrEmpty(model.Department) && !Departments.Staff.Contains(model.Department))
                ModelState.AddModelError(nameof(model.Department), "Please choose a department from the list");

            TeamMember? member = null;
            if (model.Id != 0)
            {
                member = await _db.TeamMembers.FindAsync(model.Id);
                if (member == null) return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.PhotoPath = member?.PhotoPath;
                return View(model);
            }

            string? newPhoto = null;
            if (model.Photo != null && model.Photo.Length > 0)
            {
                newPhoto = await PhotoStore.SaveAsync(model.Photo, _env.WebRootPath, "team", "team");
                if (newPhoto == null)
                {
                    ModelState.AddModelError(nameof(model.Photo), "Please choose a JPG, PNG or WebP image under 2 MB");
                    model.PhotoPath = member?.PhotoPath;
                    return View(model);
                }
            }

            if (member == null)
            {
                member = new TeamMember();
                _db.TeamMembers.Add(member);
            }

            member.Name = model.Name.Trim();
            member.JobTitle = model.JobTitle.Trim();
            member.Group = model.Group;
            member.Department = string.IsNullOrEmpty(model.Department) ? null : model.Department;
            member.Bio = string.IsNullOrWhiteSpace(model.Bio) ? null : model.Bio.Trim();
            member.SortOrder = model.SortOrder;
            member.IsVisible = model.IsVisible;

            if (newPhoto != null)
            {
                PhotoStore.Delete(member.PhotoPath, _env.WebRootPath, "team");
                member.PhotoPath = newPhoto;
            }
            else if (model.RemovePhoto)
            {
                PhotoStore.Delete(member.PhotoPath, _env.WebRootPath, "team");
                member.PhotoPath = null;
            }

            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Team member saved.";
            return RedirectToAction(nameof(Index));
        } //end of save

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVisible(int id)
        {
            var m = await _db.TeamMembers.FindAsync(id);
            if (m == null) return NotFound();

            m.IsVisible = !m.IsVisible;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _db.TeamMembers.FindAsync(id);
            if (m == null) return NotFound();

            PhotoStore.Delete(m.PhotoPath, _env.WebRootPath, "team");
            _db.TeamMembers.Remove(m);
            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Team member removed.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSamples()
        { //start of samples
            if (await _db.TeamMembers.AnyAsync())
                return RedirectToAction(nameof(Index));

            const string name = "Name to be added";

            _db.TeamMembers.AddRange(
                new TeamMember { Name = name, JobTitle = "Chief Executive Officer", Group = TeamGroups.Leadership, Department = Departments.Management, SortOrder = 1 },
                new TeamMember { Name = name, JobTitle = "Chief Operating Officer", Group = TeamGroups.Leadership, Department = Departments.Management, SortOrder = 2 },
                new TeamMember { Name = name, JobTitle = "Marketing Consultant", Group = TeamGroups.Consultants, Department = Departments.Marketing, SortOrder = 1 },
                new TeamMember { Name = name, JobTitle = "Strategic Market Development Consultant", Group = TeamGroups.Consultants, Department = Departments.MarketDevelopment, SortOrder = 2 },
                new TeamMember { Name = name, JobTitle = "Legal and Compliance Consultant", Group = TeamGroups.Consultants, Department = Departments.Legal, SortOrder = 3 },
                new TeamMember { Name = name, JobTitle = "IT Consultant", Group = TeamGroups.Consultants, Department = Departments.IT, SortOrder = 4 },
                new TeamMember { Name = name, JobTitle = "Agent", Group = TeamGroups.Agents, SortOrder = 1 });

            await _db.SaveChangesAsync();

            TempData["Thanks"] = "Sample profiles added. Edit each one with a real name, photo and bio.";
            return RedirectToAction(nameof(Index));
        } //end of samples
    } //end of class
} //end of namespace