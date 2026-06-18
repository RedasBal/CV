using System.Text.Json;
using CV.Models;

namespace CV.Services;

/// <summary>
/// Loads and caches the CV content from <c>Data/cv.json</c>. Registered as a
/// singleton — the file is read once on first access.
/// </summary>
public class CvService
{
    private readonly Lazy<CvData> _data;

    public CvService(IWebHostEnvironment env)
    {
        _data = new Lazy<CvData>(() =>
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "cv.json");
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<CvData>(json, options)
                   ?? throw new InvalidOperationException("cv.json could not be parsed.");
        });
    }

    public CvData Get() => _data.Value;
}
