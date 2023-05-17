using Defra.Trade.ReMoS.AssuranceService.API.Domain.DTO;
using Defra.Trade.ReMoS.AssuranceService.API.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IAPIIntegration
{
    Task<List<TradeParty>?> GetAllTradePartiesAsync();
    Task<TradeParty> AddTradePartyAsync(TradePartyDTO tradePartyToCreate);
    Task<TradeParty> UpdateTradePartyAsync(TradePartyDTO tradePartyToCreate);
}