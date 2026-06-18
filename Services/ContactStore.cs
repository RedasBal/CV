using System.Text.Json;
using CV.Models;

namespace CV.Services;

/// <summary>
/// Persists contact-form submissions to a JSON-lines file under <c>App_Data</c>.
/// In a production build this is where you'd send an email or push to a queue,
/// but appending to a local file keeps the demo fully self-contained.
/// </summary>
public class ContactStore
{
    private readonly string _path;
    private readonly ILogger<ContactStore> _logger;
    private static readonly SemaphoreSlim Gate = new(1, 1);

    public ContactStore(IWebHostEnvironment env, ILogger<ContactStore> logger)
    {
        _logger = logger;
        var dir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "messages.jsonl");
    }

    public async Task SaveAsync(ContactMessage message)
    {
        var record = new
        {
            receivedUtc = DateTime.UtcNow,
            message.Name,
            message.Email,
            message.Message
        };

        var line = JsonSerializer.Serialize(record);

        await Gate.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_path, line + Environment.NewLine);
        }
        finally
        {
            Gate.Release();
        }

        _logger.LogInformation("Stored contact message from {Email}", message.Email);
    }
}
