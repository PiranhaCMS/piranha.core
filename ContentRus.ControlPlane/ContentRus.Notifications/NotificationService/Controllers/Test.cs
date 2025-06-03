using Microsoft.AspNetCore.Mvc;
using Notifications.Services;

[ApiController]
[Route("api")]
public class TestEmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public TestEmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    // [HttpGet("send")]
    // public async Task<IActionResult> SendTestEmail()
    // {
    //     //Put the email to witch you want to send(first argument)
    //     await _emailService.SendEmailAsync("", "Welcome to ContentRus", "<h1>Your ContenRus account is ready to be used.</h1><p>Sent from ContentRus.</p>");
    //     return Ok("Email sent (if config is correct)");
    // }
}
