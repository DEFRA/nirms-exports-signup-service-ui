namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class LogisticsLocationDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public string? NI_GBFlag { get; set; }
    public Guid? TradeAddressId { get; set; }
    public TradeAddressDTO? Address { get; set; }
    //public virtual LogisticLocationBusinessRelationship? EstablishmentBusinessRelationship { get; set; }
}
