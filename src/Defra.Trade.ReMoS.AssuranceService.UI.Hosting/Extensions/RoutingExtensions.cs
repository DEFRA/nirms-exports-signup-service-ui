using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Extensions;

/// <summary>
/// routing extensions to add to program.cs
/// </summary>
[ExcludeFromCodeCoverage]
public static class RoutingExtensions
{
    /// <summary>
    /// Routing configuration - Static Helper Function
    /// </summary>
    /// <param name="builder">IMvcBuilder to add to the IServiceCollection.</param>
    /// <returns>IMvcBuilder</returns>
    public static IMvcBuilder AddCustomRouting(this IMvcBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.Configure(new Action<RazorPagesOptions>(options =>
        {
            foreach (var (page, route) in Routes.RouteList)
            {
                options.Conventions.AddPageRoute(page, route);
            }
        }));

        return builder;
    }
}

