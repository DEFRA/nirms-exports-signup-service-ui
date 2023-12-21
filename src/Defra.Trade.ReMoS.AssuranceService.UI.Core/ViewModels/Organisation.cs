using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;

public class Organisation
{
    [JsonProperty("organisationId")]
    public Guid OrganisationId { get; set; } = Guid.Empty;
    
    [JsonProperty("practiceName")]
    public string PracticeName { get; set; } = default!;
    
    [JsonProperty("userRole")]
    public string UserRole { get; set; } = default!;

    public bool Enrolled { get; set; } = default!;
    public TradePartyApprovalStatus ApprovalStatus { get; set; } = TradePartyApprovalStatus.NotSignedUp;

    public override bool Equals(object? obj)
    {
        var item = obj as Organisation;

        if (item == null || item.OrganisationId == Guid.Empty || item.PracticeName == string.Empty || item.PracticeName == null)
        {
            return false;
        }

        return this.OrganisationId.Equals(item.OrganisationId) 
            && this.PracticeName.Equals(item.PracticeName);
    }

    public override int GetHashCode()
    {
        return this.OrganisationId.GetHashCode();
    }
}
