using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs
{
    public class LogisticsLocationRelationshipDTO
    {
        public Guid TraderId { get; set; }
        public Guid EstablishmentId { get; set; }
        public string? ContactEmail { get; set; }
    }
}
