using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
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
    [TestFixture]
    public class IsAuthorisedSignatoryTests : PageModelTestsBase
    {
        private IsAuthorisedSignatoryModel? _systemUnderTest;

        protected Mock<ILogger<IsAuthorisedSignatoryModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new IsAuthorisedSignatoryModel(_mockTraderService.Object, _mockEstablishmentService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task OnGet_PopulateModelProperties()
        {
            //Arrange
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");            

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId
                });

            //Act
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

            //Assert
            _systemUnderTest.TradePartyId.Should().NotBeEmpty();
            _systemUnderTest.SignatoryId.Should().BeEmpty();              
            _systemUnderTest.IsAuthorisedSignatory.Should().Be(null);
        }

        [Test]
        public async Task OnPostSubmit_InvalidInput()
        {
            //Arrange
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");
            _systemUnderTest!.IsAuthorisedSignatory = null;
            _systemUnderTest!.ModelState.AddModelError("IsAuthorisedSignatory", "Tick Yes or No");

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    Contact = new TradeContactDTO()
                    {
                        IsAuthorisedSignatory = null
                    }
                });

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
            _systemUnderTest!.IsAuthorisedSignatory = null;
            _systemUnderTest!.ModelState.AddModelError("IsAuthorisedSignatory", "Tick Yes or No");

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    Contact = new TradeContactDTO()
                    {
                        IsAuthorisedSignatory = null
                    }
                });

            //Act
            await _systemUnderTest!.OnPostSaveAsync();

            //Assert

            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(1);
        }


        [Test]
        public async Task OnPostSubmit_SignatoryTrue_RedirectToEstablishments()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = "true";
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");

            _mockTraderService
               .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
               .ReturnsAsync(new Core.DTOs.TradePartyDTO()
               {
                   Id = tradePartyId,
                   Contact = new Core.DTOs.TradeContactDTO()
                   {
                       IsAuthorisedSignatory = true
                   },
                   AuthorisedSignatory = new AuthorisedSignatoryDto()
                   {
                       Id = Guid.NewGuid(),
                       TradePartyId = tradePartyId
                   }
               });

            _mockTraderService
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDTO>()).Result)
                .Returns(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDTO()
                    {
                        IsAuthorisedSignatory = true
                    },
                    AuthorisedSignatory = new AuthorisedSignatoryDto()
                    {
                        Id = Guid.NewGuid()
                    }
                });

            //Act
            var result = await _systemUnderTest!.OnPostSubmitAsync();
            var redirectResult = result as RedirectToPageResult;

            //Assert
            redirectResult!.PageName.Should().Be("/Establishments/EstablishmentNameAndAddress");
            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSave_SignatoryTrue_RedirectToTaskListPath()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = "true";
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");

            _mockTraderService
               .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
               .ReturnsAsync(new Core.DTOs.TradePartyDTO()
               {
                   Id = tradePartyId,
                   Contact = new Core.DTOs.TradeContactDTO()
                   {
                       IsAuthorisedSignatory = true
                   },
                   AuthorisedSignatory = new AuthorisedSignatoryDto()
                   {
                       Id = Guid.NewGuid(),
                       TradePartyId = tradePartyId
                   }
               });

            _mockTraderService
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDTO>()).Result)
                .Returns(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDTO()
                    {
                        IsAuthorisedSignatory = true
                    },
                    AuthorisedSignatory = new AuthorisedSignatoryDto()
                    {
                        Id = Guid.NewGuid()
                    }
                });

            //Act
            var result = await _systemUnderTest!.OnPostSaveAsync();
            var redirectResult = result as RedirectToPageResult;

            //Assert
            redirectResult!.PageName.Should().Be("/Registration/TaskList/RegistrationTaskList");
            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SignatoryFalse_AuthorisedSignatoryNamePath()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = "false";
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");

            _mockTraderService
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDTO>()).Result)
                .Returns(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDTO()
                    {
                        IsAuthorisedSignatory = false
                    }
                });

            //Act
            var result = await _systemUnderTest!.OnPostSubmitAsync();
            var redirectResult = result as RedirectToPageResult;

            //Assert
            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/AuthorisedSignatory/AuthorisedSignatoryName");
            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSave_SignatoryFalse_AuthorisedSignatoryNamePath()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = "false";
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");

            _mockTraderService
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDTO>()).Result)
                .Returns(new Core.DTOs.TradePartyDTO()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDTO()
                    {
                        IsAuthorisedSignatory = false
                    }
                });

            //Act
            var result = await _systemUnderTest!.OnPostSaveAsync();
            var redirectResult = result as RedirectToPageResult;

            //Assert
            redirectResult!.PageName.Should().Be("/Registration/TaskList/RegistrationTaskList");
            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(0);
        }
    }
}
