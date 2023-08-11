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
        public async Task ReadyForCheckAnswers_Returns_False_When_NullTradePartyDtoAsync()
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
        public async Task ReadyForCheckAnswers_Returns_True_When_AllDataCompleteAsync()
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
                AuthorisedSignatory = AuthorisedSignatoryDTO,
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsTrue(returnedValue);
        }

        [Test]
        public async Task ReadyForCheckAnswers_Returns_False_When_GetEligibilityProgressNotCompleteAsync()
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
        public async Task ReadyForCheckAnswers_Returns_False_When_GetBusinessDetailsProgressNotStartedAsync()
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
        public async Task ReadyForCheckAnswers_Returns_False_When_GetBusinessDetailsProgressInProgressAsync()
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
        public async Task ReadyForCheckAnswers_Returns_False_When_GetAuthorisedSignatoryProgressNotStartedAsync()
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
        public async Task ReadyForCheckAnswers_Returns_False_When_GetAuthorisedSignatoryProgressInProgressAsync()
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
        public async Task GetAuthorisedSignatoryProgress_ReturnsCannotStart_WhenPersonName_NullAsync()
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
        public async Task GetAuthorisedSignatoryProgress_ReturnsComplete_WhenAuthorisedSignaturyFalse_ButAuthorisedSigdetailsComplete_NullAsync()
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
        public async Task GetAuthorisedSignatoryProgress_ReturnsInProgress_WhenAuthorisedSignaturyFalse_ButAuthorisedSigIDNotNullAsync()
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
        public async Task GetEligibilityProgress_ReturnsNotStart_When_Address_NullAsync()
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
        public async Task ReadyForCheckAnswers_Returns_False_When_GetContactDetailsNotCompleteAsync()
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
        public async Task ReadyForCheckAnswers_Returns_False_When_GetContactDetailsNotStartedAsync()
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
                RegulationsConfirmed = true
            };

            // Act
            var returnedValue = _checkAnswersService.ReadyForCheckAnswers(TradePartyDTO);

            // Assert
            Assert.IsFalse(returnedValue);
        }
    }
}