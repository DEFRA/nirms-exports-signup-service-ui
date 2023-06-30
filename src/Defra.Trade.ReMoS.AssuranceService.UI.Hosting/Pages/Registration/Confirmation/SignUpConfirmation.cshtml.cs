using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation
{
    public class SignUpConfirmationModel : PageModel
    {
        [BindProperty]
        public Guid TraderId { get; set; }                

        public IActionResult OnGet(Guid id)
        {
            TraderId = id;
            return Page();
        }
    }
}
