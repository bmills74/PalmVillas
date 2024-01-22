namespace Palm.Models.Book
{
    public class BookingInput
    {
        public string Date { get; set; }
        public string UserId { get; internal set; }
        public string UserName { get; internal set; }
    }
}
