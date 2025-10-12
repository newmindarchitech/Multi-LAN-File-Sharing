using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class ImageSubmitForm
    {
        public Guid ImageId { get; set; }
        [Required(ErrorMessage ="An Image is required")]
        public IFormFile File { get; set; }
        public userFolder Folder { get; set; }
        public Guid FolderID { get; set; }
    }
}
