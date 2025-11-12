using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace makatizen_app.Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.From);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
                smtp.Credentials = new NetworkCredential(_emailSettings.User, _emailSettings.Pass);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // IMPORTANT: In a real application, you would log this error (e.g., using ILogger) 
                // and potentially save it to a FailedEmails table (based on schema).
                Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}