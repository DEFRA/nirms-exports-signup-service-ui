using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    [ExcludeFromCodeCoverage]
    public class RegisteredBusinessNameModel : PageModel
    {
        [BindProperty]
        public string? Name { get; set; }
        public void OnGet()
        {
        }
    }
}
