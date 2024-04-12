using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using SignUp = Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class EstablishmentNameAndAddressModel : SignUp.EstablishmentNameAndAddressModel
{
    public EstablishmentNameAndAddressModel(
        ILogger<EstablishmentNameAndAddressModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, establishmentService, traderService)
    { }

    public override bool IsInputValid()
    {
        if (!ModelState.IsValid)
            return false;

        if (EstablishmentName != null && EstablishmentName.Length > 100)
            ModelState.AddModelError(nameof(EstablishmentName), "Establishment name must be 100 characters or less");

        if (LineOne != null && LineOne.Length > 50)
            ModelState.AddModelError(nameof(LineOne), "Address line 1 must be 50 characters or less");

        if (LineTwo != null && LineTwo.Length > 50)
            ModelState.AddModelError(nameof(LineTwo), "Address line 2 must be 50 characters or less");

        if (CityName != null && CityName.Length > 100)
            ModelState.AddModelError(nameof(CityName), "Town or city must be 100 characters or less");

        if (PostCode != null && PostCode.Length > 100)
            ModelState.AddModelError(nameof(PostCode), "Post code must be 100 characters or less");

        if (County != null && County.Length > 100)
            ModelState.AddModelError(nameof(County), "County must be 100 characters or less");

        if (ModelState.ErrorCount > 0)
            return false;

        return true;
    }
}
