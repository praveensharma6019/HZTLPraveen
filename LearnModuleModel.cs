using Sitecore.Collections;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using static SitecoreSolrIndexing.Models.ArticleSearchModel;
using static SitecoreSolrIndexing.Models.LearnModuleModel;
namespace SitecoreSolrIndexing.Models
{
    public class LearnModuleModel : SearchResultItem
    {
        [IndexField("title_t")]
        public virtual string Title { get; set; }
        [IndexField("customimagemobile_t")]
        public virtual string ImageMobile { get; set; }
        [IndexField("customimage_t")]
        public virtual string Image { get; set; }
        [IndexField("customimagetab_t")]
        public virtual string ImageTab { get; set; }
        [IndexField("customdate_t")]
        public virtual string Date { get; set; }
        [IndexField("datetime_tdt")]
        public virtual DateTime OriginalDate { get; set; }
        [IndexField("customvideolink_t")]
        public virtual string Video { get; set; }
        [IndexField("customtopicslist_t")]
        public virtual string Topics { get; set; }
        [IndexField("customcontenttypelist_t")]
        public virtual string ContentType { get; set; }
        [IndexField("description_t")]
        public virtual string Description { get; set; }
        [IndexField("customitemurl_t")]
        public virtual string PageUrl { get; set; }
        [IndexField("shortdescription_t")]
        public virtual string ShortDescription { get; set; }
        public class LearnModuleResult
        {
            public string Title { get; set; }
            public string Image { get; set; }
            public string ImageTab { get; set; }
            //public string ImageMobile { get; set; }
            public string Date { get; set; }
            public string Video { get; set; }
            public DescriptionObject Description { get; set; }
            public string ItemId { get; set; }
            public string PageUrl { get; set; }
            public List<string> Topics { get; set; }
            public List<string> ContentType { get; set; }
            public ImageModel ImageMobile { get; set; }
            public string ShortDescription { get; set; }
        }
        public class ImageModel
        {
            public string Src { get; set; }
        }
        public class LearnModuleResults
        {
            public int noOfResults { get; set; }
            public int totalCount { get; set; }
            public List<LearnModuleResult> SearchData { get; set; }
            public List<ILandingComponent> data { get; set; } = new List<ILandingComponent>();
            public string Errormsg { get; set; }
            public bool IsSuccess { get; set; }
        }
        public interface ILandingComponent
        {
            string ComponentName { get; set; }
        }
        public interface ILandingContent : ILandingComponent
        {
            string Heading { get; set; }
            DescriptionObject Description { get; set; }
        }
        public class LandingContent : ILandingContent
        {
            public string Heading { get; set; }
            public DescriptionObject Description { get; set; }
            public string ComponentName { get; set; }
        }
        public interface ILandingSearch : ILandingComponent
        {
            string Search { get; set; }
            string FilterContent { get; set; }
            string Reset { get; set; }
            string ContentTypeText { get; set; }
            string TopicsText { get; set; }
            string ApplyFilter { get; set; }
            string Filter { get; set; }
            string SortbyText { get; set; }
            string LoadMore { get; set; }
            string RefineText { get; set; }
            string ClearAllText { get; set; }
            string Results { get; set; }
            string ResultNotFound { get; set; }
            int PageSizeApp { get; set; }
            int PageSizeWeb { get; set; }
            List<ListFormatKeyValuePair> ContentType { get; set; }
            List<ListFormatKeyValuePair> Topics { get; set; }
        }
        public class LandingSearch : ILandingSearch
        {
            public string ComponentName { get; set; }
            public string Search { get; set; }
            public string FilterContent { get; set; }
            public string Reset { get; set; }
            public string ContentTypeText { get; set; }
            public string TopicsText { get; set; }
            public string ApplyFilter { get; set; }
            public string Filter { get; set; }
            public string SortbyText { get; set; }
            public string LoadMore { get; set; }
            public string RefineText { get; set; }
            public string ClearAllText { get; set; }
            public string Results { get; set; }
            public string ResultNotFound { get; set; }
            public int PageSizeApp { get; set; }
            public int PageSizeWeb { get; set; }
            public List<ListFormatKeyValuePair> SortBy { get; set; }
            public List<ListFormatKeyValuePair> ContentType { get; set; }
            public List<ListFormatKeyValuePair> Topics { get; set; }
        }
        public class ListFormatKeyValuePair
        {
            public string Value { get; set; }
            public string Key { get; set; }
        }
        public interface ILandingPromo : ILandingComponent
        {
            string Title { get; set; }
            DescriptionObject Description { get; set; }
            string ShortDescription { get; set; }
            bool IsChecked { get; set; }
            string Link { get; set; }
            string ItemId { get; set; }
            LandingImage ImageMobile { get; set; }
            LandingImage ImageTab { get; set; }
            LandingImage Image { get; set; }
            string Video { get; set; }
        }
        public class LandingPromo : ILandingPromo
        {
            public string ComponentName { get; set; }
            public string Title { get; set; }
            public DescriptionObject Description { get; set; }
            public string ShortDescription { get; set; }
            public bool IsChecked { get; set; }
            public string Link { get; set; }
            public string ItemId { get; set; }
            public LandingImage ImageMobile { get; set; }
            public LandingImage ImageTab { get; set; }
            public LandingImage Image { get; set; }
            public string Video { get; set; }
        }
        public interface ISuggestedArticlesDetail : ILandingComponent
        {
            string Heading { get; set; }
            List<LearningSuggestedArticalDetail> RelatedContent { get; set; }
        }
        public class SuggestedArticlesDetail : ISuggestedArticlesDetail
        {
            public string ComponentName { get; set; }
            public string Heading { get; set; }
            public List<LearningSuggestedArticalDetail> RelatedContent { get; set; }
        }
        public class LearningSuggestedArticalDetail
        {
            public string Heading { get; set; }
            public DescriptionObject Description { get; set; }
            public string ShortDescription { get; set; }
            public bool IsChecked { get; set; }
            public string Date { get; set; }
            public string Link { get; set; }
            public string ItemId { get; set; }
            public LandingImage ImageMobile { get; set; }
            public LandingImage ImageTab { get; set; }
            public LandingImage Image { get; set; }
            public string Video { get; set; }
        }
        public interface ILearningNavigationDetail : ILandingComponent
        {
        }
        public class LearningNavigationDetail : ILearningNavigationDetail
        {
            public string ComponentName { get; set; }
        }
        public interface ISearchSettings : ILandingComponent
        {
            string SpecialCharactersRegExMobile { get; set; }
            string SpecialCharactersRegExWeb { get; set; }
            string SpecialLetters { get; set; }
        }
        public class SearchSettings : ISearchSettings
        {
            public string ComponentName { get; set; }
            public string SpecialCharactersRegExMobile { get; set; }
            public string SpecialCharactersRegExWeb { get; set; }
            public string SpecialLetters { get; set; }
        }
        public class LandingImage
        {
            public string Src { get; set; }
            public string Alt { get; set; }
            public string Width { get; set; }
            public string Height { get; set; }
        }
        public class DescriptionObject
        {
            public string value { get; set; }
            public bool IsHTML { get; set; }
        }
    }
}