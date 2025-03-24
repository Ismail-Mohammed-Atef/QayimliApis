using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Qayimli.Core.Service;

namespace Qayimli.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Set to true if message contains HTML
            };

            mail.To.Add(toEmail);

            using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                smtpClient.EnableSsl = true; // Gmail requires SSL

                await smtpClient.SendMailAsync(mail);
            }
        }
    }
}
