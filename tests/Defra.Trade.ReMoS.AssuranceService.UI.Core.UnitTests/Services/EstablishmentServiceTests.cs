using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    internal class EstablishmentServiceTests
    {
        private IEstablishmentService? _establishmentService;
        protected Mock<I> _mockApiIntegration = new();
    }
}
