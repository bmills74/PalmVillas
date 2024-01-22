namespace Palm.Models.Account
{
    public class GoogleResponse
    {
        public required string Sub { get; set; }
        public required string Locale { get; set; }
        public required string Name { get; set; }
        public bool EmailVerified { get; set; }
        public required string Email { get; set; }        
        public required string Picture { get; set; }

    }
}
