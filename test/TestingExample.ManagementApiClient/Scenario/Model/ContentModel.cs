namespace TestingExample.ManagementApiClient.Scenario.Model;

public class ContentModel(VariationModel variation)
{
    private readonly VariationModel _variation = variation;
    public VariationType Variation => _variation.Variation;

    public void SetName(string name) => _variation.Name = name;

    public void Publish() => _variation.Publish();
}

public class ContentModel<T>(VariationModel variation, PageModel<T> page)
    : ContentModel(variation)
    where T : ContentModel<T>
{
    public PageModel<T> Page { get; } = page;
}