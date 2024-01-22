using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PalmVillas.Domain;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace PalmVillas.Pages
{
    public class AddRandomModel : PageModel
    {
        private readonly PalmContext _context;
        private readonly UserManager<User> _userManager;
        public AddRandomModel(PalmContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public void OnGet()
        {
            // Generate a random first name          
            var nameRandomiser = RandomizerFactory.GetRandomizer(new FieldOptionsFullName());
            var emailRandomiser = RandomizerFactory.GetRandomizer(new FieldOptionsEmailAddress());

            

            for (int i = 0; i < 20; i++)
            {
                var name = nameRandomiser.Generate();
                var email = emailRandomiser.Generate();

                var user = new User()
                {
                    
                    Email = email,
                    UserName = email,
                    Name = name
                };
                var result =  _userManager.CreateAsync(user);
               
            }
        }
    }
}
