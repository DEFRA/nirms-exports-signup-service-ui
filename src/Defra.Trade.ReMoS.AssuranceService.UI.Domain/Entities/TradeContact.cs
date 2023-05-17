using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities
{
    public class TradeContact
    {
        public Guid Id { get; set; }
        public string? PersonName { get; set; }
        public string? TelephoneNumber { get; set; }
    }
}
