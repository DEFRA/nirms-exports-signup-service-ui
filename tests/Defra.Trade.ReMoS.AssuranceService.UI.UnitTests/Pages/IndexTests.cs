using Defra.ReMoS.AssuranceService.UI.Hosting.Pages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.ReMoS.AssuranceService.UI.UnitTests.Pages;

public class IndexTests
{
    private readonly IndexModel _pageModel;

    public IndexTests()
    {
        var _mockLogger = new Mock<ILogger<IndexModel>>();
        _pageModel = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void OnGet_SetMessageToHelloWorld()
    {
        //Act
        _pageModel.OnGet();

        //Assert
        _pageModel.Message.Should().Be("Hello World!");
    }

    [Fact]
    public void OnPost_WhenModelStatisIsValid_SetErrorMessageToEmpty()
    {
        //Act
        _pageModel.OnPost();

        //Assert
        _pageModel.ErrorMessage.Should().Be(string.Empty);
    }

    [Fact]
    public void OnPost_WhenModelStatisIsInvalid_SetErrorMessageToError()
    {
        //Arrange
        _pageModel.ModelState.AddModelError("Message", "The message is required.");

        //Act
        _pageModel.OnPost();

        //Assert
        _pageModel.ErrorMessage.Should().Be("There is an error");
    }
}
