using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAN_File_Sharing_Application.Models
{
    public class Images
    {
        [Key]
        public Guid ID { get; set; }

        // ✅ Tên file ảnh
        [Required]
        public string FileName { get; set; } = string.Empty;

        // ✅ Đường dẫn lưu file trong wwwroot/uploads
        public string FilePath { get; set; } = string.Empty;

        // ✅ Ngày upload
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // ✅ Mối quan hệ 1 FileItem chứa nhiều ảnh
        [ForeignKey("FileItem")]
        public Guid? FileItemId { get; set; }   // Có thể null nếu ảnh không thuộc FileItem
        public FileItem? FileItem { get; set; }

        // ✅ Mối quan hệ 1 Folder chứa nhiều ảnh
        [ForeignKey("Folder")]
        public Guid? FolderID { get; set; }     // Có thể null nếu ảnh chỉ thuộc FileItem
        public userFolder? Folder { get; set; }
    }
}
