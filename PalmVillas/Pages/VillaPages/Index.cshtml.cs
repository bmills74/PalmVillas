using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PalmVillas.Domain;

namespace PalmVillas.Pages.VillaPages
{
    [Authorize(Policy = "ElevatedRights")]
    public class IndexModel : PageModel
    {
        private readonly PalmContext _context;

        public IndexModel(PalmContext context)
        {
            _context = context;
        }

        public IList<Domain.Villa> Villa { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Villas != null)
            {
                Villa = await _context.Villas.ToListAsync();
            }
        }
    }
}
