using Defra.Trade.Address.V1.ApiClient.Model;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IApiIntegration
{
    Task<List<TradePartyDto>?> GetAllTradePartiesAsync();
    public Task<TradePartyDto?> GetTradePartyByIdAsync(Guid id);
    Task<Guid> AddTradePartyAsync(TradePartyDto tradePartyToCreate);
    Task<Guid> UpdateTradePartyAsync(TradePartyDto tradePartyToUpdate);
    public Task<Guid> UpdateTradePartyAddressAsync(TradePartyDto tradePartyToUpdate);
    public Task<Guid> UpdateTradePartyContactAsync(TradePartyDto tradePartyToUpdate);
    public Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid id);
    Task<List<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId, bool isRejected);
    Task<Guid?> AddEstablishmentToPartyAsync(Guid partyId, LogisticsLocationDto logisticsLocationDTO);
    public Task<List<LogisticsLocationDto>?> GetEstablishmentsByPostcodeAsync(string postcode);
    Task RemoveEstablishmentAsync(Guid locationId);
    public Task<TradePartyDto?> UpdateAuthorisedSignatoryAsync(TradePartyDto tradePartyToUpdate);
    Task<bool> UpdateEstablishmentAsync(LogisticsLocationDto establishmentDto);
    Task<Guid> AddAddressToPartyAsync(Guid partyId, TradeAddressDto addressDTO);
    Task<TradePartyDto?> GetTradePartyByOrgIdAsync(Guid orgId);
    Task<List<AddressDto>> GetTradeAddresApiByPostcodeAsync(string postcode);
    Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn);
    Task<Guid> UpdateTradePartyContactSelfServeAsync(TradePartyDto tradePartyToUpdate);
    Task<Guid> UpdateTradePartyAuthRepSelfServeAsync(TradePartyDto tradePartyToUpdate);
    Task<bool> UpdateEstablishmentSelfServeAsync(LogisticsLocationDto establishmentDto);
}