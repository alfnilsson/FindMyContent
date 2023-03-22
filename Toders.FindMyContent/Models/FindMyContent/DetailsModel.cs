using System.Collections.Generic;

namespace Toders.FindMyContent.Models.FindMyContent
{
    public class DetailsModel
    {
        public ContentTypeModel ContentType { get; set; }
        public IEnumerable<ContentSummaryModel> Content { get; set; }
    }
}