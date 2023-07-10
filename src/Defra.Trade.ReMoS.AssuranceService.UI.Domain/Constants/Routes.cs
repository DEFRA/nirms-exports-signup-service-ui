namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

public static class Routes
{
    public const string RegisteredBusinessCountry = "/registered-business-country";
    public const string RegisteredBusinessRegulations = "/registered-business-regulations";
    public const string RegisteredBusinessFboNumber = "/registered-business-fbo-number";
    public const string RegisteredBusinessCanNotRegister = "/registered-business-cannot-register";
    public const string RegistrationTasklist = "/registration-tasklist";
    public const string RegisteredBusinessContactName = "/registered-business-contact-name";
    public const string RegisteredBusinessContactPosition = "/registered-business-contact-position";
    public const string RegisteredBusinessName = "/registered-business-name";
    public const string RegisteredBusinessEmail = "/registered-business-contact-email";
    public const string RegisteredBusinessContactPhoneNumber = "/registered-business-contact-phone";
    public const string RegisteredBusinessType = "/registered-business-type";
    public const string RegisteredBusinessAddress = "/registered-business-address";
    public const string RegisteredBusinessNatureOfBusiness = "/registered-business-nature-of-business";
    public const string EstablishmentPostcodeSearch = "/establishment-postcode-search";
    public const string EstablishmentNameAndAddress = "/establishment-name-address";
    public const string EstablishmentPostcodeResult = "/establishment-postcode-result";
    public const string EstablishmentContactEmail = "/establishment-contact-email";
    public const string AdditionalEstablishmentAddress = "/additional-establishment-address";
    public const string AuthorisedSignatoryDetails = "/authorised-signatory-details";
    public const string AuthorisedSignatoryName = "/authorised-signatory-name";
    public const string AuthorisedSignatoryPosition = "/authorised-signatory-position";
    public const string AuthorisedSignatoryEmail = "/authorised-signatory-email";
    public const string RegistrationCheckYourAnswers = "/registration-check-your-answers";
    public const string TermsAndConditions = "/registration-terms-and-conditions";
    public const string SignUpConfirmation = "/confirmation";
    public const string RegisteredBusiness = "/registration-business";

    public static readonly IReadOnlyCollection<(string page, string route)> RouteList = new List<(string page, string route)>
    {
        (Pages.Path.RegisteredBusinessCountryPath, RegisteredBusinessCountry),
        (Pages.Path.RegisteredBusinessRegulationsPath, RegisteredBusinessRegulations),
        (Pages.Path.RegisteredBusinessFboNumberPath, RegisteredBusinessFboNumber),
        (Pages.Path.RegisteredBusinessCanNotRegisterPath, RegisteredBusinessCanNotRegister),
        (Pages.Path.RegistrationTaskListPath, RegistrationTasklist),
        (Pages.Path.RegisteredBusinessContactNamePath, RegisteredBusinessContactName),
        (Pages.Path.RegisteredBusinessContactPositionPath, RegisteredBusinessContactPosition),
        (Pages.Path.RegisteredBusinessNamePath, RegisteredBusinessName),
        (Pages.Path.RegisteredBusinessContactEmailPath, RegisteredBusinessEmail),
        (Pages.Path.RegisteredBusinessContactPhonePath, RegisteredBusinessContactPhoneNumber),
        (Pages.Path.RegisteredBusinessTypePath, RegisteredBusinessType),
        (Pages.Path.RegisteredBusinessAddressPath, RegisteredBusinessAddress),
        (Pages.Path.RegisteredBusinessNatureOfBusinessPath, RegisteredBusinessNatureOfBusiness),
        (Pages.Path.EstablishmentPostcodeSearchPath, EstablishmentPostcodeSearch),
        (Pages.Path.EstablishmentNameAndAddressPath, EstablishmentNameAndAddress),
        (Pages.Path.AdditionalEstablishmentAddressPath, AdditionalEstablishmentAddress),
        (Pages.Path.EstablishmentPostcodeResultPath, EstablishmentPostcodeResult),
        (Pages.Path.EstablishmentContactEmailPath, EstablishmentContactEmail),
        (Pages.Path.AuthorisedSignatoryDetailsPath, AuthorisedSignatoryDetails),
        (Pages.Path.AuthorisedSignatoryNamePath, AuthorisedSignatoryName),
        (Pages.Path.AuthorisedSignatoryEmailPath, AuthorisedSignatoryEmail),
        (Pages.Path.AuthorisedSignatoryPositionPath, AuthorisedSignatoryPosition),
        (Pages.Path.RegistrationCheckYourAnswersPath, RegistrationCheckYourAnswers),
        (Pages.Path.TermsAndConditionsPath, TermsAndConditions),
        (Pages.Path.SignUpConfirmationPath, SignUpConfirmation),
        (Pages.Path.RegisteredBusinessPath, RegisteredBusiness)
    };

    public static class Pages
    {
        public static class Names
        {
            public const string RegisteredBusinessCountryName = "RegisteredBusinessCountry";
            public const string RegisteredBusinessRegulationsName = "EligibilityRegulations";
            public const string RegisteredBusinessFboNumberName = "RegisteredBusinessFboNumber";
            public const string RegisteredBusinessCanNotRegisterName = "RegisteredBusinessCanNotRegister";
            public const string RegistrationTasklistName = "RegistrationTaskList";
            public const string RegisteredBusinessContactNameName = "RegisteredBusinessContactName";
            public const string RegisteredBusinessContactPositionName = "RegisteredBusinessContactPosition";
            public const string RegisteredBusinessNameName = "RegisteredBusinessName";
            public const string RegisteredBusinessContactEmailName = "RegisteredBusinessContactEmail";
            public const string RegisteredBusinessContactPhoneName = "RegisteredBusinessContactPhone";
            public const string RegisteredBusinessTypeName = "RegisteredBusinessType";
            public const string RegisteredBusinessAddressName = "RegisteredBusinessAddress";
            public const string RegisteredBusinessNatureOfBusinessName = "RegisteredBusinessNatureOfBusiness";
            public const string EstablishmentPostcodeSearchName = "PostcodeSearch";
            public const string EstablishmentNameAndAddressName = "EstablishmentNameAndAddress";
            public const string AdditionalEstablishmentAddressName = "AdditionalEstablishmentAddress";
            public const string EstablishmentPostcodeResultName = "PostcodeResult";
            public const string EstablishmentContactEmailName = "ContactEmail";
            public const string AuthorisedSignatoryDetailsName = "IsAuthorisedSignatory";
            public const string AuthorisedSignatoryNameName = "AuthorisedSignatoryName";
            public const string AuthorisedSignatoryEmailName = "AuthorisedSignatoryEmail";
            public const string AuthorisedSignatoryPositionName = "AuthorisedSignatoryPosition";
            public const string RegistrationCheckYourAnwersName = "CheckYourAnswers";
            public const string TermsAndConditionsName = "TermsAndConditions";
            public const string SignUpConfirmationName = "SignUpConfirmation";
            public const string RegisteredBusinessBusinessName = "RegisteredBusiness";
        }

        public static class Path
        {
            public const string RegisteredBusinessCountryPath = $"/Registration/RegisteredBusiness/Eligibility/{Names.RegisteredBusinessCountryName}";
            public const string RegisteredBusinessRegulationsPath = $"/Registration/RegisteredBusiness/Eligibility/{Names.RegisteredBusinessRegulationsName}";
            public const string RegisteredBusinessFboNumberPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessFboNumberName}";
            public const string RegisteredBusinessCanNotRegisterPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessCanNotRegisterName}";
            public const string RegistrationTaskListPath = $"/Registration/TaskList/{Names.RegistrationTasklistName}";
            public const string RegisteredBusinessContactNamePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactNameName}";
            public const string RegisteredBusinessContactPositionPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPositionName}";
            public const string RegisteredBusinessNamePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessNameName}";
            public const string RegisteredBusinessContactEmailPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactEmailName}";
            public const string RegisteredBusinessContactPhonePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPhoneName}";
            public const string RegisteredBusinessTypePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessTypeName}";
            public const string RegisteredBusinessAddressPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessAddressName}";
            public const string RegisteredBusinessNatureOfBusinessPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessNatureOfBusinessName}";
            public const string EstablishmentPostcodeSearchPath = $"/Establishments/{Names.EstablishmentPostcodeSearchName}";
            public const string EstablishmentNameAndAddressPath = $"/Establishments/{Names.EstablishmentNameAndAddressName}";
            public const string AdditionalEstablishmentAddressPath = $"/Establishments/{Names.AdditionalEstablishmentAddressName}";
            public const string EstablishmentPostcodeResultPath = $"/Establishments/{Names.EstablishmentPostcodeResultName}";
            public const string EstablishmentContactEmailPath = $"/Establishments/{Names.EstablishmentContactEmailName}";
            public const string AuthorisedSignatoryDetailsPath = $"/Registration/RegisteredBusiness/AuthorisedSignatory/{Names.AuthorisedSignatoryDetailsName}";
            public const string AuthorisedSignatoryNamePath = $"/Registration/RegisteredBusiness/AuthorisedSignatory/{Names.AuthorisedSignatoryNameName}";
            public const string AuthorisedSignatoryPositionPath = $"/Registration/RegisteredBusiness/AuthorisedSignatory/{Names.AuthorisedSignatoryPositionName}";
            public const string AuthorisedSignatoryEmailPath = $"/Registration/RegisteredBusiness/AuthorisedSignatory/{Names.AuthorisedSignatoryEmailName}";
            public const string RegistrationCheckYourAnswersPath = $"/Registration/CheckYourAnswers/{Names.RegistrationCheckYourAnwersName}";
            public const string TermsAndConditionsPath = $"/Registration/Assurances/{Names.TermsAndConditionsName}";
            public const string SignUpConfirmationPath = $"/Registration/Confirmation/{Names.SignUpConfirmationName}";
            public const string RegisteredBusinessPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessBusinessName}";
        }
    }
}