using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;

public class AuthorisedSignatoryNameModel : BasePageModel<AuthorisedSignatoryNameModel>
{
    #region ui model
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z\s-']*$", ErrorMessage = "Enter a name using only letters, hyphens or apostrophes")]
    [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a name")]
    public string Name { get; set; } = string.Empty;
    [BindProperty]
    public string? BusinessName { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public Guid SignatoryId { get; set; }
    #endregion

    public AuthorisedSignatoryNameModel(
        ITraderService traderService, 
        ILogger<AuthorisedSignatoryNameModel> logger) : base(traderService, logger)
    {}
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        OrgId = id;
        TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(TradePartyId).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        _logger.LogInformation("Name OnGet");

        await GetSavedDataFromApi();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Signatory Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitName();
        return RedirectToPage(
            Routes.Pages.Path.AuthorisedSignatoryPositionPath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Signatory Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitName();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
    }

    #region private methods
    private async Task GetSavedDataFromApi()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        if (tradeParty == null)
            return;

        BusinessName = tradeParty.PracticeName;

        if (tradeParty.AuthorisedSignatory != null)
        {
            SignatoryId = tradeParty.AuthorisedSignatory.Id;
            Name = string.IsNullOrEmpty(Name) ? tradeParty.AuthorisedSignatory.Name ?? "" : Name;
        }
    }

    private async Task SubmitName()
    {
        var tradeParty = await GenerateDTO();
        await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);
    }

    private async Task <TradePartyDto?> GetSignatoryNameFromApiAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.AuthorisedSignatory!= null)
        {
            SignatoryId = tradeParty.AuthorisedSignatory.Id;
            Name = string.IsNullOrEmpty(Name) ? tradeParty.AuthorisedSignatory.Name ?? "" : Name;

            return tradeParty;
        }

        return null;
    }

    private async Task<TradePartyDto> GenerateDTO()
    {
        var tradeParty = await GetSignatoryNameFromApiAsync();
        return new TradePartyDto()
        {
            Id = TradePartyId,
            AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = SignatoryId,
                Name = Name,
                Position = tradeParty?.AuthorisedSignatory?.Position,
                EmailAddress = tradeParty?.AuthorisedSignatory?.EmailAddress
            }
        };
    }
    #endregion
}
