using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessNatureOfBusinessModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "error message")]
    public string NatureOfBusiness { get; set; } = string.Empty;

    private readonly ILogger<RegisteredBusinessNatureOfBusinessModel> _logger;

    public RegisteredBusinessNatureOfBusinessModel(ILogger<RegisteredBusinessNatureOfBusinessModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Nature of business OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Nature of business OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        return await OnGetAsync();
    }
}
