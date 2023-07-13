using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessBusinessPickerModel : PageModel
{
    #region model properties
    [BindProperty]
    [Required(ErrorMessage = "Select a business")]
    public string Business { get; set; } = string.Empty;
    public string[] Businesses { get; set; } = Array.Empty<string>();

    [BindProperty]
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessBusinessPickerModel> _logger;

    public RegisteredBusinessBusinessPickerModel(ILogger<RegisteredBusinessBusinessPickerModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("Business picker OnGet");
        TraderId = Id;

        Businesses = await GetDefraBusinessesForUserAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Business picker OnPostSubmit");

        if (string.Equals(Business, "Another business", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("UnregisteredBusiness", "UnregisteredBusiness");
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessCountryPath,
            new { id = TraderId });
    }

    private static async Task<string[]> GetDefraBusinessesForUserAsync()
    {
        return await Task.FromResult( new[] { "ACME Ltd", "ACME2 Ltd", "AMCE3", "AMCE4"});
    }
}
