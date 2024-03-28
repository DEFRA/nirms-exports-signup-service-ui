using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.FeatureManagement.Mvc;
using SignUp = Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class PostcodeResultModel : SignUp.PostcodeResultModel
{
    public PostcodeResultModel(ILogger<PostcodeResultModel> logger, IEstablishmentService establishmentService, ITraderService traderService) : base(logger, establishmentService, traderService)
    { }
}
