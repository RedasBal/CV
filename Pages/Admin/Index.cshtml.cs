using System.Text;
using CV.Models;
using CV.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CV.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly CvService _cv;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(CvService cv, ILogger<IndexModel> logger)
    {
        _cv = cv;
        _logger = logger;
    }

    [BindProperty]
    public CvEditModel Input { get; set; } = new();

    public bool Saved { get; private set; }

    public void OnGet()
    {
        Input = CvEditModel.FromCvData(_cv.Get());
    }

    public IActionResult OnPostSave()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _cv.Save(Input.ToCvData());
        _logger.LogInformation("CV content updated by {User}", User.Identity?.Name);

        // Reload from the canonical saved data so the form reflects normalisation.
        Input = CvEditModel.FromCvData(_cv.Get());
        Saved = true;
        return Page();
    }

    public IActionResult OnGetExport()
    {
        var bytes = Encoding.UTF8.GetBytes(_cv.ToJson());
        return File(bytes, "application/json", "cv.json");
    }

    public async Task<IActionResult> OnPostLogout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Admin/Login");
    }
}
