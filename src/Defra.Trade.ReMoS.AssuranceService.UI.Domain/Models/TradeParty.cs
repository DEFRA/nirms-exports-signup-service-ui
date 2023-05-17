namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;

public class TradePartyCollectionResponse
{
    public List<TradeParty> TradeParties { get; set; } = default!;
}
public class RegisterTradePartyResponse
{
    public TradeParty RegisterTradeParty { get; set; } = default!;

}
public class TradeParty
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
