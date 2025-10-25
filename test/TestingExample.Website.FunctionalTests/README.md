# Acceptance testing / End-to-end testing
This testing project demonstrates acceptance testing. When an application is deployed to a real environment, you can use acceptance testing to interact with your system as if you're a user and you can verify that your application responds how you expect.

## How to run the tests in this project
In order to run these tests, you need to host the website in a real environment. The easiest way to "host" the project is by simply starting the website in debug mode:

```
dotnet run --project .\src\TestingExample.Website\
```

When the website is running, you need to configure several settings. You need to log in to the backoffice and create an API user with client credentials. Make a note of the client id and client secret. This API User must have full access to the content tree, so give it at least the role of Editor. Use the following commands to configure user secrets in this project:

```
cd .\test\TestingExample.Website.FunctionalTests
dotnet user-secrets set ScenarioBuilder:ClientSecret "INSERT CLIENT SECRET HERE"
dotnet user-secrets set ScenarioBuilder:ClientID "INSERT CLIENT ID HERE"
dotnet user-secrets set domain "localhost:44376"
```

You are now ready to run the tests. Use the following commands to run the end-to-end tests:

```
cd .\test\TestingExample.Website.FunctionalTests
dotnet test
```

| ⚠️ Loss of content! |
|---|
| These tests interact with the environment that you connect to. This includes deleting the entire content tree and reconstructing it for testing scenarios. Do not point these tests at a production environment or any other environment where the content is important! |

## Relevant source code

- [Example content model for constructing a content scenario](./Domain/HomepageContent.cs)
- [Example test that tests a simple content scenario](./HomepageTests.cs)
- [Management API client implementation](../TestingExample.ManagementApiClient/)
- [Implementation of the scenario builder](../TestingExample.ManagementApiClient/Scenario/ScenarioBuilder.cs)

## How it works
Each test prepares a scenario using the `ScenarioBuilder`. This builder is an interface that allows you to specify content and their relations. You use the scenario builder to define how the content tree should look for your specific test. By calling the `.BuildAsync( ... )` method, this content tree definition is applied to your Umbraco website.

The ScenarioBuilder works because Umbraco exposes a _Content Management API_ which has operations for creating and changing content in your application. The scenario builder has access to the content management api, because you created an API User in the backoffice.

## How to use
There is a decent amount of upfront work that you need to do to get started. All the steps are explained here in recommended order.

### Copy/paste the api client project
You need all the code in `TestingExample.ManagementApiClient`. It contains all the base types that we need and it has the api client that connects to your Umbraco website. Copy and paste it into your own project and add a reference to it in your automated tests.

_Fun fact: I built the api client and the scenario builder using test-driven development. The tests that I used to develop it, are in [TestingExample.ManagementApiClient.UnitTests](../TestingExample.ManagementApiClient.UnitTests/)._

### Configure a connection to your application
The `ScenarioBuilder` has several dependencies that you need before you can use it:
- A `TokenConfiguration`
- A `TokenClient`
- A `TokenManager`
- A `DocumentClient`
- An `HttpClient`

All of these things are set up by [UmbracoWebsiteFixture](./UmbracoWebsiteFixture.cs) and [TestConfiguration](./TestConfiguration.cs). You may copy/paste those types into your testing project to get started. [If you use these types, you can configure your connection as explained in the earlier chapter](#how-to-run-the-tests-in-this-project).

### Define a minimal content tree
You should begin by defining your content types as models. A minimal content type model would look like this:

```csharp
public class DetailPageContentModel(VariationModel variation, PageModel<DetailPageContentModel> page)
    : ContentModel<DetailPageContentModel>(variation, page)
{
}
```

Look at your application and define what a minimal content model would look like. What content is strictly necessary, in order to have a working application? For example: "For a minimal functional application, I need a root node, a homepage, a 404 page and at least one content page". Define empty content models, like the example above, for the content types that you need for your minimal example. In this case: "I need a website root type, a homepage type and a content page type".

The next step is to create your minimal content tree. Only the tree structure, without the property values. You would make a call like this to the scenario builder to create new content items:

```csharp
var detailpage = Scenario.Add<DetailPageContentModel>(
    contentType: Guid.Parse("fe1eef0b-4752-4c91-a6d4-fa0a46f3e7e3"),
    template: Guid.Parse("08c41579-0c2e-4bd8-9a8d-8cb21b1841ca"),
    parent: Guid.Parse("e1ed3777-9ac5-422d-9a36-f3227cd01845"));
```

You'll find that it quickly becomes very verbose, so it is wise to define some crucial ids in a single place. This project saves the ids of important content in [DefaultContent.cs](./Domain/DefaultContent.cs). References to content type ids and template ids are stored as static properties on each model, on the [HomepageContent](./Domain/HomepageContent.cs) for example. You should also consider an extension method for creating specific content types:

```csharp
public static class BaseContentScenario
{
    public static PageModel<DetailPageContentModel> AddNotFound(this ScenarioBuilder scenario)
    {
        var result = scenario.Add<DetailPageContentModel>(
            id: DefaultContent.NotFoundPageId,
            contentType: DetailPageContentModel.ContentType,
            template: DetailPageContentModel.Template,
            parent: DefaultContent.HomepageId);

        result.AddVariation(new VariationModel(Variant.Culture("en-US"), "404 Page not found"));

        return result;
    } 
}

var detailpage = Scenario.AddNotFound();
```

When you have extensions for your essential content types, you can create a single extension to set up your base scenario:

```csharp
public static class BaseContentScenario
{
    public static ScenarioBuilder WithBasicContent(this ScenarioBuilder builder)
    {
        builder.AddWebsiteRoot();
        builder.AddHomepage();
        builder.AddNotFoundPage();
        builder.AddPrivacyPolicy();

        return builder;
    }
}
```

At this point, you should write a dummy test and let the scenario builder build this content tree for you.

```csharp
[Collection(nameof(UmbracoWebsiteFixture))]
public sealed class HomepageTests(UmbracoWebsiteFixture website)
{
    public UmbracoWebsiteFixture Website { get; } = website;
    public ScenarioBuilder Scenario { get; } = website.NewScenario().WithBasicContent();

    [Fact]
    public async Task ShouldProduceContentTree()
    {
        await Scenario.BuildAsync(TestContext.Current.CancellationToken);

        // Let's fail the test for now, because it's not really testing anything yet, we're just setting up the content
        Assert.Fail();
    }
}
```

After you run the test, do you see the content in the backoffice that you defined?

### Populate properties and publish
To populate the properties on your content, you need to add operations to your content model. These operations should reflect the structure of your content. For example: your homepage could have a title and an introduction, which together form a "header". In that case, define a method `WithHeader( ... )` on your homepage content model:

```csharp
public class HomePageContentModel(VariationModel variation, PageModel<HomePageContentModel> page)
    : ContentModel<HomePageContentModel>(variation, page)
{
    public HomePageContentModel WithHeader(string title, string intro)
        => this
        .SetValue("title", title)
        .SetValue("intro", intro);
}
```

The string `"title"` is the alias of the property as defined in the backoffice. The value is an object, same as what Umbraco would post to the management api when editing content in the backoffice.

Your base content method can be updated to configure the content, to add domains and to publish the content:

```csharp
public static class BaseContentScenario
{
    public static ScenarioBuilder WithBasicContent(this ScenarioBuilder builder)
    {
        builder.AddWebsiteRoot()
            .HasContent(Variation.Culture("en-US"), content => content.IsPublished())
            .HasDomain(CultureInfo.GetCultureInfo("en-US"), builder.BaseUri.ToString());
        
        builder.AddHomepage()
            .HasContent(Variation.Culture("en-US"), content => content
                .WithHeader(title: "Welcome to our website", intro: "Find all our services below.")
                .IsPublished());
        
        builder.AddNotFoundPage();
        builder.AddPrivacyPolicy();

        return builder;
    }
}
```

Run your dummy test again. Is the content published? Does it have the properties that you configured? Can you visit the website and see the content in your browser?

You are now ready to simulate scenario's and run automated tests.