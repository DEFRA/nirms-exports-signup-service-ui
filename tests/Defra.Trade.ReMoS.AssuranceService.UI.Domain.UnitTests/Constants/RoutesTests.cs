using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.UnitTests.Constants;

[TestFixture]
public class RoutesTests
{
    [TestCase(Routes.RegisteredBusinessCountry, Routes.Pages.Path.RegisteredBusinessCountryPath)]
    [TestCase(Routes.RegistrationTasklist, Routes.Pages.Path.RegistrationTaskListPath)]
    [TestCase(Routes.RegisteredBusinessContactPosition, Routes.Pages.Path.RegisteredBusinessContactPositionPath)]
    [TestCase(Routes.RegisteredBusinessContactName, Routes.Pages.Path.RegisteredBusinessContactNamePath)]
    [TestCase(Routes.RegisteredBusinessEmail, Routes.Pages.Path.RegisteredBusinessContactEmailPath)]
    [TestCase(Routes.RegisteredBusinessContactPhoneNumber, Routes.Pages.Path.RegisteredBusinessContactPhonePath)]
    [TestCase(Routes.RegisteredBusinessName, Routes.Pages.Path.RegisteredBusinessNamePath)]
    [TestCase(Routes.RegisteredBusinessType, Routes.Pages.Path.RegisteredBusinessTypePath)]
    [TestCase(Routes.RegisteredBusinessAddress, Routes.Pages.Path.RegisteredBusinessAddressPath)]
    [TestCase(Routes.AdditionalEstablishmentAddress, Routes.Pages.Path.AdditionalEstablishmentAddressPath)]
    [TestCase(Routes.EstablishmentNameAndAddress, Routes.Pages.Path.EstablishmentNameAndAddressPath)]
    [TestCase(Routes.EstablishmentPostcodeSearch, Routes.Pages.Path.EstablishmentPostcodeSearchPath)]
    [TestCase(Routes.EstablishmentPostcodeSearch, Routes.Pages.Path.EstablishmentPostcodeSearchPath)]
    [TestCase(Routes.EstablishmentContactEmail, Routes.Pages.Path.EstablishmentContactEmailPath)]
    [TestCase(Routes.AuthorisedSignatoryPosition, Routes.Pages.Path.AuthorisedSignatoryPositionPath)]
    [TestCase(Routes.RegistrationCheckYourAnswers, Routes.Pages.Path.RegistrationCheckYourAnswersPath)]
    [TestCase(Routes.RegistrationSpsAssurance, Routes.Pages.Path.RegistrationSpsAssurancePath)]
    public void TestRoutesMatchPaths(string routeName, string routePath)
    {
        //arrange
        var expected = routePath;
        var routeList = Routes.RouteList;

        //act
        var actual = routeList.Where(x => x.route == routeName).ToList();

        //assert
        actual.Count.Should().Be(1);
        actual[0].page.Should().Be(expected);
    }

    [TestCase(Routes.Pages.Names.RegisteredBusinessCountryName, Routes.Pages.Path.RegisteredBusinessCountryPath)]
    [TestCase(Routes.Pages.Names.RegistrationTasklistName, Routes.Pages.Path.RegistrationTaskListPath)]
    [TestCase(Routes.Pages.Names.RegisteredBusinessNameName, Routes.Pages.Path.RegisteredBusinessNamePath)]
    [TestCase(Routes.Pages.Names.RegisteredBusinessContactNameName, Routes.Pages.Path.RegisteredBusinessContactNamePath)]
    [TestCase(Routes.Pages.Names.RegisteredBusinessContactPositionName, Routes.Pages.Path.RegisteredBusinessContactPositionPath)]
    [TestCase(Routes.Pages.Names.RegisteredBusinessContactPhoneName, Routes.Pages.Path.RegisteredBusinessContactPhonePath)]
    [TestCase(Routes.Pages.Names.RegisteredBusinessTypeName, Routes.Pages.Path.RegisteredBusinessTypePath)]
    [TestCase(Routes.Pages.Names.RegisteredBusinessAddressName, Routes.Pages.Path.RegisteredBusinessAddressPath)]
    [TestCase(Routes.Pages.Names.AdditionalEstablishmentAddressName, Routes.Pages.Path.AdditionalEstablishmentAddressPath)]
    [TestCase(Routes.Pages.Names.EstablishmentNameAndAddressName, Routes.Pages.Path.EstablishmentNameAndAddressPath)]
    [TestCase(Routes.Pages.Names.EstablishmentPostcodeSearchName, Routes.Pages.Path.EstablishmentPostcodeSearchPath)]
    [TestCase(Routes.Pages.Names.EstablishmentContactEmailName, Routes.Pages.Path.EstablishmentContactEmailPath)]
    [TestCase(Routes.Pages.Names.AuthorisedSignatoryPositionName, Routes.Pages.Path.AuthorisedSignatoryPositionPath)]
    [TestCase(Routes.Pages.Names.RegistrationSpsAssuranceName, Routes.Pages.Path.RegistrationSpsAssurancePath)]
    public void TestPathsContainPageNames(string pageName, string path)
    {
        //assert
        path.Should().Contain(pageName);
    }
}


