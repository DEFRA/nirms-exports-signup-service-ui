namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class LogisticsLocationDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Guid? TradePartyId { get; set; }
    public Guid? TradeAddressId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public string? NI_GBFlag { get; set; }

    public TradePartyDTO? Party { get; set; }
    public TradeAddressDTO? Address { get; set; }
}
