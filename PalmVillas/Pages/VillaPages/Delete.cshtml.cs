using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PalmVillas.DbServices;
using PalmVillas.Domain;

namespace PalmVillas.Pages.VillaPages
{
    public class DeleteModel : PageModel
    {
        private readonly PalmContext _context;
        private readonly IVillaDbService _villaDbService;

        public DeleteModel(PalmContext context, IVillaDbService villaDbService)
        {
            _context = context;
            _villaDbService = villaDbService;
        }

        [BindProperty]
        public Domain.Villa Villa { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null || _context.Villas == null)
            {
                return NotFound();
            }

            var villa = await _context.Villas
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (villa == null)
            {
                return NotFound();
            }
            else
            {
                Villa = villa;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            //to do: put this in villadbService and use a transaction
            if (id == null || _context.Villas == null)
            {
                return NotFound();
            }
            var villa = await _context.Villas.FindAsync(id);

            if (villa != null)
            {
                var bookings = _context.Bookings.Where(x=> x.VillaId == id).ToList();
                _context.Bookings.RemoveRange(bookings);
                await _context.SaveChangesAsync();

                Villa = villa;
                _context.Villas.Remove(Villa);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
