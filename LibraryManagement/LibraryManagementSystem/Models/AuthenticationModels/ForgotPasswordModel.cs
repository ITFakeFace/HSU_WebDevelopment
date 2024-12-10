using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }
    }
}
