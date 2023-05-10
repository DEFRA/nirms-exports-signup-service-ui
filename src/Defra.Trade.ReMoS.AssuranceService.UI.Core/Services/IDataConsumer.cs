using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public interface IDataConsumer
{
    Task<List<TradeParty>> GetAllTradeParties();
}