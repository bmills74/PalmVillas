using PalmVillas;
using PalmVillas.Models.Villas;

namespace Palm.Models.Book
{
    public class BookingSummaryView
    {
        public VillaItem Villa { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserName { get;  set; }
        public  string Token { get; set; }
    }

   
}
