namespace ResearchHub.Models
{
    public class Collaborations
    {
        public int collaboratorID { get; set; }
        public int collaborateeID { get; set; }
        public System.DateTime timeRequestMade { get; set; }
    }
}
