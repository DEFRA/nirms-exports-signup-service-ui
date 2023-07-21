namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class TradePartyDTO
{
    public Guid Id { get; set; }
    public string? PartyName { get; set; }
    public Guid OrgId { get; set; }
    public string? PracticeName { get; set; }
    public string? CountryName { get; set; }
    public string? NatureOfBusiness { get; set; }
    public string? FboNumber { get; set; }
    public DateTime AssuranceCommitmentsSignedDate { get; set; }
    public DateTime TermsAndConditionsSignedDate { get; set; }
    public TradeAddressDTO? Address { get; set; }
    public TradeContactDTO? Contact { get; set; }
    public AuthorisedSignatoryDto? AuthorisedSignatory { get; set; }
}
