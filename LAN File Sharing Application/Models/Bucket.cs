using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class Bucket
    {
        [Key]
        public Guid Id { get; set; }
        public UserAccount Account { get; set; }

        public Guid AccountID { get; set; }
        public ICollection<userFolder> Folders { get; set; }
    }
}
