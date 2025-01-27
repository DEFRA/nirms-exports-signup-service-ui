﻿using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Helpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;

/// <summary>
/// Contains calls to Sign-up Service API via HttpClient
/// </summary>
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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        _appConfigurationSettings = appConfigurationSettings;
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Gets all trade parties from SuS API
    /// </summary>
    /// <returns>List of trade parties</returns>
    public async Task<List<TradePartyDto>?> GetAllTradePartiesAsync()
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync("TradeParties");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<TradePartyDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<TradePartyDto>();
    }

    /// <summary>
    /// Gets an individual trade party
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Trade party</returns>
    public async Task<TradePartyDto?> GetTradePartyByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TradePartyDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new TradePartyDto();
    }

    /// <summary>
    /// Get saved details for a DEFRA org
    /// </summary>
    /// <param name="orgId"></param>
    /// <returns>Trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
    public async Task<TradePartyDto?> GetTradePartyByOrgIdAsync(Guid orgId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"TradeParties/Organisation/{orgId}");

        if ((int)response.StatusCode == StatusCodes.Status200OK)
        {
            return await JsonSerializer.DeserializeAsync<TradePartyDto>(
                await response.Content.ReadAsStreamAsync(),
                options: _jsonSerializerOptions) ?? new TradePartyDto();
        }
        else if ((int)response.StatusCode == StatusCodes.Status404NotFound || (int)response.StatusCode == StatusCodes.Status204NoContent)
        {
            return null;
        }
        else
        {
            throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
        }
    }

    /// <summary>
    /// Adds a new trade party
    /// </summary>
    /// <param name="tradePartyToCreate"></param>
    /// <returns>Id of the newly created trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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
        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Updates an existing trade party
    /// </summary>
    /// <param name="tradePartyToUpdate"></param>
    /// <returns>Id of the updated trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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
        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Updates an existing trade party's address
    /// </summary>
    /// <param name="tradePartyToUpdate"></param>
    /// <returns>Id of the trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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
        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Creates a new address and adds to a trade party
    /// </summary>
    /// <param name="partyId"></param>
    /// <param name="addressDTO"></param>
    /// <returns>Id of the trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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

        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Updates trade party contact
    /// </summary>
    /// <param name="tradePartyToUpdate"></param>
    /// <returns>Id of the trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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
        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    public async Task<Guid> UpdateTradePartyContactSelfServeAsync(TradePartyDto tradePartyToUpdate)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/SelfServe/Contact/{tradePartyToUpdate.Id}", requestBody);

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
        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    public async Task<Guid> UpdateTradePartyAuthRepSelfServeAsync(TradePartyDto tradePartyToUpdate)
    {
        Guid results = Guid.Empty;
        var requestBody = new StringContent(
            JsonSerializer.Serialize(tradePartyToUpdate),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"TradeParties/SelfServe/Authorised-Signatory/{tradePartyToUpdate.Id}", requestBody);

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
        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Add an establishment to a trade party
    /// </summary>
    /// <param name="partyId"></param>
    /// <param name="logisticsLocationDTO"></param>
    /// <returns>Id of the establishment</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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

            if (results != Guid.Empty)
            {
                return results;
            }
        }

        var errResponseMessage = await response.Content.ReadAsStringAsync();

        switch (errResponseMessage)
        {
            case "\"Establishment already exists\"":
                throw new BadHttpRequestException(errResponseMessage);
            default:
                break;
        }

        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Get an establishment using Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Establishment</returns>
    public async Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid id)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/{id}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationDto();
    }

    /// <summary>
    /// Get all establishments belonging to the trade party
    /// </summary>
    /// <param name="tradePartyId"></param>
    /// <returns>List of establishments</returns>
    public async Task<PagedList<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(
        Guid tradePartyId,
        bool includeRejected,
        string? searchTerm,
        string? sortColumn,
        string? sortDirection,
        string? NI_GBFlag,
        int pageNumber = 1,
        int pageSize = 50)
    {
        var httpClient = CreateHttpClient();
        var requestUrl = $"Establishments/Party/{tradePartyId}?includeRejected={includeRejected}&ni_gbFlag={NI_GBFlag}&sortColumn={sortColumn}&sortDirection={sortDirection}&pageNumber={pageNumber}&pageSize={pageSize}" + (searchTerm != null ? $"&searchTerm={searchTerm.Replace("&", "%26")}" : "");
        var response = await httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                return await JsonSerializer.DeserializeAsync<PagedList<LogisticsLocationDto>>(
                await response.Content.ReadAsStreamAsync(),
                options: _jsonSerializerOptions) ?? new PagedList<LogisticsLocationDto>();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get all establishments in a post code
    /// </summary>
    /// <param name="postcode"></param>
    /// <returns>List of establishments</returns>
    public async Task<List<LogisticsLocationDto>?> GetEstablishmentsByPostcodeAsync(string postcode)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Postcode/{postcode}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<LogisticsLocationDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<LogisticsLocationDto>();
    }

    /// <summary>
    /// Deletes a establishment with a given id
    /// </summary>
    /// <param name="locationId"></param>
    /// <returns></returns>
    public async Task RemoveEstablishmentAsync(Guid locationId)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.DeleteAsync($"Establishments/{locationId}");

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Updates establishment
    /// </summary>
    /// <param name="establishmentDto"></param>
    /// <returns><c>true</c> if establishment updated</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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

        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Updates authorised signatory
    /// </summary>
    /// <param name="tradePartyToUpdate"></param>
    /// <returns>Trade party</returns>
    /// <exception cref="BadHttpRequestException"></exception>
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

        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }

    /// <summary>
    /// Creates HttpClient used to call the SuS API
    /// </summary>
    /// <returns>Http client</returns>
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

    /// <summary>
    /// Get Trade.Address addresses by postcode
    /// </summary>
    /// <param name="postcode"></param>
    /// <returns>List of addresses</returns>
    public async Task<List<AddressDto>> GetTradeAddresApiByPostcodeAsync(string postcode)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Trade/Postcode/{postcode}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<AddressDto>>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new List<AddressDto>();
    }

    /// <summary>
    /// Get a logistics location by UPRN
    /// </summary>
    /// <param name="uprn"></param>
    /// <returns>Establishment</returns>
    public async Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn)
    {
        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync($"Establishments/Trade/Uprn/{uprn}");

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<LogisticsLocationDto>(
            await response.Content.ReadAsStreamAsync(),
            options: _jsonSerializerOptions) ?? new LogisticsLocationDto();
    }

    /// <summary>
    /// Updates establishment
    /// </summary>
    /// <param name="establishmentDto"></param>
    /// <returns><c>true</c> if establishment updated</returns>
    /// <exception cref="BadHttpRequestException"></exception>
    public async Task<bool> UpdateEstablishmentSelfServeAsync(LogisticsLocationDto establishmentDto)
    {
        var requestBody = new StringContent(
            JsonSerializer.Serialize(establishmentDto),
            Encoding.UTF8,
            Application.Json);

        var httpClient = CreateHttpClient();
        var response = await httpClient.PutAsync($"Establishments/SelfServe/{establishmentDto.Id}", requestBody);

        if (response.IsSuccessStatusCode)
            return true;

        throw new BadHttpRequestException(ErrorMessages.NULLRETURNFROMAPI);
    }
}