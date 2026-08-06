using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;

using var httpClient = new HttpClient();

var schemaJson = await httpClient.GetStringAsync("https://localhost:44376/umbraco/openapi/management.json");

var schema = await OpenApiDocument.FromJsonAsync(schemaJson);

var settings = new CSharpClientGeneratorSettings
{
    ClientBaseClass = "ManagementApiClientBase",
    AdditionalNamespaceUsages = ["TestingExample.ManagementApiClient.Authentication"],
    GenerateClientClasses = true,
    GeneratePrepareRequestAndProcessResponseAsAsyncMethods = true,
    InjectHttpClient = true,
    GenerateUpdateJsonSerializerSettingsMethod = true,
    ExceptionClass = "ManagementApiException",
    WrapDtoExceptions = true,
    ConfigurationClass = "TokenManager",
    UseBaseUrl = false,
    OperationNameGenerator = new MultipleClientsFromFirstTagAndOperationIdGenerator(),
    CSharpGeneratorSettings =
    {
        Namespace = "TestingExample.ManagementApiClient",
        GenerateDataAnnotations = true,
        InlineNamedTuples = true,
        GenerateDefaultValues = true,
        JsonLibraryVersion = 10.0M,
        RequiredPropertiesMustBeDefined = true,
        GenerateNullableReferenceTypes = true,
        GenerateOptionalPropertiesAsNullable = false,
        GenerateNativeRecords = true,
        TimeType = "System.TimeOnly",
        DateType = "System.DateOnly",
        ClassStyle = CSharpClassStyle.Record,
        JsonLibrary = CSharpJsonLibrary.SystemTextJson,
        JsonPolymorphicSerializationStyle = CSharpJsonPolymorphicSerializationStyle.SystemTextJson
    }
};

var generator = new CSharpClientGenerator(schema, settings);

var file = generator.GenerateFile();
var path = Path.Join("D:\\src\\personal\\testingexample\\test\\TestingExample.ManagementApiClient", "ManagementApiClient.generated.cs");

await File.WriteAllTextAsync(path, file);
Console.WriteLine("Done!");