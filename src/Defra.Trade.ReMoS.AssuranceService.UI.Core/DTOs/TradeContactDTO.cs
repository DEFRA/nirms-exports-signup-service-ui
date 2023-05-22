namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs
{
    public class TradeContactDTO
    {
        public Guid Id { get; set; }
        public Guid TradePartyId { get; set; }
        public string? PersonName { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? EmailAddress { get; set; }
    }
}
