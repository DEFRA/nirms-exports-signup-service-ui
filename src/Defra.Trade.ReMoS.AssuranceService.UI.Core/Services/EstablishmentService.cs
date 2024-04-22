using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class EstablishmentService : IEstablishmentService
{
    private readonly IApiIntegration _api;

    public EstablishmentService(IApiIntegration api)
    {
        _api = api;
    }
    public async Task<Guid?> CreateEstablishmentForTradePartyAsync(
        Guid partyId,
        LogisticsLocationDto logisticsLocationDTO)
    {
        var establishmentId = await _api.AddEstablishmentToPartyAsync(partyId, logisticsLocationDTO);

        return establishmentId;
    }

    public async Task<IEnumerable<LogisticsLocationDto>?> GetEstablishmentsForTradePartyAsync(Guid tradePartyId, bool isRejected)
    {
        return await _api.GetEstablishmentsForTradePartyAsync(tradePartyId, isRejected);
    }

    public async Task<LogisticsLocationDto?> GetEstablishmentByIdAsync(Guid Id)
    {
        return (Id != Guid.Empty) ? await _api.GetEstablishmentByIdAsync(Id) : null;
    }
    public async Task<List<LogisticsLocationDto>?> GetEstablishmentByPostcodeAsync(string postcode)
    {
        return await _api.GetEstablishmentsByPostcodeAsync(postcode);
    }

    public async Task<bool> RemoveEstablishmentAsync(Guid establishmentId)
    {
        await _api.RemoveEstablishmentAsync(establishmentId);
        return true;
    }

    public async Task<bool> UpdateEstablishmentDetailsAsync(LogisticsLocationDto establishmentDto)
    {
        return await _api.UpdateEstablishmentAsync(establishmentDto);
    }

    public async Task<List<AddressDto>> GetTradeAddressApiByPostcodeAsync(string postcode)
    {
        return await _api.GetTradeAddresApiByPostcodeAsync(postcode);
    }

    public async Task<LogisticsLocationDto> GetLogisticsLocationByUprnAsync(string uprn)
    {
        return await _api.GetLogisticsLocationByUprnAsync(uprn);
    }

    public async Task<bool> UpdateEstablishmentDetailsSelfServeAsync(LogisticsLocationDto establishmentDto)
    {
        return await _api.UpdateEstablishmentSelfServeAsync(establishmentDto);
    }

    public async Task<Guid?> SaveEstablishmentDetails(Guid? establishmentid, Guid tradePartyId, LogisticsLocationDto establishmentDto, string NI_GBFlag, string? uprn)
    {        
        var establishmentFromApi = (establishmentid != Guid.Empty && establishmentid != null) ? 
            await GetEstablishmentByIdAsync((Guid)establishmentid) : 
            new LogisticsLocationDto() { Address = new TradeAddressDto() };


        establishmentFromApi!.Name = establishmentDto.Name;
        establishmentFromApi.Address!.LineOne = establishmentDto.Address?.LineOne;
        establishmentFromApi.Address.LineTwo = establishmentDto.Address?.LineTwo;
        establishmentFromApi.Address.County = establishmentDto.Address?.County;
        establishmentFromApi.Address.CityName = establishmentDto.Address?.CityName;
        establishmentFromApi.Address.PostCode = establishmentDto.Address?.PostCode;
        establishmentFromApi.NI_GBFlag = NI_GBFlag;
        establishmentFromApi.ApprovalStatus = establishmentDto.ApprovalStatus;

        if (establishmentid == Guid.Empty || uprn != null || establishmentid == null)
        {
            return await CreateEstablishmentForTradePartyAsync(tradePartyId, establishmentFromApi);
        }
        else
        {
            await UpdateEstablishmentDetailsAsync(establishmentFromApi);
            return establishmentid;
        }
    }

    public async Task<bool> IsEstablishmentDraft(Guid? establishmentId)
    {
        var establishmentFromApi = (establishmentId != Guid.Empty && establishmentId != null) ?
            await GetEstablishmentByIdAsync((Guid)establishmentId) :
            new LogisticsLocationDto() { Address = new TradeAddressDto() };

        if (establishmentFromApi!.ApprovalStatus == LogisticsLocationApprovalStatus.Draft)
            return true;
        else
            return false;
    }
}
