using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Our.Umbraco.BlockTypeGridViewPreview.Models
{
    [DataContract]
    public class BlockPreview
    {
        [DataMember(Name = "contentTypeAlias")]
        public string ContentTypeAlias { get; set; }

        [DataMember(Name = "propertyAlias")]
        public string PropertyAlias { get; set; }

        [DataMember(Name = "pageId")]
        public int PageId { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
