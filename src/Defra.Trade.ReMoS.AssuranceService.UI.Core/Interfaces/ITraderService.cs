using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ITraderService
    {
        public Task<TradeParty> CreateTradePartyAsync(TraderDTO tradePartyDTO);
        public Task<TradeParty> UpdateTradePartyAsync(TraderDTO tradePartyDTO);
    }
}
