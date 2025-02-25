using DCX.Feature.EServices.AccountServices.Logging;
using DCX.Feature.EServices.AccountServices.Models;
using DCX.Feature.EServices.AccountServices.Models.DemandLoad;
using DCX.Feature.EServices.AccountServices.Models.GetPayFortPaymentURLDetails;
using DCX.Feature.EServices.AccountServices.Models.OnlinePayment;
using DCX.Feature.EServices.DemandLoad.Logging;
using DCX.Feature.EServices.DemandLoad.Models.DemandLoad;
using DCX.Foundation.EServices.ServiceClient;
using DCX.Foundation.EServices.ServiceClient.Client;
using DCX.Foundation.EServices.ServiceClient.Constant;
using DCX.Foundation.EServices.ServiceClient.Models;
using DCX.Foundation.Extension.Extensions;
using DCX.Foundation.Logging.IService;
using DCX.Foundation.SessionManager;
using DCX.Foundation.SessionManager.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Sitecore.Data.Items;
using Sitecore.Web.UI.HtmlControls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Sitecore.Configuration.State;

namespace DCX.Feature.EServices.DemandLoad.Service
{
    public class DemandLoadService
    {
        #region -- Properties --

        private readonly ClientApi clientApi;
        private readonly ClientCommon clientCommon;
        private ILoggerServices demandLoadLoggerService;

        #endregion

        #region -- Constructor --

        public DemandLoadService()
        {
            clientApi = new ClientApi();
            clientCommon = new ClientCommon();
            demandLoadLoggerService = new DemandLoadLoggerService();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get Unbilled Amount
        /// </summary>
        /// <param name="contractAccountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>premiseSearchMOResponse</returns>
        public async Task<GetUnbilledAmountResponse> GetUnbilledAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetUnbilledAmountResponse response = new GetUnbilledAmountResponse();

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.ZUMC_ERP_ISU_SERVICES_SRV;
                serviceRequest.Endpoint = ServiceConstant.GET_UNBILLED_AMOUNT_ENDPOINT + "?" + ServiceConstant.Contract_Account + "='" + contractAccountNumber + "'";
                serviceRequest.RequestHeaders = clientCommon.GetRequestHeaders(httpRequestBase);

                var SECXCSRFToken = SessionStorage.SECXCSRFToken != null ? SessionStorage.SECXCSRFToken : string.Empty;

                serviceRequest.RequestHeaders.XCSRFToken = SECXCSRFToken;

                demandLoadLoggerService.Error("GetUnbilledAmount SAP endpoint: " + serviceRequest.BaseUrl + serviceRequest.Endpoint);

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                demandLoadLoggerService.Error("GetUnbilledAmount SAP apiResponse: " + apiResponse);

                if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    response.Error.ErrorMessage = httpResponseMessage.ReasonPhrase;
                    return response;
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var objResp = JsonConvert.DeserializeObject<GetUnbilledAmountResponse>(apiResponse);

                    if (objResp != null && objResp.UnbilledAmounDetails != null)
                    {
                        response.UnbilledAmounDetails = objResp.UnbilledAmounDetails;
                    }
                    else
                    {
                        response.Error = clientCommon.SetErrorResponse(apiResponse);
                    }
                }
                else
                {
                    response.Error = clientCommon.SetAPIErrorResponse(httpResponseMessage.ReasonPhrase, apiResponse);
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error(ex.Message, ex);
                response.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_TECHERR);
            }

            return response;
        }

        /// <summary>
        /// Get Predicted Bill Amount
        /// </summary>
        /// <param name="contractAccountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>premiseSearchMOResponse</returns>
        public async Task<GetPredictedBillResponse> GetPredictedBillAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetPredictedBillResponse response = new GetPredictedBillResponse();

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.ZUMC_ERP_ISU_SERVICES_SRV;
                serviceRequest.Endpoint = ServiceConstant.GET_PREDICTEDBILL_AMOUNT_ENDPOINT + "?" + ServiceConstant.Contract_Account + "='" + contractAccountNumber + "'";
                serviceRequest.RequestHeaders = clientCommon.GetRequestHeaders(httpRequestBase);

                var SECXCSRFToken = SessionStorage.SECXCSRFToken != null ? SessionStorage.SECXCSRFToken : string.Empty;

                demandLoadLoggerService.Error("GetPredictedBillAmount SAP endpoint: " + serviceRequest.BaseUrl + serviceRequest.Endpoint);
                serviceRequest.RequestHeaders.XCSRFToken = SECXCSRFToken;

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                demandLoadLoggerService.Error("GetPredictedBillAmount SAP apiResponse: " + apiResponse);
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    response.Error.ErrorMessage = httpResponseMessage.ReasonPhrase;
                    return response;
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var objResp = JsonConvert.DeserializeObject<GetPredictedBillResponse>(apiResponse);

                    if (objResp != null && objResp.PredictedAmountDetails != null)
                    {
                        response.PredictedAmountDetails = objResp.PredictedAmountDetails;
                    }
                    else
                    {
                        response.Error = clientCommon.SetErrorResponse(apiResponse);
                    }
                }
                else
                {
                    response.Error = clientCommon.SetAPIErrorResponse(httpResponseMessage.ReasonPhrase, apiResponse);
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error(ex.Message, ex);
                response.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_TECHERR);
            }

            return response;
        }

        /// <summary>
        /// GetDemandLoadSet
        /// </summary>
        /// <param name="contractAccountNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="httpRequestBase"></param>
        /// <returns></returns>
        public async Task<GetDemandLoadSetAPIResponse> GetDemandLoadSet(string contractAccountNumber, string startDate, string endDate, HttpRequestBase httpRequestBase)
        {
            GetDemandLoadSetAPIResponse response = new GetDemandLoadSetAPIResponse();

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.ZUMC_ERP_ISU_SERVICES_SRV;
                serviceRequest.Endpoint = ServiceConstant.GET_DEMANDLOADSET_AMOUNT_ENDPOINT + "?";
                serviceRequest.Endpoint += "$expand=DemandLoadHalfHourSet,DemandLoadAvgHalfHourSet,DemandLoadAvgDaySet";
                serviceRequest.Endpoint += "&" + ServiceConstant.FILTERPARAMETER + "=" + ServiceConstant.Contract_Account + " eq '" + contractAccountNumber + "'";
                serviceRequest.Endpoint += " and " + ServiceConstant.Start_Date + " eq datetime'" + startDate + "'";
                serviceRequest.Endpoint += " and " + ServiceConstant.End_Date + " eq datetime'" + endDate + "'";
                serviceRequest.Endpoint += "&" + ServiceConstant.FILTER_JSONFORMATTER;

                serviceRequest.RequestHeaders = clientCommon.GetRequestHeaders(httpRequestBase);

                var SECXCSRFToken = SessionStorage.SECXCSRFToken != null ? SessionStorage.SECXCSRFToken : string.Empty;

                serviceRequest.RequestHeaders.XCSRFToken = SECXCSRFToken;

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    response.result.Error.ErrorMessage = httpResponseMessage.ReasonPhrase;
                    return response;
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var objResp = JsonConvert.DeserializeObject<GetDemandLoadSetAPIResponse>(apiResponse);

                    if (objResp != null && objResp.result != null)
                    {
                        objResp.result.results[0].EndDate = clientCommon.ConvertToDateTimeString(objResp.result.results[0].EndDate);
                        objResp.result.results[0].StartDate = clientCommon.ConvertToDateTimeString(objResp.result.results[0].StartDate);

                        if (objResp.result.results[0].DemandLoadAvgHalfHourSet != null && objResp.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData != null
                            && objResp.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData.Count > 0)
                        {
                            for (int i = 0; i < objResp.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData.Count; i++)
                            {
                                string IntervalEndTime = objResp.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData[i].IntervalEndTime.Replace("PT", "").Replace("H", ":").Replace("M", ":").Replace("S", "");
                                TimeSpan endTime = TimeSpan.Parse(IntervalEndTime);
                                objResp.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData[i].IntervalEndTime = new DateTime().Add(endTime).ToString("HH:mm");
                            }
                        }
                        response.result = objResp.result;
                    }
                    else
                    {
                        response.result.Error = clientCommon.SetErrorResponse(apiResponse);
                    }
                }
                else
                {
                    response.result.Error = clientCommon.SetAPIErrorResponse(httpResponseMessage.ReasonPhrase, apiResponse);
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error(ex.Message, ex);
                response.result.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_TECHERR);
            }

            return response;
        }

        /// <summary>
        /// GetCASet
        /// </summary>
        /// <param name="partnerNo"></param>
        /// <param name="status"></param>
        /// <param name="httpRequestBase"></param>
        /// <returns></returns>
        public async Task<ODataApiResponse<CASetListResponse>> GetCASet(string partnerNo, string status, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<CASetListResponse> CASetResponse = new ODataApiResponse<CASetListResponse>(new CASetListResponse());
            try
            {
                var xcsrftoken = SessionStorage.SECXCSRFToken ?? string.Empty;
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.REGISTERATION_BASEURL_ZUMC_ERP_UTILITIES_UMC_SRV;
                serviceRequest.Endpoint = ServiceConstant.ACCOUNT_DASHBOARDBILLSSET + "?" + ServiceConstant.FILTER_JSONFORMATTER + "&" + ServiceConstant.FILTERPARAMETER + "=" + ServiceConstant.PARTNERLABEL + " " + ServiceConstant.Eq_Label + " '" + partnerNo + "' and " +
                  ServiceConstant.STATUSLABEL + " " + ServiceConstant.Eq_Label + " '" + status + "'";
                serviceRequest.RequestHeaders = clientCommon.GetRequestHeaders(httpRequestBase);
                serviceRequest.RequestHeaders.XCSRFToken = xcsrftoken;
                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    CASetResponse.Error.ErrorMessage = httpResponseMessage.ReasonPhrase;
                    return CASetResponse;
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    CASetResponse.ResponseData = JsonConvert.DeserializeObject<CASetListResponse>(apiResponse);
                }
                else
                {
                    CASetResponse.Error = clientCommon.SetAPIErrorResponse(httpResponseMessage.ReasonPhrase, apiResponse);
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error(ex.Message, ex);
                CASetResponse.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_TECHERR);
            }

            return CASetResponse;
        }


        /// <summary>
        /// Get FixedBill Amount
        /// </summary>
        /// <param name="contractAccountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>premiseSearchMOResponse</returns>
        public async Task<GetFixedBillAmountResponse> GetFixedBillAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetFixedBillAmountResponse response = new GetFixedBillAmountResponse();

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.ZUMC_ERP_ISU_SERVICES_SRV;
                serviceRequest.Endpoint = ServiceConstant.GET_FIXEDBILL_AMOUNT_ENDPOINT + "?" + ServiceConstant.Contract_Account + "='" + contractAccountNumber + "'";
                serviceRequest.RequestHeaders = clientCommon.GetRequestHeaders(httpRequestBase);

                var SECXCSRFToken = SessionStorage.SECXCSRFToken != null ? SessionStorage.SECXCSRFToken : string.Empty;

                serviceRequest.RequestHeaders.XCSRFToken = SECXCSRFToken;

                demandLoadLoggerService.Error("GetFixedBillAmount SAP endpoint: " + serviceRequest.BaseUrl + serviceRequest.Endpoint);

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                demandLoadLoggerService.Error("GetFixedBillAmount SAP apiResponse: " + apiResponse);

                if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    response.Error.ErrorMessage = httpResponseMessage.ReasonPhrase;
                    return response;
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var objResp = JsonConvert.DeserializeObject<GetFixedBillAmountResponse>(apiResponse);

                    if (objResp != null && objResp.FixedBillAmountDetails != null)
                    {
                        response.FixedBillAmountDetails = objResp.FixedBillAmountDetails;
                    }
                    else
                    {
                        response.Error = clientCommon.SetErrorResponse(apiResponse);
                    }
                }
                else
                {
                    response.Error = clientCommon.SetAPIErrorResponse(httpResponseMessage.ReasonPhrase, apiResponse);
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error(ex.Message, ex);
                response.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_TECHERR);
            }

            return response;
        }


        /// <summary>
        ///Get IPAmount Response
        /// </summary>
        /// <param name="contractAccountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>premiseSearchMOResponse</returns>
        public async Task<GetIPAmountResponse> GetInstalmentPlanAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetIPAmountResponse response = new GetIPAmountResponse();

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.ZUMC_ERP_ISU_SERVICES_SRV;
                serviceRequest.Endpoint = ServiceConstant.GET_IP_AMOUNT_ENDPOINT + "?" + ServiceConstant.Contract_Account + "='" + contractAccountNumber + "'";
                serviceRequest.RequestHeaders = clientCommon.GetRequestHeaders(httpRequestBase);

                var SECXCSRFToken = SessionStorage.SECXCSRFToken != null ? SessionStorage.SECXCSRFToken : string.Empty;

                serviceRequest.RequestHeaders.XCSRFToken = SECXCSRFToken;

                demandLoadLoggerService.Error("GetInstalmentPlanAmount SAP endpoint: " + serviceRequest.BaseUrl + serviceRequest.Endpoint);

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                demandLoadLoggerService.Error("GetInstalmentPlanAmount SAP apiResponse: " + apiResponse);

                if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    response.Error.ErrorMessage = httpResponseMessage.ReasonPhrase;
                    return response;
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var objResp = JsonConvert.DeserializeObject<GetIPAmountResponse>(apiResponse);

                    if (objResp != null && objResp.IPAmountDetails != null)
                    {
                        response.IPAmountDetails = objResp.IPAmountDetails;
                    }
                    else
                    {
                        response.Error = clientCommon.SetErrorResponse(apiResponse);
                    }
                }
                else
                {
                    response.Error = clientCommon.SetAPIErrorResponse(httpResponseMessage.ReasonPhrase, apiResponse);
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error(ex.Message, ex);
                response.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_TECHERR);
            }

            return response;
        }

        #endregion
    }
}