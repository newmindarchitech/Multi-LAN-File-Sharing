using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class FileItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string UploadedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Một file có thể chứa nhiều ảnh
        public ICollection<Images> Images { get; set; } = new List<Images>();
    }
}
