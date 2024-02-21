using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.Address.V1.ApiClient.Api;
using Defra.Trade.Address.V1.ApiClient.Model;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class EstablishmentService : IEstablishmentService
{
    private readonly IApiIntegration _api;

    public EstablishmentService(IApiIntegration api)
    {
        _api = api;
    }
    public async Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId,
        LogisticsLocationDto logisticsLocationDTO)
    {
        var establishmentId = await _api.AddEstablishmentToPartyAsync(partyId, logisticsLocationDTO);

        return establishmentId;
    }

    public async Task<IEnumerable<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId, bool isRejected)
    {
        return await _api.GetEstablishmentsForTradePartyAsync(tradePartyId, isRejected);
    }

    public async Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid Id)
    {
        return (Id != Guid.Empty) ? await _api.GetEstablishmentByIdAsync(Id) : null;
    }
    public async Task<List<LogisticsLocationDto>?> GetEstablishmentByPostcodeAsync(string postcode)
    {
        return await _api.GetEstablishmentsByPostcodeAsync(postcode);
    }

    public async Task<bool> RemoveEstablishmentAsync(Guid establishmentId)
    {
        await _api.RemoveEstablishmentAsync(establishmentId);
        return true;
    }

    public async Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDto establishmentDto)
    {
        return await _api.UpdateEstablishmentAsync(establishmentDto);
    }

    public async Task<List<AddressDto>> GetTradeAddressApiByPostcodeAsync(string postcode)
    {
        return await _api.GetTradeAddresApiByPostcodeAsync(postcode);
    }

    public async Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn)
    {
        return await _api.GetLogisticsLocationByUprnAsync(uprn);
    }

    public async Task<bool> UpdateEstablishmentDetailsSelfServeAsync(LogisticsLocationDto establishmentDto)
    {
        return await _api.UpdateEstablishmentSelfServeAsync(establishmentDto);
    }
}
