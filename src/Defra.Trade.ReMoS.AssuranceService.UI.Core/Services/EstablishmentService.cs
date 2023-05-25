using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task<Guid?> CreateEstablishmentAndAddToPartyAsync(
        Guid partyId, 
        LogisticsLocationDTO logisticsLocationDTO)
    {
        //create new establishment
        var establishmentId = await _api.CreateEstablishmentAsync(logisticsLocationDTO);

        //add created establishment to party
        LogisticsLocationBusinessRelationshipDTO relationDto = new LogisticsLocationBusinessRelationshipDTO
        {
            TradePartyId = partyId,
            LogisticsLocationId = establishmentId.Value,
        };
        Guid? relationId = await _api.AddEstablishmentToPartyAsync(relationDto);

        return establishmentId;
    }

    public Task<IEnumerable<LogisticsLocationDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
    {
        throw new NotImplementedException();
    }

    public async Task<LogisticsLocationDTO?> GetEstablishmentByIdAsync(Guid Id)
    {
        return (Id != Guid.Empty) ? await _api.GetEstablishmentByIdAsync(Id) : null;
    }
}
