using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;

[ExcludeFromCodeCoverage]
public static class TaskListStatus
{
    public const string NOTSTART = "Not started";
    public const string COMPLETE = "Completed";
    public const string INPROGRESS = "In progress";
    public const string CANNOTSTART = "Cannot start yet";
}
