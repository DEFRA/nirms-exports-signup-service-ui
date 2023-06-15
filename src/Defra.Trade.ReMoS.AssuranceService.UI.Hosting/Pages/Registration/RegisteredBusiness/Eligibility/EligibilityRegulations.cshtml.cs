using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class EligibilityRegulationsModel : PageModel
{
    [BindProperty]
    public bool Confirmed { get; set; }
    [BindProperty]
    public Guid TraderId { get; set; }
    private readonly ILogger<EligibilityRegulationsModel> _logger;

    public EligibilityRegulationsModel(ILogger<EligibilityRegulationsModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TraderId = id;
        _logger.LogInformation("Eligibility Regulations OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Eligibility Regulations On Post Submit");
        if (!Confirmed)
        {
            
            ModelState.AddModelError(nameof(Confirmed), "Confirm that you have understood the guidance and regulations");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Eligibility Regulations Model validation failed");
            return await OnGetAsync(TraderId);
        }

        return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = TraderId });
    }
}
