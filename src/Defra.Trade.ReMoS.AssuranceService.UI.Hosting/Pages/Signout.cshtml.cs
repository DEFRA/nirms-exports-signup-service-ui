using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages
{
    [AllowAnonymous]
    public class SignoutModel : PageModel
    {
        private readonly ILogger _logger;
        private readonly IOptions<EhcoIntegration> _ehcoIntegrationSettings;

        public SignoutModel(ILogger<SignoutModel> logger, IOptions<EhcoIntegration> ehcoIntegrationSettings)
        {
            _logger = logger;
            _ehcoIntegrationSettings = ehcoIntegrationSettings;
        }

        public async Task<IActionResult> OnGet()
        {
            _logger.LogInformation("User {Name} logged out at {Time}.",
                User.Identity!.Name, DateTime.UtcNow);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
            
            var correlationId = Guid.NewGuid();

            Response.Redirect(_ehcoIntegrationSettings.Value.ValidIssuer + "/b2c/remos_signup/correlationId/" + correlationId + "/logout");

            //Response.Redirect("https://www.gov.uk/export-health-certificates/general-certificate-for-moving-goods-under-the-ni-retail-movement-scheme");

            return Page();
        }
    }
}
