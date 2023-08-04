using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages
{
    [AllowAnonymous]
    [ExcludeFromCodeCoverage]
    public class LogoutBannerModel : PageModel
    {
        private readonly IConfiguration _config;

        public LogoutBannerModel(IConfiguration config)
        {
            _config = config;
        }
        public void OnGet()
        {
            Response.Redirect(_config.GetValue<string>("ExternalLinks:StartNowPage"));
        }
    }
}
