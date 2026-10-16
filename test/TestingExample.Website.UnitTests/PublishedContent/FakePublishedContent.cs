using Bogus;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal sealed class FakePublishedContent(int id, string name)
    : FakePublishedElement(id, name), IPublishedContent
{
    private static readonly Faker<FakePublishedContent> _contentGenerator = new Faker<FakePublishedContent>()
        .CustomInstantiator(f => new FakePublishedContent(f.Random.Number(), f.Random.Words(5)));

    public string? UrlSegment { get; set; }

    public int Level { get; set; }

    public string Path { get; set; } = string.Empty;

    public int? TemplateId { get; set; }

    public static T Generate<T>()
        where T : IPublishedContent
        => Generate().WrapIn<T>();

    public static FakePublishedContent Generate()
        => _contentGenerator.Generate();
}
