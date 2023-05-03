using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Extensions;

public class ModelStateExtensionsTests
{
    [Fact]
    public void HasError_Returns_True_If_Error_Exists()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("TestError", "Something broke");

        // Act
        var isValid = modelState.HasError("TestError");

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void HasError_Returns_False_If_Error_Doesnt_Exist()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Error1", "Something broke");
        modelState.AddModelError("Error2", "Something broke");
        modelState.AddModelError("Error3", "Something broke");

        // Act
        var isValid = modelState.HasError("Error4");

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void HasError_Returns_False_If_ModelState_Empty()
    {
        // Arrange
        var modelState = new ModelStateDictionary();

        // Act
        var isValid = modelState.HasError("Error4");

        // Assert
        isValid.Should().BeFalse();
    }
}
