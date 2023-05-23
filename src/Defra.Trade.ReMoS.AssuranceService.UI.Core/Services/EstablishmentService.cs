using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services
{
    public class EstablishmentService : IEstablishmentService
    {
        private readonly IAPIIntegration _apiIntegration;

        public EstablishmentService(IAPIIntegration apiIntegration) 
        {
            _apiIntegration = apiIntegration;
        }

        public async Task<List<LogisticsLocation>?> GetLogisticsLocationByPostcodeAsync(string postcode)
        {
            return await _apiIntegration.GetLogisticsLocationByPostcodeAsync(postcode);
        }
    }
}
