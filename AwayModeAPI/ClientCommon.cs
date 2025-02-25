using DCX.Feature.ExportToExternal;
using DCX.Foundation.EServices.ServiceClient.Client;
using DCX.Foundation.EServices.ServiceClient.Constant;
using DCX.Foundation.EServices.ServiceClient.Models;
using DCX.Foundation.Logging.IService;
using DCX.Foundation.SessionManager;
using DCX.Foundation.SessionManager.CookieService;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DCX.Foundation.EServices.ServiceClient
{
    public class ClientCommon
    {
        #region -- Properties --

        private readonly ClientApi clientApi;
        Sitecore.Data.ID eserviceMappingSettings;
        #endregion

        #region -- Constructors --

        public ClientCommon()
        {
            clientApi = new ClientApi();
            eserviceMappingSettings = new Sitecore.Data.ID(ServiceConstant.SITECORE_ID_ESERVICEMAPPING);
        }

        #endregion

        #region -- Methods --

        #region 1. Numeric captcha validation.

        /// <summary>
        /// Numerica  captcha validation
        /// </summary>
        /// <param name="captchaKey">captchaKey</param>
        /// <returns>NumericCaptchaResponse</returns>
        public NumericCaptchaResponse NumericCaptchaValidation(string captchaKey)
        {
            NumericCaptchaResponse numericCaptchaResponse = new NumericCaptchaResponse();

            try
            {
                string captchaCode = SessionStorage.SECCaptcha;
                numericCaptchaResponse.Success = false;

                if (string.IsNullOrWhiteSpace(captchaKey))
                {
                    SessionStorage.IsCaptchaValidated = false;
                    numericCaptchaResponse.FailureMessage = GetErrorMessageByCode(ServiceConstant.ERRORCODE_INVALIDCAPTCHA);
                    return numericCaptchaResponse;
                }

                if (string.IsNullOrWhiteSpace(captchaCode))
                {
                    SessionStorage.IsCaptchaValidated = false;
                    numericCaptchaResponse.FailureMessage = "Session Expired";
                    return numericCaptchaResponse;
                }
                else
                {
                    if (!captchaKey.Equals(captchaCode))
                    {
                        SessionStorage.IsCaptchaValidated = false;
                        numericCaptchaResponse.FailureMessage = GetErrorMessageByCode(ServiceConstant.ERRORCODE_INVALIDCAPTCHA);
                        return numericCaptchaResponse;
                    }
                    else
                    {
                        SessionStorage.IsCaptchaValidated = true;
                        numericCaptchaResponse.Success = true;
                        return numericCaptchaResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                SessionStorage.IsCaptchaValidated = false;
                numericCaptchaResponse.Success = false;
                numericCaptchaResponse.FailureMessage = ex.Message;
            }

            return numericCaptchaResponse;
        }

        #endregion

        #region 2. Custom error message.

        /// <summary>
        /// Custom error message
        /// </summary>
        /// <param name="errorCode">errorCode</param>
        /// <returns>errorMessage</returns>
        public string CustomErrorMessage(string errorCode, string language = "")
        {
            string errorMessage = string.Empty;
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("web");
            Sitecore.Data.Items.Item parentItem = master.GetItem("{CE86F266-D3AB-4122-9576-778C5CD1D764}");


            if (!string.IsNullOrWhiteSpace(errorCode))
            {
                foreach (Item item in parentItem.Children)
                {
                    var keyValue = item.Fields["Key"].Value;
                    if (!string.IsNullOrWhiteSpace(keyValue))
                    {
                        if (keyValue == errorCode)
                        {
                            var contextLanguage = Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName.ToLower(CultureInfo.InvariantCulture);
                            if (string.IsNullOrWhiteSpace(language))
                                language = contextLanguage;

                            using (new LanguageSwitcher(language))
                            {
                                errorMessage = item.Fields["Phrase"].Value;
                            }
                        }
                    }
                }
            }

            return errorMessage;
        }

        #endregion

        #region 3. Convert to date time string.

        /// <summary>
        /// Convert to date time string
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>dateResp</returns>
        public string ConvertToDateTimeString(string date)
        {
            string dateResp = date;

            if (!string.IsNullOrWhiteSpace(date))
                dateResp = new DateTime(1970, 1, 1, 3, 0, 0).AddMilliseconds(double.Parse(Regex.Match(date, @"(\d+)").Value)).ToString(ServiceConstant.DATEFORMAT);

            return dateResp;
        }

        public string ConvertToShortDateTimeString(string date)
        {
            string dateResp = date;

            if (!string.IsNullOrWhiteSpace(date))
                dateResp = new DateTime(1970, 1, 1, 3, 0, 0).AddMilliseconds(double.Parse(Regex.Match(date, @"(\d+)").Value)).ToString(ServiceConstant.SHORTDATEFORMAT);

            return dateResp;
        }

        public double RoundUptoDecimalPlaces(double value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces);
        }

        public decimal ToDecimal(string value)
        {
            decimal result = 0;

            if (!string.IsNullOrWhiteSpace(value))
                return Convert.ToDecimal(value);

            return result;
        }

        public string DecimalToString(decimal value)
        {
            return value.ToString("0.00");
        }
        #endregion

        #region g. GetKeyValuePairList.

        /// <summary>
        /// GetKeyValuePairList
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>dateResp</returns>
        public List<KeyValuePair<string, string>> GetKeyValuePairList(string data)
        {
            string[] keyValuePairArray = data.ToString().Split('&');
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (keyValuePairArray != null && keyValuePairArray.Length > 0 && !String.IsNullOrEmpty(keyValuePairArray[0]))
            {
                foreach (var item in keyValuePairArray)
                {
                    string[] keyValues = item.Split('=');
                    var key = keyValues[0];
                    var value = Uri.UnescapeDataString(keyValues[1]);
                    result.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            return result;
        }
        #endregion


        #region 4. Get request headers.

        /// <summary>
        /// Get request headers
        /// </summary>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>Request headers</returns>
        public RequestHeaders GetRequestHeaders(HttpRequestBase httpRequestBase)
        {
            RequestHeaders requestHeaders = new RequestHeaders();

            if (httpRequestBase != null)
            {
                requestHeaders.ContentType = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_CONTENTTYPE] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_CONTENTTYPE].Trim().ToString() : string.Empty;
                requestHeaders.AcceptLanguage = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE].Trim().ToString() : string.Empty;
                requestHeaders.XRequestedWith = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XREQUESTEDWITH] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XREQUESTEDWITH].Trim().ToString() : string.Empty;
                requestHeaders.XCSRFToken = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XCSRFTOKEN] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XCSRFTOKEN].Trim().ToString() : string.Empty;
                requestHeaders.Authorization = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_AUTHORIZATION] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_AUTHORIZATION].Trim().ToString() : string.Empty;

                requestHeaders.RequestFrom = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_REQUESTFROM] != null ?
                    httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_REQUESTFROM].Trim().ToString() : string.Empty;
                requestHeaders.XForwardedFor = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XFORWARDEDFOR] != null ?
                    httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XFORWARDEDFOR].Trim().ToString() : string.Empty;

            }

            return requestHeaders;
        }
        public RequestHeaders GetRequestSurvey2ConnectHeaders(HttpRequestBase httpRequestBase)
        {
            RequestHeaders requestHeaders = new RequestHeaders();

            if (httpRequestBase != null)
            {
                requestHeaders.ContentType = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_CONTENTTYPE] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_CONTENTTYPE].Trim().ToString() : string.Empty;
                requestHeaders.XAPIKey = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XAPIKey] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_XAPIKey].Trim().ToString() : string.Empty;
            }
            return requestHeaders;
        }
        #endregion

        #region 5. Decodeing login user.

        /// <summary>
        /// Get request headers
        /// </summary>
        /// <param name="authHeader">authHeader</param>
        /// <returns>string</returns>
        public string DecodedLoginUser(string authHeader)
        {
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ', (char)2, (char)StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                // Decode from Base64 to string
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                // Split username and password
                var username = decodedUsernamePassword.Split(':', (char)2)[0];
                var password = decodedUsernamePassword.Split(':', (char)2)[1];
                return decodedUsernamePassword;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region 6. Set error response.

        /// <summary>
        /// Set api error response
        /// </summary>
        /// <param name="reasonPhrase">reasonPhrase</param>
        /// <param name="apiResponse">apiResponse</param>
        /// <returns>errorResponse</returns>
        public ErrorResponse SetAPIErrorResponse(string reasonPhrase, string apiResponse)
        {
            ErrorResponse errorResponse = new ErrorResponse();

            try
            {
                if (reasonPhrase == ServiceConstant.UNAUTHORIZED)
                {
                    errorResponse.ErrorMessage = ServiceConstant.UNAUTHORIZED;
                }
                else
                {
                    errorResponse = SetErrorResponse(apiResponse);
                }
            }
            catch (Exception ex)
            {
                errorResponse.ErrorMessage = ex.Message;
            }

            return errorResponse;
        }

        /// <summary>
        /// SetErrorResponse
        /// </summary>
        /// <param name="apiResponse">apiResponse</param>
        /// <returns>errorResponse</returns>
        public ErrorResponse SetErrorResponse(string apiResponse)
        {
            ErrorResponse errorResponse = new ErrorResponse();
            try
            {
                if (!string.IsNullOrEmpty(apiResponse) && apiResponse.ToLower() == ServiceConstant.CSRFFAILEDMESSAGE)
                {
                    errorResponse.ErrorMessage = ServiceConstant.FORBIDDEN;
                    return errorResponse;
                }
                ErrorDetails errResp = JsonConvert.DeserializeObject<ErrorDetails>(apiResponse);
                if (errResp != null && errResp.error != null)
                {
                    var errMsg = GetErrorMessageByCode(errResp.error?.code?.Replace("/", "-"), ServiceConstant.SITECORE_FIELD_DEFAULTERRORMAPPING);
                    if (!String.IsNullOrEmpty(errMsg))
                    {
                        errorResponse.ErrorMessage = errMsg;
                    }
                    else
                    {
                        errorResponse.ErrorMessage = errResp.error?.message?.value;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Data.ID siteSettings = new Sitecore.Data.ID(ServiceConstant.SITECORE_ID_SITESETTINGS);
                if (!siteSettings.IsNull)
                {
                    Item sItem = Sitecore.Context.Database.GetItem(siteSettings);
                    Sitecore.Data.Fields.CheckboxField enableErrorLog = sItem?.Fields[ServiceConstant.SITECORE_FIELD_ENABLELOG];
                    if (enableErrorLog != null && enableErrorLog.Checked)
                    {
                        Log.Error($"Error in SetErrorResponse deserialization {ex.Message}: Response: {apiResponse}", this);
                    }
                }
                string customErrorMessage = GetErrorMessageByCode(ServiceConstant.ERRORCODE_APPERR);
                errorResponse.ErrorMessage = customErrorMessage;
            }
            return errorResponse;
        }

        /// <summary>
        /// SetErrorResponse
        /// </summary>
        /// <param name="apiResponse">apiResponse</param>
        /// <returns>errorResponse</returns>
        public ErrorResponse SetCustomErrorResponse(string reasonPhrase)
        {
            ErrorResponse errorResponse = new ErrorResponse();

            try
            {
                var apiResponse = new ErrorDetails();
                var errResp = JsonConvert.DeserializeObject<ErrorDetails>("");

                if (errResp != null && errResp.error != null)
                {
                    errorResponse.ErrorMessage = errResp.error.message.value;
                }
            }
            catch (Exception ex)
            {
                errorResponse.ErrorMessage = ex.Message;
            }

            return errorResponse;
        }

        #endregion

        #region 7. Send and validate otp.

        /// <summary>
        /// Send otp and validate otp
        /// </summary>
        /// <param name="otpValidateRequest">otpValidateRequest</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>otpValidateResponse</returns>
        public async Task<ODataApiResponse<OTPValidateResponse>> SendAndValidateOTP(OTPValidateRequest otpValidateRequest, string otp, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<OTPValidateResponse> otpValidateResponse = new ODataApiResponse<OTPValidateResponse>(new OTPValidateResponse());

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.REGISTERATION_BASEURL_ZUMC_ERP_UTILITIES_UMC_SRV;
                serviceRequest.Endpoint = ServiceConstant.FIXEDBILL_SENDOTPSETENDPOINT + "(" + ServiceConstant.FIXEDBILL_ACCOUNTIDPARAMETER + "='" + otpValidateRequest.AccountId + "'," + ServiceConstant.FIXEDBILL_MOBILENUMBERPARAMETER + "='" + otpValidateRequest.MobileNumber + "'," + ServiceConstant.FIXEDBILL_OTPNUMBERPARAMETER + "='" + otp + "'," + ServiceConstant.FIXEDBILL_EMAILIDPARAMETER + "='" + otpValidateRequest.EmailId + "'," + ServiceConstant.REGISTERATION_PROCESSTYPEPARAMETER + "='" + otpValidateRequest.ProcessType + "')" + "?" + ServiceConstant.FILTER_JSONFORMATTER;
                serviceRequest.RequestHeaders = GetRequestHeaders(httpRequestBase);


                string apiResponse = string.Empty;
                if (!ServiceConstant.ISMOCKTESTDATA)
                {
                    HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                    apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                        {
                            otpValidateResponse.Error.ErrorMessage = ServiceConstant.UNAUTHORIZED;
                        }
                        else
                        {
                            var objResp = JsonConvert.DeserializeObject<OTPValidate>(apiResponse);

                            if (objResp != null && objResp.OTPValidateData != null)
                            {
                                otpValidateResponse.ResponseData.ProcessType = objResp.OTPValidateData.ProcessType;
                                otpValidateResponse.ResponseData.EmailId = objResp.OTPValidateData.EmailId;
                                otpValidateResponse.ResponseData.AccountId = objResp.OTPValidateData.AccountId;
                                otpValidateResponse.ResponseData.MobileNumber = objResp.OTPValidateData.MobileNumber;
                                otpValidateResponse.ResponseData.OTPValidated = objResp.OTPValidateData.OTPValidated;
                                if (!string.IsNullOrEmpty(otp) && (bool)!otpValidateResponse.ResponseData.OTPValidated)
                                {
                                    otpValidateResponse.Error.ErrorMessage = GetErrorMessageByCode(ServiceConstant.ERRORCODE_OTPMISMATCH);

                                }
                            }
                            else
                            {
                                otpValidateResponse.Error = SetErrorResponse(apiResponse);
                            }
                        }
                    }
                    else
                    {
                        otpValidateResponse.Error = SetErrorResponse(apiResponse);
                    }
                }
                else
                {
                    //Hardcoded string response
                    apiResponse = SendandValidateOTPStringResponse();
                }
            }
            catch (Exception ex)
            {
                otpValidateResponse.Error.ErrorMessage = ex.Message;
            }

            return otpValidateResponse;
        }

        public string SendandValidateOTPStringResponse()
        {
            string response = "{\"d\":{\"__metadata\":{\"id\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/SendOTPSet(ProcessType='',EmailId='',AccountId='7000747363',MobileNumber='0512121212',OTPNumber='000000')\",\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/SendOTPSet(ProcessType='',EmailId='',AccountId='7000747363',MobileNumber='0512121212',OTPNumber='000000')\",\"type\":\"ERP_UTILITIES_UMC.SendOTP\"},\"ProcessType\":\"\",\"EmailId\":\"\",\"AccountId\":\"7000747363\",\"MobileNumber\":\"0512121212\",\"OTPNumber\":\"000000\",\"OTPValidated\":true}}";
            return response;
        }

        #endregion

        #region 8. Fetch mobile number

        /// <summary>
        /// Get mobile number
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>mobileResponse</returns>
        public async Task<ODataApiResponse<MobileResponse>> GetMobileNumber(string accountNumber, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<MobileResponse> mobileResponse = new ODataApiResponse<MobileResponse>(new MobileResponse());

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.REGISTERATION_BASEURL_ZUMC_ERP_UTILITIES_UMC_SRV;
                serviceRequest.Endpoint = ServiceConstant.ACCOUNT_ACCOUNTSENDPOINT + "(" + "'" + accountNumber + "'" + ")?" + ServiceConstant.FIXEDBILL_EXPANDPARAMETER + "=" + ServiceConstant.FIXEDBILL_ACCOUNTADDRESSINDEPENDENTMOBILEPHONES;
                serviceRequest.RequestHeaders = GetRequestHeaders(httpRequestBase);


                string apiResponse = string.Empty;

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                    {
                        mobileResponse.Error.ErrorMessage = ServiceConstant.UNAUTHORIZED;
                    }
                    else
                    {
                        var objResp = JsonConvert.DeserializeObject<Mobile>(apiResponse);

                        if (objResp != null && objResp.MobileData != null)
                        {
                            GetAddressIndependentMobilePhonesResponse(mobileResponse, objResp);

                        }
                        else
                        {
                            mobileResponse.Error = SetErrorResponse(apiResponse);
                        }
                    }
                }
                else
                {
                    mobileResponse.Error = SetErrorResponse(apiResponse);
                }


            }
            catch (Exception ex)
            {
                mobileResponse.Error.ErrorMessage = ex.Message;
            }

            return mobileResponse;
        }

        private void GetAddressIndependentMobilePhonesResponse(ODataApiResponse<MobileResponse> mobileResponse, Mobile objResp)
        {
            mobileResponse.ResponseData.AccountTypeID = objResp.MobileData.AccountTypeID;
            mobileResponse.ResponseData.AccountID = objResp.MobileData.AccountID;
            mobileResponse.ResponseData.AccountTitleID = objResp.MobileData.AccountTitleID;
            mobileResponse.ResponseData.FirstName = objResp.MobileData.FirstName;
            mobileResponse.ResponseData.LastName = objResp.MobileData.LastName;
            mobileResponse.ResponseData.MiddleName = objResp.MobileData.MiddleName;
            mobileResponse.ResponseData.SecondName = objResp.MobileData.SecondName;
            mobileResponse.ResponseData.Sex = objResp.MobileData.Sex;
            mobileResponse.ResponseData.Name1 = objResp.MobileData.Name1;
            mobileResponse.ResponseData.Name2 = objResp.MobileData.Name2;
            mobileResponse.ResponseData.Name3 = objResp.MobileData.Name3;
            mobileResponse.ResponseData.Name4 = objResp.MobileData.Name4;
            mobileResponse.ResponseData.GroupName1 = objResp.MobileData.GroupName1;
            mobileResponse.ResponseData.GroupName2 = objResp.MobileData.GroupName2;
            mobileResponse.ResponseData.FullName = objResp.MobileData.FullName;
            mobileResponse.ResponseData.CorrespondenceLanguage = objResp.MobileData.CorrespondenceLanguage;
            mobileResponse.ResponseData.CorrespondenceLanguageISO = objResp.MobileData.CorrespondenceLanguageISO;
            mobileResponse.ResponseData.Language = objResp.MobileData.Language;
            mobileResponse.ResponseData.LanguageISO = objResp.MobileData.LanguageISO;
            mobileResponse.ResponseData.AccelerateFlag = objResp.MobileData.AccelerateFlag;
            mobileResponse.ResponseData.IsCAAvailable = objResp.MobileData.IsCAAvailable;
            mobileResponse.ResponseData.PassportNumber = objResp.MobileData.PassportNumber;
            mobileResponse.ResponseData.VATNumber = objResp.MobileData.VATNumber;
            mobileResponse.ResponseData.Nickname = objResp.MobileData.Nickname;
            mobileResponse.ResponseData.BPChangeFlag = objResp.MobileData.BPChangeFlag;
            mobileResponse.ResponseData.BPFullName = objResp.MobileData.BPFullName;
            mobileResponse.ResponseData.CRNUpdate = objResp.MobileData.CRNUpdate;
            mobileResponse.ResponseData.ProcessManual = objResp.MobileData.ProcessManual;
            mobileResponse.ResponseData.RayahInd = objResp.MobileData.RayahInd;
            mobileResponse.ResponseData.DOBFlag = objResp.MobileData.DOBFlag;
            mobileResponse.ResponseData.DOBUpdate = objResp.MobileData.DOBUpdate;
            mobileResponse.ResponseData.NationalityDesc = objResp.MobileData.NationalityDesc;
            mobileResponse.ResponseData.AddressFlag = objResp.MobileData.AddressFlag;
            mobileResponse.ResponseData.CustomerFioriFlag = objResp.MobileData.CustomerFioriFlag;
            mobileResponse.ResponseData.IDType = objResp.MobileData.IDType;
            mobileResponse.ResponseData.Nationality = objResp.MobileData.Nationality;
            mobileResponse.ResponseData.IDIssueDate = ConvertToDateTimeString(objResp.MobileData.IDIssueDate);
            mobileResponse.ResponseData.DOB = ConvertToDateTimeString(objResp.MobileData.DOB);
            mobileResponse.ResponseData.IDTypeDesc = objResp.MobileData.IDTypeDesc;
            mobileResponse.ResponseData.IDNumber = objResp.MobileData.IDNumber;

            //List of phone and mobile phone number
            if (objResp.MobileData.AccountAddressIndependentMobilePhones != null && objResp.MobileData.AccountAddressIndependentMobilePhones.MobileResults.Count > 0)
            {
                mobileResponse.ResponseData.AccountAddressIndependentMobilePhones.IndepenedentMobilePhonesResults = objResp.MobileData.AccountAddressIndependentMobilePhones.MobileResults.Select(x => new IndepenedentMobilePhonesResult()
                { PhoneNo = x.PhoneNo, StandardFlag = x.StandardFlag, DefaultFlag = x.DefaultFlag, PhoneType = x.PhoneType }).ToList();
                mobileResponse.ResponseData.MobilePhoneNumber = mobileResponse.ResponseData.AccountAddressIndependentMobilePhones.IndepenedentMobilePhonesResults
                    .Where(x => x.DefaultFlag == true).Select(y => y.PhoneNo).FirstOrDefault();
            }
        }


        /// <summary>
        /// Update CSRF Token
        /// </summary>
        /// <param name="">accountNumber</param>
        /// <returns>mobileResponse</returns>
        public async Task<string> FetchXCSRFToken(ServiceRequest serviceRequest)
        {
            ODataApiResponse<DefaultResponse> proxyResponse = new ODataApiResponse<DefaultResponse>(new DefaultResponse());
            string xcsrftoken = string.Empty;
            try
            {
                serviceRequest.BaseUrl = ServiceConstant.REGISTERATION_BASEURL_ZUMC_ERP_UTILITIES_UMC_SRV;
                serviceRequest.Endpoint = string.Empty;
                serviceRequest.RequestHeaders.XCSRFToken = "fetch";

                string apiResponse = string.Empty;

                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    HttpCookieService cookieService = new HttpCookieService();
                    xcsrftoken = SessionStorage.SECXCSRFToken = httpResponseMessage.Headers.GetValues("x-csrf-token")?.FirstOrDefault();
                    List<Cookie> responseCookies = cookieService.GetCookies(httpResponseMessage);
                    SessionStorage.CookieJar = responseCookies;
                }

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return xcsrftoken;
        }

        #region 1. Get account address independent emails.

        /// <summary>
        /// Get account address independent emails
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>accountIndependentEmailsResponse</returns>
        public async Task<ODataApiResponse<AccountIndependentEmailsResponse>> GetAccountAddressIndependentEmails(string accountNumber, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<AccountIndependentEmailsResponse> accountIndependentEmailsResponse = new ODataApiResponse<AccountIndependentEmailsResponse>(new AccountIndependentEmailsResponse());

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.REGISTERATION_BASEURL_ZUMC_ERP_UTILITIES_UMC_SRV;
                serviceRequest.Endpoint = ServiceConstant.ACCOUNT_ACCOUNTSENDPOINT + "(" + "'" + accountNumber + "'" + ")/" + ServiceConstant.FIXEDBILL_ACCOUNTADDRESSINDEPENDENTEMAILS;
                serviceRequest.RequestHeaders = GetRequestHeaders(httpRequestBase);


                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                    {
                        accountIndependentEmailsResponse.Error.ErrorMessage = ServiceConstant.UNAUTHORIZED;
                    }
                    else
                    {
                        var objResp = JsonConvert.DeserializeObject<AccountIndependentEmails>(apiResponse);

                        if (objResp != null && objResp.AccountIndependentEmailsData != null)
                        {
                            if (objResp.AccountIndependentEmailsData.AccountIndependentEmailsResults.Count > 0)
                            {
                                accountIndependentEmailsResponse.ResponseData = objResp.AccountIndependentEmailsData.AccountIndependentEmailsResults.Select(x => new AccountIndependentEmailsResponse
                                {
                                    AccountID = x.AccountID,
                                    Email = x.Email,
                                    HomeFlag = x.HomeFlag,
                                    SequenceNo = x.SequenceNo,
                                    StandardFlag = x.StandardFlag
                                }).FirstOrDefault();
                            }
                        }
                        else
                        {
                            accountIndependentEmailsResponse.Error = SetErrorResponse(apiResponse);
                        }
                    }
                }
                else
                {
                    accountIndependentEmailsResponse.Error = SetErrorResponse(apiResponse);
                }
            }
            catch (Exception ex)
            {
                accountIndependentEmailsResponse.Error.ErrorMessage = ex.Message;
            }

            return accountIndependentEmailsResponse;
        }

        #endregion

        #region 2. Get account address independent phones.

        /// <summary>
        /// Get account address independent phones
        /// </summary>
        /// <param name="accountNumber">accountNumber</param>
        /// <param name="httpRequestBase">httpRequestBase</param>
        /// <returns>accountIndependentPhonesResponse</returns>
        public async Task<ODataApiResponse<AccountIndependentPhonesResponse>> GetAccountAddressIndependentPhones(string accountNumber, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<AccountIndependentPhonesResponse> accountIndependentPhonesResponse = new ODataApiResponse<AccountIndependentPhonesResponse>(new AccountIndependentPhonesResponse());

            try
            {
                ServiceRequest serviceRequest = new ServiceRequest();
                serviceRequest.BaseUrl = ServiceConstant.REGISTERATION_BASEURL_ZUMC_ERP_UTILITIES_UMC_SRV;
                serviceRequest.Endpoint = ServiceConstant.ACCOUNT_ACCOUNTSENDPOINT + "(" + "'" + accountNumber + "'" + ")/" + ServiceConstant.FIXEDBILL_ACCOUNTADDRESSINDEPENDENTPHONES;
                serviceRequest.RequestHeaders = GetRequestHeaders(httpRequestBase);


                HttpResponseMessage httpResponseMessage = await clientApi.GetOdataApiResponseByPassingCookieContainer(serviceRequest);
                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.ReasonPhrase == ServiceConstant.UNAUTHORIZED)
                    {
                        accountIndependentPhonesResponse.Error.ErrorMessage = ServiceConstant.UNAUTHORIZED;
                    }
                    else
                    {
                        var objResp = JsonConvert.DeserializeObject<AccountIndependentPhones>(apiResponse);

                        if (objResp != null && objResp.AccountAddressIndependentMobilePhones != null)
                        {
                            if (objResp.AccountAddressIndependentMobilePhones.MobileResults.Count > 0)
                            {
                                accountIndependentPhonesResponse.ResponseData = objResp.AccountAddressIndependentMobilePhones.MobileResults.Where(x => x.HomeFlag == true).Select(y => new AccountIndependentPhonesResponse
                                {
                                    AccountID = y.AccountID,
                                    SequenceNo = y.SequenceNo,
                                    PhoneNo = y.PhoneNo,
                                    HomeFlag = y.HomeFlag,
                                    StandardFlag = y.StandardFlag,
                                    Extension = y.Extension,
                                    CompletePhoneNo = y.CompletePhoneNo,
                                    CountryID = y.CountryID,
                                    DefaultFlag = y.DefaultFlag,
                                    PhoneType = y.PhoneType
                                }).FirstOrDefault();
                            }
                        }
                        else
                        {
                            accountIndependentPhonesResponse.Error = SetErrorResponse(apiResponse);
                        }
                    }
                }
                else
                {
                    accountIndependentPhonesResponse.Error = SetErrorResponse(apiResponse);
                }
            }
            catch (Exception ex)
            {
                accountIndependentPhonesResponse.Error.ErrorMessage = ex.Message;
            }

            return accountIndependentPhonesResponse;
        }

        #endregion

        public string FetchMobileNumberStringResponse()
        {
            string response = "{\"d\":{\"__metadata\":{\"id\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')\",\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')\",\"type\":\"ERP_UTILITIES_UMC.Account\"},\"AccountTypeID\":\"1\",\"AccountID\":\"7000015302\",\"AccountTitleID\":\"\",\"FirstName\":\"خالد\",\"LastName\":\"العواجي\",\"MiddleName\":\"عساف\",\"SecondName\":\"محمد\",\"Sex\":\"2\",\"Name1\":\"\",\"Name2\":\"\",\"Name3\":\"\",\"Name4\":\"\",\"GroupName1\":\"\",\"GroupName2\":\"\",\"FullName\":\"KHALEDALAWAJI\",\"CorrespondenceLanguage\":\"A\",\"CorrespondenceLanguageISO\":\"AR\",\"Language\":\"A\",\"LanguageISO\":\"AR\",\"AccelerateFlag\":\"\",\"IsCAAvailable\":false,\"PassportNumber\":\"\",\"VATNumber\":\"\",\"Nickname\":\"\",\"BPChangeFlag\":false,\"BPFullName\":\"KHALEDASAFMOHAMMADALAWAJI\",\"CRNUpdate\":false,\"ProcessManual\":false,\"RayahInd\":\"1\",\"DOBFlag\":true,\"DOBUpdate\":false,\"NationalityDesc\":\"SaudiArabia\",\"AddressFlag\":false,\"CustomerFioriFlag\":\"\",\"IDType\":\"ZNID\",\"Nationality\":\"SA\",\"IDIssueDate\":\"/Date(61948800000)/\",\"IDNumber\":\"1047852320\",\"DOB\":\"/Date(61862400000)/\",\"IDTypeDesc\":\"SaudiNationalID\",\"AccountContacts\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountContacts\"}},\"AccountType\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountType\"}},\"Outages\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/Outages\"}},\"AccountAlerts\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountAlerts\"}},\"StandardAccountAddress\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/StandardAccountAddress\"}},\"CommunicationPreferences\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/CommunicationPreferences\"}},\"Correspondences\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/Correspondences\"}},\"ServiceNotifications\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/ServiceNotifications\"}},\"ContractAccounts\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/ContractAccounts\"}},\"AccountAddresses\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountAddresses\"}},\"AccountRelationships\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountRelationships\"}},\"AccountBalance\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountBalance\"}},\"PaymentCards\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/PaymentCards\"}},\"ServiceOrders\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/ServiceOrders\"}},\"AccountAddressIndependentEmails\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountAddressIndependentEmails\"}},\"AccountAddressIndependentMobilePhones\":{\"results\":[{\"__metadata\":{\"id\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/AccountAddressIndependentMobilePhones(AccountID='7000015302',SequenceNo='001')\",\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/AccountAddressIndependentMobilePhones(AccountID='7000015302',SequenceNo='001')\",\"type\":\"ERP_UTILITIES_UMC.AccountAddressIndependentMobilePhone\"},\"AccountID\":\"7000015302\",\"SequenceNo\":\"001\",\"PhoneNo\":\"0500333911\",\"HomeFlag\":true,\"StandardFlag\":true,\"Extension\":\"\",\"CompletePhoneNo\":\"0500333911\",\"CountryID\":\"\",\"DefaultFlag\":true,\"PhoneType\":\"3\"}]},\"AccountAddressIndependentPhones\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountAddressIndependentPhones\"}},\"AccountAddressIndependentFaxes\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountAddressIndependentFaxes\"}},\"PaymentDocuments\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/PaymentDocuments\"}},\"Invoices\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/Invoices\"}},\"BankAccounts\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/BankAccounts\"}},\"AccountSex\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountSex\"}},\"AccountTitle\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountTitle\"}},\"UDSRequestSet\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/UDSRequestSet\"}},\"AccountOverviews\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountOverviews\"}},\"ContactPersons\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/ContactPersons\"}},\"AccountAttachment\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountAttachment\"}},\"AccountBPBalance\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AccountBPBalance\"}},\"AddressSearch\":{\"__deferred\":{\"uri\":\"http://e-dhq-bglcs.sec.se.com.sa:8021/sap/opu/odata/sap/ZUMC_ERP_UTILITIES_UMC_SRV/Accounts('7000015302')/AddressSearch\"}}}}";
            return response;
        }

        #endregion


        #region 10. Error Key mapping
        /// <summary>
        /// Error key mapping
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public string GetErrorMessageByCode(string errorCode, string fieldName = "")
        {
            string errorMessage = string.Empty;
            Dictionary<string, string> errorCodes = null;
            if (!string.IsNullOrEmpty(errorCode))
            {
                errorCodes = new Dictionary<string, string>();
                errorCodes = Sitecore.Context.Language.ToString() == "en" ? (Dictionary<string, string>)ServiceConstant.ErrorDictionary : (Dictionary<string, string>)ServiceConstant.ArErrorDictionary;

                var errorMessageValue = errorCodes.FirstOrDefault(x => x.Key == errorCode).Value;
                if (errorMessageValue != null)
                {
                    errorMessage = errorMessageValue;
                }
            }
            //Sitecore.Data.ID errorMappingSettings = new Sitecore.Data.ID(ServiceConstant.SITECORE_ID_ERRORMAPPING);
            //if (!errorMappingSettings.IsNull)
            //{
            //    Item sItem = Sitecore.Context.Database.GetItem(errorMappingSettings);
            //    if (sItem == null)
            //        return errorMessage;

            //    string errorMappingFieldName = string.Empty;
            //    if (fieldName != string.Empty)
            //    {
            //        errorMappingFieldName = fieldName;
            //    }
            //    else
            //    {
            //        errorMappingFieldName = ServiceConstant.SITECORE_FIELD_DEFAULTERRORMAPPING;
            //    }
            //    var errorMappingFieldValue = sItem.Fields[errorMappingFieldName]?.Value;
            //    if (!String.IsNullOrEmpty(errorMappingFieldValue))
            //    {
            //        var errorMappingList = GetKeyValuePairList(errorMappingFieldValue);
            //        if (errorMappingList != null)
            //        {
            //            var errorMeesageValue = errorMappingList.Find(s => s.Key == errorCode).Value;
            //            if (errorMeesageValue != null)
            //            {
            //                errorMessage = errorMeesageValue;
            //            }
            //        }
            //    }
            //}
            return errorMessage;

        }
        #endregion

        #region 10. EService Key mapping
        /// <summary>
        /// EService key mapping
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public string GetEServiceMappingsByCode(string mappingCode, string fieldName = "")
        {
            string mappingText = mappingCode;
            string eserviceMappingFieldName = string.Empty;
            if (fieldName != string.Empty)
            {
                eserviceMappingFieldName = fieldName;
            }
            else
            {
                eserviceMappingFieldName = ServiceConstant.SITECORE_FIELD_PROCESSTYPE;
            }
            Dictionary<string, string> mappingCodes = null;
            if (!string.IsNullOrEmpty(mappingCode))
            {
                string mappedValue = string.Empty;
                mappingCodes = new Dictionary<string, string>();
                if (eserviceMappingFieldName.Equals(ServiceConstant.SITECORE_FIELD_FIXEDBILLSTATUS))
                {
                    mappingCodes = Sitecore.Context.Language.ToString() == "en" ? (Dictionary<string, string>)ServiceConstant.FixedBillStatusTextDictionary : (Dictionary<string, string>)ServiceConstant.ArFixedBillStatusTextDictionary;
                    mappedValue = mappingCodes.FirstOrDefault(x => x.Key == mappingCode).Value;
                }
                else
                {
                    mappingCodes = Sitecore.Context.Language.ToString() == "en" ? (Dictionary<string, string>)ServiceConstant.EServiceKeyDictionary : (Dictionary<string, string>)ServiceConstant.ArEServiceKeyDictionary;
                    mappedValue = mappingCodes.FirstOrDefault(x => x.Key == mappingCode).Value;
                }

                if (mappedValue != null)
                {
                    mappingText = mappedValue;
                }
            }
            //Sitecore.Data.ID eserviceMappingSettings = new Sitecore.Data.ID(ServiceConstant.SITECORE_ID_ESERVICEMAPPING);
            //if (!eserviceMappingSettings.IsNull)
            //{
            //    Item sItem = Sitecore.Context.Database.GetItem(eserviceMappingSettings);
            //    if (sItem == null)
            //        return mappingText;

            //    string eserviceMappingFieldName = string.Empty;
            //    if (fieldName != string.Empty)
            //    {
            //        eserviceMappingFieldName = fieldName;
            //    }
            //    else
            //    {
            //        eserviceMappingFieldName = ServiceConstant.SITECORE_FIELD_PROCESSTYPE;
            //    }
            //    var eserviceMappingFieldValue = sItem.Fields[eserviceMappingFieldName]?.Value;
            //    if (!String.IsNullOrEmpty(eserviceMappingFieldValue))
            //    {
            //        var eserviceMappingList = GetKeyValuePairList(eserviceMappingFieldValue);
            //        if (eserviceMappingList != null)
            //        {
            //            mappingCode = mappingCode.Replace(" ", "").Trim();
            //            var mappedValue = eserviceMappingList.Find(s => s.Key == mappingCode).Value;
            //            if (mappedValue != null)
            //            {
            //                mappingText = mappedValue;
            //            }
            //        }
            //    }

            //}
            return mappingText;

        }
        #endregion

        #region 10.a. EService mapping list
        /// <summary>
        /// EService key mapping
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetEServiceMappingsList(string fieldName = "")
        {
            var eserviceMappingList = new List<KeyValuePair<string, string>>();
            string eserviceMappingFieldName = string.Empty;
            if (fieldName != string.Empty)
            {
                eserviceMappingFieldName = fieldName;
            }
            else
            {
                eserviceMappingFieldName = ServiceConstant.SITECORE_FIELD_PROCESSTYPE;
            }
            Dictionary<string, string> mappingCodes = null;
            mappingCodes = new Dictionary<string, string>();
            if (eserviceMappingFieldName.Equals(ServiceConstant.SITECORE_FIELD_FIXEDBILLSTATUS))
            {
                mappingCodes = Sitecore.Context.Language.ToString() == "en" ? (Dictionary<string, string>)ServiceConstant.FixedBillStatusTextDictionary : (Dictionary<string, string>)ServiceConstant.ArFixedBillStatusTextDictionary;
                eserviceMappingList = mappingCodes.ToList<KeyValuePair<string, string>>();
            }
            else
            {
                mappingCodes = Sitecore.Context.Language.ToString() == "en" ? (Dictionary<string, string>)ServiceConstant.EServiceKeyDictionary : (Dictionary<string, string>)ServiceConstant.ArEServiceKeyDictionary;
                eserviceMappingList = mappingCodes.ToList<KeyValuePair<string, string>>();
            }
            return eserviceMappingList;
            //var _eserviceMappingSettings = eserviceMappingSettings;
            //var eserviceMappingList = new List<KeyValuePair<string, string>>();
            //if (!eserviceMappingSettings.IsNull)
            //{
            //    Item sItem = Sitecore.Context.Database.GetItem(_eserviceMappingSettings);
            //    if (sItem == null)
            //        return new List<KeyValuePair<string, string>>();

            //    string eserviceMappingFieldName = string.Empty;
            //    if (fieldName != string.Empty)
            //    {
            //        eserviceMappingFieldName = fieldName;
            //    }
            //    else
            //    {
            //        eserviceMappingFieldName = ServiceConstant.SITECORE_FIELD_PROCESSTYPE;
            //    }
            //    var eserviceMappingFieldValue = sItem.Fields[eserviceMappingFieldName]?.Value;
            //    if (!String.IsNullOrEmpty(eserviceMappingFieldValue))
            //    {
            //        eserviceMappingList = GetKeyValuePairList(eserviceMappingFieldValue);
            //    }
            //}
            //return eserviceMappingList;
        }
        #endregion

        #region 11. Decrypt String

        /// <summary>
        /// Decrypt encrypted String
        /// </summary>
        /// <param name="cipherText">cipherText</param>
        /// <returns>string</returns>
        public string DecryptString(string cipherText, bool IsEncrypt)
        {
            if (!string.IsNullOrEmpty(cipherText) && IsEncrypt)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(ServiceConstant.ENCRYPTIONKEY);
                byte[] iv = Encoding.UTF8.GetBytes(ServiceConstant.ENCRYPTIONKEY); //new byte[16];
                cipherText = cipherText.Replace(" ", "+");
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                return cipherText;
            }
        }

        #endregion

        #region 12. Encrypt String

        /// <summary>
        /// Encrypt encrypted String
        /// </summary>
        /// <param name="plainText">plainText</param>
        /// <returns>string</returns>
        public string EncryptString(string plainText, bool IsEncrypt)
        {
            if (!string.IsNullOrEmpty(plainText) && IsEncrypt)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(ServiceConstant.ENCRYPTIONKEY);
                byte[] iv = Encoding.UTF8.GetBytes(ServiceConstant.ENCRYPTIONKEY); // Ensure this is 16 bytes for AES
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = iv;
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                        }
                        // Return the encrypted string as Base64 encoded text
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            else
            {
                return plainText;
            }
        }

        #endregion

        #endregion
    }

    public sealed class EmptyStringModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;
            return base.BindModel(controllerContext, bindingContext);
        }
    }

    public class DefaultResponse
    {
        private bool IsSuccess { get; set; }
    }
}