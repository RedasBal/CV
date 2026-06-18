using System.Security.Claims;
using CV.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace CV.Pages.Admin;

public class LoginModel : PageModel
{
    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockWindow = TimeSpan.FromMinutes(15);

    private readonly AdminAuth _auth;
    private readonly IMemoryCache _cache;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(AdminAuth auth, IMemoryCache cache, ILogger<LoginModel> logger)
    {
        _auth = auth;
        _cache = cache;
        _logger = logger;
    }

    [BindProperty] public string? Username { get; set; }
    [BindProperty] public string? Password { get; set; }

    public bool Enabled => _auth.IsEnabled;
    public string? Error { get; private set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Admin/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!_auth.IsEnabled)
        {
            Error = "The admin area is disabled. Set the ADMIN_PASSWORD environment variable to enable it.";
            return Page();
        }

        var key = $"login-attempts:{ClientIp()}";
        var attempts = _cache.GetOrCreate(key, e => { e.AbsoluteExpirationRelativeToNow = LockWindow; return 0; });
        if (attempts >= MaxAttempts)
        {
            _logger.LogWarning("Admin login locked out for {Ip}", ClientIp());
            Error = "Too many failed attempts. Please try again later.";
            return Page();
        }

        if (!_auth.Verify(Username, Password))
        {
            _cache.Set(key, attempts + 1, LockWindow);
            _logger.LogWarning("Failed admin login from {Ip}", ClientIp());
            Error = "Invalid username or password.";
            return Page();
        }

        _cache.Remove(key);

        var claims = new List<Claim> { new(ClaimTypes.Name, _auth.Username), new(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        _logger.LogInformation("Admin login succeeded from {Ip}", ClientIp());
        return RedirectToPage("/Admin/Index");
    }

    private string ClientIp() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
