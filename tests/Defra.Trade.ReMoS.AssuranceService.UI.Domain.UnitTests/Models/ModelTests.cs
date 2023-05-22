using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using System.Reactive;

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
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies"
        };

        //Assert
        tradeParty.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));
        tradeParty.PartyName.Should().Be("Trade party Ltd");
        tradeParty.NatureOfBusiness.Should().Be("Wholesale Hamster Supplies");
    }
}

[TestFixture]
public class TradePartyForCreationTests
{
    [Test]
    public void GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var registerTradeParty = new TradePartyDTO
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
        var tradeContact = new TradeContact
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
        var tradeContact = new TradeContactDTO
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            TradePartyId = Guid.Parse("c16eb7a7-2949-4880-b5d7-1205f4f7d548"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890",
            EmailAddress = "John.Doe@contactemail.com"
        };

        //Assert
        tradeContact.Id.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"));
        tradeContact.TradePartyId.Should().Be(Guid.Parse("c16eb7a7-2949-4880-b5d7-1205f4f7d548"));
        tradeContact.PersonName.Should().Be("John Doe");
        tradeContact.TelephoneNumber.Should().Be("1234567890");
        tradeContact.EmailAddress.Should().Be("John.Doe@contactemail.com");
    }
}

[TestFixture]
public class TradeAddressTests
{
    [Test]
    public void SetTradeAddress_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeAddress = new TradeAddress
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
public class TradeAddressDTOTests
{
    [Test]
    public void SetTradeAddressDTO_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeAddress = new TradeAddressDTO
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
        var tradeContact = new TradeContact
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890"
        };

        var tradeAddress = new TradeAddress
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

        var tradeParty = new TradeParty
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            TradeContact = tradeContact,
            TradeAddress = tradeAddress
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

        tradeParty.TradeContact.Should().Be(tradeContact);
        tradeParty.TradeAddress.Should().Be(tradeAddress);
    }
}

[TestFixture]
public class TradePartyDTORelationshipTests
{
    [Test]
    public void SetTradeContactDTOAndTradeContactDTOAndTradeAddressDTO_GivenValidValues_FieldsSetToGivenValues()
    {
        //Arrange
        var tradeContact = new TradeContactDTO
        {
            //Act
            Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d901"),
            PersonName = "John Doe",
            TelephoneNumber = "1234567890",
            EmailAddress = "John.Doe@contactemail.com"
        };

        var tradeAddress = new TradeAddressDTO
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

        var tradeParty = new TradePartyDTO
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