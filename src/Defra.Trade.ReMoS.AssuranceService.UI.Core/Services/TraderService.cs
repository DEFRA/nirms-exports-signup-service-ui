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

        /// <summary>
        /// Calculates a business's sign-up progress by inspecting data related to various stages
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>A tuple containing the business and its sign-up status</returns>
        public async Task<(TradePartyDto? tradeParty, TradePartySignupStatus signupStatus)> GetDefraOrgBusinessSignupStatus(Guid orgId)
        {
            var tradeParty = await _apiIntegration.GetTradePartyByOrgIdAsync(orgId);
            var signupStatus = TradePartySignupStatus.New;

            //if org is rejected, we need to blank it out to allow a retry
            if (tradeParty != null && tradeParty.ApprovalStatus == TradePartyApprovalStatus.Rejected)
                tradeParty = null;

            if (tradeParty == null || tradeParty.Address == null)
                signupStatus = TradePartySignupStatus.New;

            else if (tradeParty.TermsAndConditionsSignedDate != default && tradeParty.TermsAndConditionsSignedDate != DateTime.MinValue)
                signupStatus = TradePartySignupStatus.Complete;
            else if (tradeParty.Address != null)
            {
                if (tradeParty.Address.TradeCountry != null && !string.IsNullOrEmpty(tradeParty.FboPhrOption) && tradeParty.RegulationsConfirmed)
                    signupStatus = TradePartySignupStatus.InProgress;
                else if (tradeParty.Address.TradeCountry == null)
                    signupStatus = TradePartySignupStatus.InProgressEligibilityCountry;
                else if (tradeParty.FboPhrOption == null)
                    signupStatus = TradePartySignupStatus.InProgressEligibilityFboNumber;
                else if(!tradeParty.RegulationsConfirmed)
                    signupStatus = TradePartySignupStatus.InProgressEligibilityRegulations;
            }

            return (tradeParty, signupStatus);
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
