using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Eligibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessAlreadyRegisteredModel : BasePageModel<RegisteredBusinessAlreadyRegisteredModel>
{
    [BindProperty]
    public Guid OrgId { get; set; }

    public RegisteredBusinessAlreadyRegisteredModel(ILogger<RegisteredBusinessAlreadyRegisteredModel> logger) : base(logger)
    {}
    public IActionResult OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessAlreadyRegisteredModel), nameof(OnGetAsync));

        OrgId = id;

        return Page();
    }
}
