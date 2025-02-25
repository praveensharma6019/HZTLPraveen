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






