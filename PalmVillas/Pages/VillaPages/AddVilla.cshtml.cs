using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using PalmVillas.Static;
using System.Text.Json;
using PalmVillas.DbServices;


namespace PalmVillas.Pages.VillaPages
{
    [Authorize(Policy = "ElevatedRights")]

    public class AddVillaModel : PageModel
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private IVillaDbService _villaDbService;

        public AddVillaModel(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IVillaDbService villaDbService)
        {
            _villaDbService = villaDbService;
            _environment = environment;
        }


        public void OnGet()
        {
            
        }

        [BindProperty]
        public Domain.Villa? Villa { get; set; }

        [BindProperty]
        public IFormFile? Image1 { get; set; }
        [BindProperty]
        public IFormFile? Image2 { get; set; }
        [BindProperty]
        public IFormFile? Image3 { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Villa == null)
            {
                TempData["Warning"] = "It looks like you didn't fill out the fields correctly. Please try again";
                return Page();
            }
            var images = new List<IFormFile>() { Image1, Image2, Image3 };
            var fileNames = new List<string>();
            var templateNames = new List<string>() { "/images/villa1_image1.jpg", "/images/villa1_image2.jpg", "/images/villa1_image3.jpg" };
            var i = 0;
            foreach (var image in images)
            {
                if (image != null)
                {
                    fileNames.Add("/images/" + image.FileName);
                   ImageExtensions.UploadImages(image,_environment);
                }
                else
                {
                    fileNames.Add(templateNames[i]);
                }
                i++;
            }
            Villa.Images = JsonSerializer.Serialize<List<string>>(fileNames);
            _villaDbService.AddVilla(Villa);
            TempData["Message"] = "Villa successfully edited";
            return new RedirectToPageResult("Index");
        }

       
    }
}
