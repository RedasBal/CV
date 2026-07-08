using CV.Middleware;
using CV.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using QuestPDF.Infrastructure;

// QuestPDF Community licence — free for individuals and small companies.
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Don't leak the server technology in the "Server" response header.
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Hosts like Render/Railway inject the listening port via the PORT env var.
// Bind to it when present; otherwise fall back to the local launch settings.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container. The /Admin folder requires an authenticated
// user; the login page itself is anonymous.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AllowAnonymousToPage("/Admin/Login");
});
builder.Services.AddSingleton<CvService>();
builder.Services.AddSingleton<ContactStore>();
builder.Services.AddSingleton<AdminAuth>();
builder.Services.AddSingleton<CvPdfGenerator>();
builder.Services.AddMemoryCache(); // backs the contact-form + login rate limiters

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.Name = "cv_admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

// Behind Render's proxy the real client IP arrives in X-Forwarded-For.
// Honour it so the contact-form rate limiter partitions by real visitor.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Security response headers (CSP, anti-clickjacking, no-sniff, etc.).
app.UseMiddleware<SecurityHeadersMiddleware>();

// In production we run behind a TLS-terminating proxy (Render), so HTTPS
// redirection is handled at the edge — only enforce it locally.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Download the CV as a generated PDF.
app.MapGet("/cv.pdf", (CvService cv, CvPdfGenerator pdf) =>
{
    var data = cv.Get();
    var bytes = pdf.Generate(data);
    var safeName = new string((data.Profile.Name ?? "cv")
        .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
    return Results.File(bytes, "application/pdf", $"{safeName}_CV.pdf");
});

app.Run();
