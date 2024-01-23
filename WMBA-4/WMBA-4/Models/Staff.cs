namespace WMBA_4.Models
{
    public class Staff
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string email { get; set;}

        public ICollection<TeamStaff> TeamStaff { get; set; } = new HashSet<TeamStaff>();
    }
}
