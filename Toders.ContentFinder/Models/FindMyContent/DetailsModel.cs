using System.Collections.Generic;
using Toders.FindMyContent.Core;

namespace Toders.FindMyContent.Models.FindMyContent
{
    public class DetailsModel
    {
        public ContentTypeModel ContentType { get; set; }
        public IEnumerable<ContentSummaryModel> Content { get; set; }
    }
}