using System.Net;
using System.Net.Mail;
using CV.Models;
using Microsoft.Extensions.Options;

namespace CV.Services;

/// <summary>
/// Sends contact-form submissions as email over SMTP. If SMTP isn't configured
/// the sender is a no-op (the message is still saved to disk), and any send
/// failure is logged but never surfaced to the visitor.
/// </summary>
public class EmailSender
{
    private readonly EmailOptions _o;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailOptions> options, ILogger<EmailSender> logger)
    {
        _o = options.Value;
        _logger = logger;

        if (!IsConfigured)
        {
            _logger.LogInformation(
                "Email notifications are OFF — set Email__SmtpHost / Email__SmtpUser / " +
                "Email__SmtpPassword (and Email__To) to enable them.");
        }
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_o.SmtpHost) &&
        !string.IsNullOrWhiteSpace(_o.SmtpUser) &&
        !string.IsNullOrWhiteSpace(_o.SmtpPassword) &&
        !string.IsNullOrWhiteSpace(_o.To);

    public async Task SendContactAsync(ContactMessage m)
    {
        if (!IsConfigured) return;

        try
        {
            var from = string.IsNullOrWhiteSpace(_o.From) ? _o.SmtpUser! : _o.From;

            using var mail = new MailMessage
            {
                From = new MailAddress(from, "CV Website"),
                Subject = $"New CV message from {m.Name}",
                Body =
                    $"You received a new message from your CV contact form.\n\n" +
                    $"Name:  {m.Name}\n" +
                    $"Email: {m.Email}\n\n" +
                    $"Message:\n{m.Message}\n\n" +
                    $"— Reply directly to this email to respond.",
                IsBodyHtml = false
            };
            mail.To.Add(_o.To);
            // Let the owner hit "Reply" and answer the visitor directly.
            mail.ReplyToList.Add(new MailAddress(m.Email, m.Name));

            using var client = new SmtpClient(_o.SmtpHost, _o.SmtpPort)
            {
                EnableSsl = _o.UseSsl,
                Credentials = new NetworkCredential(_o.SmtpUser, _o.SmtpPassword),
                Timeout = 15000
            };

            await client.SendMailAsync(mail);
            _logger.LogInformation("Contact email delivered to {To}", _o.To);
        }
        catch (Exception ex)
        {
            // Never fail the visitor's request because email didn't go through.
            _logger.LogError(ex, "Failed to send contact email.");
        }
    }
}
