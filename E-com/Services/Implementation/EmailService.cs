using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using E_com.Models;
using E_com.Repositories;
using E_com.Services.interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Recipient email address is required.", nameof(toEmail));
            }

            // Validate email format
            try
            {
                var mailAddress = new MailAddress(toEmail);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid email address format.", nameof(toEmail));
            }

            var smtpConfig = _config.GetSection("Smtp");

            if (string.IsNullOrEmpty(smtpConfig["Host"]))
                throw new ArgumentNullException("SMTP Host is not configured.");
            if (string.IsNullOrEmpty(smtpConfig["Port"]))
                throw new ArgumentNullException("SMTP Port is not configured.");
            if (string.IsNullOrEmpty(smtpConfig["UserName"]))
                throw new ArgumentNullException("SMTP UserName is not configured.");
            if (string.IsNullOrEmpty(smtpConfig["Password"]))
                throw new ArgumentNullException("SMTP Password is not configured.");

            using var smtpClient = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"]!))
            {
                Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]),
                EnableSsl = bool.Parse(smtpConfig["EnableSsl"]!)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpConfig["UserName"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (FormatException ex)
        {
            // Handle invalid email address format
            throw new InvalidOperationException("The specified string is not in the form required for an email address.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new InvalidOperationException("An unexpected error occurred while sending the email.", ex);
        }
    }

}
