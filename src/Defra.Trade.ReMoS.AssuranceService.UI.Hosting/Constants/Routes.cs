namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;

public static class Routes
{
    public const string SelectedBusiness = "/selected-business";
    public const string RegisteredBusinessBusinessPicker = "/registered-business-picker";
    public const string RegisteredBusinessNoBusinessChosen = "/registered-business-not-chosen";
    public const string RegisteredBusinessAlreadyRegistered = "/business-already-registered";
    public const string RegisteredBusinessCountry = "/registered-business-country";
    public const string RegisteredBusinessCountryStatic = "/registered-business-country-complete";
    public const string RegisteredBusinessRegulations = "/registered-business-regulations";
    public const string RegisteredBusinessFboNumber = "/registered-business-fbo-number";
    public const string RegisteredBusinessFboPhrGuidance = "/registered-business-fbo-phr-guidance";
    public const string RegistrationTasklist = "/registration-tasklist";
    public const string RegisteredBusinessContactName = "/registered-business-contact-name";
    public const string RegisteredBusinessContactPosition = "/registered-business-contact-position";
    public const string RegisteredBusinessName = "/registered-business-name";
    public const string RegisteredBusinessEmail = "/registered-business-contact-email";
    public const string RegisteredBusinessContactPhoneNumber = "/registered-business-contact-phone";
    public const string RegisteredBusinessType = "/registered-business-type";
    public const string RegisteredBusinessAddress = "/registered-business-address";
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
    public const string RegistrationTermsAndConditions = "/registration-terms-and-conditions";
    public const string SignUpConfirmation = "/confirmation";
    public const string TermsAndConditions = "/terms-conditions";
    public const string ContactDetails = "/contact";
    public const string AccessibilityStatement = "/accessibility-statement";
    public const string PrivacyPolicy = "/privacy-policy";
    public const string Cookies = "/cookies";
    public const string PostcodeNoResult = "/establishment-postcode-no-result";
    public const string RegisterBusinessForExporterService = "/register-business-error-admin";
    public const string RegisterBusinessForExporterServiceNonAdmin = "register-business-error-non-admin";

    public static readonly IReadOnlyCollection<(string page, string route)> RouteList = new List<(string page, string route)>
    {
        (Pages.Path.SelectedBusinessPath, SelectedBusiness),
        (Pages.Path.RegisteredBusinessBusinessPickerPath, RegisteredBusinessBusinessPicker),
        (Pages.Path.RegisteredBusinessPickerNoBusinessPickedPath, RegisteredBusinessNoBusinessChosen),
        (Pages.Path.RegisteredBusinessAlreadyRegisteredPath, RegisteredBusinessAlreadyRegistered),
        (Pages.Path.RegisteredBusinessCountryPath, RegisteredBusinessCountry),
        (Pages.Path.RegisteredBusinessCountryStaticPath, RegisteredBusinessCountryStatic),
        (Pages.Path.RegisteredBusinessRegulationsPath, RegisteredBusinessRegulations),
        (Pages.Path.RegisteredBusinessFboNumberPath, RegisteredBusinessFboNumber),
        (Pages.Path.RegisteredBusinessFboPhrGuidancePath, RegisteredBusinessFboPhrGuidance),
        (Pages.Path.RegistrationTaskListPath, RegistrationTasklist),
        (Pages.Path.RegisteredBusinessContactNamePath, RegisteredBusinessContactName),
        (Pages.Path.RegisteredBusinessContactPositionPath, RegisteredBusinessContactPosition),
        (Pages.Path.RegisteredBusinessNamePath, RegisteredBusinessName),
        (Pages.Path.RegisteredBusinessContactEmailPath, RegisteredBusinessEmail),
        (Pages.Path.RegisteredBusinessContactPhonePath, RegisteredBusinessContactPhoneNumber),
        (Pages.Path.RegisteredBusinessTypePath, RegisteredBusinessType),
        (Pages.Path.RegisteredBusinessAddressPath, RegisteredBusinessAddress),
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
        (Pages.Path.RegistrationTermsAndConditionsPath, RegistrationTermsAndConditions),
        (Pages.Path.SignUpConfirmationPath, SignUpConfirmation),
        (Pages.Path.ContactDetailsPath, ContactDetails),
        (Pages.Path.AccessibilityStatementPath, AccessibilityStatement),
        (Pages.Path.PrivacyPolicyPath, PrivacyPolicy),
        (Pages.Path.CookiesPath, Cookies),
        (Pages.Path.PostcodeNoResultPath, PostcodeNoResult),
        (Pages.Path.RegisterBusinessForExporterServicePath, RegisterBusinessForExporterService),
        (Pages.Path.RegisterBusinessForExporterServiceNonAdminPath, RegisterBusinessForExporterServiceNonAdmin),

    };

    public static class Pages
    {
        public static class Names
        {
            public const string SelectedBusinessName = "SelectedBusiness";
            public const string RegisteredBusinessBusinessPickerName = "RegisteredBusinessBusinessPicker";
            public const string RegisteredBusinessPickerNoBusinessPickedName = "RegisteredBusinessPickerNoBusinessPicked";
            public const string RegisteredBusinessAlreadyRegisteredName = "RegisteredBusinessAlreadyRegistered";
            public const string RegisteredBusinessCountryName = "RegisteredBusinessCountry";
            public const string RegisteredBusinessCountryStaticName = "RegisteredBusinessCountryStatic";
            public const string RegisteredBusinessRegulationsName = "EligibilityRegulations";
            public const string RegisteredBusinessFboNumberName = "RegisteredBusinessFboNumber";
            public const string RegisteredBusinessFboPhrGuidanceName = "RegisteredBusinessFboPhrGuidance";
            public const string RegistrationTasklistName = "RegistrationTaskList";
            public const string RegisteredBusinessContactNameName = "RegisteredBusinessContactName";
            public const string RegisteredBusinessContactPositionName = "RegisteredBusinessContactPosition";
            public const string RegisteredBusinessNameName = "RegisteredBusinessName";
            public const string RegisteredBusinessContactEmailName = "RegisteredBusinessContactEmail";
            public const string RegisteredBusinessContactPhoneName = "RegisteredBusinessContactPhone";
            public const string RegisteredBusinessTypeName = "RegisteredBusinessType";
            public const string RegisteredBusinessAddressName = "RegisteredBusinessAddress";
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
            public const string RegistrationTermsAndConditionsName = "TermsAndConditions";
            public const string SignUpConfirmationName = "SignUpConfirmation";
            public const string TermsAndConditionsName = "TermsConditions";
            public const string ContactDetailsName = "Contact";
            public const string AccessibilityStatementName = "AccessibilityStatement";
            public const string PrivacyPolicyName = "Privacy";
            public const string CookiesName = "Cookies";
            public const string PostcodeNoResultName = "PostcodeNoResult";
            public const string RegisterBusinessForExporterServiceName = "RegisterBusinessForExporterService";
            public const string RegisterBusinessForExporterServiceNonAdminName = "RegisterBusinessForExporterServiceNonAdmin";
        }

        public static class Path
        {
            public const string SelectedBusinessPath = $"/Registration/RegisteredBusiness/{Names.SelectedBusinessName}";
            public const string RegisteredBusinessBusinessPickerPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessBusinessPickerName}";
            public const string RegisteredBusinessPickerNoBusinessPickedPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessPickerNoBusinessPickedName}";
            public const string RegisteredBusinessAlreadyRegisteredPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessAlreadyRegisteredName}";
            public const string RegisteredBusinessCountryPath = $"/Registration/RegisteredBusiness/Eligibility/{Names.RegisteredBusinessCountryName}";
            public const string RegisteredBusinessCountryStaticPath = $"/Registration/RegisteredBusiness/Eligibility/{Names.RegisteredBusinessCountryStaticName}";
            public const string RegisteredBusinessRegulationsPath = $"/Registration/RegisteredBusiness/Eligibility/{Names.RegisteredBusinessRegulationsName}";
            public const string RegisteredBusinessFboNumberPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessFboNumberName}";
            public const string RegisteredBusinessFboPhrGuidancePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessFboPhrGuidanceName}";
            public const string RegistrationTaskListPath = $"/Registration/TaskList/{Names.RegistrationTasklistName}";
            public const string RegisteredBusinessContactNamePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactNameName}";
            public const string RegisteredBusinessContactPositionPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPositionName}";
            public const string RegisteredBusinessNamePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessNameName}";
            public const string RegisteredBusinessContactEmailPath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactEmailName}";
            public const string RegisteredBusinessContactPhonePath = $"/Registration/RegisteredBusiness/Contact/{Names.RegisteredBusinessContactPhoneName}";
            public const string RegisteredBusinessTypePath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessTypeName}";
            public const string RegisteredBusinessAddressPath = $"/Registration/RegisteredBusiness/{Names.RegisteredBusinessAddressName}";
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
            public const string RegistrationTermsAndConditionsPath = $"/Registration/Assurances/{Names.RegistrationTermsAndConditionsName}";
            public const string SignUpConfirmationPath = $"/Registration/Confirmation/{Names.SignUpConfirmationName}";
            public const string TermsAndConditionsPath = $"/Footer/{Names.TermsAndConditionsName}";
            public const string ContactDetailsPath = $"/Footer/{Names.ContactDetailsName}";
            public const string AccessibilityStatementPath = $"/Footer/{Names.AccessibilityStatementName}";
            public const string PrivacyPolicyPath = $"/Footer/{Names.PrivacyPolicyName}";
            public const string CookiesPath = $"/Footer/{Names.CookiesName}";
            public const string PostcodeNoResultPath = $"/Establishments/{Names.PostcodeNoResultName}";
            public const string RegisterBusinessForExporterServicePath = $"/Registration/RegisteredBusiness/{Names.RegisterBusinessForExporterServiceName}";
            public const string RegisterBusinessForExporterServiceNonAdminPath = $"/Registration/RegisteredBusiness/{Names.RegisterBusinessForExporterServiceNonAdminName}";
        }
    }
}
