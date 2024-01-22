using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PalmVillas.DbServices;
using PalmVillas.Models.Villas;
using System.Text.Json;

namespace PalmVillas.Pages.VillaPages
{    
   
    public class ListVillasModel : PageModel
    {
        private readonly IVillaDbService _villaDbService;
        public List<VillaItem> VillaItems { get; set; }

        public ListVillasModel(IVillaDbService villaDbService)
        {
            _villaDbService = villaDbService;
        }
        public void OnGet()
        {           
            var villas = _villaDbService.ListVillas();           
            VillaItems = villas.Select(x => new VillaItem()
            {
                Id = (int)x.Id,
                Name = x.Name,
                Images = JsonSerializer.Deserialize<List<string>>(x.Images).ToList(),
                Rooms = x.Rooms,
                Price = x.Price
            }).ToList();
        }
    }
}
