using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services
{
    public class CheckAnswersService : ICheckAnswersService
    {
        public bool ReadyForCheckAnswers(TradePartyDto tradeParty)
        {
            if (tradeParty == null)
            {
                return false;
            }

            if (GetBusinessDetailsProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetEligibilityProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetPurposeOfBusinessProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetFboPhrProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetContactDetailsProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            if (GetAuthorisedSignatoryProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return false;
            }

            return true;
        }

        public string GetBusinessDetailsProgress(TradePartyDto tradeParty)
        {
            if (tradeParty.PracticeName != null
                && tradeParty.Address != null
                && tradeParty.Address.TradeCountry != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty.PracticeName != null
                && (tradeParty.Address == null || tradeParty.Address?.TradeCountry == null))
            {
                return TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetContactDetailsProgress(TradePartyDto tradeParty)
        {
            if (tradeParty.Contact != null && tradeParty.Contact.PersonName != null && tradeParty.Contact.Email != null && tradeParty.Contact.TelephoneNumber != null && tradeParty.Contact.Position != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty.Contact != null || tradeParty.Contact?.PersonName != null || tradeParty.Contact?.Email != null || tradeParty.Contact?.TelephoneNumber != null || tradeParty.Contact?.Position != null)
            {
                return TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetAuthorisedSignatoryProgress(TradePartyDto tradeParty)
        {
            if (tradeParty.Contact == null || tradeParty.Contact?.PersonName == null || tradeParty.Contact?.Email == null || tradeParty.Contact?.TelephoneNumber == null || tradeParty.Contact?.Position == null)
            {
                return TaskListStatus.CANNOTSTART;
            }

            if (tradeParty.Contact?.IsAuthorisedSignatory == true)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty.Contact?.IsAuthorisedSignatory == false && tradeParty.AuthorisedSignatory?.Name != null && tradeParty.AuthorisedSignatory.Position != null && tradeParty.AuthorisedSignatory.EmailAddress != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty.Contact?.IsAuthorisedSignatory == false && (tradeParty.AuthorisedSignatory == null || (tradeParty.AuthorisedSignatory?.Id != null || tradeParty.AuthorisedSignatory?.Id != Guid.Empty)))
            {
                return TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetEligibilityProgress(TradePartyDto tradeParty)
        {
            if (tradeParty.Address != null)
            {
                if (tradeParty.Address!.TradeCountry != null && tradeParty.RegulationsConfirmed)
                {
                    return TaskListStatus.COMPLETE;
                }
                if (tradeParty.Address!.TradeCountry == null || !tradeParty.RegulationsConfirmed)
                {
                    return TaskListStatus.INPROGRESS;
                }
            }
            return TaskListStatus.NOTSTART;
        }

        public bool IsLogisticsLocationsDataPresent(TradePartyDto? tradeParty, IEnumerable<LogisticsLocationDto> logisticsLocations)
        {
            if (tradeParty == null || logisticsLocations == null || !logisticsLocations.Any())
            {
                return false;
            }
            return true;
        }

        public string GetPurposeOfBusinessProgress(TradePartyDto tradeParty)
        {
            if (tradeParty.Address != null)
            {                
                return tradeParty.Address!.TradeCountry != null ? TaskListStatus.COMPLETE : TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetFboPhrProgress(TradePartyDto tradeParty)
        {
            return string.IsNullOrEmpty(tradeParty.FboPhrOption) ? TaskListStatus.NOTSTART : TaskListStatus.COMPLETE;
        }
    }
}