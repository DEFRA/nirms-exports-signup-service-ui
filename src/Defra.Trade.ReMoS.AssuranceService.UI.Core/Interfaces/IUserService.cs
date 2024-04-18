using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IUserService
{
    List<Organisation> GetDefraOrgsForUser(ClaimsPrincipal user);
    Organisation? GetOrgDetailsById(ClaimsPrincipal user, Guid orgId);
    Guid GetUserContactId(ClaimsPrincipal user);
}
