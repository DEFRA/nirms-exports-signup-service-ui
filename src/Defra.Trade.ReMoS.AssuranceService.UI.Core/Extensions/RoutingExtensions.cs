using Defra.Trade.ReMoS.AssuranceService.UI.Core.Utilities;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;

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
            options.Conventions.Add(new DefaultRouteRemovalPageRouteModelConvention(string.Empty));
            options.Conventions.AddPageRoute("/Index", "");

            foreach (var (page, route) in Routes.RouteList)
            {
                options.Conventions.AddPageRoute(page, route);
            }
        }));

        return builder;
    }
    }
