using Defra.Trade.Common.Api.Health;
using Defra.Trade.Common.Api.Infrastructure;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.FeatureManagement;
using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1998

[ExcludeFromCodeCoverage]
internal sealed class Program
{
    private Program()
    {
    }

    private static async Task Main(string[] args)
    {
        //Configure basic logging to be able to log errors during ASP.NET Core startup
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting the web host");

            var builder = WebApplication.CreateBuilder(args);

            // Full setup of serilog. We read log settings from appsettings.json
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            // Add services to the container.
            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToFolder("/Errors");
                options.Conventions.AllowAnonymousToFolder("/Footer");
            });

            builder.Services.AddApplicationInsightsTelemetry();
            builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");
            builder.Services.Configure<AppConfigurationService>(builder.Configuration.GetSection("Apim:Internal"));
            builder.Services.Configure<EhcoIntegration>(builder.Configuration.GetSection("EhcoIntegration"));
            builder.Services.Configure<GoogleTagManager>(builder.Configuration.GetSection("GoogleTagManager"));
            builder.Services.AddServiceConfigurations(builder.Configuration);
            builder.Services.AddApimAuthentication(builder.Configuration.GetSection("Apim:Internal"));
            builder.Services.AddTradeApi(builder.Configuration);
            builder.Services.AddHealthChecks();
            builder.Services.AddFeatureManagement();

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
                options.Secure = CookieSecurePolicy.Always;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSerilogRequestLogging(configure =>
            {
                configure.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";
            }); // We want to log all HTTP requests

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Cache-control", "no-cache, no-store, must-revalidate");
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    $"default-src *; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline' https:; img-src 'self' www.googletagmanager.com data:;");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "same-origin");
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
                await next();
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Errors/Error");
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
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }       

    }
}