using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServiceConfigurations(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("Assurance", httpClient =>
        {
            httpClient.BaseAddress = new Uri(config.GetValue<string>("APISettings:APIUrl"));
        });

        services.AddTransient<IAPIIntegration, ApiIntegration>();
        services.AddTransient<ITraderService, TraderService>();
        return services;
    }
}