using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using GraphQL;
using GraphQL.Client.Abstractions;
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


    public async Task<List<TradeParty>?> GetAllTradePartiesAsync()
    {
        List<TradeParty>? results = new();
        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.GetAsync("/TradeParties/Parties");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<List<TradeParty>>(contentStream);
            }
        }
        return results;
    }

    public async Task<TradeParty> AddTradePartyAsync(TraderDTO tradePartyToCreate)
    {
        TradeParty? results = new();
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
                results = await JsonSerializer.DeserializeAsync<TradeParty>(contentStream);
            }
        }
        if (results != null)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<TradeParty> UpdateTradePartyAsync(TraderDTO tradePartyToCreate)
    {
        TradeParty? results = new();
        var guid = Guid.NewGuid();
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToCreate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = _httpClientFactory.CreateClient("Assurance");
        var httpResponseMessage = await httpClient.PutAsync($"/TradeParties/Parties/{guid}", requestBody);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                results = await JsonSerializer.DeserializeAsync<TradeParty>(contentStream);
            }
        }
        if (results != null)
        {
            return results;
        }
        throw new BadHttpRequestException("null return from API");
    }

    public async Task<TradeAddressDTO> AddTradeAddressForParty(Guid partyId, TradeAddressAddUpdateDTO tradeAddressAddUpdateDTO)
    {
        TradeAddressDTO? result = new();

        var httpClient = _httpClientFactory.CreateClient("Assurance");

        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradeAddressAddUpdateDTO),
            Encoding.UTF8,
            Application.Json);

        var response = await httpClient.PostAsync($"/TradeParties/Parties/{partyId}/addresses", requestBody);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        if (content != null)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            result = JsonSerializer.Deserialize<TradeAddressDTO>(content, options);
        }

        if (result != null)
        {
            return result;
        }
        throw new BadHttpRequestException("null return from API");
    }

}
