using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;

public class AuthorisedSignatoryNameModel : PageModel
{
    #region ui model
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z\s-']*$", ErrorMessage = "Enter a name using only letters, apostrophes or hyphens")]
    [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a name")]
    public string Name { get; set; } = string.Empty;
    [BindProperty]
    public string? BusinessName { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid SignatoryId { get; set; }
    #endregion

    private readonly ITraderService _traderService;
    private readonly ILogger<AuthorisedSignatoryNameModel> _logger;

    public AuthorisedSignatoryNameModel(ITraderService traderService, ILogger<AuthorisedSignatoryNameModel> logger)
    {
        _traderService = traderService;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        _logger.LogInformation("Name OnGet");

        var party = await GetSignatoryNameFromApiAsync();
        BusinessName = party?.PracticeName;

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Signatory Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitName();
        return RedirectToPage(
            Routes.Pages.Path.AuthorisedSignatoryPositionPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Signatory Name OnPostSubmit");

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
