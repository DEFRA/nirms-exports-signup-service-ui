using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

public static class Routes
{
    public const string RegisteredBusinessCompany = "/registered-business-country";
    public const string RegistrationTasklist = "/registration-tasklist";

    public static readonly IReadOnlyCollection<(string page, string route)> RouteList = new List<(string page, string route)>
    {
        (Pages.Path.RegisteredBusinessCompanyPath, RegisteredBusinessCompany),
        (Pages.Path.RegistrationTaskListPath, RegistrationTasklist)
    };

    public static class Pages
    {
        public static class Names
        {
            public const string RegisteredBusinessCompanyName = "RegisteredBusinessCountry";
            public const string RegistrationTasklistName = "RegistrationTaskList";
        }

        public static class Path
        {
            public const string RegisteredBusinessCompanyPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessCompanyName}";
            public const string RegistrationTaskListPath = $"/Registration/TaskList/{Names.RegistrationTasklistName}";
        }
    }
}