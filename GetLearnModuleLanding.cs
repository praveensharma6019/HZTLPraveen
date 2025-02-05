namespace Sitecore.Feature.SiteSearch.Services
{

    using Sitecore.Feature.SiteSearch.Models;
    using System;
    using System.Collections.Generic;
    using static SitecoreSolrIndexing.Models.LearnModuleModel;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.ServiceProcess;
    using DCX.Foundation.Extension.Extensions;
    using DCX.Foundation.EServices.ServiceClient.Constant;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Data.Items;
    using Sitecore.Data;
    public class LearningModule
    {
        public async Task<List<ILandingComponent>> GetLearnModuleLanding(string layoutServiceUrl, LearnModuleResults components, string deviceType)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(layoutServiceUrl);
                var json = await response.Content.ReadAsStringAsync();
                LearningModuleLandingResponse responsejson = new LearningModuleLandingResponse();
                if (response.IsSuccessStatusCode)
                {
                    responsejson = JsonConvert.DeserializeObject<LearningModuleLandingResponse>(json);
                    components.data = LandingTransformJson(responsejson, components, deviceType);
                    if (components != null)
                        components.IsSuccess = true;
                }
                else
                {
                    components.Errormsg = "Error producing jss response";
                    components.IsSuccess = false;
                }
                return components.data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public List<ILandingComponent> LandingTransformJson(LearningModuleLandingResponse responsejson, LearnModuleResults customJson, string deviceType)
        {
            Database webDb = Sitecore.Configuration.Factory.GetDatabase(ServiceConstant.SITECORE_WEB_DATABASE);
            dynamic learningContent;
            customJson.Errormsg = responsejson?.Errormsg;
            StringToDateConverter stringToDateConverter = new StringToDateConverter();
            LayoutService layoutservice = new LayoutService();
            string rootUrl = layoutservice.getRootURL();
            if (responsejson?.Sitecore?.Route?.Placeholders?.JssMain != null && responsejson?.Sitecore?.Route?.Placeholders?.JssMain?.Length > 0)
            {
                if (responsejson != null)
                {
                    for (int i = 0; i < responsejson?.Sitecore?.Route?.Placeholders?.JssMain?.Length; i++)
                    {
                        string componentName = responsejson?.Sitecore?.Route?.Placeholders?.JssMain[i]?.ComponentName;
                        switch (componentName)
                        {
                            case ServiceConstant.LANDING_HEADING_COMPONENT:
                                learningContent = responsejson?.Sitecore?.Route?.Placeholders?.JssMain[i]?.Fields?.Data?.LandingContent;
                                break;
                            case ServiceConstant.LANDING_SEARCH_COMPONENT:
                                learningContent = responsejson?.Sitecore?.Route?.Placeholders?.JssMain[i]?.Fields?.Data?.LandingSearchDetail;
                                break;
                            case ServiceConstant.LANDING_PROMO_COMPONENT:
                                learningContent = responsejson?.Sitecore?.Route?.Placeholders?.JssMain[i]?.Fields?.Data?.LadingPromo;
                                break;
                            case ServiceConstant.LANDING_SUGGESTED_ARTICLES_COMPONENT:
                                learningContent = responsejson?.Sitecore?.Route?.Placeholders?.JssMain[i]?.Fields?.Data?.SuggestedArticlesDetail;
                                break;
                            case ServiceConstant.LANDING_NAVIGATION_COMPONENT:
                                learningContent = responsejson?.Sitecore?.Route?.Placeholders?.JssMain[i]?.Fields?.Data?.LearningNavigationDetail;
                                break;
                            default:
                                learningContent = null;
                                break;
                        }
                        if (componentName == ServiceConstant.LANDING_HEADING_COMPONENT && !deviceType.ToLowerInvariant().Equals(ServiceConstant.SITECORE_LAYOUTSERVICE_DEVICE_TYPE_WEB))
                        {
                            customJson.data.Add(new LandingContent
                            {
                                ComponentName = componentName,
                                Heading = learningContent?.heading?.value,
                                Description = new DescriptionObject
                                {
                                    value = learningContent?.description?.value,
                                    IsHTML = true
                                },
                            });
                        }
                        else if (componentName == ServiceConstant.LANDING_SEARCH_COMPONENT && !deviceType.ToLowerInvariant().Equals(ServiceConstant.SITECORE_LAYOUTSERVICE_DEVICE_TYPE_WEB))
                        {
                            List<string> sortBy = new List<string>();
                            List<string> contentType = new List<string>();
                            List<string> topics = new List<string>();
                            List<Dictionary<string, string>> dictionaryTopicslist = new List<Dictionary<string, string>>();
                            List<Dictionary<string, string>> dictionarySortBylist = new List<Dictionary<string, string>>();
                            List<Dictionary<string, string>> dictionaryContentTypelist = new List<Dictionary<string, string>>();
                            List<ListFormatKeyValuePair> keyValuePairSortByList = new List<ListFormatKeyValuePair>();
                            List<ListFormatKeyValuePair> keyValuePairContentTypeList = new List<ListFormatKeyValuePair>();
                            List<ListFormatKeyValuePair> keyValuePairTopicsList = new List<ListFormatKeyValuePair>();
                            foreach (var item in learningContent?.sortBy?.targetItems)
                            {
                                string sortByValue = item?.title?.value;
                                ListFormatKeyValuePair listFormatKeyValuePair = new ListFormatKeyValuePair();
                                listFormatKeyValuePair.Key = item?.key?.value;
                                //  listFormatKeyValuePair.Key = sortByValue.Replace(" ", "").ToLowerInvariant();
                                listFormatKeyValuePair.Value = sortByValue;
                                keyValuePairSortByList.Add(listFormatKeyValuePair);
                            }
                            foreach (var item in learningContent?.contentType?.targetItems)
                            {
                                string contentTypeValue = item?.title?.value;
                                ListFormatKeyValuePair listFormatKeyValuePair = new ListFormatKeyValuePair();
                                listFormatKeyValuePair.Key = contentTypeValue.Replace(" ", "").ToLowerInvariant();
                                listFormatKeyValuePair.Value = contentTypeValue;
                                keyValuePairContentTypeList.Add(listFormatKeyValuePair);
                            }
                            foreach (var item in learningContent?.topics?.targetItems)
                            {
                                string topicsValue = item?.title?.value;
                                ListFormatKeyValuePair listFormatKeyValuePair = new ListFormatKeyValuePair();
                                listFormatKeyValuePair.Key = topicsValue.Replace(" ", "").ToLowerInvariant();
                                listFormatKeyValuePair.Value = topicsValue;
                                keyValuePairTopicsList.Add(listFormatKeyValuePair);
                            }
                            customJson.data.Add(new LandingSearch
                            {
                                ComponentName = componentName,
                                Search = learningContent?.search?.value,
                                FilterContent = learningContent?.filterContent?.value,
                                Reset = learningContent?.reset?.value,
                                ContentTypeText = learningContent?.contentTypeText?.value,
                                TopicsText = learningContent?.topicsText?.value,
                                ApplyFilter = learningContent?.applyFilter?.value,
                                Filter = learningContent?.filter?.value,
                                SortbyText = learningContent?.sortbyText?.value,
                                LoadMore = learningContent?.loadMore?.value,
                                RefineText = learningContent?.refineText?.value,
                                ClearAllText = learningContent?.resetAllText?.value,
                                Results = learningContent?.results?.value,
                                ResultNotFound = learningContent?.resultsNotFound?.value,
                                PageSizeApp = learningContent?.pageSizeApp?.value,
                                PageSizeWeb = learningContent?.pageSizeWeb?.value,
                                SortBy = keyValuePairSortByList,
                                ContentType = keyValuePairContentTypeList,
                                Topics = keyValuePairTopicsList
                            }); ;
                        }
                        else if (componentName == ServiceConstant.LANDING_PROMO_COMPONENT)
                        {
                            LandingPromo landingPromo = new LandingPromo();
                            landingPromo.ComponentName = componentName;
                            foreach (var promoContent in learningContent?.promoLearning?.targetItems)
                            {
                                landingPromo.Title = promoContent?.title?.value;
                                DescriptionObject descriptionObject = new DescriptionObject
                                {
                                    value = promoContent?.description?.value,
                                    IsHTML = true
                                };
                                landingPromo.Description = descriptionObject;
                                landingPromo.ShortDescription = promoContent?.shortDescription?.value;
                                landingPromo.IsChecked = promoContent?.isChecked?.boolValue;
                                LandingImage landingMobileImage = new LandingImage
                                {
                                    Src = promoContent?.imageMobile?.src,
                                    Alt = promoContent?.imageMobile?.alt,
                                    Width = promoContent?.imageMobile?.width,
                                    Height = promoContent?.imageMobile?.height,
                                };
                                if (!string.IsNullOrEmpty(landingMobileImage?.Src)) { landingMobileImage.Src = rootUrl + promoContent?.imageMobile?.src; }
                                landingPromo.ImageMobile = landingMobileImage;
                                LandingImage landingTabImage = new LandingImage
                                {
                                    Src = promoContent?.imageTab?.src,
                                    Alt = promoContent?.imageTab?.alt,
                                    Width = promoContent?.imageTab?.width,
                                    Height = promoContent?.imageTab?.height,
                                };
                                if (!string.IsNullOrEmpty(landingTabImage?.Src)) { landingTabImage.Src = rootUrl + promoContent?.imageTab?.src; }
                                landingPromo.ImageTab = landingTabImage;
                                LandingImage landingImage = new LandingImage
                                {
                                    Src = promoContent?.image?.src,
                                    Alt = promoContent?.image?.alt,
                                    Width = promoContent?.image?.width,
                                    Height = promoContent?.image?.height,
                                };
                                if (!string.IsNullOrEmpty(landingImage.Src)) { landingImage.Src = rootUrl + promoContent?.image?.src; }
                                landingPromo.Image = landingImage;
                                landingPromo.Video = promoContent?.video?.url;
                                landingPromo.Link = promoContent?.link?.url;
                                landingPromo.ItemId = promoContent?.link?.targetItem != null ? promoContent.link.targetItem.id : null;
                                if (!string.IsNullOrEmpty(landingPromo?.ItemId))
                                {
                                    var formatId = StringExtensions.FormatGuid(landingPromo?.ItemId);
                                    landingPromo.ItemId = formatId;
                                }
                            }
                            customJson.data.Add(landingPromo);
                        }
                        else if (componentName == ServiceConstant.LANDING_SUGGESTED_ARTICLES_COMPONENT)
                        {
                            List<LearningSuggestedArticalDetail> learningSuggestedArticleDetail = new List<LearningSuggestedArticalDetail>();
                            SuggestedArticlesDetail suggestedArticlesDetail = new SuggestedArticlesDetail();
                            suggestedArticlesDetail.ComponentName = componentName;
                            suggestedArticlesDetail.Heading = learningContent?.heading?.value;
                            foreach (var suggestedContent in learningContent?.suggestedArticlesList?.targetItems)
                            {
                                LearningSuggestedArticalDetail suggestedContentDetail = new LearningSuggestedArticalDetail();
                                suggestedContentDetail.Heading = suggestedContent?.title?.value;
                                suggestedContentDetail.ShortDescription = suggestedContent?.shortDescription?.value;
                                DescriptionObject descriptionObject = new DescriptionObject
                                {
                                    value = suggestedContent?.description?.value,
                                    IsHTML = true
                                };
                                suggestedContentDetail.Description = descriptionObject;
                                suggestedContentDetail.IsChecked = suggestedContent?.isChecked?.boolValue;
                                suggestedContentDetail.Date = stringToDateConverter.DateTimeConvert(suggestedContent?.dateTime?.value.ToString());
                                suggestedContentDetail.Link = suggestedContent?.link?.url;
                                suggestedContentDetail.ItemId = suggestedContent?.link?.targetItem != null ? suggestedContent.link.targetItem.id : null;
                                if (!string.IsNullOrEmpty(suggestedContentDetail?.ItemId))
                                {
                                    var formatId = StringExtensions.FormatGuid(suggestedContentDetail?.ItemId);
                                    suggestedContentDetail.ItemId = formatId;
                                }
                                suggestedContentDetail.ImageMobile = new LandingImage
                                {
                                    Alt = suggestedContent?.imageMobile?.alt,
                                    Src = suggestedContent?.imageMobile?.src,
                                    Width = suggestedContent?.imageMobile?.width,
                                    Height = suggestedContent?.imageMobile?.hight
                                };
                                if (!string.IsNullOrEmpty(suggestedContentDetail?.ImageMobile?.Src)) { suggestedContentDetail.ImageMobile.Src = rootUrl + suggestedContent?.imageMobile?.src; }
                                suggestedContentDetail.ImageTab = new LandingImage
                                {
                                    Src = suggestedContent?.imageTab?.src,
                                    Alt = suggestedContent?.imageTab?.alt,
                                    Width = suggestedContent?.imageTab?.width,
                                    Height = suggestedContent?.imageTab?.height,
                                };
                                if (!string.IsNullOrEmpty(suggestedContentDetail?.ImageTab?.Src)) { suggestedContentDetail.ImageTab.Src = rootUrl + suggestedContent?.imageTab?.src; }
                                suggestedContentDetail.Image = new LandingImage
                                {
                                    Src = suggestedContent?.image?.src,
                                    Alt = suggestedContent?.image?.alt,
                                    Width = suggestedContent?.image?.width,
                                    Height = suggestedContent?.image?.height,
                                };
                                if (!string.IsNullOrEmpty(suggestedContentDetail?.Image?.Src)) { suggestedContentDetail.Image.Src = rootUrl + suggestedContent?.image?.src; }
                                suggestedContentDetail.Video = suggestedContent?.video?.url;
                                learningSuggestedArticleDetail.Add(suggestedContentDetail);
                            };
                            suggestedArticlesDetail.RelatedContent = learningSuggestedArticleDetail;
                            customJson.data.Add(suggestedArticlesDetail);
                        }
                    }
                }
            }
            Sitecore.Data.ID siteSettings = new Sitecore.Data.ID(ServiceConstant.SITECORE_ID_SITESETTINGS);
            if (!siteSettings.IsNull)
            {
                Item sItem = webDb.GetItem(siteSettings);
                if (sItem != null)
                {
                    customJson.data.Add(new SearchSettings
                    {
                        ComponentName = ServiceConstant.LEARNING_SEARCH_SETTINGS,
                        SpecialCharactersRegExMobile = sItem?.Fields[ServiceConstant.LEARNING_SEARCH_SPECIAL_CHARACTERS_REGEX_MOBILE]?.Value,
                        SpecialCharactersRegExWeb = sItem?.Fields[ServiceConstant.LEARNING_SEARCH_SPECIAL_CHARACTERS_REGEX_WEB]?.Value,
                        SpecialLetters = sItem?.Fields[ServiceConstant.LEARNING_SEARCH_SPECIAL_LETTERS]?.Value
                    });
                }
            }
            return customJson.data;
        }
    }
}