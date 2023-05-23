﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.UnitTests.Models;

[TestFixture]
public class TradePartyTests
{
    [Test]
    public void SetTradeParty_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeParty = new TradeParty
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd"
        };

        //Assert
        tradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        tradeParty.PartyName.Should().Be("Trade party Ltd");
    }
}

[TestFixture]
public class TradePartyForCreationTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeParty = new TradePartyDTO
        {
            //Act
            PartyName = "Trade party Ltd"
        };

        //Assert
        tradeParty.PartyName.Should().Be("Trade party Ltd");
    }
}

[TestFixture]
public class RegisterTradePartyResponseTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var registerTradeParty = new TradePartyDTO
        {
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd"
        };

        //Assert
        registerTradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        registerTradeParty.PartyName.Should().Be("Trade party Ltd");
    }
}

[TestFixture]
public class LogisticsLocationTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        // arrange
        var logisticsLocationId = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");
        var tradePartyId = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d191");
        var tradeAddressId = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d189");
        var relationshipId = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d190");
        var tradeContactId = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d192");
        var created = DateTime.Now;
        var lastModified = DateTime.Now;
        var logisticsLocation = new LogisticsLocation
        {
            Id = logisticsLocationId,
            Name = "Test name",
            Address = new TradeAddress
            {
                Id = tradeAddressId,
                LineOne = "Line One",
                LineTwo = "Line Two",
                LineThree = "Line Three",
                LineFour = "Line Four",
                LineFive = "Line Five",
                PostCode = "TE12 1ST",
                CityName = "Test",
                TradeCountry = "UK"
            },
            TradeAddressId = tradeAddressId,
            CreatedDate = created,
            LastModifiedDate = lastModified,
            NI_GBFlag = "true",
            EstablishmentBusinessRelationship = new LogisticLocationBusinessRelationship
            {
                RelationshipId = relationshipId,
                TradePartyId = tradePartyId,
                LogisticsLocationId = logisticsLocationId,
                CreatedDate = created,
                ModifiedDate= lastModified,
                Status = "Test Status",
                ContactEmail = "test@test.com",
                TradeParty = new TradeParty
                {
                    Id = tradePartyId,
                    PartyName = "Test Party Name",
                    NatureOfBusiness = "Nature of business",
                    TradeContact = new TradeContact
                    {
                        Id = tradeContactId,
                        PersonName = "Test name",
                        TelephoneNumber = "1234567890"
                    }
                }
            }

        };

        logisticsLocation.EstablishmentBusinessRelationship.LogisticsLocation = logisticsLocation;
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.TradeAddress = logisticsLocation.Address;

        // assert
        logisticsLocation.Id.Should().Be(logisticsLocationId);
        logisticsLocation.Name.Should().Be("Test name");
        logisticsLocation.Address.Id.Should().Be(tradeAddressId);
        logisticsLocation.Address.LineOne.Should().Be("Line One");
        logisticsLocation.Address.LineTwo.Should().Be("Line Two");
        logisticsLocation.Address.LineThree.Should().Be("Line Three");
        logisticsLocation.Address.LineFour.Should().Be("Line Four");
        logisticsLocation.Address.LineFive.Should().Be("Line Five");
        logisticsLocation.Address.PostCode.Should().Be("TE12 1ST");
        logisticsLocation.Address.CityName.Should().Be("Test");
        logisticsLocation.Address.TradeCountry.Should().Be("UK");
        logisticsLocation.TradeAddressId.Should().Be(tradeAddressId);
        logisticsLocation.CreatedDate.Should().Be(created);
        logisticsLocation.LastModifiedDate.Should().Be(lastModified);
        logisticsLocation.NI_GBFlag.Should().Be("true");
        logisticsLocation.EstablishmentBusinessRelationship.RelationshipId.Should().Be(relationshipId);
        logisticsLocation.EstablishmentBusinessRelationship.TradePartyId.Should().Be(tradePartyId);
        logisticsLocation.EstablishmentBusinessRelationship.LogisticsLocationId.Should().Be(logisticsLocationId);
        logisticsLocation.EstablishmentBusinessRelationship.CreatedDate.Should().Be(created);
        logisticsLocation.EstablishmentBusinessRelationship.ModifiedDate.Should().Be(lastModified);
        logisticsLocation.EstablishmentBusinessRelationship.Status.Should().Be("Test Status");
        logisticsLocation.EstablishmentBusinessRelationship.ContactEmail.Should().Be("test@test.com");
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.Id.Should().Be(tradePartyId);
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.PartyName.Should().Be("Test Party Name");
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.NatureOfBusiness.Should().Be("Nature of business");
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.TradeContact.Id.Should().Be(tradeContactId);
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.TradeContact.PersonName.Should().Be("Test name");
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.TradeContact.TelephoneNumber.Should().Be("1234567890");
        logisticsLocation.EstablishmentBusinessRelationship.LogisticsLocation.Should().Be(logisticsLocation);
        logisticsLocation.EstablishmentBusinessRelationship.TradeParty.TradeAddress.Should().Be(logisticsLocation.Address);
    }
}