using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.TaskList
{
    [TestFixture]
    public class RegistratonTaskListTests : PageModelTestsBase
    {
        private RegistrationTaskListModel? _systemUnderTest;
        protected Mock<ILogger<RegistrationTaskListModel>> _mockLogger = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegistrationTaskListModel(_mockLogger.Object);
        }

        [Test]
        public async Task OnGet_NoCountryPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced
            Guid guid = Guid.NewGuid();

            //Act
            if (_systemUnderTest != null)
            {
                _ = await _systemUnderTest.OnGetAsync(guid);
            }

            //Assert
            if (_systemUnderTest != null)
            {
                _ = _systemUnderTest.RegistrationID.Should().NotBeNull();
            }
        }
    }
}
