using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reactive;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Domain.UnitTests.Models;

[TestFixture]
public class TradePartyTests
{
    [Test]
    public void SetTradeParty_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeParty = new TradePartyDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            FboNumber = "fbonum-123456-fbonum",
            RegulationsConfirmed = true,
            SignUpRequestSubmittedBy = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
        };

        //Assert
        tradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        tradeParty.PartyName.Should().Be("Trade party Ltd");
        tradeParty.NatureOfBusiness.Should().Be("Wholesale Hamster Supplies");
        tradeParty.FboNumber.Should().Be("fbonum-123456-fbonum");
        tradeParty.RegulationsConfirmed.Should().Be(true);
        tradeParty.SignUpRequestSubmittedBy.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
    }
}

[TestFixture]
public class TradePartyForCreationTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var registerTradeParty = new TradePartyDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };

        //Assert
        registerTradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        registerTradeParty.PartyName.Should().Be("Trade party Ltd");
        registerTradeParty.NatureOfBusiness.Should().Be("Wholesale Hamster Supplies");
        registerTradeParty.CountryName.Should().Be("United Kingdom");
    }
}

[TestFixture]
public class TradeContactTests
{
    [Test]
    public void SetTradeContact_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeContact = new TradeContactDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890"
        };

        //Assert
        tradeContact.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"));
        tradeContact.PersonName.Should().Be("John Doe");
        tradeContact.TelephoneNumber.Should().Be("1234567890");
    }
}

[TestFixture]
public class TradeContactDTOTests
{
    [Test]
    public void SetTradeContactDTO_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeContact = new TradeContactDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            TradePartyId = Guid.Parse("c16eb7a7-2949-4880-b5d7-1205f4f7d548"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890",
            Email = "John.Doe@contactemail.com"
        };

        //Assert
        tradeContact.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"));
        tradeContact.TradePartyId.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-1205f4f7d548"));
        tradeContact.PersonName.Should().Be("John Doe");
        tradeContact.TelephoneNumber.Should().Be("1234567890");
        tradeContact.Email.Should().Be("John.Doe@contactemail.com");
    }
}

[TestFixture]
public class TradeAddressTests
{
    [Test]
    public void SetTradeAddress_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeAddress = new TradeAddressDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"),
            LineOne = "123 Lorem Ipsum Drive",
            LineTwo = "Ada Lovelace Lane",
            LineThree = "Macintosh",
            LineFour = "Hello World County",
            LineFive = String.Empty,
            PostCode = "EC1N 2PB",
            CityName = "London",
            County = "Surrey",
            TradeCountry = "United Kingdom", 
        };

        //Assert
        tradeAddress.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"));
        tradeAddress.LineOne.Should().Be("123 Lorem Ipsum Drive");
        tradeAddress.LineTwo.Should().Be("Ada Lovelace Lane");
        tradeAddress.LineThree.Should().Be("Macintosh");
        tradeAddress.LineFour.Should().Be("Hello World County");
        tradeAddress.LineFive.Should().Be(String.Empty);
        tradeAddress.PostCode.Should().Be("EC1N 2PB");
        tradeAddress.CityName.Should().Be("London");
        tradeAddress.County.Should().Be("Surrey");
        tradeAddress.TradeCountry.Should().Be("United Kingdom");
    }
}

[TestFixture]
public class TradeAddressDTOTests
{
    [Test]
    public void SetTradeAddressDTO_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeAddress = new TradeAddressDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"),
            LineOne = "123 Lorem Ipsum Drive",
            LineTwo = "Ada Lovelace Lane",
            LineThree = "Macintosh",
            LineFour = "Hello World County",
            LineFive = String.Empty,
            PostCode = "EC1N 2PB",
            CityName = "London",
            TradeCountry = "United Kingdom",
        };

        //Assert
        tradeAddress.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"));
        tradeAddress.LineOne.Should().Be("123 Lorem Ipsum Drive");
        tradeAddress.LineTwo.Should().Be("Ada Lovelace Lane");
        tradeAddress.LineThree.Should().Be("Macintosh");
        tradeAddress.LineFour.Should().Be("Hello World County");
        tradeAddress.LineFive.Should().Be(String.Empty);
        tradeAddress.PostCode.Should().Be("EC1N 2PB");
        tradeAddress.CityName.Should().Be("London");
        tradeAddress.TradeCountry.Should().Be("United Kingdom");
    }
}

[TestFixture]
public class TradePartyRelationshipTests
{
    [Test]
    public void SetTradeContactAndTradeContactAndTradeAddress_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeContact = new TradeContactDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890"
        };

        var tradeAddress = new TradeAddressDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"),
            LineOne = "123 Lorem Ipsum Drive",
            LineTwo = "Ada Lovelace Lane",
            LineThree = "Macintosh",
            LineFour = "Hello World County",
            LineFive = String.Empty,
            PostCode = "EC1N 2PB",
            CityName = "London",
            TradeCountry = "United Kingdom",
        };

        var tradeParty = new TradePartyDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            Contact = tradeContact,
            Address = tradeAddress
        };

        //Assert
        tradeContact.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"));
        tradeContact.PersonName.Should().Be("John Doe");
        tradeContact.TelephoneNumber.Should().Be("1234567890");

        tradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        tradeParty.PartyName.Should().Be("Trade party Ltd");
        tradeParty.NatureOfBusiness.Should().Be("Wholesale Hamster Supplies");

        tradeAddress.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"));
        tradeAddress.LineOne.Should().Be("123 Lorem Ipsum Drive");
        tradeAddress.LineTwo.Should().Be("Ada Lovelace Lane");
        tradeAddress.LineThree.Should().Be("Macintosh");
        tradeAddress.LineFour.Should().Be("Hello World County");
        tradeAddress.LineFive.Should().Be(String.Empty);
        tradeAddress.PostCode.Should().Be("EC1N 2PB");
        tradeAddress.CityName.Should().Be("London");
        tradeAddress.TradeCountry.Should().Be("United Kingdom");

        tradeParty.Contact.Should().Be(tradeContact);
        tradeParty.Address.Should().Be(tradeAddress);
    }
}

[TestFixture]
public class TradePartyDTORelationshipTests
{
    [Test]
    public void SetTradeContactDTOAndTradeContactDTOAndTradeAddressDTO_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeContact = new TradeContactDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            TradePartyId = Guid.Parse("c14eb7a7-2949-4880-b5d7-0405f4f7d901"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890",
            Email = "John.Doe@contactemail.com"
        };

        var tradeAddress = new TradeAddressDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"),
            LineOne = "123 Lorem Ipsum Drive",
            LineTwo = "Ada Lovelace Lane",
            LineThree = "Macintosh",
            LineFour = "Hello World County",
            LineFive = String.Empty,
            PostCode = "EC1N 2PB",
            CityName = "London",
            TradeCountry = "United Kingdom",
        };

        var tradeParty = new TradePartyDto
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom",
            Contact = tradeContact,
            Address = tradeAddress
        };

        //Assert
        tradeContact.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"));
        tradeContact.TradePartyId.Should().Be(Guid.Parse("c14eb7a7-2949-4880-b5d7-0405f4f7d901"));
        tradeContact.PersonName.Should().Be("John Doe");
        tradeContact.TelephoneNumber.Should().Be("1234567890");

        tradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        tradeParty.PartyName.Should().Be("Trade party Ltd");
        tradeParty.NatureOfBusiness.Should().Be("Wholesale Hamster Supplies");
        tradeParty.CountryName.Should().Be("United Kingdom");

        tradeAddress.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d568"));
        tradeAddress.LineOne.Should().Be("123 Lorem Ipsum Drive");
        tradeAddress.LineTwo.Should().Be("Ada Lovelace Lane");
        tradeAddress.LineThree.Should().Be("Macintosh");
        tradeAddress.LineFour.Should().Be("Hello World County");
        tradeAddress.LineFive.Should().Be(String.Empty);
        tradeAddress.PostCode.Should().Be("EC1N 2PB");
        tradeAddress.CityName.Should().Be("London");
        tradeAddress.TradeCountry.Should().Be("United Kingdom");


        tradeParty.Contact.Should().Be(tradeContact);
        tradeParty.Address.Should().Be(tradeAddress);
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
        var logisticsLocation = new LogisticsLocationDto
        {
            Id = logisticsLocationId,
            Name = "Test name",
            Address = new TradeAddressDto
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
            NI_GBFlag = "GB",

        };

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
        logisticsLocation.NI_GBFlag.Should().Be("GB");
    }
}