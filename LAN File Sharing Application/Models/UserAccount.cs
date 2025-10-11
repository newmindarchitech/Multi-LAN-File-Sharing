using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace LAN_File_Sharing_Application.Models
{
    public class UserAccount
    {
        [Key]
        public Guid Id { get; set; }

        [MinLength(5)]
        [Required]
        public required string UserName { get; set; }

        [Required, EmailAddress]
        public required string EmailAddress { get; set; }

        [Required]
        [MinLength(8)]
        public required string PasswordHash { get; set; }

        public required string PasswordSalt { get; set; }

        public Guid BucketID { get; set; }

        public Bucket bucket { get; set; }

        //public ICollection<userFolder> Folders { get; set; }
    }
}
