using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessAddressModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter address line 1 using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Address line 1 must be 100 characters or less")]
    [Required(ErrorMessage = "Enter address line 1.")]
    public string LineOne { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter address line 2 using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Address line 2 must be 100 characters or less")]
    public string? LineTwo { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a town or city using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Town or city must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a town or city.")]
    public string CityName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Enter a real postcode.")]
    [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a post code.")]
    public string PostCode { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<RegisteredBusinessAddressModel> _logger;
    private readonly ITraderService _traderService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public RegisteredBusinessAddressModel(
        ILogger<RegisteredBusinessAddressModel> logger,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService;
    }

    //Remove warning when API integration added (has to be async for OnPost functionality but throws this error)
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Address OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        TradeAddressAddUpdateDTO addressDto = CreateAddressDto();

        var partyId = Guid.Parse("82c5f034-b931-4964-bcc9-08db554a682e"); //TODO - get id of an existing party or create a new trade party
        await _traderService.AddTradeAddressForPartyAsync(partyId, addressDto);

        return Redirect(Routes.RegistrationTasklist);
    }

    private TradeAddressAddUpdateDTO CreateAddressDto()
    {
        TradeAddressAddUpdateDTO DTO = new()
        {
            LineOne = LineOne,
            LineTwo = LineTwo,
            CityName = CityName,
            PostCode = PostCode,
        };
        return DTO;
    }
}
