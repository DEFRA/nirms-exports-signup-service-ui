using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class LogisticsLocationBusinessRelationshipDTO
{
    public Guid RelationshipId { get; set; }
    public Guid TradePartyId { get; set; }
    public Guid LogisticsLocationId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? Status { get; set; }
    public string? ContactEmail { get; set; }

    //public virtual LogisticsLocation? LogisticsLocation { get; set; }
    //public virtual TradeParty? TradeParty { get; set; }
}
