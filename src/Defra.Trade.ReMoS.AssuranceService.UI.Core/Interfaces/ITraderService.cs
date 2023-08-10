using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ITraderService
    {
        public Task<Guid> CreateTradePartyAsync(TradePartyDto tradePartyDTO);
        public Task<Guid> UpdateTradePartyAsync(TradePartyDto tradePartyDTO);
        public Task<Guid> UpdateTradePartyAddressAsync(TradePartyDto tradePartyDTO);
        public Task<Guid> UpdateTradePartyContactAsync(TradePartyDto tradePartyDTO);
        public Task<TradePartyDto?> UpdateAuthorisedSignatoryAsync(TradePartyDto tradePartyDTO);
        public Task<TradePartyDto?> GetTradePartyByIdAsync(Guid Id);
        Task<Guid> AddTradePartyAddressAsync(Guid partyId, TradeAddressDto addressDTO);
        Task<(TradePartyDto? tradeParty, TradePartySignupStatus signupStatus)> GetDefraOrgBusinessSignupStatus(Guid orgId);
        Task<bool> ValidateOrgId(IEnumerable<Claim> claims, Guid id);
    }
}
