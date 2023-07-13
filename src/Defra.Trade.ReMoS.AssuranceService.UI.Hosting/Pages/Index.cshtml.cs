﻿using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS1998

namespace Defra.ReMoS.AssuranceService.UI.Hosting.Pages;

//Remove when start page added
[ExcludeFromCodeCoverage]
public class IndexModel : PageModel
{
    [BindProperty]
    public Guid? Id { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        if (Id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(Id), "Enter Guid");
        }

        return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = Id });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        return RedirectToPage(Routes.Pages.Path.RegisteredBusinessBusinessPickerPath, new { id = Guid.Empty });
    }
}
