using Newtonsoft.Json;
using Our.Umbraco.BlockTypeGridViewPreview.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.Blocks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.BlockTypeGridViewPreview.Controllers
{
    [PluginController("BlockTypeGridViewPreview")]
    public class PreviewController : UmbracoAuthorizedApiController
    {
        private readonly IProfilingLogger _profilingLogger;
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public PreviewController() { }

        public PreviewController(
            IProfilingLogger profilingLogger,
            IPublishedContentTypeFactory publishedContentTypeFactory,
            IPublishedModelFactory publishedModelFactory,
            IPublishedSnapshotAccessor publishedSnapshotAccessor)
        {
            _profilingLogger = profilingLogger;
            _publishedContentTypeFactory = publishedContentTypeFactory;
            _publishedModelFactory = publishedModelFactory;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
        }

        [HttpPost]
        public HttpResponseMessage GetBlockPreviewMarkup([FromBody] BlockPreview data)
        {
            var contentType = Services.ContentTypeService.Get(data.ContentTypeAlias);
            var publishedContentType = new Lazy<IPublishedContentType>(() => _publishedContentTypeFactory.CreateContentType(contentType)).Value;
            var propertyType = publishedContentType.PropertyTypes.FirstOrDefault(x => x.Alias == data.PropertyAlias);

            var editor = new BlockListPropertyValueConverter(
                _profilingLogger,
                new BlockEditorConverter(_publishedSnapshotAccessor, _publishedModelFactory));

            var page = default(IPublishedContent);

            // If the page is new, then the ID will be zero
            if (data.PageId > 0)
            {
                // Get page container node
                page = Umbraco.Content(data.PageId);
                if (page == null)
                {
                    // If unpublished, then fake PublishedContent
                    page = new UnpublishedContent(data.PageId, Services, _publishedContentTypeFactory, Current.PropertyEditors);
                }
            }

            var converted = editor.ConvertIntermediateToObject(page, propertyType, PropertyCacheLevel.None, data.Value, false) as BlockListModel;
            var model = converted[0];
            // Render view
            var partialName = string.Format(ConfigurationManager.AppSettings["BlockTypeGridViewPreviewPath"] ?? "~/Views/Partials/BlockList/Components/{0}.cshtml", model.Content.ContentType.Alias);
            var markup = Helpers.ViewHelper.RenderPartial(partialName, model, UmbracoContext.HttpContext, UmbracoContext);

            // Return response
            var response = new HttpResponseMessage
            {
                Content = new StringContent(markup ?? string.Empty)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Html);

            return response;
        }
    }
}
