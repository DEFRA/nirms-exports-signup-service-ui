using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain;

public class BasePageModel : PageModel
{
    public List<string>? ErrorMessages { get; set; }

    public Boolean checkModelState()
    {
        ErrorMessages = new List<string>();
        if (!ModelState.IsValid)
        {
            var modelErrors = ModelState.Values.Where(E => E.Errors.Count > 0)
                                .SelectMany(E => E.Errors)
                                .Select(E => E.ErrorMessage)
                                .ToList();
            foreach (var error in modelErrors)
            {
                ErrorMessages.Add(error);
            }

            return false;
        } else
        {
            return true;
        }
    }
}