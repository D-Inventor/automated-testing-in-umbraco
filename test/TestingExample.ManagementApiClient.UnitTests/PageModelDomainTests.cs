using System.Globalization;

using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.ManagementApiClient.UnitTests;

/* Test cases:
 * - Can add a domain
 */
public class PageModelDomainTests
{
    [Fact]
    public void ShouldAddDomain()
    {
        // given
        var page = new PageModel<ExampleContentModel>(contentType: Guid.NewGuid());

        // when
        page.AddDomain(new DomainModel(CultureInfo.GetCultureInfo("en-US"), "example.com"));

        // then
        Assert.Contains(page.Domains, domain => domain.Culture.Equals(CultureInfo.GetCultureInfo("en-US")) && domain.Domain == "example.com");
    }

    private class ExampleContentModel(VariationModel variation, PageModel<ExampleContentModel> page)
        : ContentModel<ExampleContentModel>(variation, page);
}