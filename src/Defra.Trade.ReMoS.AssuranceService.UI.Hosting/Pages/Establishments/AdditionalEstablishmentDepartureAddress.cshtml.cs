using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.TagHelpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class AdditionalEstablishmentDepartureAddressModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [Required(ErrorMessage = "Select yes if you want to add another point of departure")]
    public string AdditionalAddress { get; set; } = string.Empty;

    [BindProperty]
    public List<LogisticsLocation>? LogisticsLocations { get; } = new List<LogisticsLocation>();

    [BindProperty]
    public Guid TradePartyId { get; set; }
    #endregion

    private readonly ILogger<AdditionalEstablishmentDepartureAddressModel> _logger;

    public AdditionalEstablishmentDepartureAddressModel(
        ILogger<AdditionalEstablishmentDepartureAddressModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Additional establishment manual address OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Additional establishment manual address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        if (AdditionalAddress == "yes")
        {
            return RedirectToPage(Routes.Pages.Path.EstablishmentDeparturePostcodeSearchPath);
        }
        else return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath);
    }
}
