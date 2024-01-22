using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Models.Book;
using System.Linq;

namespace PalmVillas.Models
{
    public class BookingUtility
    {
        private IVillaDbService _villaDbService;
        public BookingUtility(IVillaDbService villaDbService) 
        {
            _villaDbService = villaDbService;
        }

        /// <summary>
        /// Checks for any conflict between the passed in booking and a list of future bookings retrieved by the database
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public virtual bool IfConflictReturnTrue(Booking booking) 
        {
            var bookings = _villaDbService.GetFutureBookings((int)booking.VillaId);

            var dateTimeRanges = new List<DateTimeRange>();
            var currentFrom = DateTime.Parse(booking.StartDate).Date;
            var currentTo = DateTime.Parse(booking.EndDate).Date;
            var numNightsForInfo = (currentTo - currentFrom).Days;
            foreach (var bookingRange in bookings)
            {
                dateTimeRanges.Add(new DateTimeRange()
                {
                    From = DateTime.Parse(bookingRange.StartDate).Date,
                    To = DateTime.Parse(bookingRange.EndDate).Date
                });
            }

            if (dateTimeRanges.Any(x=> x.From < currentTo && x.To > currentFrom))
            {
                return true;
            }
            return false;
        }
    }
}
