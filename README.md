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
- **PDF download** at `/cv.pdf` — a print-quality PDF generated server-side from `cv.json` with QuestPDF, so it always matches the live site.
- **Dark / light theme** toggle, persisted in `localStorage`.
- **Responsive** layout with a mobile nav, scroll-reveal animations, and `prefers-reduced-motion` support.

## Updating your CV

**Easiest: the admin editor at `/Admin`** — a browser form for every field, with
add/remove for experience, projects, skills, etc. Saving writes `Data/cv.json`
and the live site updates immediately.

Or edit `Data/cv.json` directly:
- `experience[]` / `projects[]` / `education[]` — add or reorder entries.
- `skills[]` / `languages[]` — `level` is `1–5` (rendered as a bar at `level × 20%`).
- Set `"upcoming": true` on a job to show the "Upcoming" badge.

## Admin area (`/Admin`)

Authenticated, single-user editor. **Fail-closed**: disabled unless a password
is configured.

| Setting | How |
|---------|-----|
| Enable it | Set the **`ADMIN_PASSWORD`** environment variable (on Render: dashboard → Environment). Optionally `ADMIN_USERNAME` (defaults to `admin`). |
| Local dev | If `ADMIN_PASSWORD` is unset, Development uses `admin` / `admin` (logged as a warning). |

Security: cookie auth (HttpOnly, SameSite=Strict, Secure in prod), antiforgery
on every form, constant-time credential check, and login lockout after 5 failed
attempts per IP / 15 min.

> ⚠️ **Render free tier has an ephemeral disk.** Admin edits persist until the
> instance restarts/redeploys, then revert to the committed `cv.json`. To make a
> change permanent, use the **Export JSON** button in the admin, replace
> `Data/cv.json` in the repo, and `git push` (which also redeploys). For
> always-persistent editing, move the data to a database.

## Where messages go

Every contact submission is:
1. appended to `App_Data/messages.jsonl` (`Services/ContactStore.cs`), and
2. emailed to you (`Services/EmailSender.cs`) — **if** SMTP is configured.

### Enabling email notifications

The recipient (`Email:To`) defaults to `redas.baltrenas@gmail.com` in
`appsettings.json`. Credentials are **never** committed — set them as
environment variables.

**Gmail (recommended):**
1. Enable **2-Step Verification** on your Google account.
2. Create an **App Password**: Google Account → Security → App passwords → pick
   "Mail" → copy the 16-character password.
3. Set these environment variables (on Render: service → **Environment**):

   | Variable | Value |
   |----------|-------|
   | `Email__SmtpUser` | `redas.baltrenas@gmail.com` |
   | `Email__SmtpPassword` | *(the 16-char app password)* |

   `SmtpHost` (`smtp.gmail.com`), `SmtpPort` (`587`) and `To` already default in
   `appsettings.json`. Override any of them with `Email__SmtpHost`, etc.

4. Save → redeploy. Submissions now arrive in your inbox, with the visitor's
   address as **Reply-To** so you can reply directly.

> Locally, set the same variables (Rider run config or user-secrets). If SMTP is
> left unset, email is simply skipped and messages are still saved to the file.
> The double underscore `__` maps to the `Email:` config section — that's the
> .NET convention for nested keys in environment variables.
