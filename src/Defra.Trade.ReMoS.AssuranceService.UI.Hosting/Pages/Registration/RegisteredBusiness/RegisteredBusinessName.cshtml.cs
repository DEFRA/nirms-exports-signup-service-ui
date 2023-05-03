using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    public class RegisteredBusinessNameModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Enter your business name")]
        [RegularExpression(@"^[a-zA-Z0-9' -]*$", ErrorMessage = "Enter your business name using only letters, numbers, hyphens (-) and apostrophes (')")]
        [MaxLength(100, ErrorMessage = "Business name is too long")]
        public string Name { get; set; } = string.Empty;

        private readonly ILogger<RegisteredBusinessNameModel> _logger;

        public RegisteredBusinessNameModel(ILogger<RegisteredBusinessNameModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Business Name OnGet");
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Business Name OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            return await OnGetAsync();
        }
    }
}
