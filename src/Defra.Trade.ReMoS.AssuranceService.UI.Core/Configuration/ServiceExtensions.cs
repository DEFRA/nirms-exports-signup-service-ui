using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;


namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServiceConfigurations(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("Assurance", httpClient =>
        {
            httpClient.BaseAddress = new Uri($"{config.GetValue<string>("APISettings:APIUrl")}/");
            httpClient.DefaultRequestHeaders.Add("x-api-version", "1");
        });

        services.AddTransient<IApiIntegration, ApiIntegration>();
        services.AddTransient<ITraderService, TraderService>();
        services.AddTransient<IEstablishmentService, EstablishmentService>();
        services.AddTransient<ICheckAnswersService, CheckAnswersService>();
        services.AddSingleton<IValidationParameters, ValidationParameters>();
        services.AddTransient<IUserService, UserService>();

        return services;
    }
}