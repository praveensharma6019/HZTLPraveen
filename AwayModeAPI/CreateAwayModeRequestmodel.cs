using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace DCX.Feature.EServices.AccountServices.Models.AwayMode
{
    public class CreateAwayModeRequest
    {
        public string PartnerNo { get; set; }
        public List<AwayModeDetailsSet> AwayModeDetailsSet { get; set; }
    }
    public class AwayModeDetailsSet
    {
        public string ContractAccount { get; set; }
        public int? PushFrequency { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}






using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace DCX.Feature.EServices.AccountServices.Models.AwayMode
{
    public class CreateAwayModeResponse
    {
        [JsonProperty("d")]
        public CreateAwayModeResponseData CreateAwayModeResponseData { get; set; }
    }
    public class CreateAwayModeResponseData
    {
        [JsonProperty("PartnerNo")]
        public string PartnerNo { get; set; }
        [JsonProperty("AwayModeDetailsSet")]
        public CreateAwayModeDetailsSet CreateAwayModeDetailsSet { get; set; }
    }
    public class CreateAwayModeDetailsSet
    {
        [JsonProperty("results")]
        public List<CreateAwayModeDetailsSetResult> Results { get; set; }
    }
    public class CreateAwayModeDetailsSetResult
    {
        [JsonProperty("AwayModeID")]
        public string AwayModeID { get; set; }
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("PartnerNo")]
        public string PartnerNo { get; set; }
        [JsonProperty("ContractAccount")]
        public string ContractAccount { get; set; }
        [JsonProperty("StartDate")]
        public string StartDate { get; set; }
        [JsonProperty("EndDate")]
        public string EndDate { get; set; }
        [JsonProperty("PushFrequency")]
        public int PushFrequency { get; set; }
    }
}
