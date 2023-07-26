using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using static System.Net.Mime.MediaTypeNames;
using Defra.Trade.Common.Api.Infrastructure;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Defra.Trade.Common.Api.Health;
#pragma warning disable CS1998

[ExcludeFromCodeCoverage]
internal sealed class Program
{
    private Program()
    {
    }

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");
        builder.Services.Configure<AppConfigurationService>(builder.Configuration.GetSection("Apim:Internal"));
        builder.Services.Configure<EhcoIntegration>(builder.Configuration.GetSection("EhcoIntegration"));
        builder.Services.AddServiceConfigurations(builder.Configuration);
        builder.Services.AddApimAuthentication(builder.Configuration.GetSection("Apim:Internal"));
        builder.Services.AddTradeApi(builder.Configuration);
        builder.Services.AddHealthChecks();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Index";
                options.SlidingExpiration = true;
                options.Cookie.Name = "authentication";
            });

        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.Name = "anti-forgery";
        });

        builder.Services.AddMvc(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
        }).AddCustomRouting(); ;

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential 
            // cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseStatusCodePagesWithReExecute("/Errors/{0}");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTradeHealthChecks();
        app.UseCookiePolicy();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();


        app.Run();
    }
}