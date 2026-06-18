using CV.Services;

var builder = WebApplication.CreateBuilder(args);

// Hosts like Render/Railway inject the listening port via the PORT env var.
// Bind to it when present; otherwise fall back to the local launch settings.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<CvService>();
builder.Services.AddSingleton<ContactStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// In production we run behind a TLS-terminating proxy (Render), so HTTPS
// redirection is handled at the edge — only enforce it locally.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
