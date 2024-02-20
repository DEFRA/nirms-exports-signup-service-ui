using System.ComponentModel;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;

public enum LogisticsLocationApprovalStatus
{
    [Description("None")]
    None,
    [Description("Active")]
    Approved,
    [Description("Rejected")]
    Rejected,
    [Description("Draft")]
    Draft,
    [Description("Pending approval")]
    PendingApproval,
    [Description("Suspended")]
    Suspended,
    [Description("Removed")]
    Removed
}
