using System.Diagnostics;

using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.ManagementApiClient.Scenario;

public static class MapPageToRequestBody
{
    public static CreateDocumentRequestModel MapToCreateDocumentRequest(this PageModel page)
        => new(
        page.ContentType.AsReference(),
        page.Id,
        page.Parent?.Id.AsReference(),
        page.Template?.AsReference(),
        [.. page.Values.Select(value => value.MapToValueRequest())],
        [.. page.Variations.Select(variation => variation.MapToVariantRequest())]);

    public static PublishDocumentRequestModel MapToPublishDocumentRequest(this PageModel page)
        => new(
            [.. page.Variations
                .Where(variation => variation.Published)
                .Select(variation => new CultureAndScheduleRequestModel(variation.Variation.Culture.ToCultureRequest(), null))]
        );

    public static UpdateDomainsRequestModel MapToUpdateDomainsRequest(this PageModel page)
        => new(
            null, [.. page.Domains.Select(domain => new DomainPresentationModel(domain.Domain, domain.Culture.Name))]);

    private static DocumentValueModel MapToValueRequest(this ValueModel value)
        => new(value.Alias, value.Variation.Culture.ToCultureRequest(), value.Variation.Segment.ToSegmentRequest(), value.Value);

    private static DocumentVariantRequestModel MapToVariantRequest(this VariationModel variant)
        => new(variant.Variation.Culture.ToCultureRequest(), variant.Name, variant.Variation.Segment.ToSegmentRequest());

    private static ReferenceByIdModel AsReference(this Guid guid)
        => new(guid);

    private static string? ToCultureRequest(this LocaleType locale)
        => locale switch
        {
            CultureLocale culture => culture.Culture.Name,
            InvariantLocale => null,
            _ => throw new UnreachableException("Unknown type for locale")
        };

    private static string? ToSegmentRequest(this SegmentType segment)
        => segment switch
        {
            VariantSegment variant => variant.Alias,
            InvariantSegment => null,
            _ => throw new UnreachableException("Unknown type for segment")
        };
}
