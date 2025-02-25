using DCX.Feature.EServices.AccountServices.Models;
using DCX.Feature.EServices.AccountServices.Models.DemandLoad;
using DCX.Feature.EServices.AccountServices.Models.GetPayFortPaymentURLDetails;
using DCX.Feature.EServices.AccountServices.Models.OnlinePayment;
using DCX.Feature.EServices.DemandLoad.Models.DemandLoad;
using DCX.Foundation.EServices.ServiceClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace DCX.Feature.EServices.DemandLoad.IRepository
{
    public interface IDemandLoadRepository
    {
        Task<GetFixedBillAmountResponse> GetFixedBillAmount(string contractAccountNumber, HttpRequestBase httpRequestBase);
        Task<GetIPAmountResponse> GetInstalmentPlanAmount(string contractAccountNumber, HttpRequestBase httpRequestBase);
        Task<GetUnbilledAmountResponse> GetUnbilledAmount(string contractAccountNumber, HttpRequestBase httpRequestBase);
        Task<GetPredictedBillResponse> GetPredictedBillAmount(string contractAccountNumber, HttpRequestBase httpRequestBase);
        Task<ODataApiResponse<GetDemandLoadSetResponse>> GetDemandLoadSetDetails(string contractAccountNumber, string startDate, string endDate, bool IsLatestDay, bool IsLatest7Day, HttpRequestBase httpRequestBase);
        Task<ODataApiResponse<GetForecastResponse>> GetForecastBillAmountDetails(string contractAccountNumber, bool isInstalmentPlan, bool isFixedBill, HttpRequestBase httpRequestBase);
    }
}