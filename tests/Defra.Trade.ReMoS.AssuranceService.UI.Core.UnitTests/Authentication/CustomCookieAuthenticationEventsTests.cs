using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Authentication
{
    [TestFixture]
    public class CustomCookieAuthenticationEventsTests
    {
        [Test]
        public void ValidatePrincipal_WhenContactIdNull_SignsOut()
        {
            // arrange
            var mockContext = new Mock<CookieValidatePrincipalContext>();
            //mockContext.Setup(x => x.Principal);
            var claims = new List<Claim>();
            var httpContext = new Mock<HttpContext>();  
            mockContext.Setup(x => x.Principal!.Claims).Returns(claims);
            mockContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

            // act
            var customCookieAuthenticationEvents = new CustomCookieAuthenticationEvents();
            var result = customCookieAuthenticationEvents.ValidatePrincipal(mockContext.Object);

            // assert
            
        }
    }
}
