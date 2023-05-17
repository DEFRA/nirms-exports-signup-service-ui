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
        public string? Name { get; set; }
        public ICollection<TradeAddress>? TradeAddresses { get; set; } = new List<TradeAddress>();
        public ICollection<TradeContact>? TradeContacts { get; set; } = new List<TradeContact>();

        public string? NatureOfBusiness { get; set; }
    }
}
