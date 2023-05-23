using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
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

        public async Task<Guid> CreateTradePartyAsync(TradePartyDTO tradePartyDTO)
        {
            return await _apiIntegration.AddTradePartyAsync(tradePartyDTO);
        }

        public async Task<Guid> UpdateTradePartyAsync(TradePartyDTO tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyAsync(tradePartyDTO);
        }

        public async Task<Guid> UpdateTradePartyAddressAsync(TradePartyDTO tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyAddressAsync(tradePartyDTO);
        }

        public async Task<Guid> UpdateTradePartyContactAsync(TradePartyDTO tradePartyDTO)
        {
            return await _apiIntegration.UpdateTradePartyContactAsync(tradePartyDTO);
        }

        public async Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid Id)
        {
            if(Id != Guid.Empty)
            {
                return await _apiIntegration.GetTradePartyByIdAsync(Id);
            }
            return new TradePartyDTO();
        }
    }
}
