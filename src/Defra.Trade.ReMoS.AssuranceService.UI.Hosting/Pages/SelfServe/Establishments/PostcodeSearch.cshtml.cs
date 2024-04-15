using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using SignUp = Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class PostcodeSearchModel : SignUp.PostcodeSearchModel
{
    public PostcodeSearchModel(ILogger<PostcodeSearchModel> logger, ITraderService traderService) : base(logger, traderService)
    { }

}


