using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.BlockTypeGridViewPreview.Models
{
    public class BlockPublishedPropertyType : IPublishedPropertyType
    {
        public BlockPublishedPropertyType(PublishedDataType dataType)
        {
            DataType = dataType;
        }

        public IPublishedContentType ContentType => throw new NotImplementedException();

        public PublishedDataType DataType { get; private set; }

        public string Alias => throw new NotImplementedException();

        public string EditorAlias => throw new NotImplementedException();

        public bool IsUserProperty => throw new NotImplementedException();

        public ContentVariation Variations => throw new NotImplementedException();

        public PropertyCacheLevel CacheLevel => throw new NotImplementedException();

        public Type ModelClrType => throw new NotImplementedException();

        public Type ClrType => throw new NotImplementedException();

        public object ConvertInterToObject(IPublishedElement owner, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            throw new NotImplementedException();
        }

        public object ConvertInterToXPath(IPublishedElement owner, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            throw new NotImplementedException();
        }

        public object ConvertSourceToInter(IPublishedElement owner, object source, bool preview)
        {
            throw new NotImplementedException();
        }

        public bool? IsValue(object value, PropertyValueLevel level)
        {
            throw new NotImplementedException();
        }
    }
}
