using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs
{
    public class TraderDTO
    {
        public Guid Id { get; set; }

        public string? PartyName { get; set; }

        public string? CountryName { get; set; }
        public string? NatureOfBusiness { get; set; }

        public TradeAddress? Address { get; set; }

        public TradeContact? Contact { get; set; }
    }
}
