using System.Diagnostics;

namespace TestingExample.ManagementApiClient.Scenario.Model;

public abstract class PageModel(Guid contentType, Guid? id = null)
{
    public Guid Id { get; } = id ?? Guid.NewGuid();
    public Guid ContentType { get; } = contentType;
    public Guid? Template { get; init; }

    public PageModel? Parent { get; set; }

    private readonly Dictionary<LocaleType, UrlModel> _urls = [];
    private readonly List<DomainModel> _domains = [];
    private readonly List<ValueModel> _values = [];
    private readonly Dictionary<VariationType, VariationModel> _variations = [];

    public int Level => Parent is not null ? Parent.Level + 1
        : 0;

    internal void SetUrl(UrlModel url)
    {
        _urls[url.Locale] = url;
    }

    public Uri Url() => Url(Locale.Invariant);

    public Uri Url(LocaleType locale)
        => _urls.TryGetValue(locale, out var result) ? result.Url
        : throw new InvalidOperationException("This content item does not have a URL for the given culture");

    public void AddDomain(DomainModel domain)
        => _domains.Add(domain);

    public void AddVariation(VariationModel variation)
        => _variations[variation.Variation] = variation;

    protected VariationModel? GetVariation(VariationType variation)
        => _variations.TryGetValue(variation, out var result) ? result
        : null;

    internal void SetValue(ValueModel value)
    {
        _values.RemoveAll(item => item.Variation == value.Variation && item.Alias == value.Alias);
        _values.Add(value);
    }

    public IEnumerable<ValueModel> Values => [.. _values];
    public IEnumerable<DomainModel> Domains => [.. _domains];
    public IEnumerable<VariationModel> Variations => [.. _variations.Values];
}

public sealed class PageModel<T>(Guid contentType, Guid? id = null)
    : PageModel(contentType, id)
    where T : ContentModel<T>
{
    /// <summary>
    /// Add a content variation to this page.
    /// </summary>
    /// <param name="variation">The culture and segment of this content variation</param>
    /// <param name="name">The name as it is displayed in the backoffice</param>
    /// <param name="configure">(Optional) Set content values for this variation</param>
    /// <returns>This page after the variation has been added and configured</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PageModel<T> HasVariation(VariationType variation, string name, Action<T> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        AddVariation(new VariationModel(variation, name));
        HasContent(variation, configure);

        return this;
    }

    /// <summary>
    /// Configure content values for a given variation.
    /// </summary>
    /// <param name="variation">The variant to configure</param>
    /// <param name="configure">A configuration to execute</param>
    /// <returns>This page after the variation has been configured</returns>
    /// <remarks>
    /// <para><see cref="HasVariation"/> must be called before this method for the variant that you wish to configure</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException"></exception>
    public PageModel<T> HasContent(VariationType variation, Action<T> configure)
    {
        if (GetVariation(variation) is not VariationModel variationModel) throw new InvalidOperationException("You must call 'HasVariation()' for the given variation before using 'HasContent()'");
        
        var content = Activator.CreateInstance(typeof(T), variationModel, this) as T ?? throw new UnreachableException();
        configure(content);

        return this;
    }
}