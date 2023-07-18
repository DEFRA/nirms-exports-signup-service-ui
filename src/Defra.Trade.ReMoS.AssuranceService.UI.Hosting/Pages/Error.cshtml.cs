using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.ContainerRegistry.Fluent.Models;

namespace Defra.ReMoS.AssuranceService.UI.Hosting.Pages;

[ExcludeFromCodeCoverage]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public int ErrorStatusCode { get; set; }

    public string ErrorTitle { get; set; }

    public string ErrorLineOne { get; set; }

    public string ErrorLineTwo { get; set; }
    public string ErrorContactLineOne { get; set; }
    public string ErrorContactLineTwo { get; set; }
    public string ErrorContactTitle { get; set; }

    public string ErrorContactLink { get; set; }

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int statusCode = 0)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error page");

        ErrorStatusCode = HttpContext.Response.StatusCode;

        switch(statusCode)
        {
            case 404:
                ErrorTitle = "Page not found";
                ErrorLineOne = "If you typed the web address, check it is correct.";
                ErrorLineTwo = "If you pasted the web address, check you copied the entire address.";
                ErrorContactLineOne = "";
                ErrorContactLink = "";
                ErrorContactTitle = "Contact the Tax Credits Helpline";
                ErrorContactLineTwo = " if you need to make changes to your claim or speak to someone about your tax credits.";
                break;
            case 500:
                ErrorTitle = "Sorry, there is a problem with the service";
                ErrorLineOne = "Try again later.";
                ErrorLineTwo = "We saved your answers. They will be available for 30 days.";
                ErrorContactLineOne = "If the web address is correct or you selected a link or button, <a href=\"#\" class=\"govuk-link\">contact the Tax Credits Helpline</a> if you need to speak to someone about your tax credits.\r\n        </p>";
                ErrorContactLink = "";
                ErrorContactTitle = "contact the Tax Credits Helpline";
                ErrorContactLineTwo = "if you need to speak to someone about your tax credits.";
                break;
            default:
                break;
        }
    }
}

