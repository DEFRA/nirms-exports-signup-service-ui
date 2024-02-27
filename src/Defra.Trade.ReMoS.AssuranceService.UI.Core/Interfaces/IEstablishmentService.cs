using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IEstablishmentService
{
    public Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId,
        LogisticsLocationDto logisticsLocationDTO);
    Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid Id);
    Task<IEnumerable<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId, bool isRejected);
    public Task<List<LogisticsLocationDto>?> GetEstablishmentByPostcodeAsync(string postcode);
    Task<bool> RemoveEstablishmentAsync(Guid establishmentId);
    Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDto establishmentDto);
    Task<List<AddressDto>> GetTradeAddressApiByPostcodeAsync(string postcode);
    Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn);
    Task<bool> UpdateEstablishmentDetailsSelfServeAsync(LogisticsLocationDto establishmentDto);
    Task<Guid?> SaveEstablishmentDetails(Guid? establishmentid, Guid tradePartyId, LogisticsLocationDto establishmentDto, string NI_GBFlag, string? uprn);
}
