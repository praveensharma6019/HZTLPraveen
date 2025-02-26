using Sitecore.Globalization;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Items;
using Sitecore.Data.Serialization.ObjectModel;
namespace DCX.Foundation.EServices.ServiceClient.Constant
{
    [ExcludeFromCodeCoverage]
    public static class GenericHelper
    {
        public static string GetDictionaryItem(string key, string language)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;
            string dictionaryValue = key;
            var contextLanguage = Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName.ToLower(CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(language))
                language = contextLanguage;
            using (new LanguageSwitcher(language))
            {
                var siteDomain = Sitecore.Context.Site != null ? Sitecore.Context.Site.Name + DictionaryKeys.DictionaryLabelWithSite : DictionaryKeys.DictionaryLabel;
                dictionaryValue = Translate.TextByDomain(siteDomain, key);
            }
            return dictionaryValue;
        }
        // Generic method to get any Sitecore database by name
        public static Database GetDatabase(string databaseName)
        {
            return Sitecore.Data.Database.GetDatabase(databaseName);
        }
        // Generic method to get a Sitecore Language by language code
        public static Language GetLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return null;
            }
            return LanguageManager.GetLanguage(language);
        }
        // Generic method to get a Sitecore Item
        public static Item GetDatabaseItem(string itemId, string language, string databaseName)
        {
            Database Db = Sitecore.Configuration.Factory.GetDatabase(databaseName);
            Sitecore.Globalization.Language itemlanguage = Sitecore.Data.Managers.LanguageManager.GetLanguage(language);
            Sitecore.Data.ID id = new Sitecore.Data.ID(itemId);
            if (id.IsNull)
            {
                return null;
            }
            return Db.GetItem(id, itemlanguage);
        }
    }
}