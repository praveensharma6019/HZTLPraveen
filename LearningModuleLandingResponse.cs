using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Sitecore.Feature.SiteSearch.Models
{
    public class LearningModuleLandingResponse
    {
        [JsonProperty("sitecore")]
        public LandingSitecore Sitecore { get; set; }
        public string Errormsg { get; set; }
    }
    public partial class LandingSitecore
    {
        [JsonProperty("route")]
        public LandingRoute Route { get; set; }
    }
    public partial class LandingRoute
    {
        [JsonProperty("placeholders")]
        public LandingPlaceholders Placeholders { get; set; }
    }
    public partial class LandingPlaceholders
    {
        [JsonProperty("jss-main")]
        public LandingJssMain[] JssMain { get; set; }
    }
    public partial class LandingJssMain
    {
        [JsonProperty("componentName")]
        public string ComponentName { get; set; }
        [JsonProperty("fields")]
        public LandingJssMainFields Fields { get; set; }
    }
    public partial class LandingJssMainFields
    {
        [JsonProperty("data")]
        public LandingData Data { get; set; }
    }
    public partial class LandingData
    {
        [JsonProperty("LandingContent")]
        public dynamic LandingContent { get; set; }
        [JsonProperty("LandingSearchDetail")]
        public dynamic LandingSearchDetail { get; set; }
        [JsonProperty("LadingPromo")]
        public dynamic LadingPromo { get; set; }
        [JsonProperty("SuggestedArticlesDetail")]
        public dynamic SuggestedArticlesDetail { get; set; }
        [JsonProperty("LearningNavigationDetail")]
        public dynamic LearningNavigationDetail { get; set; }
    }
}