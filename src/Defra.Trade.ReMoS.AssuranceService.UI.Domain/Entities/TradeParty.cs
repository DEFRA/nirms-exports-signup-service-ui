using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities
{
    public class TradeParty
    {
        public Guid Id { get; set; }
        public string? PartyName { get; set; }
        public string? NatureOfBusiness { get; set; }
        public TradeContact? TradeContact { get; set; }
        public TradeAddress? TradeAddress { get; set; }
    }
}
