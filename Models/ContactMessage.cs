using System.ComponentModel.DataAnnotations;

namespace CV.Models;

/// <summary>Payload bound from the contact form and validated server-side.</summary>
public class ContactMessage
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "That doesn't look like a valid email.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please enter a message.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message should be at least 10 characters.")]
    public string Message { get; set; } = "";
}
