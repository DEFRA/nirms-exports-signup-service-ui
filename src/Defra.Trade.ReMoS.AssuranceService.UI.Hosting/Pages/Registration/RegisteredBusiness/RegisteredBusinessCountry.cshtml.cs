using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessCountryModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [Required(ErrorMessage = "Enter a country")]
    public string Country { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<RegisteredBusinessCountryModel> _logger;

    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="logger">Application logging.</param>
    /// <exception cref="ArgumentNullException">Guard statement reaction.</exception>
    public RegisteredBusinessCountryModel(ILogger<RegisteredBusinessCountryModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    //Remove warning when API integration added (has to be async for OnPost functionality but throws this error)
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Country OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        return Redirect(Routes.RegistrationTasklist);
    }
}
