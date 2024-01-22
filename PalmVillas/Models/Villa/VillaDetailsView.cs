using Palm.Models.Book;
using PalmVillas.Domain;

namespace PalmVillas.Models.Villas
{
    public class VillaDetailsView
    {
        public int VillaId { get; set; }
        public Villa Villa { get; internal set; }
        public List<string> Images { get; internal set; }
        public List<DateRange> RangesBooked { get; internal set; }
    }
}
