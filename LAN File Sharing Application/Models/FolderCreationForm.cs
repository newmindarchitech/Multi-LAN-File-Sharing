using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class FolderCreationForm
    {
        [DisplayName("FolderID")]
        [ReadOnly(true)]
        public Guid FolderID { get; set; }

        [Required]
        [MinLength(2,ErrorMessage ="The name of your folder is required")]
        public required string FolderName { get; set; }
        [Required(ErrorMessage ="Access Modifiers for the folders are required")]
        public required string IsGlobal { get; set; }
        public string? FolderDescription { get; set; }

        [DisplayName("BucketID")]
        [ReadOnly(true)]
        public Guid BucketID { get; set; }

    }
}
