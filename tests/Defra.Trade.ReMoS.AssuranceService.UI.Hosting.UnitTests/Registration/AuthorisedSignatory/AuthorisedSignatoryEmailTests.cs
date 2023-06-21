using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.AuthorisedSignatory
{
    public class AuthorisedSignatoryEmailTests : PageModelTestsBase
    {
        private AuthorisedSignatoryEmailModel? _systemUnderTest;

        protected Mock<ILogger<AuthorisedSignatoryEmailModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();


        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new AuthorisedSignatoryEmailModel(_mockTraderService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task OnGet_PopulateModelProperties()
        {
            //Arrange
            var tradePartyId = Guid.NewGuid();

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    AuthorisedSignatory = new Core.DTOs.AuthorisedSignatoryDTO()
                    {
                        Id = Guid.NewGuid(),
                    }
                });

            //Act
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

            //Assert
            _systemUnderTest.TraderId.Should().NotBeEmpty();
            _systemUnderTest.SignatoryId.Should().NotBeEmpty();
        }

        [Test]
        public async Task OnPostSubmit_InvalidInput()
        {
            //Arrange
            _systemUnderTest!.ModelState.AddModelError("Email", "Enter a email.");

            //Act
            await _systemUnderTest!.OnPostSubmitAsync();

            //Assert

            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(1);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidEmail()
        {
            //Arrange
            _systemUnderTest!.Email = "Business-test@email.com";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }
    }
}
