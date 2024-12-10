using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class ChangePasswordModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
        public string OTP { get; set; }
    }
}
