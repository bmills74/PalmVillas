using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PalmVillas;
using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Static;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PalmVillas.Pages.VillaPages
{
    [Authorize(Policy = "ElevatedRights")]
    public class EditModel : PageModel
    {
        private IHostingEnvironment _environment;
        private IVillaDbService _villaDbService;
        private ILogger _logger;
        public EditModel(IHostingEnvironment environment, IVillaDbService villaDbService)
        {
            _villaDbService = villaDbService;
            _environment = environment;
           // _logger = logger;
        }
        [BindProperty]
        public IFormFile? Image1 { get; set; }
        [BindProperty]
        public IFormFile? Image2 { get; set; }
        [BindProperty]
        public IFormFile? Image3 { get; set; }

        [BindProperty]
        public Domain.Villa Villa { get; set; } = default!;
        public List<string> Images { get; set; }

        public IActionResult OnGet(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var villa = _villaDbService.GetVilla(id);
            if (villa == null)
            {
                return NotFound();
            }
            Villa = villa;
            Images = JsonSerializer.Deserialize<List<string>>(villa.Images).ToList();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var existingImages = JsonSerializer.Deserialize<List<string>>(Villa.Images).ToList();
            var formImages = new List<IFormFile>() { Image1, Image2, Image3 };
            var fileNames = new List<string>();
            for (int i = 0; i < formImages.Count; i++)
            {
                var image = formImages[i];
                if (image != null)
                {
                    try
                    {
                        fileNames.Add("/images/" + image.FileName);
                        ImageExtensions.UploadImages(image, _environment);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "Bad image format");
                        TempData["Warning"] = "It looks like you're using an unsupported image format, " +
                            "please try save the image as jpg, png or bmp first";
                        return new RedirectToPageResult("Index");
                    }

                }
                else
                {
                    fileNames.Add(existingImages[i]);
                }
            }

            Villa.Images = JsonSerializer.Serialize<List<string>>(fileNames);
            var saveResult = _villaDbService.EditVilla(Villa);
            if (saveResult == -1)
            {
                return NotFound();
            }
            TempData["Message"] = "Villa successfully edited";
            return RedirectToPage("./Index");
        }


    }
}
