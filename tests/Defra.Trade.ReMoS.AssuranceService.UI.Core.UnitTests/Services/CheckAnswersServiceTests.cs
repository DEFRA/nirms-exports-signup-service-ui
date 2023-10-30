using Defra.Trade.ReMoS.AssuranceService.UI.Core.TagHelpers;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using System.Runtime.Intrinsics.X86;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    [TestFixture]
    internal class CheckAnswersServiceTests
    {
        private ICheckAnswersService? _checkAnswersService;

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_NullTradePartyDto()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();
            TradePartyDto TradePartyDTO = null!;
            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_True_When_AllDataComplete()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = true,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                FboPhrOption = "fbo",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsTrue(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetEligibilityProgressNotComplete()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = true,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetBusinessDetailsProgressNotStarted()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = true,
            };

            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetBusinessDetailsProgressInProgress()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = true,
            };

            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Test",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetAuthorisedSignatoryProgressNotStarted()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetAuthorisedSignatoryProgressInProgress()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void GetAuthorisedSignatoryProgress_ReturnsCannotStart_WhenPersonName_Null()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid()
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.GetAuthorisedSignatoryProgress(TradePartyDTO);

            // Assert
            Assert.AreSame(TaskListStatus.CANNOTSTART, returnedValue);
        }

        [Test]
        public void GetAuthorisedSignatoryProgress_ReturnsComplete_WhenAuthorisedSignaturyFalse_ButAuthorisedSigdetailsComplete_Null()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Position = "1234567890",
                EmailAddress = "Test@Test.Com"
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.GetAuthorisedSignatoryProgress(TradePartyDTO);

            // Assert
            Assert.AreSame(TaskListStatus.COMPLETE, returnedValue);
        }

        [Test]
        public void GetAuthorisedSignatoryProgress_ReturnsInProgress_WhenAuthorisedSignaturyFalse_ButAuthorisedSigIDNotNull()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.GetAuthorisedSignatoryProgress(TradePartyDTO);

            // Assert
            Assert.AreSame(TaskListStatus.INPROGRESS, returnedValue);
        }

        [Test]
        public void GetEligibilityProgress_ReturnsNotStart_When_Address_Null()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            TradeAddressDto TradeAddressDTO = null!;

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.GetEligibilityProgress(TradePartyDTO);

            // Assert
            Assert.AreSame(TaskListStatus.NOTSTART, returnedValue);
        }

        [Test]
        public void IsLogisticsLocationsDataPresent_ReturnsTrue_When_DataPresent()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };
            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            var LogisticsLocationDTO = new LogisticsLocationDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Email = "Test@Test.Com",
                TradePartyId = TradePartyDTO.Id,
                TradeAddressId = TradeAddressDTO.Id,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                NI_GBFlag = "GB",
                Party = TradePartyDTO,
                Address = TradeAddressDTO
            };
            IEnumerable<LogisticsLocationDto> logistics = Enumerable.Empty<LogisticsLocationDto>();
            logistics = logistics.Append(LogisticsLocationDTO);

            // Act
            var returnedValue = _checkAnswersService.IsLogisticsLocationsDataPresent(TradePartyDTO, logistics);

            // Assert
            Assert.IsTrue(returnedValue);
        }

        [Test]
        public void IsLogisticsLocationsDataPresent_ReturnsFalse_When_NoLogisticsData()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };
            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            IEnumerable<LogisticsLocationDto> logistics = Enumerable.Empty<LogisticsLocationDto>();

            // Act
            var returnedValue = _checkAnswersService.IsLogisticsLocationsDataPresent(TradePartyDTO, logistics);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void IsLogisticsLocationsDataPresent_ReturnsFalse_When_LogisticsNull()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };
            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                PersonName = "Test",
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            IEnumerable<LogisticsLocationDto> logistics = null!;

            // Act
            var returnedValue = _checkAnswersService.IsLogisticsLocationsDataPresent(TradePartyDTO, logistics);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetContactDetailsNotComplete()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            var TradeContactDTO = new TradeContactDto
            {
                Id = Guid.NewGuid(),
                Email = "Test@Test.Com",
                TelephoneNumber = "1234567890",
                Position = "CEO",
                IsAuthorisedSignatory = false,
            };
            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid(),
                Name = "Test",
                Position = "CEO",
                EmailAddress = "Test@Test.Com"
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetContactDetailsNotStarted()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            TradeContactDto TradeContactDTO = null!;

            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid(),
                Name = "Test",
                Position = "CEO",
                EmailAddress = "Test@Test.Com"
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true,
                TermsAndConditionsSignedDate = DateTime.UtcNow,
                FboPhrOption = "2323"
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }

        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetContactDetailsInProgress()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            TradeContactDto TradeContactDTO = new TradeContactDto();

            var AuthorisedSignatoryDTO = new AuthorisedSignatoryDto
            {
                TradePartyId = Guid.NewGuid(),
                Name = "Test",
                Position = "CEO",
                EmailAddress = "Test@Test.Com"
            };

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true,
                TermsAndConditionsSignedDate = DateTime.UtcNow,
                FboPhrOption = "2323",
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }


        [Test]
        public void ReadyForCheckAnswers_Returns_False_When_GetAuthorisedSignatoryNotStarted()
        {
            // Arrange
            _checkAnswersService = new CheckAnswersService();

            var TradeAddressDTO = new TradeAddressDto
            {
                Id = Guid.NewGuid(),
                LineOne = "Addr 1",
                TradeCountry = "England"
            };

            TradeContactDto TradeContactDTO = new TradeContactDto 
            {
                PersonName = "Test",
                Email = "test@test.com",
                Position = "test",
                TelephoneNumber = "1234567890"
            };

            AuthorisedSignatoryDto AuthorisedSignatoryDTO = null!;

            var TradePartyDTO = new TradePartyDto
            {
                PracticeName = "Practicing",
                Address = TradeAddressDTO,
                Contact = TradeContactDTO,
                FboNumber = "1234567890",
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true,
                TermsAndConditionsSignedDate = DateTime.UtcNow,
                FboPhrOption = "2323",
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }
    }
}