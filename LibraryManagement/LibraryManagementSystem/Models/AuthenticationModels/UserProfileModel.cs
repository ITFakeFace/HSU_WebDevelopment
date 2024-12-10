using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models.AuthenticationModels
{
    public class UserProfileModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NewEmail { get; set; }
        public string OTP { get; set; }

        public string Phone { get; set; }

        public string NewPhone { get; set; }

        public string? Fullname { get; set; }

        public string? Pid { get; set; }

        public string? Address { get; set; }

        public DateOnly? Dob { get; set; }

        public int? Status { get; set; }
        public string? Gender { get; set; }

        public byte[]? Avatar { get; set; }
        public byte[]? CoverAvatar { get; set; }

    }
}
