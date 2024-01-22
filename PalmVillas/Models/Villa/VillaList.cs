namespace PalmVillas.Models.Villas
{
    public class VillaListModel
    {        
        public List<VillaItem> VillaItems { get; set; }
    }

    public class VillaItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Images { get; set; }
        public long? Rooms { get; internal set; }
        public long? Price { get; internal set; }
        public int NumNights { get; internal set; }
        public long TotalPrice {  get { return (long)(NumNights * Price); } }
    }

    
}
