using System.Threading.Tasks;

namespace makatizen_app.Server.Services
{
    public interface IEmailService
    {
        // Method to send an email to a recipient with a specific subject and body.
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}