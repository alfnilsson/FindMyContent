using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toders.FindMyContent.Core;
using Toders.FindMyContent.Models.FindMyContent;

namespace Toders.FindMyContent.Controllers
{
    [Authorize(Roles = "CmsAdmins")]
    public class FindMyContentController : Controller
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentFinder _contentFinder;
        private readonly ProtectedModuleOptions _protectedModuleOptions;

        public FindMyContentController(
            IContentTypeRepository contentTypeRepository,
            IContentFinder contentFinder,
            ProtectedModuleOptions protectedModuleOptions)
        {
            _contentTypeRepository = contentTypeRepository;
            _contentFinder = contentFinder;
            _protectedModuleOptions = protectedModuleOptions;
        }

        [Route("FindMyContent")]
        [HttpGet]
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

            return View(model);
        }

        [Route("FindMyContent/Details/{id}")]
        [HttpGet]
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

            return View(model);
        }

        private Dictionary<string, string> CreateEditContentUrls(ContentSummary contentSummary)
        {
            return contentSummary.Translations.ToDictionary(
                x => x.Key,
                x => PageEditing.GetEditUrlForLanguage(contentSummary.ContentLink, x.Key));
        }

        private string CreateEditContentTypeUrl(ContentTypeModel contentTypeModel)
        {
            var rootPath = _protectedModuleOptions.RootPath.Replace("~", string.Empty);
            return Path.Join(rootPath, $"/EPiServer.Cms.UI.Admin/default#/ContentTypes/edit-content-type/{contentTypeModel.Id}");
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