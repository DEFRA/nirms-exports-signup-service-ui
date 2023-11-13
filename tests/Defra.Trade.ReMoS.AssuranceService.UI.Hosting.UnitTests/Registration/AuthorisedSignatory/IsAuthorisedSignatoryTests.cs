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
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        }

        [Test]
        public async Task OnGet_PopulateModelProperties()
        {
            //Arrange
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");            

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId
                });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

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
            _systemUnderTest!.IsAuthorisedSignatory = null;
            _systemUnderTest.ContactName = "Joe Blogs";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();

            //Assert            
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
            _systemUnderTest.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be($"Select if Joe Blogs is the authorised representative");
        }

        [Test]
        public async Task OnPostSave_InvalidInput()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = null;
            _systemUnderTest.ContactName = "Joe Blogs";

            //Act
            await _systemUnderTest.OnPostSaveAsync();

            //Assert            
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
            _systemUnderTest.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be($"Select if Joe Blogs is the authorised representative");
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
               .ReturnsAsync(new Core.DTOs.TradePartyDto()
               {
                   Id = tradePartyId,
                   Contact = new Core.DTOs.TradeContactDto()
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
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDto>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
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

            //Assert
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
               .ReturnsAsync(new Core.DTOs.TradePartyDto()
               {
                   Id = tradePartyId,
                   Contact = new Core.DTOs.TradeContactDto()
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
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDto>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
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
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDto>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
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
        public async Task OnPostSubmit_SignatoryTrue_AuthorisedSignatoryNamePath_NI()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = "true";
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");

            _mockTraderService
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDto>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
                    {
                        IsAuthorisedSignatory = true
                    },
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "NI"
                    }
                });

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
                    {
                        IsAuthorisedSignatory = true
                    },
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "NI"
                    }
                });

            //Act
            var result = await _systemUnderTest!.OnPostSubmitAsync();
            var redirectResult = result as RedirectToPageResult;

            //Assert
            redirectResult!.PageName.Should().Be("/Establishments/PostcodeSearch");
            var validation = ValidateModel(_systemUnderTest);
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SignatoryTrue_EstablishmentsPresent_AuthorisedSignatoryNamePath_NI()
        {
            //Arrange
            _systemUnderTest!.IsAuthorisedSignatory = "true";
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            var tradePartyId = new Guid("50919f18-fb85-450a-81a9-a25e7cebc0ff");

            _mockTraderService
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDto>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
                    {
                        IsAuthorisedSignatory = true
                    },
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "NI"
                    }
                });

            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
                    {
                        IsAuthorisedSignatory = true
                    },
                    Address = new TradeAddressDto()
                    {
                        TradeCountry = "NI"
                    }
                });

            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>()).Result)
                .Returns(new List<LogisticsLocationDto>()
                {
                    new LogisticsLocationDto()
                    {
                        Id = new Guid()
                    }
                });

            //Act
            var result = await _systemUnderTest!.OnPostSubmitAsync();
            var redirectResult = result as RedirectToPageResult;

            //Assert
            redirectResult!.PageName.Should().Be("/Establishments/AdditionalEstablishmentAddress");
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
                .Setup(x => x.UpdateAuthorisedSignatoryAsync(It.IsAny<TradePartyDto>()).Result)
                .Returns(new Core.DTOs.TradePartyDto()
                {
                    Id = tradePartyId,
                    Contact = new Core.DTOs.TradeContactDto()
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

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }

        [Test]
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }
    }
}
