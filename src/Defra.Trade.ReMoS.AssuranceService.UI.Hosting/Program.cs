using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
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
        builder.Services.AddMvc().AddCustomRouting();
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");
        builder.Services.Configure<AppConfigurationService>(builder.Configuration.GetSection("Apim:Internal"));
        builder.Services.Configure<EhcoIntegrationSettings>(builder.Configuration.GetSection("EhcoIntegrationSettings"));
        builder.Services.AddServiceConfigurations(builder.Configuration);
        builder.Services.AddApimAuthentication(builder.Configuration.GetSection("Apim:Internal"));

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
            });

        builder.Services.AddScoped<CustomCookieAuthenticationEvents>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddMvc(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        
        app.Run();


    }
}