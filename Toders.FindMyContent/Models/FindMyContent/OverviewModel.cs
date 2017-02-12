using System.Collections.Generic;

namespace Toders.FindMyContent.Models.FindMyContent
{
    public class OverviewModel
    {
        public IEnumerable<ContentTypeModel> PageTypes { get; set; }

        public IEnumerable<ContentTypeModel> BlockTypes { get; set; }

        public IEnumerable<ContentTypeModel> OtherTypes { get; set; }

        public IEnumerable<ContentTypeModel> MediaTypes { get; set; }

        public Dictionary<int, string> EditUrls { get; set; }
    }
}