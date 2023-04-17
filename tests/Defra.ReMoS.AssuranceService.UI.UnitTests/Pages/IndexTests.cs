using Defra.ReMoS.AssuranceService.UI.Pages;
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
    [Fact]
    public void OnGet_SetMessageToHelloWorld()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<IndexModel>>();
        var indexModel = new IndexModel(mockLogger.Object);

        //Act
        indexModel.OnGet();

        //Assert
        Assert.Equal("Hello World!", indexModel.Message);
    }

    [Fact]
    public void OnPost_WhenModelStatisIsvalid_SetErrorMessageToEmpty()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<IndexModel>>();
        var indexModel = new IndexModel(mockLogger.Object);

        //Act
        indexModel.OnPost();

        //Assert
        Assert.Equal(string.Empty, indexModel.ErrorMessage);
    }

    [Fact]
    public void OnPost_WhenModelStatisIsInvalid_SetErrorMessageToError()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<IndexModel>>();
        var indexModel = new IndexModel(mockLogger.Object);
        indexModel.ModelState.AddModelError("Message.Text", "The Text field is required.");

        //Act
        indexModel.OnPost();

        //Assert
        Assert.Equal("There is an error", indexModel.ErrorMessage);
    }
}
