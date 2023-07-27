using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class UserService : IUserService
{
    public Dictionary<Guid, string> GetDefraOrgsForUser(ClaimsPrincipal user)
    {
        var userOrgsClaim = user.FindFirst("userEnrolledOrganisations");
        Dictionary<Guid, string> orgsDict = new Dictionary<Guid, string>();

        if (userOrgsClaim == null)
            return orgsDict;

        var orgs = JsonConvert.DeserializeObject<List<Organisation>>(userOrgsClaim.Value);

        if (orgs != null && orgs.Count > 0)
        {
            foreach (var org in orgs)
            {
                if (!orgsDict.ContainsKey(org.organisationId))
                {
                    orgsDict.Add(org.organisationId, org.practiceName);
                }
            }
        }

        return orgsDict;
    }

    public Guid GetUserContactId(ClaimsPrincipal user)
    {
        var userContactIdClaim = user.FindFirst("contactId");
        
        if (userContactIdClaim == null)
            return Guid.Empty;

        return Guid.Parse(userContactIdClaim.Value);
    }
}
