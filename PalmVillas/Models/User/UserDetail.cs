namespace PalmVillas.Models.User
{
    public class UserDetail
    {
        public UserDetail() { }
        public string UserId { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public List<KeyValuePair<string, bool>> InRoles { get; set; } = new List<KeyValuePair<string, bool>>();
    }
}
