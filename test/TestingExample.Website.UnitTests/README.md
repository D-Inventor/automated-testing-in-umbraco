# Unit testing in Umbraco

This testing project demonstrates unit testing with Umbraco content.

## Testing with Published content

When your code uses published content, you need fake published content models. The tests in [SubpageTests.cs](./SubpageTests.cs) demonstrate how you can write tests with fake models.

### Example: Setup propertyvalues on a published content model

```csharp
// Option 1: directly use builder
ExampleWebsite fakeContent = FakePublishedContent.Generate<ExampleWebsite>();

fakeContent.PropertyValues()
    // 👇 content is of type `ExampleWebsite`
    .Set(content => content.Title, "Hello world")
    .Set(content => content.Date, new DateTime(2025, 1, 8));

Assert.Equal("Hello world", fakeContent.Title);

// Option 2: Fluent API
ExampleWebsite fakeContent = FakePublishedContent
    .Generate<ExampleWebsite>()
    .WithPropertyValues(props => props
        .Set(content => content.Title, "Hello world")
        .Set(content => content.Date, new DateTime(2025, 1, 8)));

Assert.Equal("Hello world", fakeContent.Title);
```

### Example: Setup relationships between published content models

```csharp
FakePublishedContentOperations _publishedContentOperations = new ();
ExampleWebsite fakeParent = FakePublishedContent.Generate<ExampleWebsite>();
ExampleSubpage fakeChild = FakePublishedContent.Generate<ExampleSubpage>();

_publishedContentOperations.SetParent(fakeChild, fakeParent);

Assert.Equal(fakeParent, fakeChild.Parent(_publishedContentOperations));
```

### Relevant source code files:

- The implementation of fake published content models: [FakePublishedContent.cs](./PublishedContent/FakePublishedContent.cs)
- Abstracting relations between content behind an interface: [IPublishedContentOperations.cs](../../src/TestingExample.Website/PublishedContent/IPublishedContentOperations.cs) and [ExampleWebsiteRequestHandler.cs](../../src/TestingExample.Website/Homepage/ExampleWebsiteRequestHandler.cs)

### Why abstract relationsships?

When you use the `.Children()` method that comes with Umbraco out of the box, you will run into a null-reference exception. That's because behind the scenes, Umbraco uses a service locator pattern. The service locator doesn't exist while you're running unit tests.

### Additional recommendations

Do not use `IPublishedContentOperations.cs` when the relationship is complex (Anything that requires multiple calls to methods like `.Children()`, `.Ancestors()` or `.Parent()`). Instead, create a dedicated abstraction for navigating to specific types of content.

```csharp
// ❌ Complex and difficult
FakePublishedContentOperations _publishedContentOperations = new ();

ExampleWebsite fakeHome = FakePublishedContent.Generate<ExampleWebsite>();
ExampleSubpage fakeContentPage = FakePublishedContent.Generate<ExampleSubpage>();
ExampleNewsOverview fakeNewsOverview = FakePublishedContent.Generate<ExampleNewsOverview>();

_publishedContentOperations.SetParent(fakeContentPage, fakeHome);
_publishedContentOperations.SetParent(fakeNewsOverview, fakeHome);

var result = fakeContentPage
    .Parent<ExampleWebsite>(_publishedContentOperations)
    .FirstChild<ExampleNewsOverview>(_publishedContentOperations);

// ✅ Easy to read and simple to set up
FakeNewsOverviewNavigation _newsNavigation = new();

ExampleSubpage fakeContentPage = FakePublishedContent.Generate<ExampleSubpage>();
ExampleNewsOverview fakeNewsOverview = FakePublishedContent.Generate<ExampleNewsOverview>();
_newsNavigation.SetNewsOverview(fakeNewsOverview);

var result = _newsNavigation.FindNews(fakeContentPage);
```

Don't forget to test the implementation of your navigation!
