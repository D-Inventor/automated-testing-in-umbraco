namespace Umbraco.Cms.Web.Common.PublishedModels;

public partial class ContentPage
{
    public string? GetSEOTitle()
        => !string.IsNullOrWhiteSpace(SeoTitle) ? SeoTitle
        : Name;
}
