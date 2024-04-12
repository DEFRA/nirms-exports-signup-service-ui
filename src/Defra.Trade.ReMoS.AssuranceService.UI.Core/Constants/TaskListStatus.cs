using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;

[ExcludeFromCodeCoverage]
public static class TaskListStatus
{
    public const string NOTSTART = "Not started";
    public const string COMPLETE = "Completed";
    public const string INPROGRESS = "In progress";
    public const string CANNOTSTART = "Cannot start yet";
}
