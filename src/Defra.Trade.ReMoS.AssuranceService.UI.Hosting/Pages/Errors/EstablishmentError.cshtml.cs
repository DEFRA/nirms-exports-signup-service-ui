using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;

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

