using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Palm.Models.Account;
using System.Security.Claims;
using PalmVillas.DbServices;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using PalmVillas.Models.Enums;
using System.Data;
using PalmVillas.Domain;

namespace PalmVillas.Pages
{


    public class IndexModel : PageModel
    {
        [TempData]
        public string ProfilePic { get; set; }

        private readonly SignInManager<User> _signInManager;
        private readonly IAccountDbService _accountDbService;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<User> _userManager;
       
        public IndexModel(

            UserManager<User> userManager,
            IUserStore<User> userStore,
             SignInManager<User> signInManager,
            IAccountDbService accountDbService,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
           
            _signInManager = signInManager;
            _accountDbService = accountDbService;
            _logger = logger;


        }


        public void OnGet()
        {
            try
            {
                //_logger.LogInformation("Info Log test");
                ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

                // Access the claims
                Claim nameClaim = identity.FindFirst(ClaimTypes.Name);
                string userName = nameClaim?.Value;
                ProfilePic = identity.Claims.FirstOrDefault(x=> x.Type.ToString() ==CustomClaimType.Avatar.ToString())?.Value;
                TempData.Keep("ProfilePic");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

            }
        }


        public async Task<IActionResult> OnPost(GoogleResponse response)
        {
            var user = _accountDbService.GetUserById(response.Sub);
            if (user == null)
            {
                user = new User()
                {
                    Email = response.Email,
                    UserName = response.Email,
                    Name = response.Name,
                    Id = response.Sub
                };
                
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim("Avatar", response.Picture));
                }
                else
                {
                    throw new Exception("Couldn't create account");
                }
            }
          
            //update claims
            var avatarClaim = new Claim("Avatar", response.Picture);
            _accountDbService.AddOrUpdateClaim(user.Id, avatarClaim);            
            var extraClaims = _accountDbService.GetClaimsByUserId(user.Id);
            var userRoles = await _userManager.GetRolesAsync(user);
            //now create the auth cookie with all claims
            CreateAuthCookie(response, extraClaims, userRoles);
            return Partial("Pages/Shared/_header.cshtml", response.Picture);


        }

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("Index");
        }
        private void CreateAuthCookie(GoogleResponse response, List<IdentityUserClaim<string>> extraClaims, IList<string> userRoles)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.Name),
                    new Claim(ClaimTypes.Email, response.Email)
                };

            foreach (var claim in extraClaims)
            {
                claims.Add(new Claim(claim.ClaimType.ToString(), claim.ClaimValue));
            }


            claims.AddRange(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            // Create the ClaimsIdentity with the authenticated claims
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Create the authentication properties (you can use these to store additional data in the cookie)
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Set to true if you want the cookie to persist across browser sessions
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // Set the expiration time for the cookie
            };
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Create the authentication ticket
            var authenticationTicket = new AuthenticationTicket(claimsPrincipal, authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user by creating the authentication cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
               claimsPrincipal);

        }


    }
}
