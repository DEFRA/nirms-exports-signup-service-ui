﻿using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class AdditionalEstablishmentDepartureAddressTests : PageModelTestsBase
    {
        private AdditionalEstablishmentDepartureAddressModel? _systemUnderTest;
        protected Mock<ILogger<AdditionalEstablishmentDepartureAddressModel>> _mockLogger = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new AdditionalEstablishmentDepartureAddressModel(_mockLogger.Object);
        }

        [Test]
        public async Task OnGet_RadioNotSelected()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced

            //Act
            await _systemUnderTest!.OnGetAsync();

            //Assert
            _systemUnderTest.AdditionalAddress.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "yes";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidRadio()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "";
            var expectedResult = "Select yes if you want to add another point of departure";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }
    }
}
