using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
            _signInManager = signInManager;
            _userManager = userManager;
        }
        
        [Route("account/AccessDeny")]
        public IActionResult AccessDeny()
        {
            return View();
        }

        [HttpGet]
        [Route("account/facebook-login")]
        public IActionResult LoginFacebook()
        {
            var props = _signInManager.ConfigureExternalAuthenticationProperties(FacebookDefaults.AuthenticationScheme, Url.Action(nameof(LoginFacebookCallback), "Account"));
            return Challenge(props, FacebookDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginFacebookCallback()
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            string firstName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName),
                email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await Autorize(user, loginInfo);
                return RedirectToAction("Index", "Chat");
            }
            else
            {
                user = new IdentityUser { UserName = firstName, Email = email, EmailConfirmed = true };
                var userCreatinResult = await CreateNewUser(user, loginInfo);
                if (userCreatinResult)
                {
                    return RedirectToAction("Index", "Chat");
                }
            }
            return View("AccessDeny");
        }

        [Route("account/signout")]
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Chat");
        }

        [Route("account/google-login")]
        public IActionResult LoginGoogle()
        {
            var props = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.DisplayName, Url.Action(nameof(LoginGoogleCallback)));
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginGoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
                string name = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName).Value,
                email = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
                var user = await _userManager.FindByEmailAsync(email);
                if (user!=null)
                {
                    await Autorize(user, loginInfo);
                    return RedirectToAction("Index", "Chat");
                }
                else
                {
                    user = new IdentityUser() { UserName = name, Email = email, EmailConfirmed = true };
                    var res = await CreateNewUser(user, loginInfo);
                    if (res)
                    {
                        await Autorize(user, loginInfo);
                        return RedirectToAction("Index", "Chat");
                    }
                }
            }
            return RedirectToAction("AccessDeny");
        }

        [Route("account/github-login")]
        public IActionResult LoginGithub()
        {
            var props = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", Url.Action(nameof(LoginGithubCallback), "Account"));
            return Challenge(props, "GitHub");
        }

        public async Task<IActionResult> LoginGithubCallback()
        {
            var result = await HttpContext.AuthenticateAsync("GitHub");
            if (result.Succeeded)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                string userName = info.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name).Value;
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    user = new IdentityUser { UserName = userName, Email = userName };
                    bool creationResult = await CreateNewUser(user, info);
                    if (creationResult)
                    {
                        await Autorize(user, info);
                        return RedirectToAction("Index", "Chat");
                    }
                }
                else
                {
                    await Autorize(user, info);
                    return RedirectToAction("Index", "Chat");
                }
            }
            return RedirectToAction("AccessDeny", "Account");
        }

        #region Helpers

        private async Task Autorize(IdentityUser user, ExternalLoginInfo externalLoginInfo)
        {
            var ident = new ClaimsIdentity(externalLoginInfo.Principal.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        private async Task<bool> CreateNewUser(IdentityUser user, ExternalLoginInfo externalLoginInfo)
        {
            var userCreationResult = await _userManager.CreateAsync(user);
            if (userCreationResult.Succeeded)
            {
                userCreationResult = await _userManager.AddLoginAsync(user, externalLoginInfo);
                if (userCreationResult.Succeeded)
                {
                    await Autorize(user, externalLoginInfo);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}