namespace TestingExample.ManagementApiClient.Scenario.Model;

public class VariationModel
{
    private string _name;

    public VariationModel(VariationType variation, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Variation = variation;
        _name = name;
    }

    public VariationType Variation { get; }
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }
    public bool Published { get; private set; }

    public void Publish() => Published = true;
}