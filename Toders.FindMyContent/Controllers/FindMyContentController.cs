using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using Toders.FindMyContent.Core;
using Toders.FindMyContent.Models.FindMyContent;

namespace Toders.FindMyContent.Controllers
{
    [GuiPlugIn(
        Area = PlugInArea.AdminMenu,
        Url = "/FindMyContent/",
        DisplayName = "Find My Content")]
    public class FindMyContentController : Controller
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentFinder _contentFinder;

        public FindMyContentController()
        : this(ServiceLocator.Current.GetInstance<IContentTypeRepository>(),
            ServiceLocator.Current.GetInstance<IContentFinder>())
        {
        }

        public FindMyContentController(
            IContentTypeRepository contentTypeRepository,
            IContentFinder contentFinder)
        {
            _contentTypeRepository = contentTypeRepository;
            _contentFinder = contentFinder;
        }

        public ActionResult Index()
        {
            var contentTypes = _contentTypeRepository.List()
                .Select(CreateContentTypeModel)
                .OrderBy(contentType => contentType.Name)
                .ToList();
            var model = new OverviewModel
            {
                PageTypes = contentTypes
                    .Where(x => x.Category == ContentTypeModel.ContentTypeCategory.Page)
                    .ToList(),
                BlockTypes = contentTypes
                    .Where(x => x.Category == ContentTypeModel.ContentTypeCategory.Block)
                    .ToList(),
                MediaTypes = contentTypes
                    .Where(x => x.Category == ContentTypeModel.ContentTypeCategory.Media)
                    .ToList(),
                OtherTypes = contentTypes
                    .Where(x =>
                        new[]
                        {
                            ContentTypeModel.ContentTypeCategory.Page,
                            ContentTypeModel.ContentTypeCategory.Block,
                            ContentTypeModel.ContentTypeCategory.Media
                        }.Contains(x.Category) == false)
                    .ToList(),
                EditUrls = contentTypes.ToDictionary(x => x.Id, CreateEditContentTypeUrl)
            };

            return View(string.Empty, model);
        }

        public ActionResult Details(int id)
        {
            ContentType contentType = _contentTypeRepository.Load(id);
            var model = new DetailsModel
            {
                ContentType = CreateContentTypeModel(contentType),
                Content = _contentFinder.List(id).Select(x => new ContentSummaryModel
                {
                    Summary = x,
                    EditUrls = CreateEditContentUrls(x)
                }).ToList()
            };

            return View(string.Empty, model);
        }

        private Dictionary<string, string> CreateEditContentUrls(ContentSummary contentSummary)
        {
            return contentSummary.Translations.ToDictionary(
                x => x.Key,
                x => PageEditing.GetEditUrlForLanguage(contentSummary.ContentLink, x.Key));
        }

        private string CreateEditContentTypeUrl(ContentTypeModel contentTypeModel)
        {
            var urlBuilder = new UrlBuilder(UriSupport.AbsoluteUrlFromUIBySettings("Admin/EditContentType.aspx"));
            urlBuilder.QueryCollection["typeId"] = contentTypeModel.Id.ToString();
            return urlBuilder.ToString();
        }

        private ContentTypeModel CreateContentTypeModel(ContentType contentType)
        {
            return new ContentTypeModel
            {
                Name = string.Format("{0} ({1})", contentType.FullName, contentType.Name),
                AmountOfContent = _contentFinder.Count(contentType.ID),
                Id = contentType.ID,
                Category = GetCategory(contentType.ModelType)
            };
        }

        private ContentTypeModel.ContentTypeCategory GetCategory(Type modelType)
        {
            if (modelType == null)
            {
                return ContentTypeModel.ContentTypeCategory.Unknown;
            }
            var mappers = new Dictionary<Type, ContentTypeModel.ContentTypeCategory>
            {
                { typeof(PageData), ContentTypeModel.ContentTypeCategory.Page },
                { typeof(BlockData), ContentTypeModel.ContentTypeCategory.Block },
                { typeof(MediaData), ContentTypeModel.ContentTypeCategory.Media },
                { typeof(ContentFolder), ContentTypeModel.ContentTypeCategory.Folder },
            };

            foreach (var mapper in mappers)
            {
                if (mapper.Key.IsAssignableFrom(modelType))
                {
                    return mapper.Value;
                }
            }

            return ContentTypeModel.ContentTypeCategory.Unknown;
        }
    }
}