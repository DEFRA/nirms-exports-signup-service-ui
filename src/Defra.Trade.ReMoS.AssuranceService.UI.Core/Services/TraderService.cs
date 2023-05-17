using Defra.Trade.ReMoS.AssuranceService.API.Domain.DTO;
using Defra.Trade.ReMoS.AssuranceService.API.Domain.Entities;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
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
        private IAPIIntegration _apiIntegration;

        public TraderService(IAPIIntegration apiIntegration)
        {
            _apiIntegration = apiIntegration;
        }

        public async Task<TradeParty> CreateTradePartyAsync(TradePartyDTO tradePartyDTO)
        {
            return await _apiIntegration.AddTradePartyAsync(tradePartyDTO);
        }

        public async Task<TradeParty> UpdateTradePartyAsync(TradePartyDTO tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyAsync(tradePartyDTO);
        }
    }
}
