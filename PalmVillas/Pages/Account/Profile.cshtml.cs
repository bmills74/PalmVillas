using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PalmVillas.DbServices;
using System.Security.Claims;
using PalmVillas.Static;
using PalmVillas.Domain;

namespace PalmVillas.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly PalmContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountDbService _accountDbService;
        private User _user;


        [BindProperty]
        public List<KeyValuePair<string, bool>> Roles { get; set; } = new List<KeyValuePair<string, bool>>();
        public User AppUser { get; set; }

        public ProfileModel(UserManager<User> userManager,
            IAccountDbService accountDbService,
            RoleManager<IdentityRole> roleManager,
             PalmContext context)
        {
            _roleManager = roleManager;
            _accountDbService = accountDbService;
            _userManager = userManager;
            _context = context;
        }
        public async void OnGet()
        {
            _user = UserFunctions.GetUserByUserName(_accountDbService, User);
            var myRoles = await _userManager.GetRolesAsync(_user);
            var systemRoles = _roleManager.Roles.Select(x => x.Name).ToList();
            systemRoles.ForEach(x => Roles.Add(new KeyValuePair<string, bool>(x, myRoles.Contains(x))));
            AppUser = _user;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _user = UserFunctions.GetUserByUserName(_accountDbService, User);

                    var myRoles = await _userManager.GetRolesAsync(_user);
                    var roleList = Roles.Where(x=> x.Value).Select(x => x.Key).ToList();

                    var rolesToRemove = myRoles.Where(x => !roleList.Contains(x)).ToList();
                    var removeResult = _userManager.RemoveFromRolesAsync(_user, rolesToRemove);

                    //add
                    var rolesToAdd = roleList.Where(x => !myRoles.Contains(x)).ToList();
                    var result = _userManager.AddToRolesAsync(_user, rolesToAdd);
                    transaction.Commit();   
                    TempData["Message"] = "Roles successfully saved";
                    TempData["Warning"] = "You need to log out and in again for these changes to take effect";
                    return RedirectToPage();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Warning"] = "Something went wrong when saving roles";
                    return RedirectToPage();
                }
            }
        }
    }
}
