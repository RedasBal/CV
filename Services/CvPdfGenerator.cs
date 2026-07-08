using CV.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CV.Services;

/// <summary>
/// Renders <see cref="CvData"/> to a print-quality PDF using QuestPDF.
/// The layout mirrors the website so the download always matches the live CV.
/// </summary>
public class CvPdfGenerator
{
    private const string Accent = "#6366F1";
    private const string Ink = "#1A1D26";
    private const string Muted = "#5B6472";
    private const string Track = "#E4E7EF";

    public byte[] Generate(CvData d)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.4f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Ink).LineHeight(1.35f));

                page.Header().Element(c => Header(c, d.Profile));
                page.Content().PaddingTop(14).Element(c => Body(c, d));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span($"{d.Profile.Name}  ·  ").FontSize(8).FontColor(Muted);
                    t.CurrentPageNumber().FontSize(8).FontColor(Muted);
                    t.Span(" / ").FontSize(8).FontColor(Muted);
                    t.TotalPages().FontSize(8).FontColor(Muted);
                });
            });
        });

        return doc.GeneratePdf();
    }

    private static void Header(IContainer c, Profile p)
    {
        c.Column(col =>
        {
            col.Item().Text(p.Name).FontSize(24).Bold().FontColor(Ink);
            col.Item().Text(p.Title).FontSize(13).Bold().FontColor(Accent);

            var parts = new[] { p.Location, p.Phone, p.Email, p.GitHub }
                .Where(s => !string.IsNullOrWhiteSpace(s));
            col.Item().PaddingTop(4).Text(string.Join("   ·   ", parts))
                .FontSize(9).FontColor(Muted);

            col.Item().PaddingTop(8).LineHorizontal(1.2f).LineColor(Accent);
        });
    }

    private void Body(IContainer c, CvData d)
    {
        c.Column(col =>
        {
            col.Spacing(16);

            if (!string.IsNullOrWhiteSpace(d.Profile.Summary))
            {
                col.Item().Column(s =>
                {
                    SectionTitle(s.Item(), "Profile");
                    s.Item().Text(d.Profile.Summary).FontColor(Muted);
                });
            }

            if (d.Experience.Count > 0)
            {
                col.Item().Column(s =>
                {
                    SectionTitle(s.Item(), "Experience");
                    foreach (var e in d.Experience)
                    {
                        s.Item().PaddingBottom(8).Element(x => Experience(x, e));
                    }
                });
            }

            if (d.Projects.Count > 0)
            {
                col.Item().Column(s =>
                {
                    SectionTitle(s.Item(), "Projects");
                    foreach (var p in d.Projects)
                    {
                        s.Item().PaddingBottom(8).Element(x => Project(x, p));
                    }
                });
            }

            if (d.Skills.Count > 0 || d.Languages.Count > 0)
            {
                col.Item().Row(row =>
                {
                    row.Spacing(24);
                    if (d.Skills.Count > 0)
                        row.RelativeItem().Element(x => Levels(x, "Skills", d.Skills));
                    if (d.Languages.Count > 0)
                        row.RelativeItem().Element(x => Levels(x, "Languages", d.Languages));
                });
            }

            if (d.Education.Count > 0)
            {
                col.Item().Column(s =>
                {
                    SectionTitle(s.Item(), "Education & Courses");
                    foreach (var e in d.Education)
                    {
                        s.Item().PaddingBottom(6).Element(x => Education(x, e));
                    }
                });
            }
        });
    }

    private static void SectionTitle(IContainer c, string text) =>
        c.PaddingBottom(6).Text(text.ToUpper()).FontSize(11).Bold()
            .FontColor(Accent).LetterSpacing(0.06f);

    private static void Experience(IContainer c, ExperienceItem e)
    {
        c.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span(e.Role).Bold().FontSize(11);
                    if (!string.IsNullOrWhiteSpace(e.Company))
                        t.Span($"  ·  {e.Company}").FontColor(Muted);
                    if (e.Upcoming)
                        t.Span("   (Upcoming)").FontColor(Accent).FontSize(8).Bold();
                });
                row.ConstantItem(110).AlignRight().Text(e.Period).FontSize(9).FontColor(Muted);
            });
            foreach (var h in e.Highlights)
            {
                col.Item().Row(r =>
                {
                    r.ConstantItem(12).Text("•").FontColor(Accent);
                    r.RelativeItem().Text(h).FontColor(Muted).FontSize(9.5f);
                });
            }
        });
    }

    private static void Project(IContainer c, ProjectItem p)
    {
        c.Column(col =>
        {
            col.Item().Text(t =>
            {
                t.Span(p.Name).Bold().FontSize(11);
                if (p.Tech.Count > 0)
                    t.Span($"   {string.Join(" · ", p.Tech)}").FontColor(Accent).FontSize(8.5f);
            });
            if (!string.IsNullOrWhiteSpace(p.Description))
                col.Item().Text(p.Description).FontColor(Muted).FontSize(9.5f);
            if (!string.IsNullOrWhiteSpace(p.Url))
                col.Item().Text(p.Url).FontColor(Accent).FontSize(9);
        });
    }

    private static void Education(IContainer c, EducationItem e)
    {
        c.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span(e.Title).Bold().FontSize(10.5f);
                    if (!string.IsNullOrWhiteSpace(e.Institution))
                        t.Span($"  ·  {e.Institution}").FontColor(Muted);
                });
                row.ConstantItem(90).AlignRight().Text(e.Period).FontSize(9).FontColor(Muted);
            });
            if (!string.IsNullOrWhiteSpace(e.Note))
                col.Item().Text(e.Note).FontColor(Muted).FontSize(9);
        });
    }

    private void Levels(IContainer c, string title, List<Skill> items)
    {
        c.Column(col =>
        {
            SectionTitle(col.Item(), title);
            foreach (var s in items)
            {
                col.Item().PaddingBottom(4).Row(row =>
                {
                    row.RelativeItem().Text(s.Name).FontSize(9.5f);
                    row.ConstantItem(78).AlignRight().Element(x => LevelDots(x, s.Level));
                });
            }
        });
    }

    private void LevelDots(IContainer c, int level)
    {
        c.AlignRight().Row(row =>
        {
            row.Spacing(3);
            for (var i = 1; i <= 5; i++)
            {
                row.ConstantItem(12).Height(5)
                    .Background(i <= level ? Accent : Track);
            }
        });
    }
}
