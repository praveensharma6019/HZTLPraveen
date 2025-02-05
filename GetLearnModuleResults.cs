[HttpGet]
public async Task<ActionResult> GetLearnModuleResults(string lang, string selectedTopics, string deviceType, string selectedContentType, string sortBy, string searchTerm, string itemid = ServiceConstant.LEARNING_LANDING_PAGE_ITEM_ID, int pageNumber = 0, int pageSize = 10)//item id of landing page for layoutservice response
{
    var myResults = new LearnModuleResults
    {
        SearchData = new List<LearnModuleResult>()
    };
    //from here layout service response
    LayoutService layoutservice = new LayoutService();
    string layoutServiceUrl = layoutservice.LayoutServiceEndPoint(itemid, lang, deviceType);
    string rootUrl = layoutservice.getRootURL();
    LearningModule learningModule = new LearningModule();
    myResults.data = await learningModule.GetLearnModuleLanding(layoutServiceUrl, myResults, deviceType);
    //to here
    int setpageSize = pageSize;
    string[] ignoreSpecialLetters = null;
    if (myResults?.data != null && myResults?.data?.Count > 0) //get the page size from sitecore
    {
        foreach (ILandingComponent d in myResults?.data)
        {
            if (d is LandingSearch)
            {
                LandingSearch landingSearch = (LandingSearch)d;
                if (deviceType != null && deviceType.ToLowerInvariant().Equals(ServiceConstant.SITECORE_LAYOUTSERVICE_DEVICE_TYPE_MOBILE))
                    setpageSize = landingSearch.PageSizeApp;
                else if (deviceType != null && deviceType.ToLowerInvariant().Equals(ServiceConstant.SITECORE_LAYOUTSERVICE_DEVICE_TYPE_WEB))
                    setpageSize = landingSearch.PageSizeWeb;
            }
            if (d is SearchSettings)//will use these special letters set in sitecore to ignore if comes in searchTerm
            {
                SearchSettings searchSettings = (SearchSettings)d;
                if (!string.IsNullOrEmpty(searchSettings.SpecialLetters))
                {
                    ignoreSpecialLetters = searchSettings.SpecialLetters.Split(',');
                    string pattern = $@"\b({string.Join("|", ignoreSpecialLetters)})\b";
                    if (!string.IsNullOrEmpty(searchTerm))
                        searchTerm = Regex.Replace(searchTerm, pattern, "", RegexOptions.IgnoreCase).Trim();
                }
            }
        }
    }
    var searchIndex = ContentSearchManager.GetIndex(ExtensionConstants.Article_WEBINDEX); // Get the search index
    var searchPredicate = GetLearnSearchPredicate(lang, selectedTopics, selectedContentType, searchTerm);
    using (var searchContext = searchIndex.CreateSearchContext())
    {
        var searchResults = searchContext.GetQueryable<LearnModuleModel>().Where(searchPredicate).OrderByDescending(item => item.OriginalDate).Skip((pageNumber - 1) * setpageSize).Take(setpageSize); // Search the index for items which match the predicate
        if (!String.IsNullOrEmpty(sortBy) && sortBy.ToLowerInvariant().Equals(ExtensionConstants.SORTBY_ASCENDING))
        {
            searchResults = searchContext.GetQueryable<LearnModuleModel>().Where(searchPredicate).OrderBy(item => item.OriginalDate).Skip((pageNumber - 1) * setpageSize).Take(setpageSize);
        }
        var fullResults = searchResults.GetResults();
        foreach (var hit in fullResults?.Hits)
        {
            List<string> lstTopics = new List<string>();
            if (!String.IsNullOrEmpty(hit?.Document?.Topics))
            {
                lstTopics = hit?.Document?.Topics?.Split(',').Select(x => x.Trim()).ToList();
            }
            List<string> lstContentType = new List<string>();
            if (!String.IsNullOrEmpty(hit?.Document?.ContentType))
            {
                lstContentType = hit?.Document?.ContentType?.Split(',').Select(x => x.Trim()).ToList();
            }
            ImageModel imageMobile = new ImageModel()
            {
                Src = hit?.Document?.ImageMobile ?? ""
            };
            LearnModuleResult learnModuleResult = new LearnModuleResult
            {
                ItemId = hit?.Document?.ItemId?.ToString() ?? "",
                PageUrl = hit?.Document?.PageUrl ?? "",
                Title = hit?.Document?.Title ?? "",
                Description = new DescriptionObject
                {
                    value = hit?.Document?.Description ?? "",
                    IsHTML = true
                },
                ShortDescription = hit?.Document?.ShortDescription ?? "",
                Image = hit?.Document?.Image ?? "",
                ImageTab = hit?.Document?.ImageTab ?? "",
                Video = hit?.Document?.Video ?? "",
                Date = hit?.Document?.Date ?? "",
                Topics = lstTopics,
                ContentType = lstContentType,
                ImageMobile = imageMobile
            };
            if (!string.IsNullOrEmpty(learnModuleResult?.Image)) { learnModuleResult.Image = rootUrl + hit?.Document?.Image; }
            if (!string.IsNullOrEmpty(learnModuleResult?.ImageTab)) { learnModuleResult.ImageTab = rootUrl + hit?.Document?.ImageTab; }
            if (!string.IsNullOrEmpty(learnModuleResult?.ImageMobile?.Src)) { learnModuleResult.ImageMobile.Src = rootUrl + hit?.Document?.ImageMobile; }
            myResults.SearchData.Add(learnModuleResult);
        }
        myResults.noOfResults = fullResults.Hits.Count();
        myResults.totalCount = fullResults.TotalSearchResults;
        return ResultJson(myResults);
    }
}