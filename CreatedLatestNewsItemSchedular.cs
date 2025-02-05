using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Sitecore.Diagnostics;
using Sitecore.Data.Masters;
using DCX.Foundation.Extension.Constant;
using Sitecore.Web.UI.XamlSharp.Xaml.Extensions;
using Sitecore.Buckets.Managers;

namespace DCX.Foundation.SitecoreExtensions.Extensions
{
    public class CreatedLatestNewsItem
    {
        public async void Execute(Item[] items, Sitecore.Tasks.CommandItem commandItem, Sitecore.Tasks.ScheduleItem scheduleItem)
        {
            Log.Info("Getting Schedylar Start:" + commandItem, commandItem);

            // Connect to the Sitecore database
            Database masterDb = Sitecore.Configuration.Factory.GetDatabase(ExtensionConstant.MASTERDB);
            Database webDb = Sitecore.Configuration.Factory.GetDatabase(ExtensionConstant.WebDB);
            DateTime targetDate = scheduleItem.LastRun;
            try
            {
                Log.Info("Getting Schedylar Start in Try block:" + commandItem, commandItem);

                string proxyurl = ExtensionConstant.Proxyurl;
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                if (!string.IsNullOrWhiteSpace(proxyurl))
                {
                    WebProxy proxy = new WebProxy
                    {
                        Address = new Uri(proxyurl),
                    };
                    httpClientHandler.Proxy = proxy;
                }
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    Log.Info("Getting Schedylar Start Using Block:" + commandItem, commandItem);

                    Sitecore.Data.Items.Item eurolandirARApiUrl = masterDb.GetItem(ExtensionConstant.EurolandirARApiUrl_FIELDID);
                    Sitecore.Data.Items.Item eurolandirENApiUrl = masterDb.GetItem(ExtensionConstant.EurolandirENApiUrl_FIELDID);
                    Sitecore.Data.Items.Item detailEurolandirEnApiUrl = masterDb.GetItem(ExtensionConstant.DetailEurolandirEnApiUrl_FIELDID);
                    Sitecore.Data.Items.Item detailEurolandirArApiUrl = masterDb.GetItem(ExtensionConstant.DetailEurolandirArApiUrl_FIELDID);
                    Sitecore.Data.Items.Item shareholderAnnouncements = masterDb.GetItem(ExtensionConstant.ArticleTypeShareholderAnnouncement_ITEM);

                    string enApiUrl = null, arApiUrl = null, enApiUrlDetail = null, arApiUrlDetail = null;
                    arApiUrl = eurolandirARApiUrl.Fields[ExtensionConstant.Phrase_Field].Value;
                    enApiUrl = eurolandirENApiUrl.Fields[ExtensionConstant.Phrase_Field].Value;
                    enApiUrlDetail = detailEurolandirEnApiUrl.Fields[ExtensionConstant.Phrase_Field].Value;
                    arApiUrlDetail = detailEurolandirArApiUrl.Fields[ExtensionConstant.Phrase_Field].Value;

                    Log.Info("Getting EnApiUrl:" + enApiUrl, enApiUrl);
                    Log.Info("Getting targetDate using!" + targetDate, targetDate);
                    // Send GET request
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    Log.Info("Getting ArApiUrl:" + arApiUrl, arApiUrl);
                    HttpResponseMessage enResponse = await client.GetAsync(enApiUrl);
                    HttpResponseMessage arResponse = await client.GetAsync(arApiUrl);
                    Log.Info("Getting EnApiResponse!:" + enResponse, enResponse);
                    // Check if request was successful
                    if (enResponse.IsSuccessStatusCode && arResponse.IsSuccessStatusCode)
                    {
                        // Read response content as string
                        string enApiResponse = await enResponse.Content.ReadAsStringAsync();
                        string arApiResponse = await arResponse.Content.ReadAsStringAsync();
                        //Log.Info("Getting EnApiResponse IsSuccessStatusCode!:" + enApiResponse, enApiResponse);
                        // Log.Info("Getting ArApiResponse IsSuccessStatusCode!:" + arApiResponse, arApiResponse);

                        // Deserialize the JSON into a list of NewsItem objects
                        List<NewsItem> enNewsItems = JsonConvert.DeserializeObject<List<NewsItem>>(enApiResponse.ToString());
                        List<NewsItem> arNewsItems = JsonConvert.DeserializeObject<List<NewsItem>>(arApiResponse.ToString());

                        var filteredENNewsItems = enNewsItems.Where(item => item.Date.Date >= targetDate.Date).ToList();
                        var filteredARNewsItems = arNewsItems.Where(item => item.Date.Date >= targetDate.Date).ToList();
                        Log.Info("Getting FilteredENNewsItems!" + filteredENNewsItems.Count, filteredENNewsItems.Count);
                        Log.Info("Getting FilteredARNewsItems!" + filteredENNewsItems.Count, filteredARNewsItems.Count);

                        //using (new SecurityDisabler())
                        using (new Sitecore.Security.Accounts.UserSwitcher(Sitecore.Security.Accounts.User.FromName(ExtensionConstant.SitecoreUser, true)))
                        {
                            Language setLanguage = Language.Parse(ExtensionConstant.ArabicLanguageCode);
                            // Identify the parent item under which you want to create the new items
                            Item parentItem = masterDb.GetItem(ExtensionConstant.ParentItem_ARTICLE_FOLDER_ITEM, setLanguage);

                            TemplateItem template = masterDb.GetTemplate(new Sitecore.Data.ID(ExtensionConstant.ArticleItem_TEMPLATEID));
                            Log.Info("Getting ParentItem!:" + parentItem, parentItem);
                            if (parentItem != null)
                            {
                                foreach (NewsItem item in filteredARNewsItems)
                                {
                                    var eNNewsItems = filteredENNewsItems.Where(x => x.Date.ToString(ExtensionConstant.DateFormat) == item.Date.ToString(ExtensionConstant.DateFormat)).FirstOrDefault();

                                    if (item != null && eNNewsItems != null)
                                    {
                                        Log.Info("Item and ENNewsItems are not null:" + item, item);
                                        // Create the new item under the parent item
                                        var itemName = item.Date.ToString().Replace(":", "_").Replace("/", "-");

                                        HttpResponseMessage detailsEnResponse = await client.GetAsync(enApiUrlDetail + "&id=" + eNNewsItems.ID);
                                        HttpResponseMessage detailsArResponse = await client.GetAsync(arApiUrlDetail + "&id=" + item.ID);
                                        if (detailsEnResponse.IsSuccessStatusCode && detailsArResponse.IsSuccessStatusCode)
                                        {
                                            Log.Info("DetailsEnResponse IsSuccessStatusCode:" + detailsEnResponse.IsSuccessStatusCode, detailsEnResponse.IsSuccessStatusCode);
                                            // Read response content as string
                                            string enApiResponseDetail = await detailsEnResponse.Content.ReadAsStringAsync();
                                            string arApiResponseDetail = await detailsArResponse.Content.ReadAsStringAsync();

                                            // Deserialize the JSON into a list of NewsItem objects
                                            List<DetailNewsItem> enNewsDetailsItems = JsonConvert.DeserializeObject<List<DetailNewsItem>>(enApiResponseDetail.ToString());
                                            List<DetailNewsItem> arNewsDetailsItems = JsonConvert.DeserializeObject<List<DetailNewsItem>>(arApiResponseDetail.ToString());

                                            var eNNewsItemsContent = enNewsDetailsItems.Where(x => x.ID == eNNewsItems.ID).FirstOrDefault();
                                            var arNewsItemsContent = arNewsDetailsItems.Where(y => y.ID == item.ID).FirstOrDefault();


                                            Log.Info("ENNewsItemsContent:" + eNNewsItemsContent.ID, eNNewsItemsContent);
                                            Log.Info("ItemName:" + itemName, itemName);
                                            Log.Info("ParentItem:" + parentItem.ID, parentItem);
                                            Log.Info("ENNewsItems:" + eNNewsItems.ID, eNNewsItems);
                                            Item newItem = null;

                                            if (parentItem == null && string.IsNullOrEmpty(itemName) && template == null)
                                            {
                                                Sitecore.Diagnostics.Log.Error("Either Parent item or item name or Template is null", this);
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    //get bucket folder item and then its existing news item to check if item already exist.
                                                    var bucketItem = masterDb.GetItem(ExtensionConstant.ParentItem_ARTICLE_FOLDER_ITEM);
                                                    var existingChildItems = GetBucketChildItems(bucketItem);
                                                    var match = existingChildItems.FirstOrDefault(x => (x.Name.Equals(itemName)));
                                                    if (match == null)
                                                    {
                                                        newItem = parentItem.Add(itemName, template);
                                                        Sitecore.Diagnostics.Log.Info($"create Item for this: {newItem.Paths.FullPath}", this);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Sitecore.Diagnostics.Log.Error("Error creating item", ex, this);
                                                }
                                            }

                                            if (newItem != null && eNNewsItems != null)
                                            {
                                                Log.Info("NewItem:" + newItem, newItem);
                                                Log.Info("ArebicItem Start:" + newItem.ID, newItem);
                                                // Assign values to fields
                                                newItem.Editing.BeginEdit();
                                                newItem.Fields[ExtensionConstant.PageTitle_Field].Value = item.Title;
                                                newItem.Fields[ExtensionConstant.Title_Field].Value = item.Title;
                                                newItem.Fields[ExtensionConstant.Summary_Field].Value = item.Title;
                                                newItem.Fields[ExtensionConstant.ArticleDate_Field].Value = DateUtil.ToIsoDate(item.Date);
                                                newItem.Fields[ExtensionConstant.Content_Field].Value = "<div class=\"js-table-style\">" + arNewsItemsContent.Message + "</div>";
                                                newItem.Fields[ExtensionConstant.Type_Field].Value = shareholderAnnouncements.Fields[ExtensionConstant.Text_Field].Value;
                                                newItem.Fields[ExtensionConstant.CategoryType_Field].Value = ExtensionConstant.ArticleTypeShareholderAnnouncement_ITEM;
                                                newItem.Fields[ExtensionConstant.Breadcrumb_Title_Field].Value = shareholderAnnouncements.Fields[ExtensionConstant.Text_Field].Value;
                                                newItem.Editing.EndEdit();
                                                using (new LanguageSwitcher(ExtensionConstant.EnglishLanguageCode))
                                                {
                                                    newItem = masterDb.GetItem(newItem.ID);
                                                    var englishVersion = newItem.Versions.AddVersion();
                                                    Log.Info("EnglishVersion Start:" + englishVersion.ID, englishVersion);
                                                    englishVersion.Editing.BeginEdit();
                                                    englishVersion.Fields[ExtensionConstant.PageTitle_Field].Value = eNNewsItems.Title;
                                                    englishVersion.Fields[ExtensionConstant.Title_Field].Value = eNNewsItems.Title;
                                                    englishVersion.Fields[ExtensionConstant.Summary_Field].Value = eNNewsItems.Title;
                                                    englishVersion.Fields[ExtensionConstant.ArticleDate_Field].Value = DateUtil.ToIsoDate(eNNewsItems.Date);
                                                    englishVersion.Fields[ExtensionConstant.Content_Field].Value = "<div class=\"js-table-style\">" + eNNewsItemsContent.Message + "</div>";
                                                    englishVersion.Fields[ExtensionConstant.Type_Field].Value = shareholderAnnouncements.Fields[ExtensionConstant.Text_Field].Value;
                                                    englishVersion.Fields[ExtensionConstant.CategoryType_Field].Value = ExtensionConstant.ArticleTypeShareholderAnnouncement_ITEM;
                                                    englishVersion.Fields[ExtensionConstant.Breadcrumb_Title_Field].Value = shareholderAnnouncements.Fields[ExtensionConstant.Text_Field].Value;
                                                    englishVersion.Editing.EndEdit();
                                                }
                                                var languages = LanguageManager.GetLanguages(masterDb).Select(l => l.Name).ToArray();
                                                Log.Info("Get All Languages form Sitecore:" + languages, languages);
                                                // Create a publisher for each language
                                                foreach (var currentLanguage in languages)
                                                {
                                                    PublishOptions publishoptions = new PublishOptions(masterDb, webDb, PublishMode.SingleItem, Language.Parse(currentLanguage), DateTime.Now);
                                                    publishoptions.RootItem = newItem;
                                                    publishoptions.Deep = false; //Publish Sub Items
                                                    publishoptions.PublishRelatedItems = false;
                                                    publishoptions.CompareRevisions = false;
                                                    PublishOptions[] pos = { publishoptions };
                                                    PublishManager.Publish(pos);
                                                }
                                                Log.Info("Item Created and piblished:" + newItem, newItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Info("Error occurred while fetching API response", this);
                        resetSchedularLastRun(targetDate, masterDb);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error in catch!" + ex.Message, ex);
                resetSchedularLastRun(targetDate, masterDb);
            }
        }
        public void resetSchedularLastRun(DateTime targetDate, Database masterDb)
        {
            using (new Sitecore.Security.Accounts.UserSwitcher(Sitecore.Security.Accounts.User.FromName(ExtensionConstant.SitecoreUser, true)))
            {
                Sitecore.Data.Items.Item schedularItem = masterDb.GetItem(ExtensionConstant.NewsSchedular_ITEM);
                schedularItem.Editing.BeginEdit();
                schedularItem.Fields[ExtensionConstant.LastRun_Field].Value = DateUtil.ToIsoDate(targetDate);
                schedularItem.Editing.EndEdit();
                Log.Info("reset last run to back to previous last run date" + targetDate, targetDate);
            }
        }
        public IEnumerable<Item> GetBucketChildItems(Item bucketItem)
        {
            if (bucketItem == null)
            {
                throw new ArgumentNullException(nameof(bucketItem));
            }
            var childItems = new List<Item>();
            using (new Sitecore.Security.Accounts.UserSwitcher(Sitecore.Security.Accounts.User.FromName(ExtensionConstant.SitecoreUser, true)))
            {
                // Ensure the item is a bucket
                if (bucketItem != null)
                {
                    var items = bucketItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString().Equals(ExtensionConstant.ArticleItem_TEMPLATEID)).ToList();
                    childItems.AddRange(items);
                }
            }
            return childItems;
        }
    }
    public class NewsItem
    {
        public string ID { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string MessageType { get; set; }
    }
    public class DetailNewsItem
    {
        public string ID { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime InsertedDate { get; set; }
    }
}