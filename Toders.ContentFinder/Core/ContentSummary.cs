using System.Collections.Generic;
using EPiServer.Core;

namespace Toders.FindMyContent.Core
{
    public class ContentSummary
    {
        public ContentReference ContentLink { get; set; }

        public string MasterLanguage { get; set; }

        public Dictionary<string, string> Translations { get; set; }
    }
}