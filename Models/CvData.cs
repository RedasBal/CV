namespace CV.Models;

/// <summary>
/// Root object for all CV content. Deserialised from <c>Data/cv.json</c> so the
/// site is data-driven — update the JSON and the whole page updates.
/// </summary>
public class CvData
{
    public Profile Profile { get; set; } = new();
    public List<ExperienceItem> Experience { get; set; } = [];
    public List<ProjectItem> Projects { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
    public List<Skill> Languages { get; set; } = [];
    public List<EducationItem> Education { get; set; } = [];
}

public class Profile
{
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Location { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string GitHub { get; set; } = "";
    public string GitHubUrl { get; set; } = "";
    public int Age { get; set; }
    public string Studies { get; set; } = "";
}

public class ExperienceItem
{
    public string Period { get; set; } = "";
    public string Role { get; set; } = "";
    public string Company { get; set; } = "";
    public bool Upcoming { get; set; }
    public List<string> Highlights { get; set; } = [];
}

public class ProjectItem
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Tech { get; set; } = [];
    public string? Url { get; set; }
}

public class Skill
{
    public string Name { get; set; } = "";

    /// <summary>Proficiency from 1 to 5, rendered as a bar / dots.</summary>
    public int Level { get; set; }
}

public class EducationItem
{
    public string Period { get; set; } = "";
    public string Title { get; set; } = "";
    public string Institution { get; set; } = "";
    public string Note { get; set; } = "";
}
