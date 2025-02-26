using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class ComputedDateField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string ToStringFormat { get; set; }
        public string SourceField { get; set; }
        public ComputedDateField(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
            this.ToStringFormat = XmlUtil.GetAttribute("toStringFormat", configNode);
        }
        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null)
                return (object)null;
            var itemField = item.Fields[SourceField];
            if (itemField == null) return null;
            var dateField = (DateField)itemField;
            var computedDate = string.Empty;
            if (dateField.DateTime != DateTime.MinValue)
            {
                computedDate = dateField.DateTime.ToString(ToStringFormat);
            }
            return computedDate;
        }
    }
}


----------------------------------------------------------------------
    using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class ComputedImage : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public ComputedImage(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
        }
        public object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            var indexableItem = indexable as SitecoreIndexableItem;
            if (indexableItem == null)
            {
                Log.Warn(string.Format("{0} : unsupported IIndexable type : {1}", this, indexable.GetType()), this);
                return null;
            }
            ImageField img = indexableItem.Item.Fields[SourceField];
            var mediaUrlOption = new MediaUrlBuilderOptions
            {
                AbsolutePath = false
            };
            return img == null || img.MediaItem == null ? null : MediaManager.GetMediaUrl(img.MediaItem, mediaUrlOption);
        }
    }
}
----------------------------------------------------------------------
    using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.Media.Imager;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Data.Items;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class ComputedItemUrl : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public object ComputeFieldValue(IIndexable indexable)
        {
            var scIndexable = indexable as SitecoreIndexableItem;
            if (scIndexable != null)
            {
                var item = (Item)scIndexable;
                if (item != null)
                {
                    using (new LanguageSwitcher(item.Language))
                    {
                        var opts = LinkManager.GetDefaultUrlOptions();
                        opts.Site = Sitecore.Sites.SiteContext.GetSite("sec-jss-app");
                        return LinkManager.GetItemUrl(item, opts);
                    }
                }
            }
            return null;
        }
    }
}
----------------------------------------------------------------------
    using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Diagnostics;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using ExtensionConstants = DCX.Foundation.Extension.Constant.ExtensionConstant;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class GeneralLinkUrlComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public GeneralLinkUrlComputedField(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
        }
        public object ComputeFieldValue(IIndexable indexable)
        {
            var sitecoreItem = indexable as SitecoreIndexableItem;
            if (sitecoreItem == null)
            {
                return null;
            }
            var item = sitecoreItem.Item;
            if (item == null)
            {
                return null;
            }
            if (item.TemplateID.ToString().Equals(ExtensionConstants.LEARNING_DETAIL_TEMPLATE_ID))
            {
                Sitecore.Data.Fields.LinkField linkField = item.Fields[SourceField];
                if (linkField != null)
                {
                    return linkField.GetFriendlyUrl();
                }
            }
            return null;
        }
    }
}

----------------------------------------------------------------------
    using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Diagnostics;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Globalization;
using DCX.Foundation.Extension.Extensions;
using ICSharpCode.SharpZipLib.Tar;
using DCX.Foundation.Extension.Constant;
using ExtensionConstants = DCX.Foundation.Extension.Constant.ExtensionConstant;
using GraphQL;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class MultilistComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public string TargetField { get; set; }
        public MultilistComputedField(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
            this.TargetField = XmlUtil.GetAttribute("targetField", configNode);
        }
        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null)
                return (object)null;
            Language language = item.Language;
            using (new LanguageSwitcher(language))
            {
                MultilistField multilistField = item?.Fields[SourceField];
                if (multilistField == null)
                {
                    return null;
                }
                if (item.TemplateID.ToString().Equals(ExtensionConstants.LEARNING_DETAIL_TEMPLATE_ID))
                {
                    {
                        var multilist = multilistField?.GetItems();
                        if (multilist == null || multilist.Length == 0)
                            return null;
                        return string.Join(",", multilist.Select(x => x.Fields[TargetField]));
                    }
                }
            }
            return string.Empty;
        }
    }
}

----------------------------------------------------------------------

    using DCX.Foundation.Extension.Constant;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.Media.Imager;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using ExtensionConstants = DCX.Foundation.Extension.Constant.ExtensionConstant;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class CustomComputedImage : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public CustomComputedImage(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
        }
        public object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            var indexableItem = indexable as SitecoreIndexableItem;
            if (indexableItem == null)
            {
                Log.Warn(string.Format("{0} : unsupported IIndexable type : {1}", this, indexable.GetType()), this);
                return null;
            }
            if (indexableItem.Item.TemplateID.ToString().Equals(ExtensionConstants.LEARNING_DETAIL_TEMPLATE_ID))
            {
                ImageField img = indexableItem.Item.Fields[SourceField];
                var mediaUrlOption = new MediaUrlBuilderOptions
                {
                    AlwaysIncludeServerUrl = false
                };
                var imageurl = (img?.MediaItem != null) ? MediaManager.GetMediaUrl(img.MediaItem, mediaUrlOption) : string.Empty;
                if (!string.IsNullOrWhiteSpace(imageurl)) { imageurl = imageurl.Replace("/sitecore/shell", ""); }
                return imageurl;
            }
            return string.Empty;
        }
    }
}

----------------------------------------------------------------------
    using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Xml;
using System.Xml;
namespace DCX.Feature.SiteSearch.Infrastructure.Fields
{
    public class ComputedReference : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }
        public string SourceField { get; set; }
        public string TargetField { get; set; }
        public ComputedReference(XmlNode configNode)
        {
            this.FieldName = XmlUtil.GetAttribute("fieldName", configNode);
            this.ReturnType = XmlUtil.GetAttribute("returnType", configNode);
            this.SourceField = XmlUtil.GetAttribute("sourceField", configNode);
            this.TargetField = XmlUtil.GetAttribute("targetField", configNode);
        }
        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null)
                return (object)null;
            ReferenceField itemReferenceField = item.Fields[SourceField];
            if (itemReferenceField == null)
            {
                return null;
            }
            if (itemReferenceField.TargetItem == null)
            {
                return null;
            }
            using (new LanguageSwitcher(item.Language))
            {
                Item referenceItem = itemReferenceField.TargetItem;
                var titleField = referenceItem.Fields[TargetField];
                if (titleField == null) return null;
                return titleField.Value.Trim();
            }
        }
    }
}


