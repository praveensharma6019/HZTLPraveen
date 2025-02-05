public static Expression<Func<LearnModuleModel, bool>> GetLearnSearchPredicate(string language, string selectedTopics, string selectedContentType, string searchTerm)
{
    var predicate = PredicateBuilder.True<LearnModuleModel>();
    string lang = language;
    List<string> selectedTopicsList = new List<string>();
    if (!String.IsNullOrEmpty(selectedTopics))
    {
        selectedTopicsList = selectedTopics.Split(',').ToList();
    }
    List<string> selectedContentTypeList = new List<string>();
    if (!String.IsNullOrEmpty(selectedContentType))
    {
        selectedContentTypeList = selectedContentType.Split(',').ToList();
    }
    ID learningDetailTemplateId = new ID(ExtensionConstants.LEARNING_DETAIL_TEMPLATE_ID);
    predicate = predicate.And(x => x.TemplateId == learningDetailTemplateId && !x.Name.StartsWith("__Standard"));
    predicate = predicate.And(x => x.Language == lang);
    if (selectedTopicsList != null && selectedTopicsList.Any())
    {
        var topicsPredicate = PredicateBuilder.True<LearnModuleModel>();
        foreach (string topic in selectedTopicsList)
        {
            var tempTopic = topic;
            topicsPredicate = topicsPredicate.Or(x => x.Topics.Contains(tempTopic));
        }
        predicate = predicate.And(topicsPredicate);
    }
    if (selectedContentTypeList != null && selectedContentTypeList.Any())
    {
        var contentPredicate = PredicateBuilder.True<LearnModuleModel>();
        foreach (string content in selectedContentTypeList)
        {
            var tempContent = content;
            contentPredicate = contentPredicate.Or(x => x.ContentType.Contains(tempContent));
        }
        predicate = predicate.And(contentPredicate);
    }
    if (!string.IsNullOrEmpty(searchTerm))
    {
        var searchTermPredicate = PredicateBuilder.False<LearnModuleModel>();
        var tempSearchTerm = searchTerm;
        searchTermPredicate = searchTermPredicate.Or(x => x.Title.Contains(tempSearchTerm));
        searchTermPredicate = searchTermPredicate.Or(x => x.Description.Contains(tempSearchTerm));
        searchTermPredicate = searchTermPredicate.Or(x => x.ShortDescription.Contains(tempSearchTerm));
        predicate = predicate.And(searchTermPredicate);
    }
    return predicate;
}
