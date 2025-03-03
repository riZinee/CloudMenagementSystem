using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Email:Smtp:Email"], _configuration["Email:Smtp:Name"]),
            Subject = subject,
            Body = body
        };

        mailMessage.To.Add(to);
        mailMessage.IsBodyHtml = true;

        using var smtpClient = new SmtpClient();
        smtpClient.Host = _configuration["Email:Smtp:Host"];
        smtpClient.Port = int.Parse(_configuration["Email:Smtp:Port"]);
        smtpClient.Credentials = new NetworkCredential(_configuration["Email:Smtp:Email"], _configuration["Email:Smtp:Password"]);

        smtpClient.EnableSsl = true;

        await smtpClient.SendMailAsync(mailMessage);
    }
}
