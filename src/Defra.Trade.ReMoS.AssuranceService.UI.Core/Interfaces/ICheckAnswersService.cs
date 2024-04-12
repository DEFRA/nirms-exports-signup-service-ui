namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ICheckAnswersService
    {
        bool ReadyForCheckAnswers(TradePartyDto tradeParty);

        string GetBusinessDetailsProgress(TradePartyDto tradeParty);

        string GetContactDetailsProgress(TradePartyDto tradeParty);

        string GetAuthorisedSignatoryProgress(TradePartyDto tradeParty);

        string GetEligibilityProgress(TradePartyDto tradeParty);

        bool IsLogisticsLocationsDataPresent(TradePartyDto tradeParty, IEnumerable<LogisticsLocationDto> logisticsLocations);
        string GetPurposeOfBusinessProgress(TradePartyDto tradeParty);
        string GetFboPhrProgress(TradePartyDto tradeParty);
    }
}