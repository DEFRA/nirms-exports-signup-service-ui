using Defra.Trade.ReMoS.AssuranceService.UI.Core.Utilities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Utilities;

[TestFixture]
public class DefaultRouteRemovalPageRouteModelConventionTests
{
    private PageRouteModel? _pageRouteModel;
    private DefaultRouteRemovalPageRouteModelConvention? _systemUnderTest;

    public void TestCaseSetup()
    {
        var pageRoutes = new List<object>
        {
            new PageRouteMetadata("Index", string.Empty),
            new PageRouteMetadata("Create", string.Empty),
            new PageRouteMetadata("Edit", string.Empty)
        };

        var mockSelectorModel = new Mock<SelectorModel>();
        mockSelectorModel.Setup(m => m.EndpointMetadata).Returns(pageRoutes);
        var selectors = new List<SelectorModel> { mockSelectorModel.Object };

        var mockPageRouteModel = new Mock<PageRouteModel>();
        mockPageRouteModel.Setup(m => m.Selectors).Returns(selectors);
        _pageRouteModel = mockPageRouteModel.Object;
    }

    [TestCase("Index")]
    [TestCase("Create")]
    [Ignore("Needs further setup")]
    public void Apply_Should_Remove_Route(string route)
    {
        // Arrange
        _systemUnderTest = new DefaultRouteRemovalPageRouteModelConvention(route);

        // Act
        _systemUnderTest.Apply(_pageRouteModel);

        // Assert
        _pageRouteModel.Selectors.Should().HaveCount(2);
    }
}