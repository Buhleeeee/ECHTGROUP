using System.Security.Cryptography;
using System.Text;
using ECHTGROUP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECHTGROUP.Controllers
{ //start of namespace

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpGet]
        public IActionResult Register(string? type = null) =>
            View(new RegisterViewModel { AccountType = CleanType(type) });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.AccountType = CleanType(model.AccountType);
            var isStaff = model.AccountType == "Employee";

            if (!model.Consent)
                ModelState.AddModelError(nameof(model.Consent), "Please agree to the privacy policy");

            if (isStaff)
            {
                if (string.IsNullOrEmpty(model.Department) || !Departments.Staff.Contains(model.Department))
                    ModelState.AddModelError(nameof(model.Department), "Please choose your department");
                else if (!StaffCodeIsValid(model.Department, model.StaffCode))
                    ModelState.AddModelError(nameof(model.StaffCode), "That staff code isn’t valid for this department");
            }

            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                Department = isStaff ? model.Department : null,
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                ConsentGivenUtc = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (isStaff)
                {
                    await _userManager.AddToRoleAsync(user, "Consultant");
                    if (model.Department == Departments.Management)
                        await _userManager.AddToRoleAsync(user, "Admin");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return isStaff
                    ? RedirectToAction("Index", "Consultant")
                    : RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null, string? type = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { AccountType = CleanType(type) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            model.AccountType = CleanType(model.AccountType);
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

                if (check.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too many attempts. Please try again in a few minutes");
                    return View(model);
                }

                if (check.Succeeded)
                {
                    var isStaff = await _userManager.IsInRoleAsync(user, "Consultant")
                               || await _userManager.IsInRoleAsync(user, "Admin");

                    if (model.AccountType == "Employee" && !isStaff)
                    {
                        ModelState.AddModelError(string.Empty, "This account isn’t registered as ECHT GROUP staff. Please choose Client");
                        return View(model);
                    }

                    if (model.AccountType == "Client" && isStaff)
                    {
                        ModelState.AddModelError(string.Empty, "This is an ECHT GROUP staff account. Please choose ECHT GROUP employee");
                        return View(model);
                    }

                    await _signInManager.SignInAsync(user, model.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return isStaff
                        ? RedirectToAction("Index", "Consultant")
                        : RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Incorrect email or password");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchAccount(string? type)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), new { type = CleanType(type) });
        }

        public IActionResult AccessDenied() => View();

        private static string CleanType(string? type) =>
            string.Equals(type, "Employee", StringComparison.OrdinalIgnoreCase) ? "Employee" : "Client";

        private bool StaffCodeIsValid(string? department, string? code)
        { //start of bool method
            if (department == null || string.IsNullOrWhiteSpace(code)) return false;

            var key = Departments.CodeKey(department);
            var expected = key == null ? null : _config[key];
            if (string.IsNullOrWhiteSpace(expected)) return false;

            var a = SHA256.HashData(Encoding.UTF8.GetBytes(code.Trim()));
            var b = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
            return CryptographicOperations.FixedTimeEquals(a, b);
        } //end of method
    } //end of class
} //end of namespace