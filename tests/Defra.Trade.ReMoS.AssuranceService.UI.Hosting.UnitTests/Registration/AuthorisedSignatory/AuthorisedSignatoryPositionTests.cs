using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.AuthorisedSignatory
{
    public class AuthorisedSignatoryPositionTests : PageModelTestsBase
    {
        private AuthorisedSignatoryPositionModel? _systemUnderTest;

        protected Mock<ILogger<AuthorisedSignatoryPositionModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();        

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new AuthorisedSignatoryPositionModel(_mockTraderService.Object, _mockLogger.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        }

        [Test]
        public async Task OnGet_PopulateModelProperties()
        {
            //Arrange
            var tradePartyId = Guid.NewGuid();

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TradePartyDto()
                {
                    Id = tradePartyId,
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "GB"
                    },
                    AuthorisedSignatory = new AuthorisedSignatoryDto()
                    {
                        Id = Guid.NewGuid(),
                    }
                });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);

            //Act
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

            //Assert
            _systemUnderTest.TradePartyId.Should().NotBeEmpty();
            _systemUnderTest.SignatoryId.Should().NotBeEmpty();
        }

        [Test]
        public async Task OnPostSubmit_InvalidInput()
        {
            //Arrange
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");
            _systemUnderTest!.ModelState.AddModelError("Position", "Enter a position.");

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TradePartyDto()
                {
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "GB"
                    },
                    AuthorisedSignatory = new AuthorisedSignatoryDto { }
                });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);

            //Act
            await _systemUnderTest!.OnPostSubmitAsync();

            //Assert

            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(1);
        }

        [Test]
        public async Task OnPostSave_InvalidInput()
        {
            //Arrange
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");
            _systemUnderTest!.ModelState.AddModelError("Position", "Enter a position.");

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TradePartyDto()
                {
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "GB"
                    },
                    AuthorisedSignatory = new AuthorisedSignatoryDto { }
                });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);

            //Act
            await _systemUnderTest!.OnPostSaveAsync();

            //Assert

            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(1);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidPosition()
        {
            //Arrange
            _systemUnderTest!.Position = "Software Developer";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSave_SubmitValidPosition()
        {
            //Arrange
            _systemUnderTest!.Position = "Software Developer";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
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
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }
    }
}
