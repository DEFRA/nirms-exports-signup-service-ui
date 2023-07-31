using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IUserService
{
    Dictionary<Guid, string> GetDefraOrgsForUser(ClaimsPrincipal user);
    Guid GetUserContactId(ClaimsPrincipal user);
}
