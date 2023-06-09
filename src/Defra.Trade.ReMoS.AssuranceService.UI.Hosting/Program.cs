using Defra.Trade.Common.Security.AzureKeyVault;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.Common.Security.AzureKeyVault;
using Defra.Trade.Common.Security.AzureKeyVault.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.ApplicationInsights.DependencyCollector;
using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.AppConfig;
using Microsoft.Extensions.DependencyInjection;
using Defra.Trade.Common.Security.Authentication.Infrastructure;

[ExcludeFromCodeCoverage]
internal sealed class Program
{
    private Program()
    {
    }
#pragma warning disable CS1998
    private static async Task Main(string[] args)
#pragma warning restore CS1998
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddMvc().AddCustomRouting();
        builder.Services.AddApplicationInsightsTelemetry();
        //builder.Configuration.AddAzureKeyVault<KeyVaultSettings>(KeyVaultSettings.KeyVaultSecretsSettingsName, new ManagedIdentityKeyVaultAuthentication());
        builder.Configuration.ConfigureTradeAppConfiguration(true, "RemosSignUpService:Sentinel");
        builder.Services.Configure<KeyVaultSettings>(builder.Configuration.GetSection("Apim:Internal"));
        builder.Services.AddServiceConfigurations(builder.Configuration);
        builder.Services.AddApimAuthentication(builder.Configuration.GetSection("Apim:Internal"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        
        app.Run();


    }
}