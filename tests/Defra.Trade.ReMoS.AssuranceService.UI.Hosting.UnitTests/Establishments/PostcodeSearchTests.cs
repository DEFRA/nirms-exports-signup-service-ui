using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class PostcodeSearchTests : PageModelTestsBase
    {
        private PostcodeSearchModel? _systemUnderTest;
        protected Mock<ILogger<PostcodeSearchModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new PostcodeSearchModel(_mockLogger.Object, _mockTraderService.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        }

        [Test]
        public async Task OnGet_NoEmailPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced
            var id = Guid.NewGuid();

            //Act
            await _systemUnderTest!.OnGetAsync(id);

            //Assert
            _systemUnderTest.Postcode.Should().Be("");
        }

        [Test]
        public async Task OnPostSearch_SearchValidPostcode()
        {
            //Arrange
            _systemUnderTest!.Postcode = "AB120AB";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidPostcodeNotPresent()
        {
            //Arrange
            _systemUnderTest!.Postcode = "";
            var expectedResult = "Enter a postcode.";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSearch_SearchValidPostcode_ModelIsInvalid()
        {
            //Arrange
            _systemUnderTest!.Postcode = "AB120AB";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidPostcodeNotPresent_ModelIsInvalid()
        {
            //Arrange
            _systemUnderTest!.Postcode = "";
            var expectedResult = "Enter a postcode.";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidPostcode_ModelIsInvalid()
        {
            //Arrange
            _systemUnderTest!.Postcode = "TEST";
            var expectedResult = "Enter a real postcode";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
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
        public async Task OnGet_HeadingSetToParameter_Successfully()
        {
            //Arrange
            var expectedHint = "If your place of destination belongs to a different business";
            var expectedHeading = "Add a place of destination";
            var expectedContentText = "The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
            _systemUnderTest.ContentText.Should().Be(expectedContentText);
            _systemUnderTest.ContextHint.Should().Be(expectedHint);
        }

        [Test]
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        [TestCase("bt1 4tg", "NI", 0, null)]
        [TestCase("sw9 9tf", "NI", 1, "Enter a postcode in Northern Ireland")]
        [TestCase("bt1 5tg", "GB", 1, "Enter a postcode in England, Scotland or Wales")]
        [TestCase("sw9 6th", "GB", 0, null)]
        public async Task OnPostSubmitAsync_PostcodeError(string postcode, string NI_GBFlag, int errorCount, string? errorMessage)
        {
            // arrange
            _systemUnderTest!.Postcode = postcode;
            _systemUnderTest.NI_GBFlag = NI_GBFlag;

            // act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            // assert
            _systemUnderTest.ModelState.ErrorCount.Should().Be(errorCount);
            if (errorCount != 0) _systemUnderTest.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(errorMessage);
            else _systemUnderTest.ModelState.Values.Should().BeEmpty();
        }

        [Test]
        public async Task OnPostcodeGet_ReturnBusinessName()
        {
            //arrange
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(false);
            TradePartyDto dto = new()
            {
                Id = Guid.NewGuid(),
                PracticeName = "Test"
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).ReturnsAsync(dto);

            //act
            await _systemUnderTest!.OnGetAsync(dto.Id);

            //assert
            _systemUnderTest.BusinessName.Should().Be(dto.PracticeName);
        }
    }
}