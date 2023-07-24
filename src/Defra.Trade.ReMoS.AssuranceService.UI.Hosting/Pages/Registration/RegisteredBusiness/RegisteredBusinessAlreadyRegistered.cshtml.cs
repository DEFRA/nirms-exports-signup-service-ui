using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessAlreadyRegisteredModel : PageModel
{
    [BindProperty]
    public Guid TraderId { get; set; }

    private readonly ILogger<RegisteredBusinessAlreadyRegisteredModel> _logger;
    public RegisteredBusinessAlreadyRegisteredModel(ILogger<RegisteredBusinessAlreadyRegisteredModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public IActionResult OnGetAsync(Guid id)
    {
        _logger.LogInformation("Already registered OnGet");
        TraderId = id;

        return Page();
    }
}
