using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessContactNameModel : BasePageModel<RegisteredBusinessContactNameModel>
{
   
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z\s-']*$", ErrorMessage = "Enter a name using only letters, hyphens or apostrophes")]
    [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a name")]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid ContactId { get; set; }
    public bool? IsAuthorisedSignatory { get; set; }
    public Guid AuthorisedSignatoryId { get; set; }
    [BindProperty]
    public string? PracticeName { get; set; }
    public TradePartyDto TradePartyDto { get; set; } = new TradePartyDto();
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RegisteredBusinessContactNameModel(
        ILogger<RegisteredBusinessContactNameModel> logger,
        ITraderService traderService) : base(logger, traderService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (_traderService.IsTradePartySignedUp(id).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        _logger.LogInformation("Name OnGet");

        await GetTradePartyFromApiAsync();

        return Page();

    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Contact Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitName();

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactPositionPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Contact Name OnPostSave");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitName();

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TradePartyId });
    }

    #region private methods
    private async Task SubmitName()
    {
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDto = GenerateDTO();
        await _traderService.UpdateTradePartyContactAsync(TradePartyDto);
    }

    private async Task GetTradePartyFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        PracticeName = tradeParty?.PracticeName ?? string.Empty;

        if (tradeParty != null && tradeParty.Contact != null)
        {
            Name = tradeParty.Contact.PersonName ?? string.Empty;
        }
    }

    private async Task GetIsAuthorisedSignatoryFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            IsAuthorisedSignatory = tradeParty.Contact.IsAuthorisedSignatory;
            if (tradeParty!.AuthorisedSignatory != null)
            {
                AuthorisedSignatoryId = tradeParty.AuthorisedSignatory.Id;
            }
        }
    }

    private TradePartyDto GenerateDTO()
    {
        var tradePartyDto = new TradePartyDto()
        {
            Id = TradePartyId,
            Contact = new TradeContactDto()
            {
                Id = ContactId,
                PersonName = Name,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };

        if (IsAuthorisedSignatory == true)
        {
            tradePartyDto.AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = AuthorisedSignatoryId,
                Name = tradePartyDto.Contact.PersonName
            };
        }

        return tradePartyDto;
    }
    #endregion
}
