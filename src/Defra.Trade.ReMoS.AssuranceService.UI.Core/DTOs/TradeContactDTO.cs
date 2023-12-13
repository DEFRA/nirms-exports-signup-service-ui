namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs
{
    public class TradeContactDto
    {
        public Guid Id { get; set; }
        public Guid TradePartyId { get; set; }
        public string? PersonName { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public bool? IsAuthorisedSignatory { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
