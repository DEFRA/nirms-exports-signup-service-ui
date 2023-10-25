using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IUserService
{
    List<Organisation> GetDefraOrgsForUser(ClaimsPrincipal user);
    Organisation? GetOrgDetailsById(ClaimsPrincipal user, Guid orgId);
    Guid GetUserContactId(ClaimsPrincipal user);
}
