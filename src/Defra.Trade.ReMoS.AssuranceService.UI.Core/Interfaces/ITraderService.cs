using Defra.Trade.ReMoS.AssuranceService.API.Domain.DTO;
using Defra.Trade.ReMoS.AssuranceService.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces
{
    public interface ITraderService
    {
        public Task<TradeParty> CreateTradePartyAsync(TradePartyDTO tradePartyDTO);
        public Task<TradeParty> UpdateTradePartyAsync(TradePartyDTO tradePartyDTO);
    }
}
