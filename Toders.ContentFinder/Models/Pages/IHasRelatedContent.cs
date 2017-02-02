using EPiServer.Core;

namespace Toders.ContentFinder.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
