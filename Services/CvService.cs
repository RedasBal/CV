using System.Text.Json;
using CV.Models;

namespace CV.Services;

/// <summary>
/// Loads, caches and persists the CV content in <c>Data/cv.json</c>.
/// Registered as a singleton; the in-memory copy is updated whenever the
/// admin editor saves, so changes show up immediately without a restart.
/// </summary>
public class CvService
{
    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // Keep human-readable characters (e.g. ė, —) unescaped in the file.
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly string _path;
    private readonly object _lock = new();
    private CvData _data;

    public CvService(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "Data", "cv.json");
        _data = Load();
    }

    private CvData Load()
    {
        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<CvData>(json, ReadOptions)
               ?? throw new InvalidOperationException("cv.json could not be parsed.");
    }

    public CvData Get()
    {
        lock (_lock)
        {
            return _data;
        }
    }

    /// <summary>Persists the supplied data to disk and refreshes the cache.</summary>
    public void Save(CvData data)
    {
        var json = JsonSerializer.Serialize(data, WriteOptions);
        lock (_lock)
        {
            File.WriteAllText(_path, json);
            _data = data;
        }
    }

    /// <summary>Serialises the current data as indented JSON (used by export).</summary>
    public string ToJson() => JsonSerializer.Serialize(Get(), WriteOptions);
}
