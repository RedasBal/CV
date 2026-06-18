using CV.Models;
using CV.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CV.Pages;

public class IndexModel : PageModel
{
    private readonly CvService _cv;
    private readonly ContactStore _contacts;

    public IndexModel(CvService cv, ContactStore contacts)
    {
        _cv = cv;
        _contacts = contacts;
    }

    public CvData Data { get; private set; } = new();

    [BindProperty]
    public ContactMessage Contact { get; set; } = new();

    public bool MessageSent { get; private set; }

    public void OnGet()
    {
        Data = _cv.Get();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Data = _cv.Get();

        if (!ModelState.IsValid)
        {
            // Re-render the page with validation errors, scrolling to the form.
            return Page();
        }

        await _contacts.SaveAsync(Contact);
        MessageSent = true;
        Contact = new ContactMessage();
        ModelState.Clear();
        return Page();
    }
}
