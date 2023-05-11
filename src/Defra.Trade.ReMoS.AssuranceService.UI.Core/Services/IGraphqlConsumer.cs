using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public interface IGraphqlConsumer
{
    Task<List<TradeParty>> GetAllTradePartiesAsync();
    Task<TradeParty> RegisterTradePartyAsync(TradePartyForCreation tradePartyToCreate);
}