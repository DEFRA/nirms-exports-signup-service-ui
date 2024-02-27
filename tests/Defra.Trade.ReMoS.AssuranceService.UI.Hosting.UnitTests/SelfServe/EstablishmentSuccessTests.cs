using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe
{
    public class EstablishmentSuccessTests : PageModelTestsBase
    {
        private EstablishmentSuccessfulModel? _systemUnderTest;
        protected Mock<ILogger<EstablishmentSuccessfulModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();
        protected Mock<ITraderService> _mockTraderService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new EstablishmentSuccessfulModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object)
            {
                PageContext = PageModelMockingUtils.MockPageContext()
            };
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
        }

        [Test]
        public async Task OnGet_NoEstablishment_IfNoSavedData()
        {
            //Arrange
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new LogisticsLocationDto());

            //Act
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), It.IsAny<string>());

            //Assert
            _systemUnderTest.Establishment?.RemosEstablishmentSchemeNumber.Should().Be(null);
        }

        [Test]
        public async Task OnGet_ReturnsEstablishment_GBMessage_IfSavedData()
        {
            var orgId = new Guid();
            var establishmentId = new Guid();
            string NI_GBFlag = "GB";
            LogisticsLocationDto location = new()
            {
                Id = Guid.NewGuid(),
                RemosEstablishmentSchemeNumber = "RMS-GB-000001-001",
                Email = "test"
            };
            //Arrange
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            //Act
            _systemUnderTest!.OrgId = orgId;
            _systemUnderTest.EstablishmentId = establishmentId;
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), NI_GBFlag);

            //Assert
            _systemUnderTest.Establishment.Should().NotBe(null);
            _systemUnderTest.Establishment?.RemosEstablishmentSchemeNumber.Should().Be("RMS-GB-000001-001");
            _systemUnderTest.Heading.Should().Be("Place of dispatch successfully added");
        }

        [Test]
        public async Task OnGet_ReturnsEstablishment_NIMessage_IfSavedData()
        {
            var orgId = new Guid();
            var establishmentId = new Guid();
            string NI_GBFlag = "NI";
            LogisticsLocationDto location = new()
            {
                Id = Guid.NewGuid(),
                RemosEstablishmentSchemeNumber = "RMS-GB-000001-001",
                Email = "test"
            };
            //Arrange
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            //Act
            _systemUnderTest!.OrgId = orgId;
            _systemUnderTest.EstablishmentId = establishmentId;
            await _systemUnderTest!.OnGetAsync(orgId, establishmentId, NI_GBFlag);

            //Assert
            _systemUnderTest.Establishment.Should().NotBe(null);
            _systemUnderTest.Establishment?.RemosEstablishmentSchemeNumber.Should().Be("RMS-GB-000001-001");
            _systemUnderTest.Heading.Should().Be("Place of destination successfully added");
        }
    }
}
