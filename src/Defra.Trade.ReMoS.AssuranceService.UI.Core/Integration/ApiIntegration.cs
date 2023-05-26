using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;

public class ApiIntegration : IAPIIntegration
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiIntegration(IHttpClientFactory httpClientFactory)
    { 
        _httpClientFactory = httpClientFactory;
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
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                results = await JsonSerializer.DeserializeAsync<List<TradePartyDTO>>(contentStream, options);
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
            options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase}) ??
            new TradePartyDTO();
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
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream, options);
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
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream, options);
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
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                results = await JsonSerializer.DeserializeAsync<Guid>(contentStream, options);
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
            options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ??
            new LogisticsLocationDTO();
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
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                results = await JsonSerializer.DeserializeAsync<List<LogisticsLocationDetailsDTO>>(contentStream, options);
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
            options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ??
            new List<LogisticsLocationDTO>();
    }
}
