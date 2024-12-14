using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập số cccd")]
        public string PId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string UserName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu xác nhận")]
        public string ConfirmPassword { get; set; }
    }
}
