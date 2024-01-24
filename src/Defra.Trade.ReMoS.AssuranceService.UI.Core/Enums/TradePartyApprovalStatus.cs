using System.ComponentModel;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;

public enum TradePartyApprovalStatus
{
    [Description("Not signed-up")]
    NotSignedUp,

    [Description("Approved for NIRMS")]
    Approved,

    [Description("Sign-up rejected")]
    Rejected,    
    
    [Description("Sign-up started")] 
    SignupStarted,

    [Description("Pending approval")] 
    PendingApproval
}
