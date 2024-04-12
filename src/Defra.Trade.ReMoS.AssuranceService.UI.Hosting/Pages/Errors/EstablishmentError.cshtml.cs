using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Errors;

public class EstablishmentErrorModel : BasePageModel<EstablishmentErrorModel>
{
    #region ui model variables
    [BindProperty]
    public Guid OrgId { get; set; }
    #endregion

    public EstablishmentErrorModel(ILogger<EstablishmentErrorModel> logger) : base(logger)
    {

    }

    public void OnGet(Guid id)
    {
        OrgId = id;
        _logger.LogInformation("EstablishmentError OnGet");
    }
}

