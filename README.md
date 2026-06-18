# Redas Baltrėnas — CV / Portfolio

A personal CV website built with **ASP.NET Core 8 (Razor Pages)** — designed to run in **JetBrains Rider**.

## Run it

**In Rider:** open `CV.csproj`, pick the `https` (or `http`) run configuration, press ▶.

**From the terminal:**
```bash
dotnet run
```
Then open the URL printed in the console (e.g. `https://localhost:7185`).

## What's inside

| Layer | File(s) |
|-------|---------|
| Content (data-driven) | `Data/cv.json` |
| Domain models | `Models/CvData.cs`, `Models/ContactMessage.cs` |
| Services (DI singletons) | `Services/CvService.cs` (loads JSON), `Services/ContactStore.cs` (persists messages) |
| Page + backend handler | `Pages/Index.cshtml`, `Pages/Index.cshtml.cs` |
| Layout / theme bootstrap | `Pages/Shared/_Layout.cshtml` |
| Styling (design system) | `wwwroot/css/site.css` |
| Interactivity | `wwwroot/js/site.js` |

## Features

- **Single-page portfolio**: hero, about, experience timeline, projects, animated skill bars, education, contact.
- **Data-driven**: all CV content lives in `Data/cv.json`. Edit it and the site updates — no markup changes needed.
- **Working contact form**: real server-side POST handler with model validation (`[Required]`, `[EmailAddress]`, length rules) and client-side unobtrusive validation. Submissions are appended to `App_Data/messages.jsonl`.
- **Dark / light theme** toggle, persisted in `localStorage`.
- **Responsive** layout with a mobile nav, scroll-reveal animations, and `prefers-reduced-motion` support.

## Updating your CV

Edit `Data/cv.json`:
- `experience[]` / `projects[]` / `education[]` — add or reorder entries.
- `skills[]` / `languages[]` — `level` is `1–5` (rendered as a bar at `level × 20%`).
- Set `"upcoming": true` on a job to show the "Upcoming" badge.

## Where messages go

`Services/ContactStore.cs` appends each submission to `App_Data/messages.jsonl`.
In production you'd swap that for sending an email or pushing to a queue — the
service is the single place to change.
