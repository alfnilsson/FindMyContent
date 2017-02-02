using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Toders.FindMyContent.Core
{
    [ServiceConfiguration(typeof(IContentFinder))]
    public class ContentFinderFinder : IContentFinder
    {
        private readonly IContentLoader _contentLoader;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentModelUsage _contentModelUsage;

        public ContentFinderFinder(
            IContentLoader contentLoader,
            IContentTypeRepository contentTypeRepository,
            IContentModelUsage contentModelUsage)
        {
            _contentLoader = contentLoader;
            _contentTypeRepository = contentTypeRepository;
            _contentModelUsage = contentModelUsage;
        }

        public int Count(int contentTypeId)
        {
            return List(contentTypeId).Count();
        }

        public IEnumerable<ContentSummary> List(int contentTypeId)
        {
            var allContent = new List<ContentSummary>();

            var contentType = _contentTypeRepository.Load(contentTypeId);
            if (contentType == null)
            {
                return Enumerable.Empty<ContentSummary>();
            }

            IList<ContentUsage> contentUsages = _contentModelUsage.ListContentOfContentType(contentType);

            IEnumerable<ContentSummary> collection = contentUsages
                .Distinct(new Comparer())
                .Select(contentUsage =>
                {
                    ContentReference contentLink = contentUsage.ContentLink.ToReferenceWithoutVersion();

                    string masterLanguage;
                    var translations = GetTranslations(contentLink, out masterLanguage);
                    return new ContentSummary
                    {
                        ContentLink = contentLink,
                        Translations = translations,
                        MasterLanguage = masterLanguage
                    };
                });

            allContent.AddRange(collection);

            return allContent.OrderBy(item => item.ContentLink.ID).ToList();
        }

        private Dictionary<string, string> GetTranslations(ContentReference contentLink, out string masterLanguageName)
        {
            var translations = new Dictionary<string, string>();

            var content = _contentLoader.Get<IContent>(contentLink, LanguageSelector.MasterLanguage());

            CultureInfo masterLanguage = null;
            var localizable = content as ILocalizable;
            if (localizable != null)
            {
                masterLanguage = localizable.MasterLanguage;
                IEnumerable<CultureInfo> existingLanguages = localizable.ExistingLanguages;
                foreach (CultureInfo language in existingLanguages.Except(new[] { masterLanguage }))
                {
                    var translation = _contentLoader.Get<IContent>(contentLink, language);
                    translations.Add(language.Name, translation.Name);
                }
            }
            masterLanguageName = masterLanguage != null
                ? masterLanguage.Name
                : "Unknown";
            translations.Add(masterLanguageName, content.Name);

            return translations;
        }

        private class Comparer : IEqualityComparer<ContentUsage>
        {
            public bool Equals(ContentUsage x, ContentUsage y)
            {
                var value = x.ContentLink;
                if (value == null)
                {
                    return false;
                }

                var secondValue = y.ContentLink;
                if (secondValue == null)
                {
                    return false;
                }

                return value.CompareToIgnoreWorkID(secondValue);
            }

            public int GetHashCode(ContentUsage obj)
            {
                return obj.ContentLink.ToReferenceWithoutVersion().GetHashCode();
            }
        }
    }
}