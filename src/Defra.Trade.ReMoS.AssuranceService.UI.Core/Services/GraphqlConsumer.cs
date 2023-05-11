using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;

public class GraphqlConsumer : IGraphqlConsumer
{
    private readonly IGraphQLClient _client;

    public GraphqlConsumer(IGraphQLClient client)
    {
        _client = client;
    }

    public async Task<List<TradeParty>> GetAllTradePartiesAsync()
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query {
                  tradeParties {
                    id
                    name
                  }
                }"
        };

        var response = await _client.SendQueryAsync<TradePartyCollectionResponse>(request);
        var parties = response.Data.TradeParties;
        return parties;
    }

    public async Task<TradeParty> RegisterTradePartyAsync(TradePartyForCreation tradePartyToCreate)
    {
        var request = new GraphQLRequest
        {
            Query = $@"
                mutation {{
                  registerTradeParty (input: {{
                    name: ""{tradePartyToCreate.Name}"" }}) 
                  {{ 
                    name
                      id
                  }}
                }}",
            Variables = new { tradeParty = tradePartyToCreate }
        };

        var response = await _client.SendQueryAsync<RegisterTradePartyResponse>(request);
        var partyCreated = response.Data.RegisterTradeParty;
        return partyCreated;
    }


}
