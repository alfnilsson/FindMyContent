using System.Collections.Generic;

namespace Toders.FindMyContent.Core
{
    public interface IContentFinder
    {
        int Count(int contentTypeId);
        IEnumerable<ContentSummary> List(int contentTypeId);
    }
}