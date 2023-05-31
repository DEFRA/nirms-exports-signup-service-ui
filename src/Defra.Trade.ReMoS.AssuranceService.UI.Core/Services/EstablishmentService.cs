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
    public async Task<Guid?> CreateEstablishmentAndAddToPartyAsync(
        Guid partyId, 
        LogisticsLocationDTO logisticsLocationDTO)
    {
        var establishmentId = await _api.CreateEstablishmentAsync(logisticsLocationDTO);

        LogisticsLocationBusinessRelationshipDTO relationDto = new LogisticsLocationBusinessRelationshipDTO
        {
            TradePartyId = partyId,
            LogisticsLocationId = establishmentId.Value,
        };
        Guid? relationId = await _api.AddEstablishmentToPartyAsync(relationDto);

        return establishmentId;
    }

    public async Task<IEnumerable<LogisticsLocationDetailsDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId)
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

    public async Task<Guid?> AddEstablishmentToPartyAsync(LogisticsLocationBusinessRelationshipDTO logisticsLocationRelationshipDTO)
    {
        return await _api.AddEstablishmentToPartyAsync(logisticsLocationRelationshipDTO);
    }

    public async Task<bool> UpdateEstablishmentRelationship(LogisticsLocationBusinessRelationshipDTO logisticsLocationRelationshipDTO)
    {
        return await _api.UpdateEstablishmentRelationship(logisticsLocationRelationshipDTO);
    }

    public async Task<LogisticsLocationBusinessRelationshipDTO?> GetRelationshipBetweenPartyAndEstablishment(Guid partyId, Guid establishmentId)
    {
        return await _api.GetRelationshipBetweenPartyAndEstablishment(partyId, establishmentId);
    }

    public async Task<bool> RemoveEstablishmentFromPartyAsync(Guid partyId, Guid establishmentId)
    {
        await _api.RemoveEstablishmentFromPartyAsync(partyId, establishmentId);
        return true;
    }

    public async Task<bool> IsFirstTradePartyForEstablishment(Guid partyId, Guid establishmentId)
    {
        var relations = await _api.GetAllRelationsForEstablishmentAsync(establishmentId);

        var firstParty = relations?
            .OrderBy(x => x.ModifiedDate)
            .Select(x => x.TradePartyId)
            .First();

        if (partyId != firstParty)
            return false;

        return true;
    }
}
