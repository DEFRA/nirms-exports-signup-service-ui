using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.FeatureManagement;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;

public abstract class BasePageModel<T> : PageModel where T : PageModel
{
    protected readonly ILogger<T> _logger = default!;
    protected readonly ITraderService _traderService = default!;
    protected readonly IEstablishmentService _establishmentService = default!;
    protected readonly ICheckAnswersService _checkAnswersService = default!;
    protected readonly IUserService _userService = default!;
    protected readonly IConfiguration _config = default!;

    protected BasePageModel(params object[] list)
    {
        foreach (var item in list)
        {
            if (item is ILogger)
            {
                _logger = (ILogger<T>)item;
            }
           
            
            if (item is ITraderService traderService)
            {
                _traderService = traderService;
            }

            if (item is IEstablishmentService establishmentService)
            {
                _establishmentService = establishmentService;
            }

            if (item is ICheckAnswersService checkAnswersService)
            {
                _checkAnswersService = checkAnswersService;
            }
            if (item is IUserService userService)
            {
                _userService = userService;
            }
            if (item is IConfiguration config)
            {
                _config = config;
            }

        }
    }
}
