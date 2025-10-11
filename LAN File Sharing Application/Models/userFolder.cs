using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LAN_File_Sharing_Application.Models
{
    public class userFolder
    {
        [Key]
        public Guid ID { get; set; }

        [MinLength(10)]
        public string FolderName { get; set; }

        public string? Description { get; set; }

        public ICollection<Images> Images { get; set; }

        [JsonIgnore]
        public Bucket bucket { get; set; }

        public Guid BucketID { get; set; }
    }
}
