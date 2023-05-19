using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IAPIIntegration
{
    Task<List<TradePartyDTO>?> GetAllTradePartiesAsync();
    public Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid id);
    Task<Guid> AddTradePartyAsync(TradePartyDTO tradePartyToCreate);
    Task<Guid> UpdateTradePartyAsync(TradePartyDTO tradePartyToUpdate);
    //Task<TradeAddressDTO> AddTradeAddressForParty(Guid partyId, TradeAddressAddUpdateDTO tradeAddressAddUpdateDTO);
}