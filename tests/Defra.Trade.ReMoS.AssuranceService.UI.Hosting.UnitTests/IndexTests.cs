using Defra.ReMoS.AssuranceService.UI.Hosting.Pages;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests
{
    [TestFixture]
    public class IndexTests
    {
        private IndexModel? _systemUnderTest;
        protected Mock<ILogger<IndexModel>> _mockLogger = new();
        protected Mock<IOptions<EhcoIntegrationSettings>> _mockIntegrationSettings = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new IndexModel(_mockLogger.Object, _mockIntegrationSettings.Object);
        }

        [Test]
        public void DecodeJwt_Success()
        {
            var jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIiLCJ2ZXIiOjEuMCwiY29udGFjdElkIjoiZjcyNTkxYTEtNmQ4Yi1lOTExLWE5NmYtMDAwZDNhMjliNWRlIiwicm9sZXMiOlsiZjgzMGMzNWUtNzFiNi1lODExLWE5NTQtMDAwZDNhMjliNWRlIl0sImlzcyI6Imh0dHBzOlwvXC9leHBvcnRzLWF1dGhlbnRpY2F0aW9uLWV4cC0xNDk5NS5henVyZXdlYnNpdGVzLm5ldCIsImVucm9sbGVkT3JnYW5pc2F0aW9uc0NvdW50IjoxLCJhdWQiOiI2YzQ5NmE2ZC1kNDYwLTQwYjctODg3OC03OTcyYjJlNTM1NDIiLCJ1cG4iOiIiLCJuYmYiOjE2ODkxNTI3MjYsIm5hbWUiOiJzaW5nbGUgb3JnIGJ1c2luZXNzIGV4cG9ydGVyIiwiZXhwIjoxNjg5MTU0NTM2LCJpYXQiOjE2ODkxNTI3MzYsImp0aSI6IjIxNTg2NGNhLTM5Y2EtNDU2Ni05YmRjLTYyM2Y0NmFkMWMyMCJ9.KzC6mQPizxtbfc-V45n90byP1ynBsUMGR645CR8f0uRCTzgpXj7jEaZG0Ne5WmTrlMPa6kclMANAK5BDWn8MD3hD1J8gbj851mkhPE7tjiEbtZthnBXQFZn0nETpKKjGsy6y37ba5y5KSHLd2lczBI6LwEgQUAfyQQZ1PTxqJVibRPoVjLwWxN9w9rRctlZgCe0bj-a6JWiYgVEkKvAeIEF1HUlSZOj9mHXjRxG3uho0udOxGDXDvjW5nEEEVGErczkLx9gMtW0ntegLHrI7j8AietqFPZDhmqL0WaLfherx72ob2KTF_eIlZ1PdDIZCVBgPbujltKOzSkEUqqZ3Pg";

            var decodedToken = _systemUnderTest?.DecodeJwt(jwt);

            Assert.IsNotNull(decodedToken);
        }

        [Test]        
        public void DecodeJwt_Invalid_Token()
        {
            var jwt = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJTb21lQ2xhaW0iOiJkYXRhIiwibmJmIjoxNjg5MTU0ODYyLCJleHAiOjE2ODkxNTg0NjIsImlzcyI6IlRoZUJpdENvaW5LaW5nIiwiYXVkIjoiYWxsIn0.BAgR-M-1Obk5rxfWyDdCRsyPfUVkX6z2m7f2l15q9Xxpob6k-rBpZAOFFgR77YKuvI2O9P3FBjPQA05tMcR9CQlhdqVc5JsjAmYg3frYN9LsHHTOQaAljVn0YO6J3rloJHOW1ta42wK9TYxuJLWzvi9o7n-yM9kESvUOF19RVAIoQ6l4yFv4eoabDlrKxw7hh47lnXi63FDoHQCPug1289v8pXTpJTjATS1pL6kbvSPZ1V_h-I4NXA1GI38ULXZdoLwmcJJGn_6Eq5FAOZVz74EtwIbuaivd26AhmXWTGDsvyEDSPlsQKMw_MqZ5IK1_PlbjuipoPDcGoCmg7mUZOe2dVfHtE2lkiik6UDNomID4StWabgenUD31XgLD2Hu5BKJPWJ1n_4Cl0WteOnT2kD1qTV4O9C1alUhTwv3ZqiDcp0eF6CJw58JKLP0xOm8tcvMqmfBfPQXnPE2POwwviNJrm3mEweXAaVM3mD0fdmjLhItDBcu3ivK_6luXWfZHbjJ5SoBCQIOZktI1CriK7UAE1nYrqfpkgNIRae8ksX7F7otXOKG3d9wvwOCpqsCl6WKNuB8tXLvE_eZRTRD0LR2sF3-eyF9KamVyOF1icEbW3T2Rlpd3_HbLOmE1LNmByncLyQ0K50cr2QLVt_yOgqyKtRV8AJ-lWuSwqP0QD9s";

            Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _systemUnderTest?.DecodeJwt(jwt));
        }
    }
}
