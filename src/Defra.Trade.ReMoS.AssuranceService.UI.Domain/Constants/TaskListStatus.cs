using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

public class TaskListStatus
{
    public static readonly string NOTSTART = "Not Started";
    public static readonly string COMPLETE = "Completed";
    public static readonly string INPROGRESS = "In Progress";
    public static readonly string CANNOTSTART = "Cannot Start Yet";
}
