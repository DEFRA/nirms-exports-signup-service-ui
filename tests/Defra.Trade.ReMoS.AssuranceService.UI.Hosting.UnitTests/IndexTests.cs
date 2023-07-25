using Defra.ReMoS.AssuranceService.UI.Hosting.Pages;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests
{
    [TestFixture]
    public class IndexTests : PageModelTestsBase
    {
        private IndexModel? _systemUnderTest;
        private readonly Mock<ILogger<IndexModel>> _mockLogger = new();
        private readonly Mock<IOptions<EhcoIntegration>> _mockEhcoIntegrationSettings = new();
        private readonly Mock<IValidationParameters> _mockValidationParameters = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new IndexModel(_mockLogger.Object, _mockEhcoIntegrationSettings.Object, _mockValidationParameters.Object);
        }

        [TestCase("testContactId", "1", "testAud", "testUserEnrolledOrganisation", "testUnix", true)]
        [TestCase("", "1", "testAud", "testUserEnrolledOrganisation", "testUnix", false)]
        [TestCase("testContactId", "0", "testAud", "testUserEnrolledOrganisation", "testUnix", false)]
        [TestCase("testContactId", "1", "incorrectTestAud", "testUserEnrolledOrganisation", "testUnix", false)]
        [TestCase("testContactId", "1", "testAud", "testUserEnrolledOrganisation", "", false)]
        [TestCase("testContactId", "1", "testAud", "", "testUnix", false)]
        public void ValidatePrincipal_IsValid(string contactId, string enrolledOrganisationCount, string aud, string userEnrolledOrganisation, string exp, bool exptectedResult)
        {
            // arrange
            List<Claim> claims = new List<Claim>
            {
                new Claim(type: "contactId", value: contactId),
                new Claim(type: "enrolledOrganisationsCount", value: enrolledOrganisationCount),
                new Claim(type: "aud", value: aud),
                new Claim(type: "userEnrolledOrganisations", value: userEnrolledOrganisation),
                new Claim(type: "exp", value: exp)
            };
            var validationParameters = new TokenValidationParameters();
            validationParameters.ValidAudience = "testAud";
            _mockValidationParameters.Setup(x => x.TokenValidationParameters).Returns(validationParameters);
            
            // act
            var isValid = _systemUnderTest!.ValidatePrincipal(claims);

            // assert
            isValid.Should().Be(exptectedResult);

        }
    }
}
