using CV.Models;
using CV.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace CV.Pages;

public class IndexModel : PageModel
{
    // Allow at most this many contact submissions per IP per window.
    private const int MaxSubmissionsPerWindow = 3;
    private static readonly TimeSpan RateWindow = TimeSpan.FromMinutes(10);

    private readonly CvService _cv;
    private readonly ContactStore _contacts;
    private readonly IMemoryCache _cache;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(CvService cv, ContactStore contacts, IMemoryCache cache, ILogger<IndexModel> logger)
    {
        _cv = cv;
        _contacts = contacts;
        _cache = cache;
        _logger = logger;
    }

    public CvData Data { get; private set; } = new();

    [BindProperty]
    public ContactMessage Contact { get; set; } = new();

    /// <summary>
    /// Honeypot. Hidden from real users via CSS; bots tend to fill every field,
    /// so a non-empty value means the submission is almost certainly automated.
    /// </summary>
    [BindProperty]
    public string? Website { get; set; }

    public bool MessageSent { get; private set; }

    public void OnGet()
    {
        Data = _cv.Get();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Data = _cv.Get();

        // 1. Honeypot — pretend success so bots don't learn they were caught.
        if (!string.IsNullOrWhiteSpace(Website))
        {
            _logger.LogWarning("Contact honeypot triggered from {Ip}", ClientIp());
            MessageSent = true;
            return Page();
        }

        // 2. Rate limit per client IP to prevent spam / disk-fill abuse.
        if (IsRateLimited())
        {
            ModelState.AddModelError(string.Empty,
                "You've sent several messages already. Please try again later.");
            return Page();
        }

        // 3. Server-side validation (the authoritative check).
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _contacts.SaveAsync(Contact);
        MessageSent = true;
        Contact = new ContactMessage();
        ModelState.Clear();
        return Page();
    }

    private bool IsRateLimited()
    {
        var key = $"contact-rate:{ClientIp()}";
        var count = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = RateWindow;
            return 0;
        });

        if (count >= MaxSubmissionsPerWindow)
        {
            return true;
        }

        _cache.Set(key, count + 1, RateWindow);
        return false;
    }

    private string ClientIp() =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
