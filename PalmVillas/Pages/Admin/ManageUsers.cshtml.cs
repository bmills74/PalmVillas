using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Models;
using PalmVillas.Models.Enums;
using PalmVillas.Models.User;
using System.Data;
using IQueryable = Microsoft.EntityFrameworkCore.Query;

namespace PalmVillas.Pages.Admin
{
    [Authorize(Policy = "RequireAdmin")]
    public class ManageUsersModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private readonly PalmContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountDbService _accountDbService;
        private readonly UserManager<User> _userManager;

        public ManageUsersModel(IConfiguration configuration,
            PalmContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager
            , IAccountDbService accountDbService)
        {
            Configuration = configuration;
            _context = context;
            _roleManager = roleManager;
            _accountDbService = accountDbService;
            _userManager = userManager;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }


        public List<IdentityRole> SystemRoles { get; set; } = new List<IdentityRole>();

        [BindProperty]
        public PaginatedList<UserDetail> Users { get; set; }
        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<UserDetail> userAndRoles = _accountDbService.GetUsersAndRolesFromIncludes();

            IQueryable<UserDetail> userAndRolesJustForDemonstrationDifferentTechnique = _accountDbService.GetUsersAndRolesFromJoin();

            SortUsers(userAndRoles, searchString, sortOrder, pageIndex);


            SystemRoles = _roleManager.Roles.ToList();

            //this populates the Roles for the checkboxes
            foreach (var user in Users)
            {
                SystemRoles.ForEach(x => user.InRoles.Add(new KeyValuePair<string, bool>(x.Name, user.Roles.Contains(x.Id))));
            }
        }

        private async void SortUsers(IQueryable<UserDetail> userAndRoles, string searchString, string sortOrder, int? pageIndex)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                userAndRoles = userAndRoles
                    .Where(c => EF.Functions.Like(c.Name, "%" + searchString + "%"));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    userAndRoles = userAndRoles.OrderByDescending(s => s.Name);
                    break;
                default:
                    userAndRoles = userAndRoles.OrderBy(s => s.Name);
                    break;
            }

            var pageSize = Configuration.GetValue("PageSize", 4);
            Users = await PaginatedList<UserDetail>.CreateAsync(
                userAndRoles, pageIndex ?? 1, pageSize);
        }

        public IActionResult OnPost()
        {
            var success = _accountDbService.SetUserRoles(Users);
           
            if (success)
            {
                TempData["Message"] = "Roles successfully saved";                
            }
            else
            {
                TempData["Warning"] = "Something went wrong when saving roles";
            }
            return RedirectToPage();

        }
    }
}
