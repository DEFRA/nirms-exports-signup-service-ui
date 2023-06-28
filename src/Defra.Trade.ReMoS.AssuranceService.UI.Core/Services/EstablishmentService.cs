using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class EstablishmentService : IEstablishmentService
{
    private readonly IAPIIntegration _api;

    public EstablishmentService(IAPIIntegration api)
    {
        _api = api;
    }
    public async Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId, 
        LogisticsLocationDTO logisticsLocationDTO)
    {
        var establishmentId = await _api.AddEstablishmentToPartyAsync(partyId, logisticsLocationDTO);

        return establishmentId;
    }

    public async Task<IEnumerable<LogisticsLocationDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
    {
        return await _api.GetEstablishmentsForTradePartyAsync(tradePartyId);
    }

    public async Task<LogisticsLocationDTO?> GetEstablishmentByIdAsync(Guid Id)
    {
        return (Id != Guid.Empty) ? await _api.GetEstablishmentByIdAsync(Id) : null;
    }
    public async Task<List<LogisticsLocationDTO>?> GetEstablishmentByPostcodeAsync(string postcode)
    {
        return await _api.GetEstablishmentsByPostcodeAsync(postcode);
    }

    public async Task<bool> RemoveEstablishmentAsync(Guid establishmentId)
    {
        await _api.RemoveEstablishmentAsync(establishmentId);
        return true;
    }

    public async Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDTO establishmentDto)
    {
        return await _api.UpdateEstablishmentAsync(establishmentDto);
    }
}
