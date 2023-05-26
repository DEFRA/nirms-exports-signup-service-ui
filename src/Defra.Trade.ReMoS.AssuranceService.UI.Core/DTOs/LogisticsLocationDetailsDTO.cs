namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class LogisticsLocationDetailsDTO
{
    public Guid TradePartyId { get; set; }
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? NI_GBFlag { get; set; }
    public Guid? TradeAddressId { get; set; }
    public TradeAddressDTO? Address { get; set; }
    public DateTime? RelationCreatedDate { get; set; }
    public DateTime? RelationModifiedDate { get; set; }
    public string? Status { get; set; }
    public string? ContactEmail { get; set; }
}
