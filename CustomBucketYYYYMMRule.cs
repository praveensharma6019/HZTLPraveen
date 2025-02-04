using Sitecore.Buckets.Rules.Bucketing;
using Sitecore.Buckets.Rules.Bucketing.Actions;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Masters;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.Rules.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCX.Foundation.Extension;
using DCX.Foundation.Extension.Constant;
using DCX.Foundation.Extension.Extensions;
using Sitecore.Marketing.xMgmt.Extensions;
namespace DCX.Foundation.Extension.Buckets
{
    public class CustomBucketYYYYMMRule<T> : RuleAction<T> where T : BucketingRuleContext
    {
        public CustomBucketYYYYMMRule()
        {
        }
        Database masterDb = Sitecore.Configuration.Factory.GetDatabase(ExtensionConstant.MASTERDB);
        private const int LengthOfInteger = 3;
        protected static readonly Random Random = new Random();
        public string Levels { get; set; }
        public override void Apply(T ruleContext)
        {
            try
            {
                Assert.ArgumentNotNull(ruleContext, "ruleContext");
                int levels = GetLevels();
                if (levels < 1)
                {
                    Log.Error(string.Format("Cannot apply CreateRandomIntegerBasedPath action. The value of levels: {0}", Levels), this);
                    return;
                }
                IEnumerable<string> integers = GetRandomIntegers(LengthOfInteger, levels);
                Sitecore.Data.Items.Item newItemId = masterDb.GetItem(ruleContext.NewItemId);
                var languages = LanguageManager.GetLanguages(masterDb).Select(l => l.Name).ToArray();
                if (newItemId != null)
                {
                    string hasVersion = null;
                    for (int i = 0; i < languages.Length; i++)
                    {
                        Sitecore.Globalization.Language language = Sitecore.Globalization.Language.Parse(languages[i]);
                        var languageSpecificItem = masterDb.GetItem(newItemId.ID, language);
                        if (languageSpecificItem.HasLanguageVersion())
                        {
                            hasVersion = languages[i];
                            break;
                        }
                    }
                    using (new LanguageSwitcher(hasVersion))
                    {
                        Sitecore.Data.Items.Item newItemDetails = masterDb.GetItem(ruleContext.NewItemId, Sitecore.Globalization.Language.Parse(hasVersion));
                        string newItemCreatedDate = null, newItemDate = null;
                        newItemDate = newItemDetails.Fields[ExtensionConstant.ArticleDate]?.Value;
                        if (!String.IsNullOrWhiteSpace(newItemDate))
                        {
                            ruleContext.ResolvedPath = CreateResolvedPath(Convert.ToString(newItemDate), integers.Count());
                        }
                        else
                        {
                            newItemCreatedDate = newItemDetails.Fields[ExtensionConstant.CreatedDate]?.Value;
                            ruleContext.ResolvedPath = CreateResolvedPath(Convert.ToString(newItemCreatedDate), integers.Count());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Form : CustomBucketYYYYMMRule method in form " + ex.Message, this);
            }
        }
        protected virtual int GetLevels()
        {
            int levels;
            if (!int.TryParse(Levels, out levels) || levels < 1)
            {
                return 0;
            }
            return levels;
        }
        protected virtual IEnumerable<string> GetRandomIntegers(int lengthOfInteger, int count)
        {
            IList<string> strings = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int number = GetRandomPowerOfTenNumber(lengthOfInteger);
                strings.Add(Convert.ToString(number));
            }
            return strings;
        }
        private int GetRandomPowerOfTenNumber(int exponent)
        {
            return Random.Next((int)Math.Pow(10, exponent - 1), (int)Math.Pow(10, exponent) - 1);
        }
        public string CreateResolvedPath(string itemName, int levels)
        {
            Log.Info("CreateResolvedPath:" + itemName, itemName);
            List<string> folders = new List<string>();
            int[] mynum = { 0, 4, 2 };
            for (int i = 0; i < levels; i++)
            {
                string folderName = itemName.Substring(mynum[i], mynum[i + 1]);
                if (folderName.Length > 1)
                {
                    folderName = char.ToUpper(folderName.ToCharArray()[0]) + folderName.Substring(1).ToLowerInvariant();
                }
                else
                {
                    folderName = folderName.ToUpper();
                }
                folders.Add(folderName);
            }
            string[] charArray = folders.ToArray();
            return string.Join<string>(Sitecore.Buckets.Util.Constants.ContentPathSeperator, charArray);
        }
    }
}
