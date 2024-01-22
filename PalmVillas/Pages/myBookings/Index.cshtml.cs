using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Palm.Static;
using PalmVillas;
using PalmVillas.DbServices;
using PalmVillas.Domain;

namespace PalmVillas.Pages.myBookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAccountDbService _accountDbService;
        private readonly PalmVillas.PalmContext _context;

        public IndexModel(PalmVillas.PalmContext context, IAccountDbService accountDbService)
        {
            _context = context;
            _accountDbService = accountDbService;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var userName = User.Identity.GetEmail();
            if (_context.Bookings != null)
            {
                Booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Villa)
                .Where(x=> x.User.UserName== userName)
                .ToListAsync();
            }
        }
    }
}
