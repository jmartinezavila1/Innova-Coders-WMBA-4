namespace WMBA_4.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string Description { get; set; }


        public ICollection<Staff> StaffMembers { get; set; }
    }
}
