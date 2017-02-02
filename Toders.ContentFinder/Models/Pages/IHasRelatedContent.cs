using EPiServer.Core;

namespace Toders.FindMyContent.Web.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
