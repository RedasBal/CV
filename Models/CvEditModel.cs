using System.ComponentModel.DataAnnotations;

namespace CV.Models;

/// <summary>
/// Editable, form-friendly projection of <see cref="CvData"/>. List-valued
/// fields (highlights, tech) are flattened to multi-line / comma strings so
/// they bind to plain textareas, then mapped back on save.
/// </summary>
public class CvEditModel
{
    public ProfileInput Profile { get; set; } = new();
    public List<ExperienceInput> Experience { get; set; } = [];
    public List<ProjectInput> Projects { get; set; } = [];
    public List<SkillInput> Skills { get; set; } = [];
    public List<SkillInput> Languages { get; set; } = [];
    public List<EducationInput> Education { get; set; } = [];

    public static CvEditModel FromCvData(CvData d) => new()
    {
        Profile = new ProfileInput
        {
            Name = d.Profile.Name,
            Title = d.Profile.Title,
            Summary = d.Profile.Summary,
            Location = d.Profile.Location,
            Phone = d.Profile.Phone,
            Email = d.Profile.Email,
            GitHub = d.Profile.GitHub,
            GitHubUrl = d.Profile.GitHubUrl,
            Age = d.Profile.Age,
            Studies = d.Profile.Studies
        },
        Experience = d.Experience.Select(e => new ExperienceInput
        {
            Period = e.Period, Role = e.Role, Company = e.Company,
            Upcoming = e.Upcoming, Highlights = string.Join("\n", e.Highlights)
        }).ToList(),
        Projects = d.Projects.Select(p => new ProjectInput
        {
            Name = p.Name, Category = p.Category, Description = p.Description,
            Tech = string.Join(", ", p.Tech), Url = p.Url
        }).ToList(),
        Skills = d.Skills.Select(s => new SkillInput { Name = s.Name, Level = s.Level }).ToList(),
        Languages = d.Languages.Select(s => new SkillInput { Name = s.Name, Level = s.Level }).ToList(),
        Education = d.Education.Select(e => new EducationInput
        {
            Period = e.Period, Title = e.Title, Institution = e.Institution, Note = e.Note
        }).ToList()
    };

    public CvData ToCvData() => new()
    {
        Profile = new Profile
        {
            Name = Profile.Name?.Trim() ?? "",
            Title = Profile.Title?.Trim() ?? "",
            Summary = Profile.Summary?.Trim() ?? "",
            Location = Profile.Location?.Trim() ?? "",
            Phone = Profile.Phone?.Trim() ?? "",
            Email = Profile.Email?.Trim() ?? "",
            GitHub = Profile.GitHub?.Trim() ?? "",
            GitHubUrl = Profile.GitHubUrl?.Trim() ?? "",
            Age = Profile.Age,
            Studies = Profile.Studies?.Trim() ?? ""
        },
        Experience = Experience.Where(e => !string.IsNullOrWhiteSpace(e.Role)).Select(e => new ExperienceItem
        {
            Period = e.Period?.Trim() ?? "", Role = e.Role!.Trim(), Company = e.Company?.Trim() ?? "",
            Upcoming = e.Upcoming, Highlights = SplitLines(e.Highlights)
        }).ToList(),
        Projects = Projects.Where(p => !string.IsNullOrWhiteSpace(p.Name)).Select(p => new ProjectItem
        {
            Name = p.Name!.Trim(), Category = p.Category?.Trim() ?? "", Description = p.Description?.Trim() ?? "",
            Tech = SplitCsv(p.Tech), Url = string.IsNullOrWhiteSpace(p.Url) ? null : p.Url.Trim()
        }).ToList(),
        Skills = Skills.Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .Select(s => new Skill { Name = s.Name!.Trim(), Level = Math.Clamp(s.Level, 1, 5) }).ToList(),
        Languages = Languages.Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .Select(s => new Skill { Name = s.Name!.Trim(), Level = Math.Clamp(s.Level, 1, 5) }).ToList(),
        Education = Education.Where(e => !string.IsNullOrWhiteSpace(e.Title)).Select(e => new EducationItem
        {
            Period = e.Period?.Trim() ?? "", Title = e.Title!.Trim(),
            Institution = e.Institution?.Trim() ?? "", Note = e.Note?.Trim() ?? ""
        }).ToList()
    };

    private static List<string> SplitLines(string? s) =>
        (s ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private static List<string> SplitCsv(string? s) =>
        (s ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}

public class ProfileInput
{
    [Required, StringLength(120)] public string? Name { get; set; }
    [Required, StringLength(160)] public string? Title { get; set; }
    [StringLength(2000)] public string? Summary { get; set; }
    [StringLength(120)] public string? Location { get; set; }
    [StringLength(60)] public string? Phone { get; set; }
    [EmailAddress, StringLength(160)] public string? Email { get; set; }
    [StringLength(160)] public string? GitHub { get; set; }
    [StringLength(300)] public string? GitHubUrl { get; set; }
    [Range(0, 120)] public int Age { get; set; }
    [StringLength(200)] public string? Studies { get; set; }
}

public class ExperienceInput
{
    [StringLength(60)] public string? Period { get; set; }
    [StringLength(120)] public string? Role { get; set; }
    [StringLength(160)] public string? Company { get; set; }
    public bool Upcoming { get; set; }
    [StringLength(4000)] public string? Highlights { get; set; }
}

public class ProjectInput
{
    [StringLength(160)] public string? Name { get; set; }
    [StringLength(80)] public string? Category { get; set; }
    [StringLength(2000)] public string? Description { get; set; }
    [StringLength(300)] public string? Tech { get; set; }
    [StringLength(300)] public string? Url { get; set; }
}

public class SkillInput
{
    [StringLength(80)] public string? Name { get; set; }
    [Range(1, 5)] public int Level { get; set; } = 3;
}

public class EducationInput
{
    [StringLength(60)] public string? Period { get; set; }
    [StringLength(160)] public string? Title { get; set; }
    [StringLength(200)] public string? Institution { get; set; }
    [StringLength(400)] public string? Note { get; set; }
}
