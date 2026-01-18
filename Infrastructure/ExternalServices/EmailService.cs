using Application.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var gmailAccount = Environment.GetEnvironmentVariable("GMAIL_ACCOUNT")
                ?? throw new InvalidOperationException("GMAIL_ACCOUNT not set");

            var gmailPassword = Environment.GetEnvironmentVariable("GMAIL_PASSWORD_APP")
                ?? throw new InvalidOperationException("GMAIL_PASSWORD_APP not set");

            var gmailFrom = Environment.GetEnvironmentVariable("GMAIL_FROM")
                ?? throw new InvalidOperationException("GMAIL_FROM not set");

            using var client = new SmtpClient
            {
                Host = configuration["Email:Smtp:Host"]!,
                Port = int.Parse(configuration["Email:Smtp:Port"]!),
                EnableSsl = bool.Parse(configuration["Email:Smtp:EnableSsl"]!),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(gmailAccount, gmailPassword)
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(gmailFrom),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
