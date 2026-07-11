namespace CV.Services;

/// <summary>
/// SMTP settings for contact-form email notifications, bound from the "Email"
/// configuration section. Credentials come from environment variables
/// (e.g. Email__SmtpPassword) — never commit them to the repo.
/// </summary>
public class EmailOptions
{
    /// <summary>Where contact messages are delivered.</summary>
    public string To { get; set; } = "";

    /// <summary>From address; defaults to <see cref="SmtpUser"/> when empty.</summary>
    public string? From { get; set; }

    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; } = 587;
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }

    /// <summary>Use STARTTLS/SSL (true for Gmail on port 587).</summary>
    public bool UseSsl { get; set; } = true;
}
