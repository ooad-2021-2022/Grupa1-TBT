namespace ResearchHub.Models
{
    public class Ratings
    {
        public int giverID { get; set; }
        public int receiverID { get; set; }
        public int paperID { get; set; }
        public double rating { get; set; }

    }
}
