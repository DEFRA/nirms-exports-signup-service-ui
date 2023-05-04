using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact
{
    public class RegisteredBusinessContactEmailModel : PageModel
    {
        [BindProperty]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [StringLength(100, ErrorMessage = "Email is too long")]
        [Required(ErrorMessage = "Enter the email address of the contact person")]
        public string Email { get; set; } = string.Empty;

        private readonly ILogger<RegisteredBusinessContactEmailModel> _logger;

        public RegisteredBusinessContactEmailModel(ILogger<RegisteredBusinessContactEmailModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogInformation("Email OnGet");
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Email OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            return await OnGetAsync();
        }
    }
}
