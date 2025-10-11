using System.ComponentModel.DataAnnotations;

namespace LAN_File_Sharing_Application.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Your email is Required"), EmailAddress]
        public required string EmailAddress { get; set; }

        [Required(ErrorMessage = "Your username is required")]
        [MinLength(5, ErrorMessage = "Your UserName cannot be less than 5 characters")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Your password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Your password cannot be less than 8 characters")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Your password confirm is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password must match")]
        public required string ConfirmPassword { get; set; }
    }
}

