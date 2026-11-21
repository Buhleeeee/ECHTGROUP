using System.ComponentModel.DataAnnotations;
using ECHTGROUP.Data;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECHTGROUP.Controllers
{ //start of namespace

    [EnableRateLimiting("assistant")]
    public class AssistantController : Controller
    { //start of class
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssistantController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ask([FromBody] AskRequest request)
        {
            var reply = AssistantBrain.Reply(request);

            reply.Url = reply.LinkKey switch
            {
                "book" => Url.Action("Create", "Booking"),
                "reviews" => Url.Action("Index", "Reviews"),
                "register" => Url.Action("Register", "Account"),
                _ => null
            };

            return Json(reply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Handoff([FromBody] HandoffRequest request)
        { //start of handoff
            if (User.IsInRole("Consultant") || User.IsInRole("Admin"))
                return BadRequest(new { error = "Staff accounts can’t send messages here" });

            var dept = request.Department != null && Departments.Topics.Contains(request.Department)
                ? request.Department
                : Departments.General;

            var text = (request.Text ?? "").Trim();
            if (text.Length < 5 || text.Length > 2000)
                return BadRequest(new { error = "Please write your question (up to 2000 characters)" });

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;

                _db.ChatMessages.Add(new ChatMessage
                {
                    ClientId = userId,
                    SenderId = userId,
                    FromConsultant = false,
                    Body = text,
                    Department = dept
                });
                await _db.SaveChangesAsync();

                return Ok(new { ok = true, url = Url.Action("Index", "Messages", new { topic = dept }) });
            }

            var name = (request.Name ?? "").Trim();
            var email = (request.Email ?? "").Trim();

            if (name.Length == 0 || name.Length > 100)
                return BadRequest(new { error = "Please enter your name" });

            if (email.Length > 200 || !new EmailAddressAttribute().IsValid(email))
                return BadRequest(new { error = "Please enter a valid email" });

            _db.ContactMessages.Add(new ContactMessage
            {
                Name = name,
                Email = email,
                Message = text,
                Department = dept
            });
            await _db.SaveChangesAsync();

            return Ok(new { ok = true });
        } //end of handoff
    } //end of class
} //end of namespace
