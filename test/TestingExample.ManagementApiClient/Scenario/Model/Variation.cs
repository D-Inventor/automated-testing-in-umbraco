using System.Globalization;

namespace TestingExample.ManagementApiClient.Scenario.Model;

public record VariationType(LocaleType Culture, SegmentType Segment);

public static class Variation
{
    public static VariationType Invariant { get; } = new(Locale.Invariant, new InvariantSegment());
    public static VariationType Culture(CultureInfo culture) => new(Locale.Culture(culture), new InvariantSegment());
    public static VariationType Culture(string culture) => Culture(CultureInfo.GetCultureInfo(culture));
}

public record LocaleType();

public record InvariantLocale() : LocaleType;
public record CultureLocale(CultureInfo Culture) : LocaleType;

public static class Locale
{
    public static LocaleType Invariant { get; } = new InvariantLocale();
    public static LocaleType Culture(CultureInfo culture) => new CultureLocale(culture);
    public static LocaleType Culture(string culture) => Culture(CultureInfo.GetCultureInfo(culture));
}


public record SegmentType();

public record InvariantSegment() : SegmentType;
public record VariantSegment(string Alias) : SegmentType;