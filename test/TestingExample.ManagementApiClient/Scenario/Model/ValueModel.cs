using System.Diagnostics.CodeAnalysis;

namespace TestingExample.ManagementApiClient.Scenario.Model;

public record ValueModel(VariationType Variation, string Alias, object? Value);

public class ValueModelVariationAndAliasEqualityComparer : IEqualityComparer<ValueModel>
{
    public static ValueModelVariationAndAliasEqualityComparer Instance { get; } = new();

    public bool Equals(ValueModel? x, ValueModel? y)
        => x is not null
        && y is not null
        && x.Variation == y.Variation
        && x.Alias == y.Alias;

    public int GetHashCode([DisallowNull] ValueModel obj)
        => HashCode.Combine(obj.Variation, obj.Alias);
}