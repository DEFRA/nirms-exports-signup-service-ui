using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using SignUp = Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
[BindProperties]
public class ContactEmailModel : SignUp.ContactEmailModel
{
    public ContactEmailModel(
        ILogger<ContactEmailModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, establishmentService, traderService)
    {}
    public override bool IsInputValid()
    {
        if (!ModelState.IsValid || ModelState.ErrorCount > 0)
            return false;

        return true;
    }
}
