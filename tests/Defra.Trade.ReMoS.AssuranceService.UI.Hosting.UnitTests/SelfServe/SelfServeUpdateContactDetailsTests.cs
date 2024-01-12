using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe;

[TestFixture]
public class SelfServeUpdateContactDetailsTests: PageModelTestsBase
{
    private UpdateContactModel? _systemUnderTest;
    private readonly Mock<ITraderService> _mockTraderService = new();
    private readonly Mock<IUserService> _mockUserService = new();
    protected Mock<ILogger<UpdateContactModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new UpdateContactModel(_mockLogger.Object, _mockUserService.Object, _mockTraderService.Object)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf") });
    }

    [Test]
    public async Task OnGet_ContactDetails_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
        var expectedDate = new DateTime(2023, 1, 1, 0, 0, 0);
        TradePartyDto tradePartyDto = new()
        {
            Id = guid,
            Contact = new TradeContactDto()
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Position = "Test",
                Email = "test@test.com",
                TelephoneNumber = "01234567890",
                LastModifiedDate = expectedDate,
                SubmittedDate = expectedDate,
                ModifiedBy = Guid.NewGuid()
            }
        };
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert

        _systemUnderTest.TradePartyId.Should().Be(guid);
        _systemUnderTest.Name.Should().Be("Test");
        _systemUnderTest.Position.Should().Be("Test");
        _systemUnderTest.Email.Should().Be("test@test.com");
        _systemUnderTest.PhoneNumber.Should().Be("01234567890");
        _systemUnderTest.LastModifiedOn.Should().Be(expectedDate);
        _systemUnderTest.SubmittedDate.Should().Be(expectedDate);
    }

    [Test]
    public async Task OnPostSubmit_ValidInfoReturnsNoErrors()
    {
        // arrange
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _systemUnderTest!.Name = "John Doe";
        _systemUnderTest!.Position = "Test";
        _systemUnderTest!.Email = "test@test.com";
        _systemUnderTest!.PhoneNumber = "01234567890";
        _systemUnderTest!.LastModifiedOn = new DateTime(2023, 1, 1, 0, 0, 0);
        _systemUnderTest!.SubmittedDate = new DateTime(2023, 1, 1, 0, 0, 0);

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSubmit_ValidInfoReturnsErrors()
    {
        // arrange
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _systemUnderTest!.Name = "John Doe%^!£$";
        _systemUnderTest!.Position = "Tes£$%&^t";
        _systemUnderTest!.Email = "test@test.com$&£%^";
        _systemUnderTest!.PhoneNumber = "012345678903565463546";
        _systemUnderTest!.LastModifiedOn = new DateTime(2023, 1, 1, 0, 0, 0);

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(4);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnPostAync_SuccessfulWithRedirect()
    {
        //Arrange
        Guid guid = Guid.NewGuid();
        _systemUnderTest!.TradePartyId = guid;
        _systemUnderTest!.Name = "John Doe";
        _systemUnderTest!.Position = "Test";
        _systemUnderTest!.Email = "test@test.com";
        _systemUnderTest!.PhoneNumber = "01234567890";
        _systemUnderTest!.LastModifiedOn = new DateTime(2023, 1, 1, 0, 0, 0);
        _systemUnderTest!.SubmittedDate = new DateTime(2023, 1, 1, 0, 0, 0);
        _mockTraderService.Setup(x => x.UpdateTradePartyContactSelfServeAsync(It.IsAny<TradePartyDto>())).ReturnsAsync(guid);

        //Act
        var result = await _systemUnderTest.OnPostSubmitAsync();
        var redirectResult = result as RedirectToPageResult;

        //assert
        redirectResult!.PageName.Should().Be("/SelfServe/SelfServeDashboard");
    }
}
