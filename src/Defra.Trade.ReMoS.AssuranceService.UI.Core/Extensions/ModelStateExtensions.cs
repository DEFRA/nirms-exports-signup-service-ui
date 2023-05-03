using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;

/// <summary>
/// Extension methods for the <see cref="ModelStateDictionary"/>
/// </summary>
public static class ModelStateExtensions
{
    /// <summary>
    /// Checks if an error exists for a given field in the model state.
    /// </summary>
    /// <param name="modelState">The model state.</param>
    /// <param name="key">The key of the field to check.</param>
    /// <returns><c>true</c> if an error is found.</returns>
    public static bool HasError(this ModelStateDictionary modelState, string key)
    {
        if (modelState[key]?.Errors != null && modelState[key].Errors.Any())
        {
            return true;
        }

        return false;
    }
}
