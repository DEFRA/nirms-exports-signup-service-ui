using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

public static class Routes
{
    public const string RegisteredBusinessCountry = "/registered-business-country";
    public const string RegistrationTasklist = "/registration-tasklist";
    public const string RegisteredBusinessContactName = "/registered-business-contact-name";
    public const string RegisteredBusinessContactPosition = "/registered-business-contact-position";
    public const string RegisteredBusinessName = "/registered-business-name";

    public static readonly IReadOnlyCollection<(string page, string route)> RouteList = new List<(string page, string route)>
    {
        (Pages.Path.RegisteredBusinessCountryPath, RegisteredBusinessCountry),
        (Pages.Path.RegistrationTaskListPath, RegistrationTasklist),
        (Pages.Path.RegisteredBusinessContactNamePath, RegisteredBusinessContactName),
        (Pages.Path.RegisteredBusinessContactPositionPath, RegisteredBusinessContactPosition),
        (Pages.Path.RegisteredBusinessNamePath, RegisteredBusinessName)

    };

    public static class Pages
    {
        public static class Names
        {
            public const string RegisteredBusinessCountryName = "RegisteredBusinessCountry";
            public const string RegistrationTasklistName = "RegistrationTaskList";
            public const string RegisteredBusinessContactNameName = "RegisteredBusinessContactName";
            public const string RegisteredBusinessContactPositionName = "RegisteredBusinessContactPosition";
            public const string RegisteredBusinessNameName = "RegisteredBusinessName";
        }

        public static class Path
        {
            public const string RegisteredBusinessCountryPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessCountryName}";
            public const string RegistrationTaskListPath = $"/Registration/TaskList/{Names.RegistrationTasklistName}";
            public const string RegisteredBusinessContactNamePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactNameName}";
            public const string RegisteredBusinessContactPositionPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPositionName}";
            public const string RegisteredBusinessNamePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessNameName}";
        }
    }
}