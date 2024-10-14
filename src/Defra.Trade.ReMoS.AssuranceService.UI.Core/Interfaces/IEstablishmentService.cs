using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Helpers;
using System.Drawing.Printing;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IEstablishmentService
{
    public Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId,
        LogisticsLocationDto logisticsLocationDTO);
    Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid Id);
    Task<PagedList<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(
        Guid tradePartyId, bool includeRejected, string? searchTerm, string? sortColumn, string? sortDirection, string? NI_GBFlag, int pageNumber = 1, int pageSize = 50);
    public Task<List<LogisticsLocationDto>?> GetEstablishmentByPostcodeAsync(string postcode);
    Task<bool> RemoveEstablishmentAsync(Guid establishmentId);
    Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDto establishmentDto);
    Task<List<AddressDto>> GetTradeAddressApiByPostcodeAsync(string postcode);
    Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn);
    Task<bool> UpdateEstablishmentDetailsSelfServeAsync(LogisticsLocationDto establishmentDto);
    Task<Guid?> SaveEstablishmentDetails(Guid? establishmentid, Guid tradePartyId, LogisticsLocationDto establishmentDto, string NI_GBFlag, string? uprn);
    Task<bool> IsEstablishmentDraft(Guid? establishmentId);
}