using ECHTGROUP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECHTGROUP.ViewComponents
{ //start of namespace

    public class HeaderAvatarModel
    { //start of class
        public bool SignedIn { get; set; }
        public string? PhotoPath { get; set; }
        public string Initial { get; set; } = "?";
    } //end of class

    public class HeaderAvatarViewComponent : ViewComponent
    { //start of class
        private readonly UserManager<ApplicationUser> _userManager;

        public HeaderAvatarViewComponent(UserManager<ApplicationUser> userManager)
        { //start of m
            _userManager = userManager;
        } //end of 

        public async Task<IViewComponentResult> InvokeAsync()
        { //start of
            var model = new HeaderAvatarModel();

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
                if (user != null)
                {
                    model.SignedIn = true;
                    model.PhotoPath = user.PhotoPath;

                    var source = !string.IsNullOrWhiteSpace(user.FirstName) ? user.FirstName
                               : !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName
                               : user.Email ?? "";
                    if (source.Length > 0) model.Initial = source.Substring(0, 1).ToUpper();
                }
            } //end of

            return View(model);
        } //end of class
    } //end of class
} //end of namespace