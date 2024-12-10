using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng hoặc email")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }


    }
}
