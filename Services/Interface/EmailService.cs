using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Services.Interface
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["SMTP:Host"];
                var smtpPort = int.Parse(_configuration["SMTP:Port"]);
                var smtpEmail = _configuration["SMTP:Email"];
                var smtpPassword = _configuration["SMTP:Password"];
                var smtpFrom = _configuration["SMTP:From"];
                var enableSsl = bool.Parse(_configuration["SMTP:EnableSSL"]);
                var isBodyHtml = bool.Parse(_configuration["SMTP:IsBodyHtml"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpFrom, "LawToolBox"), 
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml                    
                };

                mailMessage.To.Add(to);

                mailMessage.Bcc.Add(smtpEmail);

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                    smtpClient.EnableSsl = enableSsl; 
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Exception: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

    }
}
