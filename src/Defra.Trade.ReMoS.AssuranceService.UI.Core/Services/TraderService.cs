using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services
{
    public class TraderService : ITraderService
    {
        private readonly IApiIntegration _apiIntegration;

        public TraderService(IApiIntegration apiIntegration)
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

        public async Task<Guid> UpdateTradePartyContactSelfServeAsync(TradePartyDto tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyContactSelfServeAsync(tradePartyDTO);
        }

        public async Task<TradePartyDto?> GetTradePartyByIdAsync(Guid Id)
        {
            if (Id != Guid.Empty)
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

            if (tradeParty == null)
            {
                return (tradeParty, TradePartySignupStatus.New);
            }

            if (tradeParty.ApprovalStatus == TradePartyApprovalStatus.Rejected)
            {
                tradeParty = null;
                return (tradeParty, TradePartySignupStatus.New);
            }

            if (!tradeParty.RegulationsConfirmed)
            {
                return (tradeParty, TradePartySignupStatus.InProgressEligibilityRegulations);
            }

            if (tradeParty.Address == null)
            {
                return (tradeParty, TradePartySignupStatus.InProgressEligibilityCountry);
            }

            if (tradeParty.TermsAndConditionsSignedDate != default && tradeParty.TermsAndConditionsSignedDate != DateTime.MinValue)
            {
                return (tradeParty, TradePartySignupStatus.Complete);
            }

            if (tradeParty.Address.TradeCountry == null)
            {
                return (tradeParty, TradePartySignupStatus.InProgressEligibilityCountry);
            }

            return (tradeParty, TradePartySignupStatus.InProgress);
        }

        /// <summary>
        /// Validates if a given organisation is present in user's claims
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="id"></param>
        /// <returns><c>true</c> if organisation is present in user's claims, <c>false</c> otherwise</returns>
        public async Task<bool> ValidateOrgId(IEnumerable<Claim> claims, Guid id)
        {
            var tradeParty = await _apiIntegration.GetTradePartyByIdAsync(id);
            var userEnrolledOrganisations = claims.ToList().Find(c => c.Type == "userEnrolledOrganisations")!.Value;
            var str = tradeParty?.OrgId.ToString();
            if (str != null)
            {
                return userEnrolledOrganisations.Contains(str);
            }
            return false;
        }

        public async Task<bool> IsTradePartySignedUp(Guid id)
        {
            var tradeParty = await _apiIntegration.GetTradePartyByIdAsync(id);

            return (tradeParty?.SignUpRequestSubmittedBy) != Guid.Empty;
        }

    }
}
