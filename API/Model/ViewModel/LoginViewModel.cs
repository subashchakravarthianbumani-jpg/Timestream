using Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class LoginViewModel
    {
        public string LoginId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string UserNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Privillage { get; set; } = null!;

        public AccountUserModel UserDetails { get; set; } = null!;
    }

    public class LoginRequestModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
    }

    public class ValidateOtpModel
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
    public class SaveNewPasswordModel
    {
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }

}
