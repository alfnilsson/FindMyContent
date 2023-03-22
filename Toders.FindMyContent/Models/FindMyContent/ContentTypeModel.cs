using System.Collections.Generic;
using Toders.FindMyContent.Core;

namespace Toders.FindMyContent.Models.FindMyContent
{
    public class ContentTypeModel
    {
        public enum ContentTypeCategory
        {
            Unknown,
            Page,
            Block,
            Media,
            Folder
        }

        public string Name { get; set; }

        public int AmountOfContent { get; set; }

        public int Id { get; set; }

        public ContentTypeCategory Category { get; set; }
    }

    public class ContentSummaryModel
    {
        public ContentSummary Summary { get; set; }

        public Dictionary<string, string> EditUrls { get; set; }
    }
}