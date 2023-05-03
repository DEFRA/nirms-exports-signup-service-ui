using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessNameModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [Required(ErrorMessage = "Enter a name.")]
    public string Name { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<RegisteredBusinessNameModel> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RegisteredBusinessNameModel(ILogger<RegisteredBusinessNameModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Name OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        return await OnGetAsync();
    }

}
