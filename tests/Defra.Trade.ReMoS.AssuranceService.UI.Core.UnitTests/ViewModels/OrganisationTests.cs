using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using NUnit.Framework;
using System.Collections;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.ViewModels;

[TestFixture]
public class OrganisationTests
{
    private Organisation org2 = new Organisation
    {
        OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
        PracticeName = "Test org"
    };

    [TestCaseSource(typeof(OrganisationsData), nameof(OrganisationsData.TestCases))]
    public bool Equals_Returns_False_When_Item_Passed_Is_Null(Organisation org1, Organisation org2)
    {
        return org1.Equals(org2);
    }
}

public class OrganisationsData
{
    public static IEnumerable TestCases
    {
        get
        {
            yield return new TestCaseData(
                new Organisation { },
                new Organisation { })
                .Returns(false);
            yield return new TestCaseData(
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = "test org"
                },
                new Organisation
                {

                })
                .Returns(false);
            yield return new TestCaseData(
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = "test org"
                },
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = string.Empty
                })
                .Returns(false);
            yield return new TestCaseData(
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = "test org"
                },
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = null!
                })
                .Returns(false);
            yield return new TestCaseData(
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = "test org"
                },
                new Organisation
                {
                    OrganisationId = Guid.Parse("910f895c-ab58-45d2-bbbc-d5c74c663701"),
                    PracticeName = "test org"
                })
                .Returns(true);
        }
    }
}