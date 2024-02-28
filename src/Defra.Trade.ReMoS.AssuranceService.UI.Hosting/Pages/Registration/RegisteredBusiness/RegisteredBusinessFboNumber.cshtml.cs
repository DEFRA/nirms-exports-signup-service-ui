using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessFboNumberModel : BasePageModel<RegisteredBusinessFboNumberModel>
{
    #region props and ctor

    [BindProperty]
    //[Required(ErrorMessage = "Select if your business has an FBO or PHR number")]
    public string? OptionSelected { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9-]*$", ErrorMessage = "Enter an FBO number using only letters, numbers or hyphens")]
    [StringLengthMaximum(25, ErrorMessage = "FBO number must be 25 characters or less")]
    public string? FboNumber { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-]*$", ErrorMessage = "Enter a PHR number using only letters, numbers, spaces or hyphens")]
    [StringLengthMaximum(25, ErrorMessage = "PHR number must be 25 characters or less")]
    public string? PhrNumber { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    [BindProperty]
    public string? PracticeName { get; set; } = string.Empty;

    public RegisteredBusinessFboNumberModel(
        ILogger<RegisteredBusinessFboNumberModel> logger,
        ITraderService traderService) : base(logger, traderService)
    { }

    #endregion props and ctor

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("FBO Number OnGet");
        OrgId = Id;
        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        await PopulateModelProperties(TradePartyId);

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("FBO OnPostSubmit");
        ValidateFboPhr();

        if (!ModelState.IsValid && OptionSelected != "none")
        {
            return await OnGetAsync(OrgId);
        }

        await SaveNumberToApiAsync(TradePartyId);

        if (OptionSelected == "none")
        {
            return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessFboPhrGuidancePath,
                new { id = OrgId });
        }
        else
        {
            return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessContactNamePath,
                new { id = OrgId });
        }

    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("FBO OnPostSave");
        ValidateFboPhr();

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SaveNumberToApiAsync(TradePartyId);

        if (OptionSelected == "none")
        {
            return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessFboPhrGuidancePath,
                new { id = OrgId });
        }
        else
        {
            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = OrgId });
        }
    }

    private void ValidateFboPhr()
    {
        if (ModelState.IsValid)
        {
            if (OptionSelected == "fbo" && string.IsNullOrEmpty(FboNumber))
                ModelState.AddModelError(nameof(FboNumber), "Enter your business's FBO number");

            if (OptionSelected == "phr" && string.IsNullOrEmpty(PhrNumber))
                ModelState.AddModelError(nameof(PhrNumber), "Enter your business's PHR number");

            if (OptionSelected == "")
                ModelState.AddModelError(nameof(OptionSelected), $"Select if your business has an FBO or PHR number");
        }
    }

    private async Task PopulateModelProperties(Guid TradePartyId)
    {
        if (TradePartyId == Guid.Empty)
            throw new ArgumentNullException(nameof(TradePartyId));

        TradePartyDto? tradeParty = await GetNumberFromApiAsync();

        if (tradeParty != null)
        {
            FboNumber = tradeParty.FboNumber;
            PhrNumber = tradeParty.PhrNumber;
            OptionSelected = tradeParty.FboPhrOption;
            PracticeName = tradeParty.PracticeName;
        }
    }

    private async Task<TradePartyDto?> GetNumberFromApiAsync()
    {
        TradePartyDto? tradePartyDto = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        return tradePartyDto;

    }

    private async Task SaveNumberToApiAsync(Guid TradePartyId)
    {
        if (TradePartyId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(TradePartyId));
        }
        
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null)
        {
            tradeParty.FboNumber = FboNumber;
            tradeParty.PhrNumber = PhrNumber;
            tradeParty.FboPhrOption = OptionSelected;
            await _traderService.UpdateTradePartyAsync(tradeParty);
        }
    }
}
