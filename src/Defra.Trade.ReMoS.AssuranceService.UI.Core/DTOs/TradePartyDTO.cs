namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class TradePartyDTO
{
    public Guid Id { get; set; }
    public string? PartyName { get; set; }
    public string? CountryName { get; set; }
    public string? NatureOfBusiness { get; set; }
    public string? FboNumber { get; set; }
    public TradeAddressDTO? Address { get; set; }
    public TradeContactDTO? Contact { get; set; }
    public AuthorisedSignatoryDTO? AuthorisedSignatory { get; set; }
}
