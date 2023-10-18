using Castle.Core.Logging;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Footer;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Footer;

[TestFixture]
public class CookiesTests : PageModelTestsBase
{
    private CookiesModel? _systemUnderTest;
    protected Mock<ILogger<CookiesModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new CookiesModel(_mockLogger.Object);
        //_systemUnderTest.HttpContext = DefaultHttpContext();
    }

    [Test]
    public void OnGet_SetsCookie()
    {
        // act
        _systemUnderTest!.OnGet();

        // assert
        //_systemUnderTest.moc
    }
}
