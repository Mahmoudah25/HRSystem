namespace HRManagmentSystem.DTOs.Account_User
{
    public class ForgetPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
    }
    public class ResetPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
