using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.UnitTests;

[TestFixture]
public class RoutesTests
{
    [TestCase(Routes.RegisteredBusinessCompany, Routes.Pages.Path.RegisteredBusinessCompanyPath)]
    [TestCase(Routes.RegistrationTasklist, Routes.Pages.Path.RegistrationTaskListPath)]
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

    [TestCase(Routes.Pages.Names.RegisteredBusinessCompanyName, Routes.Pages.Path.RegisteredBusinessCompanyPath)]
    [TestCase(Routes.Pages.Names.RegistrationTasklistName, Routes.Pages.Path.RegistrationTaskListPath)]
    public void TestPathsContainPageNames(string pageName, string path)
    {
        //assert
        path.Should().Contain(pageName);
    }
}


