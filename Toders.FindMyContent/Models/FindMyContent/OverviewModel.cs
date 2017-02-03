using System.Collections.Generic;

namespace Toders.FindMyContent.Models.FindMyContent
{
    public class OverviewModel
    {
        public IEnumerable<ContentTypeModel> ContentTypes { get; set; }

        public Dictionary<int, string> EditUrls { get; set; }  
    }
}