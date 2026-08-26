using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SmartFridgeAPI.Interfaces;

namespace SmartFridgeAPI.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task SendEmailConfirmationAsync(string toEmail, string toName, string confirmationLink)
        {
            var subject = "Confirme su cuenta de SmartFridge";
            var body = EmailTemplates.ConfirmationTemplate(toName, confirmationLink);
            return SendAsync(toEmail, toName, subject, body);
        }

        public Task SendPasswordResetAsync(string toEmail, string toName, string resetLink)
        {
            var subject = "Restablecer tu contraseña de SmartFridge";
            var body = EmailTemplates.PasswordResetTemplate(toName, resetLink);
            return SendAsync(toEmail, toName, subject, body);
        }

        private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["Email:FromName"], _config["Email:FromAddress"]));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    _config["Email:SmtpHost"],
                    int.Parse(_config["Email:SmtpPort"]!),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_config["Email:SmtpUser"], _config["Email:SmtpPass"]);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true);
            }
        }
    }
}