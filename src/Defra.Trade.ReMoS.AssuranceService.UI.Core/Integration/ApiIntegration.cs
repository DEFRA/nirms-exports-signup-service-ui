using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;

public class ApiIntegration : IAPIIntegration
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IOptions<AppConfigurationService> _appConfigurationSettings;
    private readonly IAuthenticationService _authenticationService;

    public ApiIntegration(IHttpClientFactory httpClientFactory, IOptions<AppConfigurationService> appConfigurationSettings, IAuthenticationService authenticationService)
    {
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _appConfigurationSettings = appConfigurationSettings;
        _authenticationService = authenticationService;
    }

    public async Task<List<TradePartyDTO>?> GetAllTradePartiesAsync()
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("TradeParties");
        
        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<TradePartyDTO>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<TradePartyDTO>();
    }

    public async Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TradePartyDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new TradePartyDTO();
    }

    public async Task<TradePartyDTO?> GetTradePartyByOrgIdAsync(Guid orgId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/Organisation/{orgId}");

        if (response.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<TradePartyDTO>(
                await response.Content.ReadAsStreamAsync(),
                options: _jsonSerializerOptions) ?? new TradePartyDTO();
        }
        else if ((int)response.StatusCode == StatusCodes.Status404NotFound)
        {
            return null;
        }
        else
        {
            throw new BadHttpRequestException("null return from API");
        }
    }

    public async Task<Guid> AddTradePartyAsync(TradePartyDTO tradePartyToCreate)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToCreate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync($"TradeParties", requestBody);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream, _jsonSerializerOptions);
            }
        }
        if (results != Guid.Empty)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<Guid> UpdateTradePartyAsync(TradePartyDTO tradePartyToUpdate)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/{tradePartyToUpdate.Id}", requestBody);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream);
            }
        }
        if (results != Guid.Empty)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<Guid> UpdateTradePartyAddressAsync(TradePartyDTO tradePartyToUpdate)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/Address/{tradePartyToUpdate.Id}", requestBody);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream);
            }
        }
        if (results != Guid.Empty)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<Guid> AddAddressToPartyAsync(Guid partyId, TradeAddressDTO addressDTO)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(addressDTO),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync($"TradeParties/{partyId}/Address", requestBody);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream, _jsonSerializerOptions);
            }
        }
        if (results != Guid.Empty)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<Guid> UpdateTradePartyContactAsync(TradePartyDTO tradePartyToUpdate)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/Contact/{tradePartyToUpdate.Id}", requestBody);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream);
            }
        }
        if (results != Guid.Empty)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<Guid?> AddEstablishmentToPartyAsync(Guid partyId, LogisticsLocationDTO logisticsLocationDTO)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(logisticsLocationDTO),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync($"Establishments/Party/{partyId}", requestBody);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream, _jsonSerializerOptions);
            }
        }
        if (results != Guid.Empty)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<LogisticsLocationDTO?> GetEstablishmentByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationDTO();
    }

    public async Task<List<LogisticsLocationDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Party/{tradePartyId}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDTO>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDTO>();
    }

    public async Task<List<LogisticsLocationDTO>?> GetEstablishmentsByPostcodeAsync(string postcode)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Postcode/{postcode}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDTO>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDTO>();
    }

    public async Task RemoveEstablishmentAsync(Guid locationId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.DeleteAsync($"Establishments/{locationId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> UpdateEstablishmentAsync(LogisticsLocationDTO establishmentDto)
    {
        var requestBody = new StringContent(
            JsonSerializer.Serialize(establishmentDto),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"Establishments/{establishmentDto.Id}", requestBody);

        if (response.IsSuccessStatusCode)
            return true;

        throw new BadHttpRequestException("null return from API");
    }

    public async Task<TradePartyDTO?> UpdateAuthorisedSignatoryAsync(TradePartyDTO tradePartyToUpdate)
    {
        var httpClient = CreateHttpClient();
        var requestBody = new StringContent(
        JsonSerializer.Serialize(tradePartyToUpdate),
        Encoding.UTF8,
        Application.Json);

        var response = await httpClient.PutAsync($"TradeParties/Authorised-Signatory/{tradePartyToUpdate.Id}", requestBody);


        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TradePartyDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new TradePartyDTO();

        throw new BadHttpRequestException("null return from API");
    }

    public HttpClient CreateHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient("Assurance");

        //Only add auth headres if calling API running on Azure
        if (!httpClient.BaseAddress!.ToString().Contains("localhost"))
        {
            httpClient.DefaultRequestHeaders.Authorization = _authenticationService.GetAuthenticationHeaderAsync().Result;
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appConfigurationSettings.Value.SubscriptionKey);
        }

        return httpClient;
    }
}