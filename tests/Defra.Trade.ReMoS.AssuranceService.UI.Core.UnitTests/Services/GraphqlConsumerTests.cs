using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services;

[TestFixture]
public class GraphqlConsumerTests
{
    [Test]
    public async Task GetAllTradePartiesAsync_ReturnsTradePartyCollection()
    {
        //Arrange
        var mockGraphqlClient = new Mock<IGraphQLClient>();
        var response = new GraphQLResponse<TradePartyCollectionResponse>()
        {
            Data = new TradePartyCollectionResponse
            {
                TradeParties = new List<TradeParty>()
                {
                    new TradeParty() { Id = Guid.NewGuid(), Name = "Any name" },
                    new TradeParty() { Id = Guid.NewGuid(), Name = "Any name" },
                }
            }
        };

        mockGraphqlClient
            .Setup(x => x.SendQueryAsync<TradePartyCollectionResponse>(
                It.IsAny<GraphQLRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var graphqlConsumer = new GraphqlConsumer(mockGraphqlClient.Object);

        //Act
        var result = await graphqlConsumer.GetAllTradePartiesAsync();

        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().BeAssignableTo<List<TradeParty>>();

    }

    [Test]
    public async Task RegisterTradePartyAsync_ReturnsTradeParty()
    {
        //Arrange
        var mockGraphqlClient = new Mock<IGraphQLClient>();
        var response = new GraphQLResponse<RegisterTradePartyResponse>()
        {
            Data = new RegisterTradePartyResponse
            {
                RegisterTradeParty = new TradeParty
                {
                    Id = Guid.NewGuid(),
                    Name = "Any name"
                }
            }
        };

        mockGraphqlClient
            .Setup(x => x.SendQueryAsync<RegisterTradePartyResponse>(
                It.IsAny<GraphQLRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var graphqlConsumer = new GraphqlConsumer(mockGraphqlClient.Object);
        var tradePartyToCreate = new TradePartyForCreation { Name = "Any name" };

        //Act
        var result = await graphqlConsumer.RegisterTradePartyAsync(tradePartyToCreate);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TradeParty>();

    }



}
