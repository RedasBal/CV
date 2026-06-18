using System.Security.Cryptography;
using System.Text;

namespace CV.Services;

/// <summary>
/// Single-user admin authentication. Credentials come from configuration /
/// environment variables — never from source. If no password is configured
/// the admin area is disabled entirely (fail-closed), except that in the
/// Development environment a default password is allowed for convenience.
/// </summary>
public class AdminAuth
{
    private readonly string _username;
    private readonly string? _password;

    public AdminAuth(IConfiguration config, IWebHostEnvironment env, ILogger<AdminAuth> logger)
    {
        _username = config["Admin:Username"] ?? "admin";

        // Accept either the config key Admin:Password (env: Admin__Password)
        // or the simpler ADMIN_PASSWORD environment variable.
        _password = config["Admin:Password"]
                    ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        if (string.IsNullOrEmpty(_password))
        {
            if (env.IsDevelopment())
            {
                _password = "admin";
                logger.LogWarning(
                    "ADMIN PASSWORD NOT SET — using the default 'admin' for Development only. " +
                    "Set ADMIN_PASSWORD before deploying.");
            }
            else
            {
                logger.LogWarning(
                    "ADMIN_PASSWORD not configured — the admin area is DISABLED. " +
                    "Set the ADMIN_PASSWORD environment variable to enable it.");
            }
        }
    }

    /// <summary>True when an admin password is configured and login is possible.</summary>
    public bool IsEnabled => !string.IsNullOrEmpty(_password);

    public string Username => _username;

    /// <summary>Verifies credentials using constant-time comparisons.</summary>
    public bool Verify(string? username, string? password)
    {
        if (!IsEnabled) return false;

        var userOk = FixedEquals(username ?? "", _username);
        var passOk = FixedEquals(password ?? "", _password!);
        return userOk && passOk;
    }

    private static bool FixedEquals(string a, string b)
    {
        var ba = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(
            Pad(ba, Math.Max(ba.Length, bb.Length)),
            Pad(bb, Math.Max(ba.Length, bb.Length)));
    }

    private static byte[] Pad(byte[] src, int len)
    {
        if (src.Length == len) return src;
        var dst = new byte[len];
        Array.Copy(src, dst, src.Length);
        return dst;
    }
}
