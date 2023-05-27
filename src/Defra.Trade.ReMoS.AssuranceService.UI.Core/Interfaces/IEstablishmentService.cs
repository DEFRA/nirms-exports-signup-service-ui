using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IEstablishmentService
{
    public Task<Guid?> CreateEstablishmentAndAddToPartyAsync(
        Guid partyId,
        LogisticsLocationDTO logisticsLocationDTO);
    Task<LogisticsLocationDTO?> GetEstablishmentByIdAsync(Guid Id);
    Task<IEnumerable<LogisticsLocationDetailsDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId);
    public Task<List<LogisticsLocationDTO>?> GetEstablishmentByPostcodeAsync(string postcode);
    public Task<Guid?> AddEstablishmentToPartyAsync(LogisticsLocationBusinessRelationshipDTO logisticsLocationRelationshipDTO);
    Task<bool> RemoveEstablishmentFromPartyAsync(Guid partyId, Guid establishmentId);
}
