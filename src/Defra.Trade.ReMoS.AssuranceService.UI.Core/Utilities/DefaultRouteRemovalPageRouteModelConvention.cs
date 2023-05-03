using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Utilities;

public class DefaultRouteRemovalPageRouteModelConvention : IPageRouteModelConvention
{
    private readonly string routeToRemove;

    public DefaultRouteRemovalPageRouteModelConvention(string pageRoute)
    {
        routeToRemove = pageRoute;
    }

    public void Apply(PageRouteModel model)
    {
        for (int i = 0; i < model.Selectors.Count; i++)
        {
            var selector = model.Selectors[i];
            for (int j = 0; j < selector.EndpointMetadata.Count; j++)
            {
                if ((selector.EndpointMetadata[j] as PageRouteMetadata)?.PageRoute == routeToRemove)
                {
                    model.Selectors.Remove(selector);
                    return;
                }
            }
        }
    }
}
