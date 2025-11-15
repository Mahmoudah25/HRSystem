namespace HRManagmentSystem.DTOs.Account_User
{
    public class GoogleLoginDTO
    {
        public string IdToken { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}