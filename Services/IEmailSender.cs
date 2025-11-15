namespace HRManagmentSystem.Services
{ 
      public interface IAppEmailSender
      {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
      }
}
