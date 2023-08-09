using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ICheckAnswersService
    {
        string GetBusinessDetailsProgress(TradePartyDto tradeParty);
        string GetContactDetailsProgress(TradePartyDto tradeParty);
        string GetAuthorisedSignatoryProgress(TradePartyDto tradeParty);
        string GetEligibilityProgress(TradePartyDto tradeParty);
    }
}