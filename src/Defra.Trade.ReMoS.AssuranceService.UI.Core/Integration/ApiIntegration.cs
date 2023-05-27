using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;

public class ApiIntegration : IAPIIntegration
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiIntegration(IHttpClientFactory httpClientFactory)
    { 
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }


    public async Task<List<TradePartyDTO>?> GetAllTradePartiesAsync()
    {
        List<TradePartyDTO>? results = new();
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.GetAsync("/TradeParties/Parties");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {                
                results = await JsonSerializer.DeserializeAsync<List<TradePartyDTO>>(contentStream, _jsonSerializerOptions);
            }
        }
        return results;
    }

    public async Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid id)
    {
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var response = await httpClient.GetAsync($"/TradeParties/Parties/{id}");

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

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PostAsync($"TradeParties/Party", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
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

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PutAsync($"/TradeParties/Parties/{tradePartyToUpdate.Id}", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
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

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PutAsync($"/TradeParties/Parties/Address/{tradePartyToUpdate.Id}", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
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

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PutAsync($"/TradeParties/Parties/Contact/{tradePartyToUpdate.Id}", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
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

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PostAsync($"Establishments", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
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

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PostAsync($"Relationships", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
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
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var response = await httpClient.GetAsync($"/Establishments/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationDTO>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationDTO();
    }

    public async Task<List<LogisticsLocationDetailsDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
    {
        List<LogisticsLocationDetailsDTO>? results = new();
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.GetAsync($"/Establishments/Party/{tradePartyId}");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {               
                results = await JsonSerializer.DeserializeAsync<List<LogisticsLocationDetailsDTO>>(contentStream, _jsonSerializerOptions);
            }
        }
        return results;
    }

    public async Task<List<LogisticsLocationDTO>?> GetEstablishmentsByPostcodeAsync(string postcode)
    {
        List<LogisticsLocationDTO>? results = new();
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var response = await httpClient.GetAsync($"/Establishments/Postcode/{postcode}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDTO>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDTO>();
    }

    public async Task RemoveEstablishmentFromPartyAsync(Guid tradePartyId, Guid locationId)
    {
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var response = await httpClient.DeleteAsync($"/Relationships?partyId={tradePartyId}&establishmentid={locationId}");

        response.EnsureSuccessStatusCode();
    }
}
