using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IAPIIntegration
{
    Task<List<TradeParty>?> GetAllTradePartiesAsync();
    Task<TradeParty> AddTradePartyAsync(TraderDTO tradePartyToCreate);
    Task<TradeParty> UpdateTradePartyAsync(TraderDTO tradePartyToCreate);
}