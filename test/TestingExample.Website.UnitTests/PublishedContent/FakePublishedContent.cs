using Bogus;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal sealed class FakePublishedContent(int id, string name)
    : FakePublishedElement, IPublishedContent
{
    private static readonly Faker<FakePublishedContent> _contentGenerator = new Faker<FakePublishedContent>()
        .CustomInstantiator(f => new FakePublishedContent(f.Random.Number(), f.Random.Words(5)));

    public int Id { get; } = id;

    public string Name { get; } = name;

    public string? UrlSegment { get; set; }

    public int SortOrder { get; set; }

    public int Level { get; set; }

    public string Path { get; set; } = string.Empty;

    public int? TemplateId { get; set; }

    public int CreatorId { get; set; }

    public DateTime CreateDate { get; set; }

    public int WriterId { get; set; }

    public DateTime UpdateDate { get; set; }

    public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; } = new Dictionary<string, PublishedCultureInfo>();

    public PublishedItemType ItemType { get; } = PublishedItemType.Content;

    public IPublishedContent? Parent => null;

    public IEnumerable<IPublishedContent> Children { get; } = [];

    public IEnumerable<IPublishedContent> ChildrenForAllCultures { get; } = [];

    public static T Generate<T>()
        where T : IPublishedContent
        => Generate().WrapIn<T>();

    public static FakePublishedContent Generate()
        => _contentGenerator.Generate();

    public bool IsDraft(string? culture = null)
    {
        return false;
    }

    public bool IsPublished(string? culture = null)
    {
        return false;
    }
}
