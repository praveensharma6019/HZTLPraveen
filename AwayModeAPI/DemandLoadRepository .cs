using DCX.Feature.EServices.AccountServices.Models.DemandLoad;
using DCX.Feature.EServices.Common.Models;
using DCX.Feature.EServices.DemandLoad.IRepository;
using DCX.Feature.EServices.DemandLoad.Logging;
using DCX.Feature.EServices.DemandLoad.Models.DemandLoad;
using DCX.Feature.EServices.DemandLoad.Service;
using DCX.Foundation.EServices.ServiceClient;
using DCX.Foundation.EServices.ServiceClient.Constant;
using DCX.Foundation.EServices.ServiceClient.Models;
using DCX.Foundation.Extension.Constant;
using DCX.Foundation.Logging.IService;
using Microsoft.Ajax.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.HtmlControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DCX.Feature.EServices.DemandLoad.Repository
{
    public class DemandLoadRepository : IDemandLoadRepository
    {
        #region -- Properties --

        private readonly DemandLoadService demandLoadService;
        private ILoggerServices demandLoadLoggerService;
        private readonly ClientCommon clientCommon;

        #endregion

        #region -- Constructors --

        public DemandLoadRepository()
        {
            demandLoadService = new DemandLoadService();
            demandLoadLoggerService = new DemandLoadLoggerService();
            clientCommon = new ClientCommon();
        }

        #endregion

        public async Task<GetUnbilledAmountResponse> GetUnbilledAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetUnbilledAmountResponse getUnbilledAmountResponse = new GetUnbilledAmountResponse();

            try
            {
                getUnbilledAmountResponse = await demandLoadService.GetUnbilledAmount(contractAccountNumber, httpRequestBase);
            }
            catch (Exception ex)
            {
                getUnbilledAmountResponse.Error.ErrorMessage = ex.Message;
            }

            return getUnbilledAmountResponse;
        }

        public async Task<GetPredictedBillResponse> GetPredictedBillAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetPredictedBillResponse getPredictedBillAmount = new GetPredictedBillResponse();

            try
            {
                getPredictedBillAmount = await demandLoadService.GetPredictedBillAmount(contractAccountNumber, httpRequestBase);
            }
            catch (Exception ex)
            {
                getPredictedBillAmount.Error.ErrorMessage = ex.Message;
            }

            return getPredictedBillAmount;
        }

        private DemandLoadContentDetails GetDemandLoadContentDetails(HttpRequestBase httpRequestBase, string headerlanguage)
        {
            DemandLoadContentDetails demandLoadContentDetails = new DemandLoadContentDetails();

            try
            {
                //Content details
                Database webDb = Sitecore.Configuration.Factory.GetDatabase(ServiceConstant.SITECORE_WEB_DATABASE);
                Sitecore.Globalization.Language itemlanguage = Sitecore.Data.Managers.LanguageManager.GetLanguage(headerlanguage);
                Sitecore.Data.ID demandLoadContentItem = new Sitecore.Data.ID(ServiceConstant.DemandLoadItemDetails);
                if (!demandLoadContentItem.IsNull)
                {
                    using (new LanguageSwitcher(itemlanguage))
                    {
                        Item demandLoadItemDetails = webDb.GetItem(demandLoadContentItem);
                        if (demandLoadItemDetails != null)
                        {
                            demandLoadContentDetails.DemandLoadText = demandLoadItemDetails.Fields[ServiceConstant.DemandLoadTextField]?.Value;
                            demandLoadContentDetails.DemandYourMeterText = demandLoadItemDetails.Fields[ServiceConstant.DemandYourMeterTextField]?.Value;
                            demandLoadContentDetails.NoSmartMeterMessageText = demandLoadItemDetails.Fields[ServiceConstant.NoSmartMeterMessageField]?.Value;
                            demandLoadContentDetails.LatestDayText = demandLoadItemDetails.Fields[ServiceConstant.LatestDayTextField]?.Value;
                            demandLoadContentDetails.Latest7DaysText = demandLoadItemDetails.Fields[ServiceConstant.Latest7DaysTextField]?.Value;
                            demandLoadContentDetails.SelectedRangeText = demandLoadItemDetails.Fields[ServiceConstant.SelectedRangeTextField]?.Value;
                            demandLoadContentDetails.SelectRangeText = demandLoadItemDetails.Fields[ServiceConstant.SelectRangeTextField]?.Value;
                            demandLoadContentDetails.HighestLoadText = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadTextField]?.Value;
                            demandLoadContentDetails.CustomDateRangeText = demandLoadItemDetails.Fields[ServiceConstant.CustomDateRangeTextField]?.Value;
                            demandLoadContentDetails.AllText = demandLoadItemDetails.Fields[ServiceConstant.AllTextField]?.Value;
                            demandLoadContentDetails.HighText = demandLoadItemDetails.Fields[ServiceConstant.HighTextField]?.Value;
                            demandLoadContentDetails.LowText = demandLoadItemDetails.Fields[ServiceConstant.LowTextField]?.Value;
                            demandLoadContentDetails.MediumText = demandLoadItemDetails.Fields[ServiceConstant.MediumTextField]?.Value;
                            demandLoadContentDetails.HighestText = demandLoadItemDetails.Fields[ServiceConstant.HighestTextField]?.Value;
                            demandLoadContentDetails.LowestText = demandLoadItemDetails.Fields[ServiceConstant.LowestTextField]?.Value;
                            demandLoadContentDetails.BreakerCapacityText = demandLoadItemDetails.Fields[ServiceConstant.BreakerCapacityTextField]?.Value;
                            demandLoadContentDetails.ViewBreakdownText = demandLoadItemDetails.Fields[ServiceConstant.ViewBreakdownTextField]?.Value;

                            demandLoadContentDetails.HighestLoadBTWText = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadBTWTextField]?.Value;
                            demandLoadContentDetails.DemandLoadErrorIcon = demandLoadItemDetails.Fields[ServiceConstant.DemandLoadErrorIconField]?.Value;
                            demandLoadContentDetails.DemandLoadErrorText = demandLoadItemDetails.Fields[ServiceConstant.DemandLoadErrorTextField]?.Value;
                            demandLoadContentDetails.DemandLoadErrorDescription = demandLoadItemDetails.Fields[ServiceConstant.DemandLoadErrorDescriptionField]?.Value;
                            demandLoadContentDetails.NoDataAvailableIcon = demandLoadItemDetails.Fields[ServiceConstant.NoDataAvailableIconField]?.Value;
                            demandLoadContentDetails.NoDataAvailableText = demandLoadItemDetails.Fields[ServiceConstant.NoDataAvailableTextField]?.Value;
                            demandLoadContentDetails.NoDataAvailableDescription = demandLoadItemDetails.Fields[ServiceConstant.NoDataAvailableDescriptionField]?.Value;
                            demandLoadContentDetails.DemandLoadBreakdownText = demandLoadItemDetails.Fields[ServiceConstant.DemandLoadBreakdownTextField]?.Value;
                            demandLoadContentDetails.HighToLowText = demandLoadItemDetails.Fields[ServiceConstant.HighToLowTextField]?.Value;
                            demandLoadContentDetails.LowToHighText = demandLoadItemDetails.Fields[ServiceConstant.LowToHighTextField]?.Value;
                            demandLoadContentDetails.DisplayByText = demandLoadItemDetails.Fields[ServiceConstant.DisplayByTextField]?.Value;
                            demandLoadContentDetails.TimeOfDayText = demandLoadItemDetails.Fields[ServiceConstant.TimeOfDayTextField]?.Value;
                            demandLoadContentDetails.CloseText = demandLoadItemDetails.Fields[ServiceConstant.CloseTextField]?.Value;
                            demandLoadContentDetails.TIMEText = demandLoadItemDetails.Fields[ServiceConstant.TIMETextField]?.Value;
                            demandLoadContentDetails.KWHText = demandLoadItemDetails.Fields[ServiceConstant.KWHTextField]?.Value;
                            demandLoadContentDetails.SelectIntervalText = demandLoadItemDetails.Fields[ServiceConstant.SelectIntervalTextField]?.Value;
                            demandLoadContentDetails.ShowResultsText = demandLoadItemDetails.Fields[ServiceConstant.ShowResultsTextField]?.Value;
                            demandLoadContentDetails.DateText = demandLoadItemDetails.Fields[ServiceConstant.DateTextField]?.Value;
                            demandLoadContentDetails.AMText = demandLoadItemDetails.Fields[ServiceConstant.AMTextField]?.Value;
                            demandLoadContentDetails.PMText = demandLoadItemDetails.Fields[ServiceConstant.PMTextField]?.Value;
                            demandLoadContentDetails.LineViewText = demandLoadItemDetails.Fields[ServiceConstant.LineViewTextField]?.Value;
                            demandLoadContentDetails.HalfHourAverageText = demandLoadItemDetails.Fields[ServiceConstant.HalfHourAverageTextField]?.Value;


                            demandLoadContentDetails.HighestLoadOnTextField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadOnTextField]?.Value;
                            demandLoadContentDetails.HighestLoadBetweenTextField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadBetweenTextField]?.Value;
                            demandLoadContentDetails.HighestLoadWasTextField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadWasTextField]?.Value;
                            demandLoadContentDetails.HighestLoadThanTextField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadThanTextField]?.Value;
                            demandLoadContentDetails.HighestLoadSameTextField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadSameTextField]?.Value;
                            demandLoadContentDetails.HighestLoadAsTextField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadAsTextField]?.Value;

                            demandLoadContentDetails.HighestLoadLessField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadLessField]?.Value;
                            demandLoadContentDetails.HighestLoadMoreField = demandLoadItemDetails.Fields[ServiceConstant.HighestLoadMoreField]?.Value;

                            demandLoadContentDetails.CurrentSelectionTextField = demandLoadItemDetails.Fields[ServiceConstant.CurrentSelectionTextField]?.Value;
                            demandLoadContentDetails.ClearSelectionTextField = demandLoadItemDetails.Fields[ServiceConstant.ClearSelectionTextField]?.Value;
                            demandLoadContentDetails.NoDataInSelectedDateTextField = demandLoadItemDetails.Fields[ServiceConstant.NoDataInSelectedDateTextField]?.Value;

                            Sitecore.Data.Fields.ImageField onLoadGraphImageField = demandLoadItemDetails.Fields[ServiceConstant.OnLoadGraphImageField];
                            if (onLoadGraphImageField != null && onLoadGraphImageField.MediaItem != null)
                            {
                                Sitecore.Data.Items.MediaItem imageItem = new MediaItem(onLoadGraphImageField?.MediaItem);
                                demandLoadContentDetails.OnLoadGraphImageField = Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageItem));
                            }
                            demandLoadContentDetails.NoAccountAvailableDescriptionField = demandLoadItemDetails.Fields[ServiceConstant.NoAccountAvailableDescriptionField]?.Value;
                            demandLoadContentDetails.NoAccountAvailableCTAField = demandLoadItemDetails.Fields[ServiceConstant.NoAccountAvailableCTAField]?.Value;
                            demandLoadContentDetails.TimePickerSelectIntervalTextField = demandLoadItemDetails.Fields[ServiceConstant.TimePickerSelectIntervalTextField]?.Value;

                            Sitecore.Data.Fields.MultilistField monthsNameOptions = demandLoadItemDetails.Fields[ServiceConstant.MonthsNameListField];
                            Sitecore.Data.Items.Item[] monthsNameitems = monthsNameOptions.GetItems();
                            foreach (Item monthsName in monthsNameitems)
                            {
                                if (monthsName != null)
                                {
                                    KeyValue monthsNameOption = new KeyValue();
                                    monthsNameOption.Key = monthsName.Fields[ServiceConstant.keyField]?.Value;
                                    monthsNameOption.Value = monthsName.Fields[ServiceConstant.ValueField]?.Value;
                                    demandLoadContentDetails.MonthsNameList.Add(monthsNameOption);
                                }
                            }

                            Sitecore.Data.Fields.MultilistField weekDaysNameOptions = demandLoadItemDetails.Fields[ServiceConstant.WeekDaysNameListField];
                            Sitecore.Data.Items.Item[] weekDaysNameitems = weekDaysNameOptions.GetItems();
                            foreach (Item weekDaysName in weekDaysNameitems)
                            {
                                if (weekDaysName != null)
                                {
                                    KeyValue weekdayOption = new KeyValue();
                                    weekdayOption.Key = weekDaysName.Fields[ServiceConstant.keyField]?.Value;
                                    weekdayOption.Value = weekDaysName.Fields[ServiceConstant.ValueField]?.Value;
                                    demandLoadContentDetails.WeekDaysNameList.Add(weekdayOption);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error("GetDemandLoadContentDetails", ex);
            }
            return demandLoadContentDetails;
        }

        private List<DemandLoadSet> GetDemandLoadSet(List<DemandLoadAvgHalfHourSetData> dataFromAPI)
        {
            List<DemandLoadSet> demandLoadSetArray = new List<DemandLoadSet>();
            TimeSpan timeSpan = new TimeSpan(0, 0, 0);
            TimeSpan newtimeSpan = timeSpan.Subtract(new TimeSpan(0, 30, 0));
            for (int i = 0; i < 48; i++)
            {
                DemandLoadSet objDemandLoadSet = new DemandLoadSet();
                objDemandLoadSet.IntervalStartTime = new DateTime().Add(timeSpan).ToString("HH:mm");
                timeSpan = timeSpan.Add(new TimeSpan(0, 30, 0));
                if (i == 47)
                    objDemandLoadSet.IntervalEndTime = "00:00";
                else
                    objDemandLoadSet.IntervalEndTime = new DateTime().Add(timeSpan).ToString("HH:mm");

                if (dataFromAPI.Where(l => l.IntervalEndTime == objDemandLoadSet.IntervalEndTime).FirstOrDefault() != null)
                {
                    objDemandLoadSet.IsDataAvailable = true;
                    objDemandLoadSet.Consumption = clientCommon.RoundUptoDecimalPlaces(dataFromAPI.Where(l => l.IntervalEndTime == objDemandLoadSet.IntervalEndTime).FirstOrDefault().AvarageValue, ServiceConstant.DecimalPlaces2);
                }

                demandLoadSetArray.Add(objDemandLoadSet);
            }
            return demandLoadSetArray;
        }

        public async Task<ODataApiResponse<GetDemandLoadSetResponse>> GetDemandLoadSetDetails(string contractAccountNumber, string startDate, string endDate, bool IsLatestDay, bool IsLatest7Day, HttpRequestBase httpRequestBase)
        {
            demandLoadLoggerService.Error("GetDemandLoadSetDetails startDate: " + startDate + " endDate: " + endDate + "IsLatestDay:" + IsLatestDay + " IsLatest7Day:" + IsLatest7Day);
            ODataApiResponse<GetDemandLoadSetResponse> LDDemandLoadResponse = new ODataApiResponse<GetDemandLoadSetResponse>(new GetDemandLoadSetResponse());
            GetDemandLoadSetResponse getDemandLoadSetResponse = new GetDemandLoadSetResponse();

            try
            {
                //get the header language to get content from sitecore
                var headerlanguage = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE].Trim().ToString() : string.Empty;
                if (string.IsNullOrEmpty(headerlanguage))
                {
                    headerlanguage = Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName.ToLower(CultureInfo.InvariantCulture);
                }

                CultureInfo cultureInfo = new CultureInfo(headerlanguage);
                if (headerlanguage.ToLowerInvariant().Equals(ExtensionConstant.ArabicLanguageCode.ToLower()))
                {
                    cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
                }

                //1. Fill the content
                getDemandLoadSetResponse.DemandLoadContentDetails = GetDemandLoadContentDetails(httpRequestBase, headerlanguage);

                //2. check parameters
                if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                {
                    LDDemandLoadResponse.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_NULLUNDEFINED);
                }
                else
                {
                    //Get Start Date and End Date
                    DateTime startDateCurrent;
                    DateTime endDateCurrent;
                    DateTime startDatePrevious;
                    DateTime endDatePrevious;
                    bool isSingleDay = false;

                    startDateCurrent = Convert.ToDateTime(startDate);
                    endDateCurrent = Convert.ToDateTime(endDate);
                    if (startDateCurrent == endDateCurrent)
                    {
                        if (!IsLatestDay)
                            isSingleDay = true;
                        startDatePrevious = startDateCurrent.AddDays(-1);
                        endDatePrevious = endDateCurrent.AddDays(-1);
                    }
                    else
                    {
                        double numberOfDaysDifference = (endDateCurrent - startDateCurrent).TotalDays;
                        endDatePrevious = startDateCurrent.AddDays(-1);
                        startDatePrevious = endDatePrevious.AddDays(-numberOfDaysDifference);
                    }

                    demandLoadLoggerService.Error("GetDemandLoadSetDetails check-2 startDateCurrent: " + startDateCurrent + " endDateCurrent: " + endDateCurrent);
                    //Fill model with Dates
                    getDemandLoadSetResponse.StartDateUTC = startDateCurrent.ToString(ServiceConstant.LDUTCDateFormat);
                    getDemandLoadSetResponse.EndDateUTC = endDateCurrent.ToString(ServiceConstant.LDUTCDateFormat);
                    getDemandLoadSetResponse.StartDatePreviousUTC = startDatePrevious.ToString(ServiceConstant.LDUTCDateFormat);
                    getDemandLoadSetResponse.EndDatePreviousUTC = endDatePrevious.ToString(ServiceConstant.LDUTCDateFormat);

                    getDemandLoadSetResponse.EndDate = endDateCurrent.ToString(ServiceConstant.LDShortDateFormat, cultureInfo);
                    getDemandLoadSetResponse.StartDate = startDateCurrent.ToString(ServiceConstant.LDShortDateFormat, cultureInfo);
                    getDemandLoadSetResponse.EndDate = endDateCurrent.ToString(ServiceConstant.LDShortDateFormat, cultureInfo);
                    getDemandLoadSetResponse.StartDatePreviousRange = startDatePrevious.ToString(ServiceConstant.LDShortDateFormat, cultureInfo);
                    getDemandLoadSetResponse.EndDatePreviousRange = endDatePrevious.ToString(ServiceConstant.LDShortDateFormat, cultureInfo);

                    getDemandLoadSetResponse.ContractAccount = contractAccountNumber;
                    getDemandLoadSetResponse.IsLatestDay = IsLatestDay;
                    getDemandLoadSetResponse.IsLatest7Day = IsLatest7Day;

                    if (IsLatestDay || isSingleDay)
                    {
                        getDemandLoadSetResponse.PreviousDateRange = startDatePrevious.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                        getDemandLoadSetResponse.SelectedDateRange = startDateCurrent.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                    }
                    else
                    {
                        getDemandLoadSetResponse.PreviousDateRange = startDatePrevious.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo) + " - " + endDatePrevious.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                        getDemandLoadSetResponse.SelectedDateRange = startDateCurrent.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo) + " - " + endDateCurrent.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                    }

                    demandLoadLoggerService.Error("GetDemandLoadSetDetails check-3 PreviousDateRange: " + getDemandLoadSetResponse.PreviousDateRange + " SelectedDateRange: " + getDemandLoadSetResponse.SelectedDateRange);
                    if (string.IsNullOrWhiteSpace(contractAccountNumber))
                    {
                        LDDemandLoadResponse.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_NULLUNDEFINED);
                    }
                    else
                    {
                        //Get DemandLoadSet for Selected and Previous date range
                        //Note: End date should be + 1 day to SAP
                        GetDemandLoadSetAPIResponse getDemandLoadSetAPIResponse = await demandLoadService.GetDemandLoadSet(contractAccountNumber, startDateCurrent.ToString(ServiceConstant.LDUTCDateFormat), endDateCurrent.AddDays(1).ToString(ServiceConstant.LDUTCDateFormat), httpRequestBase);
                        GetDemandLoadSetAPIResponse getDemandLoadSetAPIResponse_previousRange = await demandLoadService.GetDemandLoadSet(contractAccountNumber, startDatePrevious.ToString(ServiceConstant.LDUTCDateFormat), endDatePrevious.AddDays(1).ToString(ServiceConstant.LDUTCDateFormat), httpRequestBase);

                        demandLoadLoggerService.Error("GetDemandLoadSetDetails check-4 after API call");
                        getDemandLoadSetResponse.DemandLoadSetDetails = new List<DemandLoadSet>();

                        //3. check for errors
                        if (getDemandLoadSetAPIResponse?.result?.Error != null && !string.IsNullOrWhiteSpace(getDemandLoadSetAPIResponse?.result?.Error?.ErrorMessage))
                        {
                            LDDemandLoadResponse.Error = getDemandLoadSetAPIResponse.result.Error;
                        }
                        else if (getDemandLoadSetAPIResponse_previousRange?.result?.Error != null && !string.IsNullOrWhiteSpace(getDemandLoadSetAPIResponse_previousRange?.result?.Error?.ErrorMessage))
                        {
                            LDDemandLoadResponse.Error = getDemandLoadSetAPIResponse_previousRange.result.Error;
                        }
                        else if (getDemandLoadSetAPIResponse != null && getDemandLoadSetAPIResponse.result != null && getDemandLoadSetAPIResponse.result.results != null && getDemandLoadSetAPIResponse.result.results.Count > 0
                            && getDemandLoadSetAPIResponse.result.results[0].DemandLoadAvgHalfHourSet != null && getDemandLoadSetAPIResponse.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData != null
                            && getDemandLoadSetAPIResponse.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData.Count > 0 && getDemandLoadSetAPIResponse_previousRange != null && getDemandLoadSetAPIResponse_previousRange.result != null && getDemandLoadSetAPIResponse_previousRange.result.results != null && getDemandLoadSetAPIResponse_previousRange.result.results.Count > 0
                            && getDemandLoadSetAPIResponse_previousRange.result.results[0].DemandLoadAvgHalfHourSet != null && getDemandLoadSetAPIResponse_previousRange.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData != null
                            && getDemandLoadSetAPIResponse_previousRange.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData.Count > 0)
                        {
                            getDemandLoadSetResponse.DemandLoadSetDetails = GetDemandLoadSet(getDemandLoadSetAPIResponse.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData);

                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-4 DemandLoadSetDetails");
                            //Fill model with Load Details Calculations
                            double MaxLoad = getDemandLoadSetResponse.DemandLoadSetDetails.Max(d => d.Consumption);
                            getDemandLoadSetResponse.HighestLoad = clientCommon.RoundUptoDecimalPlaces(MaxLoad, ServiceConstant.DecimalPlaces2).ToString();
                            getDemandLoadSetResponse.HighestDemandLoadSetDetails = getDemandLoadSetResponse.DemandLoadSetDetails?.Where(d => d.Consumption == MaxLoad)?.Take(2).ToList();

                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-5 MaxLoad: " + MaxLoad + "getDemandLoadSetResponse.HighestLoad: " + getDemandLoadSetResponse.HighestLoad);

                            double maxLoadPreviousRange = clientCommon.RoundUptoDecimalPlaces(getDemandLoadSetAPIResponse_previousRange.result.results[0].DemandLoadAvgHalfHourSet.DemandLoadAvgHalfHourSetData.Max(d => d.AvarageValue), ServiceConstant.DecimalPlaces2);
                            getDemandLoadSetResponse.HighestLoadPreviousCycle = clientCommon.RoundUptoDecimalPlaces(maxLoadPreviousRange, ServiceConstant.DecimalPlaces2).ToString();

                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-6 maxLoadPreviousRange: " + maxLoadPreviousRange + "getDemandLoadSetResponse.HighestLoadPreviousCycle: " + getDemandLoadSetResponse.HighestLoadPreviousCycle);

                            if (MaxLoad > maxLoadPreviousRange)
                            {
                                getDemandLoadSetResponse.IsHighestLoadMoreThanPreviousCycle = true;
                                getDemandLoadSetResponse.HighestLoadComparePercentage = clientCommon.RoundUptoDecimalPlaces((((MaxLoad - maxLoadPreviousRange) * 100) / maxLoadPreviousRange), ServiceConstant.DecimalPlaces2).ToString();
                            }
                            else
                            {
                                getDemandLoadSetResponse.IsHighestLoadMoreThanPreviousCycle = false;
                                getDemandLoadSetResponse.HighestLoadComparePercentage = clientCommon.RoundUptoDecimalPlaces((((maxLoadPreviousRange - MaxLoad) * 100) / maxLoadPreviousRange), ServiceConstant.DecimalPlaces2).ToString();
                            }
                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-7 IsHighestLoadMoreThanPreviousCycle: " + getDemandLoadSetResponse.IsHighestLoadMoreThanPreviousCycle + "HighestLoadComparePercentage: " + getDemandLoadSetResponse.HighestLoadComparePercentage);

                            double MinLoad = getDemandLoadSetResponse.DemandLoadSetDetails.Where(d => d.Consumption != 0).Min(d => d.Consumption);
                            getDemandLoadSetResponse.LowestLoad = clientCommon.RoundUptoDecimalPlaces(MinLoad, ServiceConstant.DecimalPlaces2).ToString();
                            getDemandLoadSetResponse.LowestDemandLoadSetDetails = getDemandLoadSetResponse.DemandLoadSetDetails?.Where(d => d.Consumption == MinLoad)?.Take(2).ToList();

                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-8 MinLoad: " + MinLoad + "getDemandLoadSetResponse.LowestDemandLoadSetDetails: " + getDemandLoadSetResponse.LowestDemandLoadSetDetails);

                            foreach (var load in getDemandLoadSetResponse.DemandLoadSetDetails)
                            {
                                load.ConsumptionPercentage = clientCommon.RoundUptoDecimalPlaces((load.Consumption / MaxLoad) * 100, ServiceConstant.DecimalPlaces2);
                                if (load.ConsumptionPercentage <= 60)
                                    load.IsLow = true;
                                else if (load.ConsumptionPercentage <= 80)
                                    load.IsMedium = true;
                                else load.IsHigh = true;
                            }

                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-8 after ConsumptionPercentage");

                            if (getDemandLoadSetResponse?.DemandLoadContentDetails != null)
                            {
                                getDemandLoadSetResponse.HighLoadRange = clientCommon.RoundUptoDecimalPlaces(MaxLoad, ServiceConstant.DecimalPlaces2).ToString() + " - " + clientCommon.RoundUptoDecimalPlaces((MaxLoad * 81) / 100, ServiceConstant.DecimalPlaces2).ToString() + " " + getDemandLoadSetResponse.DemandLoadContentDetails?.KWHText;
                                getDemandLoadSetResponse.MediumLoadRange = clientCommon.RoundUptoDecimalPlaces((MaxLoad * 80) / 100, ServiceConstant.DecimalPlaces2).ToString() + " - " + clientCommon.RoundUptoDecimalPlaces((MaxLoad * 61) / 100, ServiceConstant.DecimalPlaces2) + " " + getDemandLoadSetResponse.DemandLoadContentDetails?.KWHText;
                                getDemandLoadSetResponse.LowLoadRange = clientCommon.RoundUptoDecimalPlaces((MaxLoad * 60) / 100, ServiceConstant.DecimalPlaces2).ToString() + " - " + "0" + " " + getDemandLoadSetResponse.DemandLoadContentDetails?.KWHText;

                                if (IsLatestDay || isSingleDay)
                                {
                                    if (MaxLoad == maxLoadPreviousRange)
                                    {
                                        getDemandLoadSetResponse.HighestLoadDescription = getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadOnTextField + " " + getDemandLoadSetResponse.SelectedDateRange + " " + getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadWasTextField + " " + getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadSameTextField + getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadAsTextField + " " + getDemandLoadSetResponse.PreviousDateRange;
                                    }
                                    else
                                    {
                                        getDemandLoadSetResponse.HighestLoadDescription = string.Format(getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadText, getDemandLoadSetResponse.SelectedDateRange, getDemandLoadSetResponse.HighestLoadComparePercentage, getDemandLoadSetResponse.IsHighestLoadMoreThanPreviousCycle ? "more" : "less", getDemandLoadSetResponse.PreviousDateRange);
                                    }
                                }
                                else
                                {
                                    if (MaxLoad == maxLoadPreviousRange)
                                    {
                                        getDemandLoadSetResponse.HighestLoadDescription = getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadBetweenTextField + " " + getDemandLoadSetResponse.SelectedDateRange + " " + getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadWasTextField + " " + getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadSameTextField + getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadAsTextField + " " + getDemandLoadSetResponse.PreviousDateRange;
                                    }
                                    else
                                    {
                                        getDemandLoadSetResponse.HighestLoadDescription = string.Format(getDemandLoadSetResponse.DemandLoadContentDetails.HighestLoadBTWText, getDemandLoadSetResponse.SelectedDateRange, getDemandLoadSetResponse.HighestLoadComparePercentage, getDemandLoadSetResponse.IsHighestLoadMoreThanPreviousCycle ? "more" : "less", getDemandLoadSetResponse.PreviousDateRange);
                                    }
                                }
                            }

                            demandLoadLoggerService.Error("GetDemandLoadSetDetails check-9 HighestLoadDescription:" + getDemandLoadSetResponse.HighestLoadDescription);
                        }
                    }
                }

                demandLoadLoggerService.Error("GetDemandLoadSetDetails check-10");
                LDDemandLoadResponse.ResponseData = getDemandLoadSetResponse;
            }
            catch (Exception ex)
            {
                LDDemandLoadResponse.Error.ErrorMessage = ex.Message;
            }

            return LDDemandLoadResponse;
        }

        private ForecastContentDetails GetForecastContentDetails(HttpRequestBase httpRequestBase, string headerlanguage)
        {
            ForecastContentDetails lDForecastContentDetails = new ForecastContentDetails();

            try
            {
                //Content details
                Database webDb = Sitecore.Configuration.Factory.GetDatabase(ServiceConstant.SITECORE_WEB_DATABASE);
                Sitecore.Globalization.Language itemlanguage = Sitecore.Data.Managers.LanguageManager.GetLanguage(headerlanguage);
                Sitecore.Data.ID forecastContentItem = new Sitecore.Data.ID(ServiceConstant.ForecastItemDetails);
                if (!forecastContentItem.IsNull)
                {
                    using (new LanguageSwitcher(itemlanguage))
                    {
                        Item forecastItemDetails = webDb.GetItem(forecastContentItem);
                        if (forecastItemDetails != null)
                        {
                            lDForecastContentDetails.ForecastText = forecastItemDetails.Fields[ServiceConstant.ForecastTextField]?.Value;
                            lDForecastContentDetails.CompareElectricityUsageText = forecastItemDetails.Fields[ServiceConstant.CompareElectricityUsageTextField]?.Value;
                            lDForecastContentDetails.BillingPeriodText = forecastItemDetails.Fields[ServiceConstant.BillingPeriodTextField]?.Value;
                            lDForecastContentDetails.PredictionAnalysisText = forecastItemDetails.Fields[ServiceConstant.PredictionAnalysisTextField]?.Value;
                            lDForecastContentDetails.CurrentPredicationText = forecastItemDetails.Fields[ServiceConstant.CurrentPredicationTextField]?.Value;
                            lDForecastContentDetails.CurrentPredictionApproximatelyText = forecastItemDetails.Fields[ServiceConstant.CurrentPredictionApproximatelyTextField]?.Value;

                            Sitecore.Data.Fields.ImageField onLoadGraphImageField = forecastItemDetails.Fields[ServiceConstant.OnLoadGraphImageField];
                            if (onLoadGraphImageField != null && onLoadGraphImageField.MediaItem != null)
                            {
                                Sitecore.Data.Items.MediaItem imageItem = new MediaItem(onLoadGraphImageField?.MediaItem);
                                lDForecastContentDetails.OnLoadGraphImage = Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageItem));
                            }

                            Sitecore.Data.Fields.ImageField predictionGraphImage = forecastItemDetails.Fields[ServiceConstant.PredictionGraphImageField];
                            if (predictionGraphImage != null && predictionGraphImage.MediaItem != null)
                            {
                                Sitecore.Data.Items.MediaItem imageItem = new MediaItem(predictionGraphImage?.MediaItem);
                                lDForecastContentDetails.predictionGraphImage = Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageItem));
                            }

                            lDForecastContentDetails.ActualSpendToDateText = forecastItemDetails.Fields[ServiceConstant.ActualSpendToDateTextField]?.Value;
                            lDForecastContentDetails.StartDayText = forecastItemDetails.Fields[ServiceConstant.StartDayTextField]?.Value;
                            lDForecastContentDetails.EndDayText = forecastItemDetails.Fields[ServiceConstant.EndDayTextField]?.Value;
                            lDForecastContentDetails.SarText = forecastItemDetails.Fields[ServiceConstant.SarTextField]?.Value;
                            lDForecastContentDetails.ExpectedAmountText = forecastItemDetails.Fields[ServiceConstant.ExpectedAmountTextField]?.Value;
                            lDForecastContentDetails.MeasureaApproximatelyText = forecastItemDetails.Fields[ServiceConstant.MeasureaApproximatelyTextField]?.Value;
                            lDForecastContentDetails.PredictedSpendToDateText = forecastItemDetails.Fields[ServiceConstant.PredictedSpendToDateTextField]?.Value;
                            lDForecastContentDetails.PredictedAmountText = forecastItemDetails.Fields[ServiceConstant.PredictedAmountTextField]?.Value;
                            lDForecastContentDetails.WhatMeanText = forecastItemDetails.Fields[ServiceConstant.WhatMeanTextField]?.Value;
                            lDForecastContentDetails.AbovePredictionText = forecastItemDetails.Fields[ServiceConstant.AbovePredictionTextField]?.Value;
                            lDForecastContentDetails.HowCalculatedText = forecastItemDetails.Fields[ServiceConstant.HowCalculatedTextField]?.Value;
                            lDForecastContentDetails.CalculatingNotificationText = forecastItemDetails.Fields[ServiceConstant.CalculatingNotificationTextField]?.Value;
                            lDForecastContentDetails.ConsumptionSpendingText = forecastItemDetails.Fields[ServiceConstant.ConsumptionSpendingTextField]?.Value;
                            lDForecastContentDetails.CalculatedPopUpText = forecastItemDetails.Fields[ServiceConstant.CalculatedPopUpTextField]?.Value;
                            Sitecore.Data.Fields.CheckboxField checkboxField = forecastItemDetails.Fields[ServiceConstant.CalculatedPopUpCheckBoxField];
                            if (checkboxField != null & checkboxField.Checked)
                            {
                                lDForecastContentDetails.CalculatedPopUpCheckBox = true;
                            }
                            lDForecastContentDetails.AccurateBillingCycleText = forecastItemDetails.Fields[ServiceConstant.AccurateBillingCycleTextField]?.Value;
                            Sitecore.Data.Fields.CheckboxField AmountFlagcheckboxField = forecastItemDetails.Fields[ServiceConstant.AmountFlagField];
                            if (AmountFlagcheckboxField != null & AmountFlagcheckboxField.Checked)
                            {
                                lDForecastContentDetails.AmountFlag = true;
                            }

                            lDForecastContentDetails.CurrentPredictionTextPart1 = forecastItemDetails.Fields[ServiceConstant.CurrentPredictionTextPart1Field]?.Value;
                            lDForecastContentDetails.CurrentPredictionTextPart2 = forecastItemDetails.Fields[ServiceConstant.CurrentPredictionTextPart2Field]?.Value;
                            lDForecastContentDetails.PredictedSpendAverageDescriptionPart1 = forecastItemDetails.Fields[ServiceConstant.PredictedSpendAverageDescriptionPart1Field]?.Value;
                            lDForecastContentDetails.PredictedSpendAverageDescriptionPart2 = forecastItemDetails.Fields[ServiceConstant.PredictedSpendAverageDescriptionPart2Field]?.Value;
                            lDForecastContentDetails.PredictedSpendAverageDescriptionPart3 = forecastItemDetails.Fields[ServiceConstant.PredictedSpendAverageDescriptionPart3Field]?.Value;
                            lDForecastContentDetails.OverText = forecastItemDetails.Fields[ServiceConstant.OverTextField]?.Value;
                            lDForecastContentDetails.UnderText = forecastItemDetails.Fields[ServiceConstant.UnderTextField]?.Value;

                            lDForecastContentDetails.SameText = forecastItemDetails.Fields[ServiceConstant.SameTextField]?.Value;
                            lDForecastContentDetails.ForecastNotavailableHeading = forecastItemDetails.Fields[ServiceConstant.ForecastNotavailableHeadingField]?.Value;
                            lDForecastContentDetails.ForecastNotavailableDescription = forecastItemDetails.Fields[ServiceConstant.ForecastNotavailableDescriptionField]?.Value;

                            lDForecastContentDetails.NoAccountAvailableText = forecastItemDetails.Fields[ServiceConstant.NoAccountAvailableText]?.Value;
                            lDForecastContentDetails.NoAccountAvailableCTAText = forecastItemDetails.Fields[ServiceConstant.NoAccountAvailableCTAText]?.Value;
                            lDForecastContentDetails.FixedBillTileText1 = forecastItemDetails.Fields[ServiceConstant.FixedBillTileText1]?.Value;
                            lDForecastContentDetails.FixedBillTileText2 = forecastItemDetails.Fields[ServiceConstant.FixedBillTileText2]?.Value;
                            lDForecastContentDetails.InstalmentPlanTileText1 = forecastItemDetails.Fields[ServiceConstant.InstalmentPlanTileText1]?.Value;
                            lDForecastContentDetails.InstalmentPlanTileText2 = forecastItemDetails.Fields[ServiceConstant.InstalmentPlanTileText2]?.Value;
                            lDForecastContentDetails.PredictedSpendAverageDescriptionPart2_same = forecastItemDetails.Fields[ServiceConstant.PredictedSpendAverageDescriptionPart2_same]?.Value;
                            lDForecastContentDetails.FinalBillCalculatingText = forecastItemDetails.Fields[ServiceConstant.FinalBillCalculatingText]?.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                demandLoadLoggerService.Error("GetForecastContentDetails", ex);
            }
            return lDForecastContentDetails;
        }

        public async Task<ODataApiResponse<GetForecastResponse>> GetForecastBillAmountDetails(string contractAccountNumber, bool isInstalmentPlan, bool isFixedBill, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<GetForecastResponse> getForecastResponse = new ODataApiResponse<GetForecastResponse>(new GetForecastResponse());

            try
            {
                //get the header language to get content from sitecore
                var headerlanguage = httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE] != null ? httpRequestBase.Headers[ServiceConstant.REQUESTHEADERS_KEY_ACCEPTLANGUAGE].Trim().ToString() : string.Empty;
                if (string.IsNullOrEmpty(headerlanguage))
                {
                    headerlanguage = Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName.ToLower(CultureInfo.InvariantCulture);
                }
                CultureInfo cultureInfo = new CultureInfo(headerlanguage);
                if (headerlanguage.ToLowerInvariant().Equals(ExtensionConstant.ArabicLanguageCode.ToLower()))
                {
                    cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
                }

                //1. Fill the content
                getForecastResponse.ResponseData.ContentDetails = GetForecastContentDetails(httpRequestBase, headerlanguage);

                //2.validation
                if (string.IsNullOrWhiteSpace(contractAccountNumber))
                {
                    getForecastResponse.Error.ErrorMessage = clientCommon.GetErrorMessageByCode(ServiceConstant.ERRORCODE_NULLUNDEFINED);
                }
                else
                {
                    var unbilledAmountResponse = await demandLoadService.GetUnbilledAmount(contractAccountNumber, httpRequestBase);
                    var predictedBillAmountResponse = await demandLoadService.GetPredictedBillAmount(contractAccountNumber, httpRequestBase);

                    if (isInstalmentPlan)
                    {
                        var instalmentPlanAmountResponse = await demandLoadService.GetInstalmentPlanAmount(contractAccountNumber, httpRequestBase);
                        getForecastResponse.ResponseData.InstalmentAmount = instalmentPlanAmountResponse?.IPAmountDetails?.Amount;
                    }
                    if (isFixedBill)
                    {
                        var fixedBillAmountResponse = await demandLoadService.GetFixedBillAmount(contractAccountNumber, httpRequestBase);
                        getForecastResponse.ResponseData.FixedBillAmount = fixedBillAmountResponse?.FixedBillAmountDetails?.Amount;
                    }

                    demandLoadLoggerService.Error("GetForecastBillAmountDetails log-1 after api call");
                    demandLoadLoggerService.Error("GetForecastBillAmountDetails log-2 API result unbilledAmountResponse: " + unbilledAmountResponse?.UnbilledAmounDetails?.StartDate + ", " + unbilledAmountResponse?.UnbilledAmounDetails?.EndDate + ", " +
                        unbilledAmountResponse?.UnbilledAmounDetails?.Amount);
                    demandLoadLoggerService.Error("GetForecastBillAmountDetails log-2 API result predictedBillAmountResponse: " + predictedBillAmountResponse?.PredictedAmountDetails?.StartDate + ", " + predictedBillAmountResponse?.PredictedAmountDetails?.EndDate + ", " +
                        predictedBillAmountResponse?.PredictedAmountDetails?.Amount);

                    if (unbilledAmountResponse != null && unbilledAmountResponse.UnbilledAmounDetails != null &&
                       !string.IsNullOrWhiteSpace(unbilledAmountResponse.UnbilledAmounDetails.StartDate) && !string.IsNullOrWhiteSpace(unbilledAmountResponse.UnbilledAmounDetails.EndDate)
                       && !string.IsNullOrWhiteSpace(unbilledAmountResponse.UnbilledAmounDetails.Amount) && unbilledAmountResponse.UnbilledAmounDetails.Amount != "0"
                        && predictedBillAmountResponse != null && predictedBillAmountResponse.PredictedAmountDetails != null &&
                        !string.IsNullOrWhiteSpace(predictedBillAmountResponse.PredictedAmountDetails.StartDate) && !string.IsNullOrWhiteSpace(predictedBillAmountResponse.PredictedAmountDetails.EndDate)
                        && !string.IsNullOrWhiteSpace(predictedBillAmountResponse.PredictedAmountDetails.Amount) && predictedBillAmountResponse.PredictedAmountDetails.Amount != "0")
                    {
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-3 inside if");

                        DateTime startDateUnbilled = DateTime.Parse(clientCommon.ConvertToDateTimeString(unbilledAmountResponse.UnbilledAmounDetails?.StartDate));
                        DateTime endDateUnbilled = DateTime.Parse(clientCommon.ConvertToDateTimeString(unbilledAmountResponse.UnbilledAmounDetails?.EndDate));

                        DateTime startDate = DateTime.Parse(clientCommon.ConvertToDateTimeString(predictedBillAmountResponse.PredictedAmountDetails?.StartDate));
                        DateTime endDate = DateTime.Parse(clientCommon.ConvertToDateTimeString(predictedBillAmountResponse.PredictedAmountDetails?.EndDate));
                        DateTime currentDate = System.DateTime.Now;

                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-4 startDate:" + startDate + ", endDate: " + endDate + ", currentDate: " + currentDate);

                        double ActualAmount = Convert.ToDouble(unbilledAmountResponse.UnbilledAmounDetails.Amount);
                        double PredictedAmount = Convert.ToDouble(predictedBillAmountResponse.PredictedAmountDetails.Amount);
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-5 ActualAmount:" + ActualAmount + ", PredictedAmount: " + PredictedAmount);

                        //SAP response directly MAP for debugging
                        getForecastResponse.ResponseData.UnbilledAmounDetails.IsDataAvailableFromSAP = true;
                        getForecastResponse.ResponseData.PredictedAmountDetails.IsDataAvailableFromSAP = true;
                        getForecastResponse.ResponseData.UnbilledAmounDetails.StartDateSAP = startDateUnbilled.ToString();
                        getForecastResponse.ResponseData.UnbilledAmounDetails.EndDateSAP = endDateUnbilled.ToString();
                        getForecastResponse.ResponseData.UnbilledAmounDetails.AmountSAP = unbilledAmountResponse.UnbilledAmounDetails.Amount;
                        getForecastResponse.ResponseData.PredictedAmountDetails.StartDateSAP = startDate.ToString();
                        getForecastResponse.ResponseData.PredictedAmountDetails.EndDateSAP = endDate.ToString();
                        getForecastResponse.ResponseData.PredictedAmountDetails.AmountSAP = predictedBillAmountResponse.PredictedAmountDetails.Amount;

                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-5 before calculations");
                        //3. Calulations and login
                        var predictedSpend = Convert.ToDouble(predictedBillAmountResponse.PredictedAmountDetails.Amount);

                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-6 predictedSpend:" + predictedSpend);

                        var totalDaysInBillingCycle = (endDate - startDate).TotalDays;
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-7 totalDaysInBillingCycle:" + totalDaysInBillingCycle);

                        var numberOfDaysUnbilled = (endDateUnbilled - startDateUnbilled).TotalDays;
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-8 numberOfDaysPassedTillToday:" + numberOfDaysUnbilled);

                        var predictedSpendPerDay = Convert.ToDouble(predictedSpend / totalDaysInBillingCycle);
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-9 predictedSpendPerDay:" + predictedSpendPerDay);

                        var unbilledSpend = Convert.ToDouble(unbilledAmountResponse?.UnbilledAmounDetails?.Amount);
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-10 unbilledSpend:" + unbilledSpend);

                        var unbilledSpendTotal = (unbilledSpend / numberOfDaysUnbilled) * totalDaysInBillingCycle;
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-11 unbilledSpendTotal:" + unbilledSpendTotal);

                        double percentage = 0;

                        //Fill the model
                        getForecastResponse.ResponseData.StartDateFull = startDate.ToString(ServiceConstant.LDUTCDateFormat);
                        getForecastResponse.ResponseData.EndDateFull = endDate.ToString(ServiceConstant.LDUTCDateFormat);
                        getForecastResponse.ResponseData.CurrentDateFull = DateTime.Now.ToString(ServiceConstant.LDUTCDateFormat);

                        getForecastResponse.ResponseData.StartDate = startDate.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                        getForecastResponse.ResponseData.EndDate = endDate.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);

                        getForecastResponse.ResponseData.CurrentDate = DateTime.Now.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);

                        getForecastResponse.ResponseData.ContractAccount = contractAccountNumber;

                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-12");
                        if (startDate.Year == endDate.Year)
                        {
                            getForecastResponse.ResponseData.BillingCycleRange = startDate.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo) + " - " + endDate.ToString(ServiceConstant.LDDateRangeFormatWithYear, cultureInfo);
                        }
                        else
                        {
                            getForecastResponse.ResponseData.BillingCycleRange = startDate.ToString(ServiceConstant.LDDateRangeFormatWithYear, cultureInfo) + " - " + endDate.ToString(ServiceConstant.LDDateRangeFormatWithYear, cultureInfo);
                        }
                        getForecastResponse.ResponseData.TotalDaysInBillingCycle = totalDaysInBillingCycle;

                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-12 BillingCycleRange:" + getForecastResponse.ResponseData.BillingCycleRange);
                        if (getForecastResponse?.ResponseData?.ContentDetails != null)
                        {
                            getForecastResponse.ResponseData.ToolTipDescription = string.Format(getForecastResponse?.ResponseData?.ContentDetails?.AccurateBillingCycleText, endDateUnbilled.ToString(ServiceConstant.LDDateRangeFormatWithYear, cultureInfo), (endDate.Date - endDateUnbilled.Date).TotalDays);
                        }

                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-13 ToolTipDescription:" + getForecastResponse.ResponseData.ToolTipDescription);

                        getForecastResponse.ResponseData.PredictedAmountDetails.PredictedAmountTotal = PredictedAmount.ToString();
                        getForecastResponse.ResponseData.UnbilledAmounDetails.ActualAmountTillDate = ActualAmount.ToString();

                        if (clientCommon.RoundUptoDecimalPlaces(unbilledSpendTotal, ServiceConstant.DecimalPlaces2) == clientCommon.RoundUptoDecimalPlaces(predictedSpend, ServiceConstant.DecimalPlaces2))
                        {
                            getForecastResponse.ResponseData.IsActualSpendMoreThanPrediction = false;
                            getForecastResponse.ResponseData.UnbilledAmounDetails.ActualSpendComparePercentage = "0";
                        }
                        else if (unbilledSpendTotal > predictedSpend)
                        {
                            percentage = clientCommon.RoundUptoDecimalPlaces((((unbilledSpendTotal - predictedSpend) / predictedSpend) * 100), ServiceConstant.DecimalPlaces2);
                            getForecastResponse.ResponseData.IsActualSpendMoreThanPrediction = true;
                            getForecastResponse.ResponseData.UnbilledAmounDetails.ActualSpendComparePercentage = percentage.ToString();
                        }
                        else
                        {
                            percentage = clientCommon.RoundUptoDecimalPlaces((((predictedSpend - unbilledSpendTotal) / predictedSpend) * 100), ServiceConstant.DecimalPlaces2);
                            getForecastResponse.ResponseData.IsActualSpendMoreThanPrediction = false;
                            getForecastResponse.ResponseData.UnbilledAmounDetails.ActualSpendComparePercentage = percentage.ToString();
                        }
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-14 percentage:" + percentage);

                        if (startDate.Month == endDate.Month)
                        {
                            getForecastResponse.ResponseData.UnbilledAmounDetails.ActualSpendTillDateRange = startDateUnbilled.Date.Day + " - " + endDateUnbilled.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                        }
                        else
                        {
                            getForecastResponse.ResponseData.UnbilledAmounDetails.ActualSpendTillDateRange = startDateUnbilled.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo) + " - " + endDateUnbilled.ToString(ServiceConstant.LDDateRangeFormat, cultureInfo);
                        }
                        demandLoadLoggerService.Error("GetForecastBillAmountDetails log-15 ActualSpendTillDateRange:" + getForecastResponse.ResponseData.UnbilledAmounDetails.ActualSpendTillDateRange);

                        getForecastResponse.ResponseData.PredictedAmountDetails.PredictedSpendAverage = clientCommon.RoundUptoDecimalPlaces((predictedSpendPerDay * numberOfDaysUnbilled), ServiceConstant.DecimalPlaces2).ToString();
                    }
                    else
                    {
                        if (unbilledAmountResponse != null && unbilledAmountResponse.UnbilledAmounDetails != null &&
                       !string.IsNullOrWhiteSpace(unbilledAmountResponse.UnbilledAmounDetails.StartDate) && !string.IsNullOrWhiteSpace(unbilledAmountResponse.UnbilledAmounDetails.EndDate)
                       && !string.IsNullOrWhiteSpace(unbilledAmountResponse.UnbilledAmounDetails.Amount) && unbilledAmountResponse.UnbilledAmounDetails.Amount != "0")
                        {
                            getForecastResponse.ResponseData.UnbilledAmounDetails.IsDataAvailableFromSAP = true;
                            getForecastResponse.ResponseData.UnbilledAmounDetails.StartDateSAP = unbilledAmountResponse.UnbilledAmounDetails.StartDate;
                            getForecastResponse.ResponseData.UnbilledAmounDetails.EndDateSAP = unbilledAmountResponse.UnbilledAmounDetails.EndDate;
                            getForecastResponse.ResponseData.UnbilledAmounDetails.AmountSAP = unbilledAmountResponse.UnbilledAmounDetails.Amount;
                        }
                        else
                        {
                            getForecastResponse.ResponseData.UnbilledAmounDetails.IsDataAvailableFromSAP = false;
                        }
                        if (predictedBillAmountResponse != null && predictedBillAmountResponse.PredictedAmountDetails != null &&
                        !string.IsNullOrWhiteSpace(predictedBillAmountResponse.PredictedAmountDetails.StartDate) && !string.IsNullOrWhiteSpace(predictedBillAmountResponse.PredictedAmountDetails.EndDate)
                        && !string.IsNullOrWhiteSpace(predictedBillAmountResponse.PredictedAmountDetails.Amount) && predictedBillAmountResponse.PredictedAmountDetails.Amount != "0")
                        {
                            getForecastResponse.ResponseData.PredictedAmountDetails.IsDataAvailableFromSAP = true;
                            getForecastResponse.ResponseData.PredictedAmountDetails.StartDateSAP = predictedBillAmountResponse.PredictedAmountDetails.StartDate;
                            getForecastResponse.ResponseData.PredictedAmountDetails.EndDateSAP = predictedBillAmountResponse.PredictedAmountDetails.EndDate;
                            getForecastResponse.ResponseData.PredictedAmountDetails.AmountSAP = predictedBillAmountResponse.PredictedAmountDetails.Amount;
                        }
                        else
                        {
                            getForecastResponse.ResponseData.PredictedAmountDetails.IsDataAvailableFromSAP = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getForecastResponse.Error.ErrorMessage = ex.Message;
            }

            return getForecastResponse;
        }

        public async Task<ODataApiResponse<CASetListResponse>> GetCASet(string partnerNo, string status, HttpRequestBase httpRequestBase)
        {
            ODataApiResponse<CASetListResponse> CASetResponse = new ODataApiResponse<CASetListResponse>(new CASetListResponse());

            try
            {
                CASetResponse = await demandLoadService.GetCASet(partnerNo, status, httpRequestBase);
                //CASetResponse.ResponseData.CASetListData.CASetResponseData = CASetResponse.ResponseData?.CASetListData?.CASetResponseData?.Where(c => c.IsSmartMeter == true).ToList();
            }
            catch (Exception ex)
            {
                CASetResponse.Error.ErrorMessage = ex.Message;
            }

            return CASetResponse;
        }

        public async Task<GetFixedBillAmountResponse> GetFixedBillAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetFixedBillAmountResponse getFixedBillAmountResponse = new GetFixedBillAmountResponse();

            try
            {
                getFixedBillAmountResponse = await demandLoadService.GetFixedBillAmount(contractAccountNumber, httpRequestBase);
            }
            catch (Exception ex)
            {
                getFixedBillAmountResponse.Error.ErrorMessage = ex.Message;
            }

            return getFixedBillAmountResponse;
        }

        public async Task<GetIPAmountResponse> GetInstalmentPlanAmount(string contractAccountNumber, HttpRequestBase httpRequestBase)
        {
            GetIPAmountResponse getIPAmountResponse = new GetIPAmountResponse();

            try
            {
                getIPAmountResponse = await demandLoadService.GetInstalmentPlanAmount(contractAccountNumber, httpRequestBase);
            }
            catch (Exception ex)
            {
                getIPAmountResponse.Error.ErrorMessage = ex.Message;
            }

            return getIPAmountResponse;
        }
    }
}