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
    public const string RegisteredBusinessEmail = "/registered-business-contact-email";
    public const string RegisteredBusinessContactPhoneNumber = "/registered-business-contact-phone";
    public const string RegisteredBusinessType = "/registered-business-type";
    public const string RegisteredBusinessAddress = "/registered-business-address";
    public const string RegisteredBusinessNatureOfBusiness = "/registered-business-nature-of-business";
    public const string EstablishmentDeparturePostcodeSearch = "/establishment-departure-postcode-search";
    public const string EstablishmentDepartureAddress = "/establishment-departure-address";
    public const string AdditionalEstablishmentDepartureAddress = "/additional-establishment-departure-address";
    public const string EstablishmentDeparturePostcodeResult = "/establishment-departure-postcode-result";
    public const string EstablishmentDepartureContactEmail = "/establishment-departure-contact-email";

    public static readonly IReadOnlyCollection<(string page, string route)> RouteList = new List<(string page, string route)>
    {
        (Pages.Path.RegisteredBusinessCountryPath, RegisteredBusinessCountry),
        (Pages.Path.RegistrationTaskListPath, RegistrationTasklist),
        (Pages.Path.RegisteredBusinessContactNamePath, RegisteredBusinessContactName),
        (Pages.Path.RegisteredBusinessContactPositionPath, RegisteredBusinessContactPosition),
        (Pages.Path.RegisteredBusinessNamePath, RegisteredBusinessName),
        (Pages.Path.RegisteredBusinessContactEmailPath, RegisteredBusinessEmail),
        (Pages.Path.RegisteredBusinessContactPhonePath, RegisteredBusinessContactPhoneNumber),
        (Pages.Path.RegisteredBusinessTypePath, RegisteredBusinessType),
        (Pages.Path.RegisteredBusinessAddressPath, RegisteredBusinessAddress),
        (Pages.Path.RegisteredBusinessNatureOfBusinessPath, RegisteredBusinessNatureOfBusiness),
        (Pages.Path.EstablishmentDeparturePostcodeSearchPath, EstablishmentDeparturePostcodeSearch),
        (Pages.Path.EstablishmentDepartureAddressPath, EstablishmentDepartureAddress),
        (Pages.Path.AdditionalEstablishmentDepartureAddressPath, AdditionalEstablishmentDepartureAddress),
        (Pages.Path.EstablishmentDeparturePostcodeResultPath, EstablishmentDeparturePostcodeResult),
        (Pages.Path.EstablishmentDepartureContactEmailPath, EstablishmentDepartureContactEmail)
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
            public const string RegisteredBusinessContactEmailName = "RegisteredBusinessContactEmail";
            public const string RegisteredBusinessContactPhoneName = "RegisteredBusinessContactPhone";
            public const string RegisteredBusinessTypeName = "RegisteredBusinessType";
            public const string RegisteredBusinessAddressName = "RegisteredBusinessAddress";
            public const string RegisteredBusinessNatureOfBusinessName = "RegisteredBusinessNatureOfBusiness";
            public const string EstablishmentDeparturePostcodeSearchName = "PostcodeSearch";
            public const string EstablishmentDepartureAddressName = "EstablishmentDepartureAddress";
            public const string AdditionalEstablishmentDepartureAddressName = "AdditionalEstablishmentDepartureAddress";
            public const string EstablishmentDeparturePostcodeResultName = "PostcodeResult";
            public const string EstablishmentDepartureContactEmailName = "ContactEmail";
        }

        public static class Path
        {
            public const string RegisteredBusinessCountryPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessCountryName}";
            public const string RegistrationTaskListPath = $"/Registration/TaskList/{Names.RegistrationTasklistName}";
            public const string RegisteredBusinessContactNamePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactNameName}";
            public const string RegisteredBusinessContactPositionPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPositionName}";
            public const string RegisteredBusinessNamePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessNameName}";
            public const string RegisteredBusinessContactEmailPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactEmailName}";
            public const string RegisteredBusinessContactPhonePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPhoneName}";
            public const string RegisteredBusinessTypePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessTypeName}";
            public const string RegisteredBusinessAddressPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessAddressName}";
            public const string RegisteredBusinessNatureOfBusinessPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessNatureOfBusinessName}";
            public const string EstablishmentDeparturePostcodeSearchPath = $"/Establishments/{Names.EstablishmentDeparturePostcodeSearchName}";
            public const string EstablishmentDepartureAddressPath = $"/Establishments/{Names.EstablishmentDepartureAddressName}";
            public const string AdditionalEstablishmentDepartureAddressPath = $"/Establishments/{Names.AdditionalEstablishmentDepartureAddressName}";
            public const string EstablishmentDeparturePostcodeResultPath = $"/Establishments/{Names.EstablishmentDeparturePostcodeResultName}";
            public const string EstablishmentDepartureContactEmailPath = $"/Establishments/{Names.EstablishmentDepartureContactEmailName}";
        }
    }
}