using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ITraderService
    {
        public Task<Guid> CreateTradePartyAsync(TradePartyDTO tradePartyDTO);
        public Task<Guid> UpdateTradePartyAsync(TradePartyDTO tradePartyDTO);
        public Task<Guid> UpdateTradePartyAddressAsync(TradePartyDTO tradePartyDTO);
        public Task<Guid> UpdateTradePartyContactAsync(TradePartyDTO tradePartyDTO);
        public Task<TradePartyDTO?> UpdateAuthorisedSignatoryAsync(TradePartyDTO tradePartyDTO);
        public Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid Id);
        Task<Guid> AddTradePartyAddressAsync(Guid partyId, TradeAddressDTO addressDTO);
        Task<(TradePartyDTO? tradeParty, TradePartySignupStatus signupStatus)> GetDefraOrgBusinessSignupStatus(Guid orgId);
    }
}
