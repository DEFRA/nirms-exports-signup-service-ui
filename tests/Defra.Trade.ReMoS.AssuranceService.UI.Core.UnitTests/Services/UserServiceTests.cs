using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services;

[TestFixture]
public class UserServiceTests
{
    private UserService? _userService;

    [Test]
    public void GetDefraOrgsForUser_Return_OrgsDictionary_When_Orgs_Claim_Present()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "example name"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("userEnrolledOrganisations", "[{\"organisationId\":\"05f7570d-ebb9-e911-a970-000d3a29be4a\",\"practiceName\":\"Kaka\"},{\"organisationId\":\"1b9ac18a-e8b9-e911-a978-000d3a28da35\",\"practiceName\":\"AM\"},{\"organisationId\":\"05f7570d-ebb9-e911-a970-000d3a29be4a\",\"practiceName\":\"Kaka\"}]"),
        }, "mock"));
        _userService = new UserService();

        // Act
        var result = _userService.GetDefraOrgsForUser(user);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        result.Should().BeOfType<Dictionary<Guid, string>>();
    }

    [Test]
    public void GetDefraOrgsForUser_Return_Empty_Dictionary_When_Orgs_Claim_Not_Present()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "example name"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("custom-claim", "example claim value"),
        }, "mock"));
        _userService = new UserService();

        // Act
        var result = _userService.GetDefraOrgsForUser(user);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetUserContactId_ReturnEmptyGuid_IfClaimNotFound()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "example name"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
        }, "mock"));
        _userService = new UserService();

        // Act
        var result = _userService.GetUserContactId(user);

        // Assert
        result.Should().Be(Guid.Empty);
    }

    [Test]
    public void GetUserContactId_ReturnValidGuid_IfClaimPresent()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "example name"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("contactId", contactId.ToString()),
        }, "mock"));
        _userService = new UserService();

        // Act
        var result = _userService.GetUserContactId(user);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Be(contactId);
    }
}
