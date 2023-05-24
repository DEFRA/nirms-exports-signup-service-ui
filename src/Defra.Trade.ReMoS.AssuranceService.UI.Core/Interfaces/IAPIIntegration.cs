using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IAPIIntegration
{
    Task<List<TradePartyDTO>?> GetAllTradePartiesAsync();
    public Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid id);
    Task<Guid> AddTradePartyAsync(TradePartyDTO tradePartyToCreate);
    Task<Guid> UpdateTradePartyAsync(TradePartyDTO tradePartyToUpdate);
    public Task<Guid> UpdateTradePartyAddressAsync(TradePartyDTO tradePartyToUpdate);
    public Task<Guid> UpdateTradePartyContactAsync(TradePartyDTO tradePartyToUpdate);
    public Task<List<LogisticsLocation>?> GetLogisticsLocationByPostcodeAsync(string postcode);
    public Task<Guid> AddLogisticsLocationRelationship(LogisticsLocationRelationshipDTO logisticsLocationRelationshipDTO);
}