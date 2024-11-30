namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class EmailConfirmRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
