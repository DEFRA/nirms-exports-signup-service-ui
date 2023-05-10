using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.ResponseTypes;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class DataConsumer : IDataConsumer
{
    private readonly IGraphQLClient _client;

    public DataConsumer(IGraphQLClient client)
    {
        _client = client;
    }

    public async Task<List<TradeParty>> GetAllTradeParties()
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query {
                  tradeParties {
                    id
                    name
                  }
                }"
        };

        var response = await _client.SendQueryAsync<ResponseTradePartyCollectionType>(query);
        return response.Data.TradeParties;
    }


}
