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
    public const string RegisteredBusinessContactName = "/registered-business-contact-name";
    public const string RegisteredBusinessContactPosition = "/registered-business-contact-position";
    public const string RegisteredBusinessName = "/registered-business-name";
    public const string RegisteredBusinessEmail = "/registered-business-contact-email";

    public static readonly IReadOnlyCollection<(string page, string route)> RouteList = new List<(string page, string route)>
    {
        (Pages.Path.RegisteredBusinessCompanyPath, RegisteredBusinessCompany),
        (Pages.Path.RegistrationTaskListPath, RegistrationTasklist),
        (Pages.Path.RegisteredBusinessContactNamePath, RegisteredBusinessContactName),
        (Pages.Path.RegisteredBusinessContactPositionPath, RegisteredBusinessContactPosition),
        (Pages.Path.RegisteredBusinessContactEmailPath, RegisteredBusinessEmail)
    };

    public static class Pages
    {
        public static class Names
        {
            public const string RegisteredBusinessCompany = "RegisteredBusinessCountry";
            public const string RegistrationTasklist = "RegistrationTaskList";
            public const string RegisteredBusinessContactName = "RegisteredBusinessContactName";
            public const string RegisteredBusinessContactPosition = "RegisteredBusinessContactPosition";
            public const string RegisteredBusinessName = "RegisteredBusinessName";
            public const string RegisteredBusinessContactEmail = "RegisteredBusinessContactEmail";
        }

        public static class Path
        {
            public const string RegisteredBusinessCompanyPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessCompany}";
            public const string RegistrationTaskListPath = $"/Registration/TaskList/{Names.RegistrationTasklist}";
            public const string RegisteredBusinessContactNamePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactName}";
            public const string RegisteredBusinessContactPositionPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPosition}";
            public const string RegisteredBusinessNamePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessName}";
            public const string RegisteredBusinessContactEmailPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactEmail}";
        }
    }
}