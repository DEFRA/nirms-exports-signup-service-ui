using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Extensions
{
    [TestFixture]
    public class ClaimsExtensionsTests
    {
        [Test]
        public void GetClaims_ReturnsClaims()
        {
            // arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            // act
            var claims = ClaimsExtensions.GetClaims(token);

            // assert
            var claimsList = claims.ToList();
            claimsList[0].Value.Should().Be("1234567890");
            claimsList[1].Value.Should().Be("John Doe");
            claimsList[2].Value.Should().Be("1516239022");
        }
    }
}
