using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class UserService : IUserService
{
    /// <summary>
    /// Extracts all organisations for user from claims principal
    /// </summary>
    /// <param name="user"></param>
    /// <returns>A dictionary of user's organisations as key, name pairs</returns>
    public List<Organisation> GetDefraOrgsForUser(ClaimsPrincipal user)
    {
        var orgs = new List<Organisation>();
        orgs.AddRange(GetOrgs(user, "userEnrolledOrganisations")!);
        foreach (var org in orgs)
        {
            org.Enrolled = true;
        }
        orgs.AddRange(GetOrgs(user, "notEnrolledUserOrganisations"));
        return orgs;
    }

    /// <summary>
    /// Extracts user's contact id from claims principal
    /// </summary>
    /// <param name="user"></param>
    /// <returns>User's contact id</returns>
    public Guid GetUserContactId(ClaimsPrincipal user)
    {
        var userContactIdClaim = user.FindFirst("contactId");
        
        if (userContactIdClaim == null)
            return Guid.Empty;

        return Guid.Parse(userContactIdClaim.Value);
    }

    public Organisation? GetOrgDetailsById(ClaimsPrincipal user, Guid orgId)
    {
        var orgs = GetDefraOrgsForUser(user);
        return orgs.Find(org => org.OrganisationId == orgId);
    }

    private static List<Organisation> GetOrgs(ClaimsPrincipal user, string orgType)
    {
        var orgsClaim = user.FindFirst(orgType);

        if (orgsClaim == null)
            return new List<Organisation>();

        var orgs = JsonConvert.DeserializeObject<List<Organisation>>(orgsClaim.Value);

        if (orgs == null)
        {
            return new List<Organisation>();
        }

        orgs = RemoveDuplicates(orgs);

        return orgs;
    }   

    private static List<Organisation> RemoveDuplicates(List<Organisation> orgs)
    {
        var n = orgs.Count;

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (orgs[i].Equals(orgs[j]))
                {
                    orgs.RemoveAt(j);
                    j--;
                    n--;
                }
            }
        }

        var distinctList = orgs.Take(n).ToList();
        return distinctList;
    }
}
