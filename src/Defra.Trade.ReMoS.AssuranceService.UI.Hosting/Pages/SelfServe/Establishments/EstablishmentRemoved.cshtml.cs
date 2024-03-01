using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments;

public class EstablishmentRemovedModel : PageModel
{
    [BindProperty]
    public Guid OrgId { get; set; }
    public string? EstablishmentName { get; set; }
    public string? NI_GBFlag { get; set; }
    public string DispatchOrDestination { get; set; } = default!;

    public IActionResult OnGet(Guid id, string? establishmentName, string NI_GBFlag = "GB")
    {
        OrgId = id;
        EstablishmentName = establishmentName;
        this.NI_GBFlag = NI_GBFlag;

        if (NI_GBFlag == "NI")
            DispatchOrDestination = "destination";
        else
            DispatchOrDestination = "dispatch";

        return Page();
    }
}
