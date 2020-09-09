using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Our.Umbraco.BlockTypeGridViewPreview.Models
{

    internal class UnpublishedContent : IPublishedContent
    {
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;
        private readonly PropertyEditorCollection _propertyEditors;
        private readonly IContent content;

        private readonly Lazy<IEnumerable<IPublishedContent>> children;
        private readonly Lazy<IPublishedContentType> contentType;
        private readonly Lazy<string> creatorName;
        private readonly Lazy<IPublishedContent> parent;
        private readonly Lazy<Dictionary<string, IPublishedProperty>> properties;
        private readonly Lazy<string> urlName;
        private readonly Lazy<string> writerName;

        public UnpublishedContent(int id, ServiceContext serviceContext, IPublishedContentTypeFactory publishedContentTypeFactory, PropertyEditorCollection propertyEditors)
            : this(serviceContext.ContentService.GetById(id), serviceContext, publishedContentTypeFactory, propertyEditors)
        {
        }

        public UnpublishedContent(IContent content, ServiceContext serviceContext, IPublishedContentTypeFactory publishedContentTypeFactory, PropertyEditorCollection propertyEditors)
            : base()
        {
            var userService = new Lazy<IUserService>(() => serviceContext.UserService);

            _publishedContentTypeFactory = publishedContentTypeFactory;
            _propertyEditors = propertyEditors;

            this.content = content;
            var contentType = serviceContext.ContentTypeService.Get(this.content.ContentType.Id);

            //this.children = new Lazy<IEnumerable<IPublishedContent>>(() => this.content.Children().Select(x => new UnpublishedContent(x, serviceContext)).ToList());
            this.contentType = new Lazy<IPublishedContentType>(() => _publishedContentTypeFactory.CreateContentType(contentType));
            this.creatorName = new Lazy<string>(() => this.content.GetCreatorProfile(userService.Value).Name);
            this.parent = new Lazy<IPublishedContent>(() => new UnpublishedContent(serviceContext.ContentService.GetById(this.content.ParentId), serviceContext, _publishedContentTypeFactory, _propertyEditors));
            this.properties = new Lazy<Dictionary<string, IPublishedProperty>>(() => MapProperties(serviceContext));
            this.urlName = new Lazy<string>(() => this.content.Name.ToUrlSegment());
            this.writerName = new Lazy<string>(() => this.content.GetWriterProfile(userService.Value).Name);
        }

        public Guid Key => this.content.Key;

        IEnumerable<IPublishedProperty> IPublishedElement.Properties => Properties;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; }

        public PublishedItemType ItemType => PublishedItemType.Content;

        public string GetUrl(string culture = null)
        {
            return null;
        }

        public PublishedCultureInfo GetCulture(string culture = null)
        {
            return null;
        }

        bool IPublishedContent.IsDraft(string culture)
        {
            return true;
        }

        public bool IsPublished(string culture = null)
        {
            return false;
        }

        public int Id => this.content.Id;

        public string UrlSegment { get; }
        public int SortOrder => this.content.SortOrder;

        public string Name => this.content.Name;

        public string UrlName => this.urlName.Value;

        public string DocumentTypeAlias => this.content.ContentType?.Alias;

        public int DocumentTypeId => this.content.ContentType?.Id ?? default(int);

        public string WriterName => this.writerName.Value;

        public string CreatorName => this.creatorName.Value;

        public int WriterId => this.content.WriterId;

        int? IPublishedContent.TemplateId => this.content.TemplateId;

        public int CreatorId => this.content.CreatorId;

        public string Path => this.content.Path;

        public DateTime CreateDate => this.content.CreateDate;

        public DateTime UpdateDate => this.content.UpdateDate;
        public string Url { get; }


        public int Level => this.content.Level;

        public bool IsDraft => true;

        public IPublishedContent Parent => this.parent.Value;

        public IEnumerable<IPublishedContent> Children => this.children.Value;

        public IPublishedContentType ContentType => this.contentType.Value;

        public ICollection<IPublishedProperty> Properties => this.properties.Value.Values;

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => this.children.Value;

        public IPublishedProperty GetProperty(string alias)
        {
            return this.properties.Value.TryGetValue(alias, out IPublishedProperty property) ? property : null;
        }

        private Dictionary<string, IPublishedProperty> MapProperties(ServiceContext services)
        {
            var contentType = this.contentType.Value;
            var properties = this.content.Properties;

            var items = new Dictionary<string, IPublishedProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var propertyType in contentType.PropertyTypes)
            {
                var property = properties.FirstOrDefault(x => x.Alias.InvariantEquals(propertyType.DataType.EditorAlias));
                var value = property?.GetValue();
                if (value != null)
                {
                    _propertyEditors.TryGet(propertyType.DataType.EditorAlias, out var editor);
                    if (editor != null)
                    {
                        value = editor.GetValueEditor().ConvertDbToString(property.PropertyType, value, services.DataTypeService);
                    }
                }

                items.Add(propertyType.DataType.EditorAlias, new UnpublishedProperty(propertyType, value));
            }

            return items;
        }
    }
}
