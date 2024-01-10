using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs
{
    public class AuthorisedSignatoryDto
    {
        public Guid Id { get; set; }
        public Guid TradePartyId { get; set; }
        public string? Name { get; set; }
        public string? EmailAddress { get; set; }
        public string? Position { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
