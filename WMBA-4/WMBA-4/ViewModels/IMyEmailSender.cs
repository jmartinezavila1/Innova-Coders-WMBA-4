using System.Net.Mail;

namespace WMBA_4.ViewModels
{
    /// <summary>
    /// Interface for my own email service
    /// </summary>
    public interface IMyEmailSender
    {
        Task SendOneAsync(string name, string email, string subject, string htmlMessage);
        Task SendToManyAsync(EmailMessage emailMessage);
    }
}
