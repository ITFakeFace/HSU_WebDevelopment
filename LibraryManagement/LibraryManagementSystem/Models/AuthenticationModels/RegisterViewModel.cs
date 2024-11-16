using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Fullname { get; set; } // Tên đầy đủ

        [Required]
        public string UserName { get; set; } // Tên ngắn
    }
}
