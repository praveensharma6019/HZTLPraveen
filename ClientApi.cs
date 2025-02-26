using DCX.Foundation.ElectricalRequestServices;
using DCX.Foundation.EServices.ServiceClient.Constant;
using DCX.Foundation.EServices.ServiceClient.Models;
using DCX.Foundation.Logging.IService;
using DCX.Foundation.SessionManager;
using DCX.Foundation.SessionManager.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DCX.Foundation.EServices.ServiceClient.Client
{
    public class ClientApi
    {

        #region -- Methods --


        /// <summary>
        /// Get odata api responses
        /// </summary>
        /// <returns>apiResponse</returns>
        public async Task<HttpResponseMessage> GetOdataApiResponse(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, null))
                {
                    httpResponseMessage = await client.GetAsync(serviceRequest.Endpoint);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Get odata api responses
        /// </summary>
        /// <returns>apiResponse</returns>
        public async Task<HttpResponseMessage> GetOdataApiResponseByPassingCookieContainer(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {

                var cookieContainer = SessionStorage.CookieJar;
                CookieContainer _cookieJar = new CookieContainer();
                if (cookieContainer != null)
                {
                    foreach (Cookie cookie in cookieContainer)
                    {
                        Cookie seccookie3 = new Cookie()
                        {
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Domain = cookie.Domain

                        };
                        _cookieJar.Add(seccookie3);

                    }
                }

                serviceRequest.RequestHeaders.XCSRFToken = "fetch";
                using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                {
                    client.Timeout = new TimeSpan(0, 1, 50);
                    httpResponseMessage = await client.GetAsync(serviceRequest.Endpoint);



                }

                var apiResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    HttpHeaders headers = httpResponseMessage.Headers;
                    IEnumerable<string> values;
                    if (headers.TryGetValues("x-csrf-token", out values))
                    {
                        SessionStorage.SECXCSRFToken = values.First();
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("Endpoint URL:" + serviceRequest.Endpoint + ex.Message + " Reason Phrase:" + httpResponseMessage.ReasonPhrase, this);

                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Post odata api responses
        /// </summary>
        /// <returns>apiResponse</returns>
        public async Task<HttpResponseMessage> PostOdataApiResponseByPassingCookieContainer(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {


                var cookieContainer = SessionStorage.CookieJar;
                CookieContainer _cookieJar = new CookieContainer();
                if (cookieContainer != null)
                {
                    foreach (Cookie cookie in cookieContainer)
                    {
                        Cookie seccookie3 = new Cookie()
                        {
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Domain = cookie.Domain
                        };
                        _cookieJar.Add(seccookie3);
                    }
                }


                using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                {
                    httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, null);
                }

                //Re-fetch token and update if expired
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.FORBIDDEN)
                {
                    var clientCommon = new ClientCommon();
                    ServiceRequest _serviceRequest = new ServiceRequest();
                    _serviceRequest.RequestHeaders = serviceRequest.RequestHeaders;
                    string xcsrftoken = await clientCommon.FetchXCSRFToken(_serviceRequest);
                    cookieContainer = SessionStorage.CookieJar;
                    _cookieJar = new CookieContainer();
                    if (cookieContainer != null)
                    {
                        foreach (Cookie cookie in cookieContainer)
                        {
                            Cookie seccookie3 = new Cookie()
                            {
                                Name = cookie.Name,
                                Value = cookie.Value,
                                Domain = cookie.Domain
                            };
                            _cookieJar.Add(seccookie3);
                        }
                    }

                    if (!String.IsNullOrEmpty(xcsrftoken))
                    {
                        serviceRequest.RequestHeaders.XCSRFToken = xcsrftoken;
                        using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                        using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                        {
                            httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Post odata api with cookie container response
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns>apiResponse</returns>
        public async Task<HttpResponseMessage> PostODataApiWithCookieResponse(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {


                var cookieContainer = SessionStorage.CookieJar;
                CookieContainer _cookieJar = new CookieContainer();
                if (cookieContainer != null)
                {
                    foreach (Cookie cookie in cookieContainer)
                    {
                        Cookie seccookie3 = new Cookie()
                        {
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Domain = cookie.Domain
                        };

                        _cookieJar.Add(seccookie3);
                    }
                }

                using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                {
                    if (!string.IsNullOrWhiteSpace(serviceRequest.RequestData))
                    {
                        HttpContent stringContent = new StringContent(serviceRequest.RequestData, Encoding.UTF8, ServiceConstant.APPLICATIONJSON);
                        httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, stringContent);
                    }
                    else
                    {
                        httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, null);
                    }
                }

                //Re-fetch token and update if expired
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.FORBIDDEN)
                {
                    var clientCommon = new ClientCommon();
                    ServiceRequest _serviceRequest = new ServiceRequest();
                    _serviceRequest.RequestHeaders = serviceRequest.RequestHeaders;
                    string xcsrftoken = await clientCommon.FetchXCSRFToken(_serviceRequest);
                    cookieContainer = SessionStorage.CookieJar;
                    _cookieJar = new CookieContainer();
                    if (cookieContainer != null)
                    {
                        foreach (Cookie cookie in cookieContainer)
                        {
                            Cookie seccookie3 = new Cookie()
                            {
                                Name = cookie.Name,
                                Value = cookie.Value,
                                Domain = cookie.Domain
                            };
                            _cookieJar.Add(seccookie3);
                        }
                    }

                    if (!String.IsNullOrEmpty(xcsrftoken))
                    {
                        serviceRequest.RequestHeaders.XCSRFToken = xcsrftoken;
                        using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                        using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                        {
                            if (!string.IsNullOrWhiteSpace(serviceRequest.RequestData))
                            {
                                HttpContent stringContent = new StringContent(serviceRequest.RequestData, Encoding.UTF8, ServiceConstant.APPLICATIONJSON);
                                httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, stringContent);
                            }
                            else
                            {
                                httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }


        /// <summary>
        /// Post odata api response
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns>apiResponse</returns>
        public async Task<HttpResponseMessage> PostODataApiResponse(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            EsrLoggerService _logger = new EsrLoggerService();

            try
            {
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, null))
                {
                    if (!string.IsNullOrWhiteSpace(serviceRequest.RequestData))
                    {
                        HttpContent stringContent = new StringContent(serviceRequest.RequestData, Encoding.UTF8, ServiceConstant.APPLICATIONJSON);

                        httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, stringContent);


                    }
                    else
                    {

                        httpResponseMessage = await client.PostAsync(serviceRequest.Endpoint, null);


                    }
                }
            }
            catch (Exception ex)
            {
                _logger._log.Error("314" + ex.Message, ex);
                httpResponseMessage.ReasonPhrase = ex.Message;

            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Put http service client
        /// </summary>
        /// <param name="baseURL"></param>
        /// <param name="endpoint"></param>
        /// <param name="request"></param>
        /// <param name="requestHeaders"></param>
        /// <returns>ServiceResponse</returns>
        public async Task<HttpResponseMessage> PutODataApiResponse(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {
                var cookieContainer = SessionStorage.CookieJar;
                CookieContainer _cookieJar = new CookieContainer();
                if (cookieContainer != null)
                {
                    foreach (Cookie cookie in cookieContainer)
                    {
                        Cookie seccookie3 = new Cookie()
                        {
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Domain = cookie.Domain
                        };
                        _cookieJar.Add(seccookie3);
                    }
                }

                using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                {
                    if (!string.IsNullOrWhiteSpace(serviceRequest.RequestData))
                    {
                        HttpContent stringContent = new StringContent(serviceRequest.RequestData, Encoding.UTF8, ServiceConstant.APPLICATIONJSON);//It should application-json
                        httpResponseMessage = await client.PutAsync(serviceRequest.Endpoint, stringContent);
                    }
                    else
                    {
                        httpResponseMessage = await client.PutAsync(serviceRequest.Endpoint, null);
                    }
                }

                //Re-fetch token and update if expired
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.FORBIDDEN)
                {
                    var clientCommon = new ClientCommon();
                    ServiceRequest _serviceRequest = new ServiceRequest();
                    _serviceRequest.RequestHeaders = serviceRequest.RequestHeaders;
                    string xcsrftoken = await clientCommon.FetchXCSRFToken(_serviceRequest);

                    cookieContainer = SessionStorage.CookieJar;
                    _cookieJar = new CookieContainer();
                    if (cookieContainer != null)
                    {
                        foreach (Cookie cookie in cookieContainer)
                        {
                            Cookie seccookie3 = new Cookie()
                            {
                                Name = cookie.Name,
                                Value = cookie.Value,
                                Domain = cookie.Domain
                            };
                            _cookieJar.Add(seccookie3);
                        }
                    }

                    if (!String.IsNullOrEmpty(xcsrftoken))
                    {
                        serviceRequest.RequestHeaders.XCSRFToken = xcsrftoken;
                        using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                        using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                        {
                            if (!string.IsNullOrWhiteSpace(serviceRequest.RequestData))
                            {
                                HttpContent stringContent = new StringContent(serviceRequest.RequestData, Encoding.UTF8, serviceRequest.RequestHeaders.ContentType);//It should application-json
                                httpResponseMessage = await client.PutAsync(serviceRequest.Endpoint, stringContent);
                            }
                            else
                            {
                                httpResponseMessage = await client.PutAsync(serviceRequest.Endpoint, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Delete http service client
        /// </summary>
        /// <param name="baseURL"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteODataApiResponse(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, null))
                {
                    httpResponseMessage = await client.DeleteAsync(serviceRequest.Endpoint);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Delete http service client api with cookie container response
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns>apiResponse</returns>
        public async Task<HttpResponseMessage> DeleteODataApiWithCookieResponse(ServiceRequest serviceRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {


                var cookieContainer = SessionStorage.CookieJar;
                CookieContainer _cookieJar = new CookieContainer();
                if (cookieContainer != null)
                {
                    foreach (Cookie cookie in cookieContainer)
                    {
                        Cookie seccookie3 = new Cookie()
                        {
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Domain = cookie.Domain
                        };
                        _cookieJar.Add(seccookie3);
                    }
                }

                using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                {
                    httpResponseMessage = await client.DeleteAsync(serviceRequest.Endpoint);
                }

                //Re-fetch token and update if expired
                if (httpResponseMessage.ReasonPhrase == ServiceConstant.FORBIDDEN)
                {
                    var clientCommon = new ClientCommon();
                    ServiceRequest _serviceRequest = new ServiceRequest();
                    _serviceRequest.RequestHeaders = serviceRequest.RequestHeaders;
                    string xcsrftoken = await clientCommon.FetchXCSRFToken(_serviceRequest);

                    cookieContainer = SessionStorage.CookieJar;
                    _cookieJar = new CookieContainer();
                    if (cookieContainer != null)
                    {
                        foreach (Cookie cookie in cookieContainer)
                        {
                            Cookie seccookie3 = new Cookie()
                            {
                                Name = cookie.Name,
                                Value = cookie.Value,
                                Domain = cookie.Domain
                            };
                            _cookieJar.Add(seccookie3);
                        }
                    }

                    if (!String.IsNullOrEmpty(xcsrftoken))
                    {
                        serviceRequest.RequestHeaders.XCSRFToken = xcsrftoken;
                        using (var handler = new HttpClientHandler { CookieContainer = _cookieJar })
                        using (HttpClient client = SetHTTPClientRequest(serviceRequest, handler))
                        {
                            httpResponseMessage = await client.DeleteAsync(serviceRequest.Endpoint);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                httpResponseMessage.ReasonPhrase = ex.Message;
            }

            return httpResponseMessage;
        }
        #endregion

        #region -- Helper methods --

        public HttpClient SetHTTPClientRequest(ServiceRequest serviceRequest, HttpClientHandler httpClientHandler)
        {
            HttpClient client = httpClientHandler != null ? new HttpClient(httpClientHandler) : new HttpClient();

            try
            {
                string contentType = string.Empty;

                if (serviceRequest.RequestHeaders.ContentType.Contains("charset=utf-8"))
                {
                    contentType = serviceRequest.RequestHeaders.ContentType.Split(';')[0].ToString();
                }
                else
                    contentType = serviceRequest.RequestHeaders.ContentType;

                if (!string.IsNullOrWhiteSpace(contentType))
                    client.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.AcceptLanguage))
                    client.DefaultRequestHeaders
                        .Add(ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE, serviceRequest.RequestHeaders.AcceptLanguage);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.XRequestedWith))
                    client.DefaultRequestHeaders
                        .Add(ServiceConstant.REQUESTHEADERS_KEY_XREQUESTEDWITH, serviceRequest.RequestHeaders.XRequestedWith);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.XCSRFToken))
                    client.DefaultRequestHeaders
                        .Add(ServiceConstant.REQUESTHEADERS_KEY_XCSRFTOKEN, serviceRequest.RequestHeaders.XCSRFToken);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.Authorization))
                    client.DefaultRequestHeaders.Add(ServiceConstant.REQUESTHEADERS_KEY_AUTHORIZATION, serviceRequest.RequestHeaders.Authorization);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.Accept))
                    client.DefaultRequestHeaders.Add(ServiceConstant.REQUESTHEADERS_KEY_ACCEPT, serviceRequest.RequestHeaders.Accept);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.XForwardedFor))
                    client.DefaultRequestHeaders.Add(ServiceConstant.REQUESTHEADERS_KEY_XFORWARDEDFOR, serviceRequest.RequestHeaders.XForwardedFor);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.RequestFrom))
                    client.DefaultRequestHeaders.Add(ServiceConstant.REQUESTHEADERS_KEY_REQUESTFROM, serviceRequest.RequestHeaders.RequestFrom);

                if (!string.IsNullOrWhiteSpace(serviceRequest.RequestHeaders.XAPIKey))
                    client.DefaultRequestHeaders.Add(ServiceConstant.REQUESTHEADERS_KEY_XAPIKey, serviceRequest.RequestHeaders.XAPIKey);

                client.BaseAddress = new Uri(serviceRequest.BaseUrl);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                client = null;
            }

            return client;
        }

        #endregion

    }
}