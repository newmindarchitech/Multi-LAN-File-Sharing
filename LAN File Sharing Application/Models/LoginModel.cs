using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Your Email Address is required"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Your password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
