using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services
{
    public class TraderService : ITraderService
    {
        private readonly IAPIIntegration _apiIntegration;

        public TraderService(IAPIIntegration apiIntegration)
        {
            _apiIntegration = apiIntegration;
        }

        public async Task<Guid> CreateTradePartyAsync(TradePartyDto tradePartyDTO)
        {
            return await _apiIntegration.AddTradePartyAsync(tradePartyDTO);
        }

        public async Task<Guid> UpdateTradePartyAsync(TradePartyDto tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyAsync(tradePartyDTO);
        }

        public async Task<Guid> UpdateTradePartyAddressAsync(TradePartyDto tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyAddressAsync(tradePartyDTO);
        }

        public async Task<Guid> AddTradePartyAddressAsync(Guid partyId, TradeAddressDto addressDTO)
        {
            return await _apiIntegration.AddAddressToPartyAsync(partyId, addressDTO);
        }

        public async Task<Guid> UpdateTradePartyContactAsync(TradePartyDto tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyContactAsync(tradePartyDTO);
        }

        public async Task<TradePartyDto?> GetTradePartyByIdAsync(Guid Id)
        {
            if(Id != Guid.Empty)
            {
                return await _apiIntegration.GetTradePartyByIdAsync(Id);
            }
            return new TradePartyDto();
        }

        public async Task<TradePartyDto?> UpdateAuthorisedSignatoryAsync(TradePartyDto tradePartyDTO)
        {
            return await _apiIntegration.UpdateAuthorisedSignatoryAsync(tradePartyDTO);
        }

        public async Task<(TradePartyDto? tradeParty, TradePartySignupStatus signupStatus)> GetDefraOrgBusinessSignupStatus(Guid orgId)
        {
            var tradeParty = await _apiIntegration.GetTradePartyByOrgIdAsync(orgId);
            var signupStatus = TradePartySignupStatus.New;

            if (tradeParty == null || tradeParty?.Address == null)
                signupStatus = TradePartySignupStatus.New;
            else if (tradeParty?.TermsAndConditionsSignedDate != default && tradeParty?.TermsAndConditionsSignedDate != DateTime.MinValue)
                signupStatus = TradePartySignupStatus.Complete;
            else if (tradeParty?.Address != null)
            {
                if (tradeParty.Address.TradeCountry != null && !string.IsNullOrEmpty(tradeParty.FboNumber) && tradeParty.RegulationsConfirmed)
                    signupStatus = TradePartySignupStatus.InProgress;
                else if (tradeParty.Address.TradeCountry == null)
                    signupStatus = TradePartySignupStatus.InProgressEligibilityCountry;
                else if (tradeParty.FboNumber == null)
                    signupStatus = TradePartySignupStatus.InProgressEligibilityFboNumber;
                else if(!tradeParty.RegulationsConfirmed)
                    signupStatus = TradePartySignupStatus.InProgressEligibilityRegulations;
            }

            return (tradeParty, signupStatus);
        }
    }
}
