using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.Address.V1.ApiClient.Model;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;

public class ApiIntegration : IApiIntegration
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

    public async Task<List<TradePartyDto>?> GetAllTradePartiesAsync()
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("TradeParties");
        
        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<TradePartyDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<TradePartyDto>();
    }

    public async Task<TradePartyDto?> GetTradePartyByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TradePartyDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new TradePartyDto();
    }

    public async Task<TradePartyDto?> GetTradePartyByOrgIdAsync(Guid orgId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/Organisation/{orgId}");

        if (response.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<TradePartyDto>(
                await response.Content.ReadAsStreamAsync(),
                options: _jsonSerializerOptions) ?? new TradePartyDto();
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

    public async Task<Guid> AddTradePartyAsync(TradePartyDto tradePartyToCreate)
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

    public async Task<Guid> UpdateTradePartyAsync(TradePartyDto tradePartyToUpdate)
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

    public async Task<Guid> UpdateTradePartyAddressAsync(TradePartyDto tradePartyToUpdate)
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

    public async Task<Guid> AddAddressToPartyAsync(Guid partyId, TradeAddressDto addressDTO)
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

    public async Task<Guid> UpdateTradePartyContactAsync(TradePartyDto tradePartyToUpdate)
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

    public async Task<Guid?> AddEstablishmentToPartyAsync(Guid partyId, LogisticsLocationDto logisticsLocationDTO)
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

    public async Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationDto();
    }

    public async Task<List<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Party/{tradePartyId}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDto>();
    }

    public async Task<List<LogisticsLocationDto>?> GetEstablishmentsByPostcodeAsync(string postcode)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Postcode/{postcode}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDto>();
    }

    public async Task RemoveEstablishmentAsync(Guid locationId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.DeleteAsync($"Establishments/{locationId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> UpdateEstablishmentAsync(LogisticsLocationDto establishmentDto)
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

    public async Task<TradePartyDto?> UpdateAuthorisedSignatoryAsync(TradePartyDto tradePartyToUpdate)
    {
        var httpClient = CreateHttpClient();
        var requestBody = new StringContent(
        JsonSerializer.Serialize(tradePartyToUpdate),
        Encoding.UTF8,
        Application.Json);

        var response = await httpClient.PutAsync($"TradeParties/Authorised-Signatory/{tradePartyToUpdate.Id}", requestBody);


        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TradePartyDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new TradePartyDto();

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

    public async Task<List<AddressDto>> GetTradeAddresApiByPostcodeAsync(string postcode)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Trade/Postcode/{postcode}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<AddressDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<AddressDto>();
    }

    public async Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Trade/Uprn/{uprn}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationDto();
    }
}