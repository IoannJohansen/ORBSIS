using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ORBSIS.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        
        private readonly UserManager<IdentityUser> _userManager;
        
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
        }
        
        [Route("account/AccessDeny")]
        public IActionResult AccessDeny()
        {
            return View();
        }

        [HttpGet]
        [Route("account/facebook-login")]
        public IActionResult ExternalLogin()
        {
            string provider = "Facebook";
            var redirectUrl = Url.Action(nameof(Login), "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> Login()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            string firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await Authenticate(user, info);
                return RedirectToAction("Index", "Chat");
            }
            else
            {
                user = new IdentityUser { UserName = firstName, Email = email, EmailConfirmed = true };
                var result2 = await _userManager.CreateAsync(user);
                if (result2.Succeeded)
                {
                    result2 = await _userManager.AddLoginAsync(user, info);
                    if (result2.Succeeded)
                    {
                        await Authenticate(user, info);
                        return RedirectToAction("Index", "Chat");
                    }
                }
            }
            return View();
        }

        private async Task Authenticate(IdentityUser identityUser, ExternalLoginInfo externalLoginInfo)
        {
            var ident = new ClaimsIdentity(externalLoginInfo.Principal.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignInAsync(identityUser, isPersistent: false);
        }

        [Route("account/signout")]
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Chat");
        }


    }
}
