﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

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
    Task<List<LogisticsLocationDetailsDTO>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId);
    Task<Guid?> CreateEstablishmentAsync(LogisticsLocationDTO logisticsLocationDTO);
    Task<Guid?> AddEstablishmentToPartyAsync(LogisticsLocationBusinessRelationshipDTO relationDto);
    public Task<List<LogisticsLocationDTO>?> GetEstablishmentsByPostcodeAsync(string postcode);
    Task RemoveEstablishmentFromPartyAsync(Guid tradePartyId, Guid locationId);
}