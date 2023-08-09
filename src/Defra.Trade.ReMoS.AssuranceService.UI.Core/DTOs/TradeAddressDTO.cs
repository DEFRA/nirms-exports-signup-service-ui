namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

public class TradeAddressDto
{
    public Guid Id { get; set; }
    public string? LineOne { get; set; }
    public string? LineTwo { get; set; }
    public string? LineThree { get; set; }
    public string? LineFour { get; set; }
    public string? LineFive { get; set; }
    public string? PostCode { get; set; }
    public string? CityName { get; set; }
    public string? County { get; set; }
    public string? TradeCountry { get; set; }
}
