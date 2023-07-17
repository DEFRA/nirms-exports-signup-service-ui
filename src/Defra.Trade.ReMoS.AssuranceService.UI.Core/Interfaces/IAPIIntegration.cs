using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

public interface IAPIIntegration
{
    Task<List<TradePartyDTO>?> GetAllTradePartiesAsync();
    public Task<TradePartyDTO?> GetTradePartyByIdAsync(Guid id);
    Task<Guid> AddTradePartyAsync(TradePartyDTO tradePartyToCreate);
    Task<Guid> UpdateTradePartyAsync(TradePartyDTO tradePartyToUpdate);
    public Task<Guid> UpdateTradePartyAddressAsync(TradePartyDTO tradePartyToUpdate);
    public Task<Guid> UpdateTradePartyContactAsync(TradePartyDTO tradePartyToUpdate);
    public Task<LogisticsLocationDTO?> GetEstablishmentByIdAsync(Guid id);
    Task<List<LogisticsLocationDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId);
    Task<Guid?> AddEstablishmentToPartyAsync(Guid partyId, LogisticsLocationDTO logisticsLocationDTO);
    public Task<List<LogisticsLocationDTO>?> GetEstablishmentsByPostcodeAsync(string postcode);
    Task RemoveEstablishmentAsync(Guid locationId);
    public Task<TradePartyDTO?> UpdateAuthorisedSignatoryAsync(TradePartyDTO tradePartyToUpdate);
    Task<bool> UpdateEstablishmentAsync(LogisticsLocationDTO establishmentDto);
}