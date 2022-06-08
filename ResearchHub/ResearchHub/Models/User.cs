namespace ResearchHub.Models
{
    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public System.DateTime dateOfBirth { get; set; }
        public string profileImageUrl { get; set; }
        public int numberOfDownloads { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }
        public int AspNetUserID { get; set; }
    }
}
