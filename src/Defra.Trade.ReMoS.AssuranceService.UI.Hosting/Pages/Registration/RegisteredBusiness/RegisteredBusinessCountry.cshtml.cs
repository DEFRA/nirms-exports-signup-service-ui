using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    [ExcludeFromCodeCoverage]
    public class RegisteredBusinessCountryModel : PageModel
    {
        [BindProperty]
        public string? Country { get; set; }
        public void OnGet()
        {
            Country = "United Kingdom";
        }
    }
}
