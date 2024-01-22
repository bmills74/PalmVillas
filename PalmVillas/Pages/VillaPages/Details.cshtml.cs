using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Palm.Models.Book;
using PalmVillas.DbServices;
using System.Text.Json;

namespace PalmVillas.Pages.VillaPages
{

    public class DetailsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public DetailsModel(IVillaDbService villaDbService, ILogger<IndexModel> logger)
        {
            _villaDbService = villaDbService;
            _logger = logger;
        }
        private readonly IVillaDbService _villaDbService;
        [FromRoute]
        public int VillaId { get; set; }
        public Domain.Villa Villa { get; internal set; }
        public List<string> Images { get; internal set; }
        public List<DateRange> RangesBooked { get; internal set; } = new List<DateRange>();
        public void OnGet()
        {
            try
            {
                Villa = _villaDbService.GetVilla(VillaId);                
                if (Villa == null) { throw new Exception("Sorry we couldn't get those details"); };

                RangesBooked = _villaDbService.GetFutureBookings(VillaId)
                    .Select(x => new DateRange
                    {
                        from = x.StartDate,
                        to = x.EndDate,
                    }).ToList();
                Images = JsonSerializer.Deserialize<List<string>>(Villa.Images).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }


        }
    }
}
