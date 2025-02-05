public static Expression<Func<ArticleSearchModel, bool>> GetLatestSearchPredicate(string language)
{
    var predicate = PredicateBuilder.True<ArticleSearchModel>();
    string lang = language;
    ID articleTemplateId = new ID(Configuration.Settings.GetSetting("DCX.Feature.SiteSearch.ArticleTemplateId"));
    predicate = predicate.And(x => x.TemplateId == articleTemplateId); // Filter by article template
    predicate = predicate.And(x => !x.isStandardValue); // Exclude standard values
    predicate = predicate.And(x => x.Language == lang); // Filter by language
    return predicate;
}

[HttpGet]
public ActionResult GetLatestArticleResults(string lang)
{
    var myResults = new ArticleSearchResults
    {
        Results = new List<ArticleSearchResult>()
    };
    var searchIndex = ContentSearchManager.GetIndex(Sitecore.Configuration.Settings.GetSetting("DCX.Feature.SiteSearch.ArticleIndexName")); // Get the search index
                                                                                                                                            // newsOrder = newsOrder.Replace("\"", "");
    var searchPredicate = GetLatestSearchPredicate(lang.Trim('"')); // Build the search predicate without the 'type' filter
    try
    {
        using (var searchContext = searchIndex.CreateSearchContext()) // Get a context of the search index
        {
            IQueryable<ArticleSearchModel> searchResults;
            searchResults = searchContext.GetQueryable<ArticleSearchModel>()
                   .Where(searchPredicate);
            var fullResults = searchResults.GetResults();
            var groupedResults = fullResults.Hits
            .OrderByDescending(hit => hit.Document.articledatetime)
            .Take(3)
            .ToList();
            // Convert the results into the expected format
            foreach (var hit in groupedResults)
            {
                var serverTime = hit.Document.articledatetime != null ? Sitecore.DateUtil.ToServerTime(hit.Document.articledatetime).ToString() : "";
                var dDate = serverTime != null ? ConvertToDateTimeString(serverTime, "dd MMM yyyy") : "";
                myResults.Results.Add(new ArticleSearchResult
                {
                    id = hit.Document.ItemId.ToString(),
                    title = hit.Document.title ?? hit.Document.headingdatasourcereference,
                    summary = hit.Document.summary ?? "",
                    picture = hit.Document.picture ?? hit.Document.imagedatasourcereference,
                    picturealt = hit.Document.picturealt ?? hit.Document.imagealtdatasourcereference,
                    date = dDate,
                    pageurl = hit.Document.pageurl ?? "",
                    type = hit.Document.categoryType.FirstOrDefault() ?? "",
                    category = hit.Document.categoryName ?? ""
                });
            }
            myResults.noOfResults = myResults.Results.Count();
            myResults.totalCount = fullResults.TotalSearchResults;
            return ResultJson(myResults);
        }
    }
    catch (Exception ex)
    {
        Log.Error("Error on GetArticleResults : " + ex.Message.ToString(), "SearchController");
        return ResultJson(myResults);
    }
}