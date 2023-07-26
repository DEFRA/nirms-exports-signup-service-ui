using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages
{
    [AllowAnonymous]
    public class logoutBannerModel : PageModel
    {
        public void OnGet()
        {
            Response.Redirect("https://www.gov.uk/export-health-certificates/general-certificate-for-moving-goods-under-the-ni-retail-movement-scheme");
        }
    }
}
