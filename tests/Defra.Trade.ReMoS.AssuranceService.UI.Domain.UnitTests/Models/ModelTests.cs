﻿using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.UnitTests.Models;

[TestFixture]
public class TradePartyTests
{
    [Test]
    public void SetTradeParty_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeParty = new TradeParty();

        //Act
        tradeParty.Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");
        tradeParty.Name = "Trade party Ltd";

        //Assert
        tradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        tradeParty.Name.Should().Be("Trade party Ltd");
    }
}

[TestFixture]
public class TradePartyForCreationTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeParty = new TradePartyForCreation();

        //Act
        tradeParty.Name = "Trade party Ltd";

        //Assert
        tradeParty.Name.Should().Be("Trade party Ltd");
    }
}

[TestFixture]
public class RegisterTradePartyResponseTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var registerTradeParty = new TradeParty
        {
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            Name = "Trade party Ltd"
        };

        //Assert
        registerTradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        registerTradeParty.Name.Should().Be("Trade party Ltd");
    }
}