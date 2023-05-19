using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class TradePartyDTO
{
    public Guid Id { get; set; }
    public string? PartyName { get; set; }
    public string? CountryName { get; set; }
    public string? NatureOfBusiness { get; set; }
    public TradeAddressDTO? Address { get; set; }
    public TradeContact? Contact { get; set; }
}
