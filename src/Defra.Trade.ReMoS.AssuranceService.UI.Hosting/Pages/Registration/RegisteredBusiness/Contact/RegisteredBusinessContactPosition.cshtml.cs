using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;


namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessContactPositionModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a position using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter the position of the contact person")]
    public string Position { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<RegisteredBusinessContactPositionModel> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RegisteredBusinessContactPositionModel(ILogger<RegisteredBusinessContactPositionModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Position OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Position OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        return await OnGetAsync();
    }
}
