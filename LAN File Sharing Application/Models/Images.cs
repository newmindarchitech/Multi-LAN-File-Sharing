using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class Images
    {
        [Key]
        public Guid ID { get; set; }

        public string? ImageName { get; set; }

        public userFolder Folder { get; set; }

        public Guid FolderID { get; set; }
    }
}
