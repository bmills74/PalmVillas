using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Palm.Static;

using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Models;
using PalmVillas.Models.Book;
using PalmVillas.Models.Villas;
using System.Text.Json;

namespace PalmVillas.Pages.Book
{
    public class BookingSummaryModel : PageModel
    {
        #region parameter
        [BindProperty(SupportsGet = true)]
        public BookingSummaryInput Input { get; set; }
        #endregion

        #region Model
        public VillaItem Villa { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool BookingConflict { get; set; }
        #endregion


        #region Services
        private IBookingDbService _bookingDbService { get; set; }
        private IAccountDbService _accountDbService { get; set; }
        private IVillaDbService _villaDbService { get; set; }
        private BookingUtility _bookingUtility { get; set; }
        #endregion
        public BookingSummaryModel(IBookingDbService bookingDbService,
            IAccountDbService accountDbService,
            IVillaDbService villaDbService
            , BookingUtility bookingUtility)
        {
            _bookingDbService = bookingDbService;
            _accountDbService = accountDbService;
            _villaDbService = villaDbService;
            _bookingUtility = bookingUtility;
        }
        public void OnGet()
        {
            Guard.Against.Null(Input);
            Input.UserName = User.Identity.GetEmail();
            var user = _accountDbService.GetUserByUserName(Input.UserName) ?? new User()
            {
                Id = "guest",
                Name = "Guest",
                Email = "guest@gmail.com"
            };
            var villa = _villaDbService.GetVilla(Input.VillaId);
            Guard.Against.Null(villa);
            var success = DateTime.TryParse(Input.StartDate, out var startDate);
            success = DateTime.TryParse(Input.EndDate, out var endDate);
            Guard.Against.AgainstExpression(x => x, success, "Couldn't parse date string");

            StartDate = startDate;
            EndDate = endDate;
            UserName = Input.UserName;

            Villa = new VillaItem()
            {
                Id = (int)villa.Id,
                Name = villa.Name,
                Images = JsonSerializer.Deserialize<List<string>>(villa.Images).ToList(),
                Rooms = villa.Rooms,
                Price = villa.Price,
                NumNights = (endDate - startDate).Days
            };
        }

        public ActionResult OnPost()
        {
            var user = _accountDbService.GetUserByUserName(Input.UserName) ?? new User() { Id = "guest" };
            var villa = _villaDbService.GetVilla(Input.VillaId);
            Guard.Against.Null(villa);

            var success = DateTime.TryParse(Input.StartDate, out var startDate);
            success = DateTime.TryParse(Input.EndDate, out var endDate);
            Guard.Against.AgainstExpression(x => x, success, "Couldn't parse date string");

            var booking = new Booking()
            {
                UserId = user.Id,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate,
                Price = (long)villa.Price * (long)(endDate - startDate).Days,
                VillaId = villa.Id,
            };
            if (_bookingUtility.IfConflictReturnTrue(booking))
            {
                BookingConflict = true;
                return RedirectToPage("/Book/DuplicateBooking");
            }

            _villaDbService.AddBooking(booking);
            return RedirectToPage("/Book/Confirmation");
        }
    }
}
