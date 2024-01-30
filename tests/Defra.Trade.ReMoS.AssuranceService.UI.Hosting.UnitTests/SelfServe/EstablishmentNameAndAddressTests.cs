using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe;

[TestFixture]
public class EstablishmentNameAndAddressTests : PageModelTestsBase
{
    private EstablishmentNameAndAddressModel? _systemUnderTest;
    protected Mock<ILogger<EstablishmentNameAndAddressModel>> _mockLogger = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ITraderService> _mockTraderService = new();        

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new EstablishmentNameAndAddressModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_NoAddressPresentIfNoSavedData()
    {
        //Act
        await _systemUnderTest!.OnGetAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"), Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"), null, "England");

        //Assert
        _systemUnderTest.EstablishmentName.Should().Be("");
        _systemUnderTest.LineOne.Should().Be("");
        _systemUnderTest.LineTwo.Should().Be("");
        _systemUnderTest.CityName.Should().Be("");
        _systemUnderTest.County.Should().Be("");
        _systemUnderTest.PostCode.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidAddress_DuplicateSpotted()
    {
        //Arrange
        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));
            

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "TES1";
        
        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
        _systemUnderTest.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be("This address has already been added as a place of dispatch - enter a different address");
        _systemUnderTest.ModelState.HasError("EstablishmentName").Should().Be(true);
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidAddress_DuplicateSpotted_FlagsChecked()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BT1";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
        _systemUnderTest.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be("This address has already been added as a place of destination - enter a different address");
        _systemUnderTest.ModelState.HasError("EstablishmentName").Should().Be(true);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidEstablishmentName_LengthCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BT1";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // Assert
        validation.Contains(new ValidationResult("Establishment name must be 100 characters or less"));
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidAddressLineOne_LengthCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BT1";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // Assert
        validation.Contains(new ValidationResult("Address line 1 must be 50 characters or less"));
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidAddressLineTwo_LengthCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BT1";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // Assert
        validation.Contains(new ValidationResult("Address line 2 must be 50 characters or less"));
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidCityName_LengthCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BT1";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // Assert
        validation.Contains(new ValidationResult("Town or city must be 100 characters or less"));
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidPostCode_LengthCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BTES1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // Assert
        validation.Contains(new ValidationResult("Post code must be 100 characters or less"));
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidCounty_LengthCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
        _systemUnderTest!.PostCode = "BT1";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // Assert
        validation.Contains(new ValidationResult("County must be 100 characters or less"));
    }


    [Test]
    public async Task OnPostSubmit_SubmitInvalidCounty_PostcodeNICheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "E15 1YL";

        _systemUnderTest.Country = "NI";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
        _systemUnderTest.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be("Enter a postcode in Northern Ireland");
        _systemUnderTest.ModelState.HasError("PostCode").Should().Be(true);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidCounty_PostcodeGBCheckFailed()
    {
        //Arrange

        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
            Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
        _mockEstablishmentService
            .Setup(action => action.CreateEstablishmentForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>()).Result)
            .Throws(new BadHttpRequestException("error message"));

        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "BT1 1YL";

        _systemUnderTest.Country = "GB";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
        _systemUnderTest.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be("Enter a postcode in England, Scotland or Wales");
        _systemUnderTest.ModelState.HasError("PostCode").Should().Be(true);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInValidRadio()
    {
        //Arrange
        _systemUnderTest!.EstablishmentName = "";
        _systemUnderTest!.LineOne = "";
        _systemUnderTest!.CityName = "";
        _systemUnderTest!.PostCode = "";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(4);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInValidRadio_GetsTradePartyData()
    {
        //Arrange
        _systemUnderTest!.EstablishmentName = "";
        _systemUnderTest!.LineOne = "";
        _systemUnderTest!.CityName = "";
        _systemUnderTest!.PostCode = "";

        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(4);
    }

    [Test]
    public async Task OnGet_HeadingSetToParameter_Successfully()
    {
        //Arrange
        var expectedHeading = "Add a place of destination";

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), null, "NI");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), null, "England");
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase(null)]
    public async Task RetrieveEstablishmentDetails_ReturnsEmptyLocation(Guid? guid)
    {
        // arrange
        _systemUnderTest!.Uprn = null;
        _systemUnderTest.EstablishmentId = guid;
        
        // act
        await _systemUnderTest.RetrieveEstablishmentDetails();

        // assert
        _systemUnderTest.EstablishmentName.Should().BeEmpty();
        _systemUnderTest.LineOne.Should().BeEmpty();
        _systemUnderTest.LineTwo.Should().BeEmpty();
        _systemUnderTest.CityName.Should().BeEmpty();
        _systemUnderTest.PostCode.Should().BeEmpty();
        _systemUnderTest.County.Should().BeEmpty();
    }

    [Test]
    public async Task RetrieveEstablishmentDetails_ReturnsTradeApiAddress()
    {
        // arrange
        _systemUnderTest!.Uprn = "1234";
        var establishment = new LogisticsLocationDto()
        {
            Name = "business name",
            Address = new TradeAddressDto()
            {
                LineOne = "line 1",
                LineTwo = "lines 2",
                PostCode = "postcode",
                CityName = "city",
                County = "county"
            }
        };
        _mockEstablishmentService.Setup(x => x.GetLogisticsLocationByUprnAsync(_systemUnderTest.Uprn)).ReturnsAsync(establishment);

        // act
        await _systemUnderTest.RetrieveEstablishmentDetails();

        // assert
        _systemUnderTest.EstablishmentName.Should().Be(establishment.Name);
        _systemUnderTest.LineOne.Should().Be(establishment.Address.LineOne);
        _systemUnderTest.LineTwo.Should().Be(establishment.Address.LineTwo);
        _systemUnderTest.CityName.Should().Be(establishment.Address.CityName);
        _systemUnderTest.PostCode.Should().Be(establishment.Address.PostCode);
        _systemUnderTest.County.Should().Be(establishment.Address.County);
    }

    [Test]
    public async Task RetrieveEstablishmentDetails_ReturnsAddress()
    {
        // arrange
        _systemUnderTest!.EstablishmentId = Guid.NewGuid();
        var establishment = new LogisticsLocationDto()
        {
            Name = "business name",
            Address = new TradeAddressDto()
            {
                LineOne = "line 1",
                LineTwo = "lines 2",
                PostCode = "postcode",
                CityName = "city",
                County = "county"
            }
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync((Guid)_systemUnderTest.EstablishmentId!)).ReturnsAsync(establishment);

        // act
        await _systemUnderTest.RetrieveEstablishmentDetails();

        // assert
        _systemUnderTest.EstablishmentName.Should().Be(establishment.Name);
        _systemUnderTest.LineOne.Should().Be(establishment.Address.LineOne);
        _systemUnderTest.LineTwo.Should().Be(establishment.Address.LineTwo);
        _systemUnderTest.CityName.Should().Be(establishment.Address.CityName);
        _systemUnderTest.PostCode.Should().Be(establishment.Address.PostCode);
        _systemUnderTest.County.Should().Be(establishment.Address.County);
    }

    [TestCase("123", "00000000-0000-0000-0000-000000000000")]
    [TestCase(null, "00000000-0000-0000-0000-000000000000")]
    [TestCase(null, null)]
    public async Task SaveEstablishmentDetails_Create(string uprn, Guid guid)
    {
        // arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "TES1";
        _systemUnderTest!.Uprn = uprn;
        _systemUnderTest.EstablishmentId = guid;
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        _mockEstablishmentService.Setup(x => x.CreateEstablishmentForTradePartyAsync(_systemUnderTest.TradePartyId, It.IsAny<LogisticsLocationDto>())).ReturnsAsync(expected);

        // act
        var result = await _systemUnderTest.SaveEstablishmentDetails();

        // assert
        result.Should().Be(expected);
        _mockEstablishmentService.Verify(x => x.CreateEstablishmentForTradePartyAsync(_systemUnderTest.TradePartyId, It.IsAny<LogisticsLocationDto>()), Times.Once());
    }

    [Test]
    public async Task SaveEstablishmentDetails_Update()
    {
        // arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.EstablishmentName = "Test name";
        _systemUnderTest!.LineOne = "Line one";
        _systemUnderTest!.LineTwo = "Line two";
        _systemUnderTest!.CityName = "City";
        _systemUnderTest!.County = "Berkshire";
        _systemUnderTest!.PostCode = "TES1";
        _systemUnderTest.EstablishmentId = Guid.NewGuid();
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync((Guid)_systemUnderTest.EstablishmentId!)).ReturnsAsync(new LogisticsLocationDto() { Address = new TradeAddressDto()});
        _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(It.IsAny<LogisticsLocationDto>())).ReturnsAsync(true);

        // act
        var result = await _systemUnderTest.SaveEstablishmentDetails();

        // assert
        result.Should().Be(_systemUnderTest.EstablishmentId);
        _mockEstablishmentService.Verify(x => x.UpdateEstablishmentDetailsAsync(It.IsAny<LogisticsLocationDto>()), Times.Once());
    }

  
}
