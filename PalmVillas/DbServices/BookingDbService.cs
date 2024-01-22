using Microsoft.EntityFrameworkCore;
using Palm.Models.Book;
using PalmVillas.Domain;
using PalmVillas.Models.Book;

namespace PalmVillas.DbServices
{
    public interface IBookingDbService
    {
        long MakeBooking(BookingSummaryInput input);
       
    }

    public class BookingDbService : IBookingDbService
    {
        private readonly PalmContext db;
        public BookingDbService(PalmContext db)
        {
            this.db = db;
        }

        public long MakeBooking(BookingSummaryInput input)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == input.UserName);
            var villa = db.Villas.Find((long)input.Villa.Id);

            if (villa == null) { throw new Exception(); }

            var booking = new Booking()
            {
                UserId = user?.Id ?? "113404713027476358186",
                //StartDate = input.StartDate.ToString("yyyy-MM-dd"),
                // EndDate = input.EndDate.ToString("yyyy-MM-dd"),
                // Price = (long)villa.Price * (long)(input.EndDate - input.StartDate).Days,
                VillaId = villa.Id,
            };
            db.Bookings.Add(booking);

            //todo
            //token then goes to payment processor before db changes are saved
            //to do: implement stripe paymnet processor code here
            db.SaveChanges();
            return booking.BookingId;
        }

        //public BookView RetrieveBookings(int villaId)
        //{
        //    var now = DateTime.Now.ToString("yyyy-MM-dd");
        //    var until = DateTime.Now.AddMonths(12);
        //    FormattableString querystring = $"SELECT * FROM Bookings WHERE date(StartDate) BETWEEN date({now}) AND date({until}) AND VIllaId={villaId}";
        //    var bookings = db.Bookings
        //        .FromSql(querystring).ToList();

        //    var dateRanges = bookings.Select(x => new DateRange
        //    {
        //        from = x.StartDate,
        //        to = x.EndDate,
        //    }).ToList();

        //    var model = new BookView()
        //    { RangesBooked = dateRanges };
        //    return model;
        //}

    }
}
