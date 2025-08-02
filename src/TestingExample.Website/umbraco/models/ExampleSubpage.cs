
namespace Umbraco.Cms.Web.Common.PublishedModels;

public partial class ExampleSubpage
{
	public string GetSEOTitle()
	{
		return !string.IsNullOrWhiteSpace(SeoTitle) ? SeoTitle : Name;
	}
}