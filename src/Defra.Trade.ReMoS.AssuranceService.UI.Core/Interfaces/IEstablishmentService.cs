using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IEstablishmentService
{
    public Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId,
        LogisticsLocationDTO logisticsLocationDTO);
    Task<LogisticsLocationDTO?> GetEstablishmentByIdAsync(Guid Id);
    Task<IEnumerable<LogisticsLocationDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId);
    public Task<List<LogisticsLocationDTO>?> GetEstablishmentByPostcodeAsync(string postcode);
    Task<bool> RemoveEstablishmentAsync(Guid establishmentId);
    Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDTO establishmentDto);
}
