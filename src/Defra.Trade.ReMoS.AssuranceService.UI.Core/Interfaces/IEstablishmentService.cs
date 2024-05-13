﻿using Defra.Trade.Address.V1.ApiClient.Model;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IEstablishmentService
{
    public Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId,
        LogisticsLocationDto logisticsLocationDTO);
    Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid Id);
    Task<IEnumerable<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId, bool isRejected, string? searchTerm);
    public Task<List<LogisticsLocationDto>?> GetEstablishmentByPostcodeAsync(string postcode);
    Task<bool> RemoveEstablishmentAsync(Guid establishmentId);
    Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDto establishmentDto);
    Task<List<AddressDto>> GetTradeAddressApiByPostcodeAsync(string postcode);
    Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn);
    Task<bool> UpdateEstablishmentDetailsSelfServeAsync(LogisticsLocationDto establishmentDto);
    Task<Guid?> SaveEstablishmentDetails(Guid? establishmentid, Guid tradePartyId, LogisticsLocationDto establishmentDto, string NI_GBFlag, string? uprn);
    Task<bool> IsEstablishmentDraft(Guid? establishmentId);
}
