using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessNatureOfBusinessModel : PageModel
{
    #region UI Model
    [BindProperty]
    [Required(ErrorMessage = "error message")]
    public string? NatureOfBusiness { get; set; } = string.Empty;

    [BindProperty]
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessNatureOfBusinessModel> _logger;
    private readonly ITraderService _traderService;

    public RegisteredBusinessNatureOfBusinessModel(ILogger<RegisteredBusinessNatureOfBusinessModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)

    {
        TraderId = id;
        _logger.LogInformation("Nature of business OnGet");

        await GetNatureOfBusiness();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Nature of business OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        TradePartyDTO dto = CreateDTO();
        await _traderService.UpdateTradePartyAsync(dto);

        return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = TraderId });
    }

    private TradePartyDTO CreateDTO()
    {
        TradePartyDTO dto = new()
        {
            Id = TraderId,
            NatureOfBusiness = this.NatureOfBusiness
        };
        return dto;
    }

    private async Task GetNatureOfBusiness()
    {
        TradePartyDTO? tp = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tp != null)
            NatureOfBusiness = tp.NatureOfBusiness;
    }
}