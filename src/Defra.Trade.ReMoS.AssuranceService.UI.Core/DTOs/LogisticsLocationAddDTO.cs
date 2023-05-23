using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs
{
    public class LogisticsLocationAddDTO
    {
        public string? Name { get; set; }
        public Guid TradeAddressId { get; set; }
        public string? NI_GBFlag { get; set; }

    }
}
