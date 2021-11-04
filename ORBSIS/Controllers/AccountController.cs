using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ORBSIS.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        
        private readonly UserManager<IdentityUser> _userManager;
        
        [TempData]
        public string ErrorMessage { get; set; }

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
        }
    
        [Route("Account/AccessDeny")]
        public IActionResult AccessDeny()
        {
            return View();
        }

        [HttpGet]
        [Route("account/facebook-login")]
        public IActionResult ExternalLogin(string returnUrl)
        {
            // Request a redirect to the external login provider.
            string provider = "Facebook";
            var redirectUrl = Url.Action(nameof(Login), "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            string firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email);
            
            if (user != null)
            {
                //await Authenticate(user, info);
                return RedirectToAction("Index", "Chat");
            }
            else
            {
                var identityUser = new IdentityUser { UserName = firstName, Email = email, EmailConfirmed = true };
                var result2 = await _userManager.CreateAsync(identityUser);
                if (result2.Succeeded)
                {
                    result2 = await _userManager.AddLoginAsync(identityUser, info);
                    if (result2.Succeeded)
                    {
                        ClaimsIdentity ident = new ClaimsIdentity(info.Principal.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await _signInManager.SignInAsync(identityUser, isPersistent: false);
                        return RedirectToAction("Index", "Chat");
                    }
                }
            }
            
            return View();
        }

        private async Task Authenticate(IdentityUser identityUser, ExternalLoginInfo externalLoginInfo)
        {
            ClaimsIdentity ident = new ClaimsIdentity(externalLoginInfo.Principal.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignInAsync(identityUser, isPersistent: false);
        }

        [HttpGet]
        [Route("Account/sign-out")]
        private async Task SignOut(IdentityUser identityUser, ExternalLoginInfo externalLoginInfo)
        {
            
        }

    }
}
