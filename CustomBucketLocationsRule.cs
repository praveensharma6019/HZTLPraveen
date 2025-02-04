using DCX.Foundation.Extension.Constant;
using Sitecore.Data.Managers;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Rules.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Buckets.Rules.Bucketing;
using DCX.Foundation.Extension.Extensions;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Marketing.xMgmt.Extensions;
using Microsoft.Ajax.Utilities;
using Sitecore.Shell.Applications.ContentEditor.Editors.Folder;
namespace DCX.Foundation.Extension.Buckets
{
    public class CustomBucketLocationsRule
    {
    }
    public class CustomBucketLocationsRule<T> : RuleAction<T> where T : BucketingRuleContext
    {
        public CustomBucketLocationsRule()
        {
        }
        Database masterDb = Sitecore.Configuration.Factory.GetDatabase(ExtensionConstant.MASTERDB);
        public string Levels { get; set; }
        public override void Apply(T ruleContext)
        {
            try
            {
                Assert.ArgumentNotNull(ruleContext, "ruleContext");
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
                        Sitecore.Data.Items.Item locationtemDetails = masterDb.GetItem(ruleContext.NewItemId, Sitecore.Globalization.Language.Parse(hasVersion));
                        string locationTarget = null;
                        if (locationtemDetails != null)
                        {
                            Sitecore.Data.Fields.ReferenceField regionField = locationtemDetails.Fields[ExtensionConstant.Region];
                            if (regionField != null && regionField.TargetItem != null)
                            {
                                locationTarget = regionField.TargetItem.Name;
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(locationTarget))
                        {
                            ruleContext.ResolvedPath = CreateResolvedPath(Convert.ToString(locationTarget));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Form : CustomBucketYYYYMMRule method in form " + ex.Message, this);
            }
        }
        public string CreateResolvedPath(string itemName)
        {
            Log.Info("CreateResolvedPath:" + itemName, itemName);
            List<string> folders = new List<string>();
            folders.Add(itemName);
            string[] charArray = folders.ToArray();
            return string.Join<string>(Sitecore.Buckets.Util.Constants.ContentPathSeperator, charArray);
        }
    }
}