using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IAPIIntegration
{
    Task<List<TradeParty>?> GetAllTradePartiesAsync();
    public Task<TradeParty?> GetTradePartyByIdAsync(Guid id);
    Task<Guid> AddTradePartyAsync(TraderDTO tradePartyToCreate);
    Task<TradeParty> UpdateTradePartyAsync(TraderDTO tradePartyToCreate);
    Task<TradeAddressDTO> AddTradeAddressForParty(Guid partyId, TradeAddressAddUpdateDTO tradeAddressAddUpdateDTO);
}