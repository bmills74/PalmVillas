namespace Palm.Models.Book
{
    public class BookView
    {
        public List<DateRange> RangesBooked { get; set; }
    }

    public class DateRange
    {
        public string from { get; set; }
        public string to { get; set; }
    }

   
}
