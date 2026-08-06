using SKM.V3.Internal;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods that related to data gathering. A complete list
    /// can be found here: https://app.cryptolens.io/docs/api/v3/AI
    /// </summary>
    public class AI
    {
        /// <summary>
        /// This method will, given a license key, generate a new access token
        /// that is locked to that particular key and return the Id of that key.
        /// The scope of the access token is preserved (i.e. all methods that were
        /// enabled in the access token used to access this method will be copied
        /// to the new access token) except for the key lock, which is going to be
        /// changed to the id of the license key. Note, for this method to work,
        /// the access token used to access this method has to have key lock set
        /// to -1. All of these details are described in Remarks.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/RegisterEvent">https://app.cryptolens.io/docs/api/v3/RegisterEvent</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static BasicResult RegisterEvent(string token, RegisterEventModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(parameters, "/ai/registerevent/", token);
        }

        /// <summary>
        /// This method will register an event that has occured in either the client app (eg. start of a certain feature or interaction within a feature) or in a third party provider (eg. a payment has occured, etc).
        /// Note: You can either use this method standalone(eg.by only providing a machine code/device identifier) or together with Cryptolens Licensing module(which requires productId and optionally keyid to be set). The more information that is provided, the better insights can be provided.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/RegisterEvent">https://app.cryptolens.io/docs/api/v3/RegisterEvent</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static BasicResult RegisterEvents(string token, RegisterEventsModel parameters)
        {
            var internalModel = new RegisterEventsModelServer
            {
                ProductId = parameters.ProductId,
                Events = Newtonsoft.Json.JsonConvert.SerializeObject(parameters.Events),
                Key = parameters.Key,
                MachineCode = parameters.MachineCode,
                LicenseServerUrl = parameters.LicenseServerUrl
            };

            return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(internalModel, "/ai/registerevents/", token);
        }

        /// <summary>
        /// This method will retrieve a list of Web API Logs. All events that get logged are related
        /// to a change of a license key or data object, eg. when license key gets activated or when
        /// a property of data object changes. More details about the method that was called are specified
        /// in the State field. You can read more about it here. These logs contain minimal information
        /// but are guaranteed to be preserved. To get more information, especially about changes not
        /// tracked in this log, please check out the Object Log.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetWebAPILog">https://app.cryptolens.io/docs/api/v3/GetWebAPILog</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static GetWebAPILogResult GetWebAPILog(string token, GetWebAPILogModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetWebAPILogResult>(parameters, "/ai/getwebapilog/", token);
        }

        /// <summary>
        /// Retrieves daily Usage Analytics totals for the products and filters available to the access token.
        /// </summary>
        /// <param name="token">The access token. It must have Usage Analytics permission.</param>
        /// <param name="parameters">The filters and paging parameters for the request.</param>
        /// <remarks>Note: for more details, please see
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetDailyAggregates">https://app.cryptolens.io/docs/api/v3/GetDailyAggregates</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="GetDailyAggregatesResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static GetDailyAggregatesResult GetDailyAggregates(string token, GetUsageAnalyticsModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetDailyAggregatesResult>(parameters, "/ai/getdailyaggregates/", token);
        }

        /// <summary>
        /// Retrieves daily Usage Analytics totals grouped by country for the products and filters available to the access token.
        /// </summary>
        /// <param name="token">The access token. It must have Usage Analytics permission.</param>
        /// <param name="parameters">The filters and paging parameters for the request.</param>
        /// <remarks>Note: for more details, please see
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetDailyCountryAggregates">https://app.cryptolens.io/docs/api/v3/GetDailyCountryAggregates</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="GetDailyCountryAggregatesResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static GetDailyCountryAggregatesResult GetDailyCountryAggregates(string token, GetUsageAnalyticsModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetDailyCountryAggregatesResult>(parameters, "/ai/getdailycountryaggregates/", token);
        }

        /// <summary>
        /// Retrieves Usage Analytics summaries for license keys available to the access token.
        /// </summary>
        /// <param name="token">The access token. It must have Usage Analytics permission.</param>
        /// <param name="parameters">The filters and paging parameters for the request.</param>
        /// <remarks>Note: for more details, please see
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetKeyUsageSummaries">https://app.cryptolens.io/docs/api/v3/GetKeyUsageSummaries</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="GetKeyUsageSummariesResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static GetKeyUsageSummariesResult GetKeyUsageSummaries(string token, GetUsageAnalyticsModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetKeyUsageSummariesResult>(parameters, "/ai/getkeyusagesummaries/", token);
        }

        /// <summary>
        /// Retrieves Usage Analytics device rows for license keys available to the access token.
        /// </summary>
        /// <param name="token">The access token. It must have Usage Analytics permission.</param>
        /// <param name="parameters">The filters and paging parameters for the request.</param>
        /// <remarks>Note: for more details, please see
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetKeyDevices">https://app.cryptolens.io/docs/api/v3/GetKeyDevices</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="GetKeyDevicesResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static GetKeyDevicesResult GetKeyDevices(string token, GetUsageAnalyticsModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetKeyDevicesResult>(parameters, "/ai/getkeydevices/", token);
        }

        /// <summary>
        /// Retrieves Usage Analytics license activity buckets for license keys available to the access token.
        /// </summary>
        /// <param name="token">The access token. It must have Usage Analytics permission.</param>
        /// <param name="parameters">The filters and paging parameters for the request.</param>
        /// <remarks>Note: for more details, please see
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetLicenseActivityBuckets">https://app.cryptolens.io/docs/api/v3/GetLicenseActivityBuckets</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="GetLicenseActivityBucketsResult"/>. Handled client failures are returned with <see cref="ResultType.Error"/>.</returns>
        public static GetLicenseActivityBucketsResult GetLicenseActivityBuckets(string token, GetUsageAnalyticsModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetLicenseActivityBucketsResult>(parameters, "/ai/getlicenseactivitybuckets/", token);
        }


        /// <summary>
        /// This method will retrieve a list of Web API Logs. All events that get logged are related
        /// to a change of a license key or data object, eg. when license key gets activated or when
        /// a property of data object changes. More details about the method that was called are specified
        /// in the State field. You can read more about it here. These logs contain minimal information
        /// but are guaranteed to be preserved. To get more information, especially about changes not
        /// tracked in this log, please check out the Object Log.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/GetWebAPILog">https://app.cryptolens.io/docs/api/v3/GetWebAPILog</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/> or null.</returns>
        //public static GetEventsResult GetEvents(string token, GetEventsModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<GetWebAPILogResult>(parameters, "/ai/getevents/", token);
        //}

        private class RegisterEventsModelServer: RequestModel
        {
            public int ProductId { get; set; }
            public string Key { get; set; }
            public string MachineCode { get; set; }
            public string Events { get; set; }
        }
    }
}
