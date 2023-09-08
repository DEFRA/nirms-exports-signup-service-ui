using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

[ExcludeFromCodeCoverage]
public class RegisteredBusinessCanNotRegisterModel : PageModel
{
    [BindProperty]
    public Guid TraderId { get; set; }

    public IActionResult OnGet(Guid id)
    {
        TraderId = id;
        
        return Page();
    }
}
