using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe.Establishments;

[TestFixture]
public class PostcodeResultsTests : PageModelTestsBase
{
    private PostcodeResultModel? _systemUnderTest;
    protected Mock<ILogger<PostcodeResultModel>> _mockLogger = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new PostcodeResultModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGetAsync_ReturnsLogisticsLocations()
    {
        // arrange
        var logisticsLocations = new List<AddressDto>
        {
            new AddressDto("1234", null, null, null, null, 0, null, 0, null, "TES1", null, null, null, null, "123", 0, 0)
            {
                Address = "Test 2, line 1, city, TES1"
            }
        };
        var orgId = Guid.NewGuid();
        var postcode = "TES1";

        _mockEstablishmentService.Setup(x => x.GetTradeAddressApiByPostcodeAsync(postcode).Result).Returns(logisticsLocations);
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(false);
        // act
        await _systemUnderTest!.OnGetAsync(orgId, postcode, "England");

        // assert
        _systemUnderTest.EstablishmentsList.Should().HaveCount(1);
        _systemUnderTest.EstablishmentsList![0].Text.Should().Be("Test 2, line 1, city, TES1");
        _systemUnderTest.EstablishmentsList[0].Value.Should().Be(logisticsLocations[0].Uprn);
    }

    [Test]
    public async Task OnPostSubmitAsync_ShouldBeValid()
    {
        //Arrange
        _systemUnderTest!.Postcode = "TES1";
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _systemUnderTest!.SelectedEstablishment = Guid.NewGuid().ToString();

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSubmitAsync_GivenInvalidModdel_ShouldBeHandled()
    {
        //Arrange
        _systemUnderTest!.Postcode = "TES1";
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _systemUnderTest!.SelectedEstablishment = Guid.NewGuid().ToString();

        _systemUnderTest!.ModelState.AddModelError("TestError", "Something broke");

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnGet_HeadingSetToParameter_Successfully()
    {
        //Arrange
        var expectedHeading = "Add a place of destination";
        var expectedContentText = "These are the establishments that consignments will go to in Northern Ireland after the port of entry under the scheme.";
        _mockEstablishmentService
            .Setup(x => x.GetEstablishmentByPostcodeAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<List<LogisticsLocationDto>?>(new List<LogisticsLocationDto>() { new LogisticsLocationDto() }));

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "aaa", "NI");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        _systemUnderTest.ContentText.Should().Be(expectedContentText);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }
}