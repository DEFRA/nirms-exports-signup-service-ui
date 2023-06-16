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
        var response = await httpClient.GetAsync("TradeParties/Parties");
        
        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<TradePartyDTO>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<TradePartyDTO>();
    }

    public async Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/Parties/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TradePartyDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new TradePartyDTO();
    }

    public async Task<Guid> AddTradePartyAsync(TradePartyDTO tradePartyToCreate)
    {
        Guid results = new();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToCreate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync($"TradeParties/Party", requestBody);

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
        Guid results = new();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/Parties/{tradePartyToUpdate.Id}", requestBody);

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
        Guid results = new();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/Parties/Address/{tradePartyToUpdate.Id}", requestBody);

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

    public async Task<Guid> UpdateTradePartyContactAsync(TradePartyDTO tradePartyToUpdate)
    {
        Guid results = new();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/Parties/Contact/{tradePartyToUpdate.Id}", requestBody);

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

    public async Task<Guid?> CreateEstablishmentAsync(LogisticsLocationDTO logisticsLocationDTO)
    {
        Guid results = new();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(logisticsLocationDTO),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync($"Establishments", requestBody);

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

    public async Task<Guid?> AddEstablishmentToPartyAsync(LogisticsLocationBusinessRelationshipDTO relationDto)
    {
        Guid results = new();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(relationDto),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync($"Relationships", requestBody);

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

    public async Task<List<LogisticsLocationDetailsDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Party/{tradePartyId}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDetailsDTO>> (
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDetailsDTO>();
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

    public async Task RemoveEstablishmentFromPartyAsync(Guid tradePartyId, Guid locationId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.DeleteAsync($"Relationships?partyId={tradePartyId}&establishmentid={locationId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<LogisticsLocationBusinessRelationshipDTO>?> GetAllRelationsForEstablishmentAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Relationships/Establishment/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationBusinessRelationshipDTO>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationBusinessRelationshipDTO>();
    }

    public async Task<bool> UpdateEstablishmentRelationship(LogisticsLocationBusinessRelationshipDTO relationDto)
    {
        var requestBody = new StringContent(
            JsonSerializer.Serialize(relationDto),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var httpResponseMessage = await httpClient.PutAsync($"Relationships/{relationDto.RelationshipId}", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
            return true;

        throw new BadHttpRequestException("null return from API");
    }

    public async Task<LogisticsLocationBusinessRelationshipDTO> GetRelationshipById(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Relationships/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationBusinessRelationshipDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationBusinessRelationshipDTO();
    }

    public async Task<LogisticsLocationBusinessRelationshipDTO> GetRelationshipBetweenPartyAndEstablishment(Guid partyId, Guid establishmentId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Relationships/Trader/{partyId}/Establishment/{establishmentId}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationBusinessRelationshipDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationBusinessRelationshipDTO();
    }

    public HttpClient CreateHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        httpClient.DefaultRequestHeaders.Authorization = _authenticationService.GetAuthenticationHeaderAsync().Result;
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appConfigurationSettings.Value.SubscriptionKey);

        return httpClient;
    }
}
