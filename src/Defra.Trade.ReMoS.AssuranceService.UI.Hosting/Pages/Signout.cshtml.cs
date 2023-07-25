using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages
{
    [AllowAnonymous]
    public class SignoutModel : PageModel
    {
        private readonly ILogger _logger;

        public SignoutModel(ILogger<SignoutModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            _logger.LogInformation("User {Name} logged out at {Time}.",
                User.Identity!.Name, DateTime.UtcNow);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);         

            Response.Redirect("https://www.gov.uk/export-health-certificates/general-certificate-for-moving-goods-under-the-ni-retail-movement-scheme");

            return Page();
        }
    }
}
