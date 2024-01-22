using PalmVillas.Models.Villas;

namespace PalmVillas.Models.Book
{
    public class BookingSummaryInput
    {
        public VillaItem Villa { get; set; }
        public int VillaId { get; set; }
       
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UserName { get;  set; }
    }
}
