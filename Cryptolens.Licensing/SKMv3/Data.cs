using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods that perform operations on a data object. A complete list
    /// can be found here: https://app.cryptolens.io/docs/api/v3/Data
    /// </summary>
    /// <example>
    /// <code language="csharp" title="Usage quota for a feature">
    /// var token = "{acesstoken to GetKey, IncrementIntValue, ListDataObjects, AddDataObjects, and Key Lock = -1 (or zero)}".
    /// var license = new LicenseKey { ProductId = 3349, Key = "GEBNC-WZZJD-VJIHG-GCMVD" };
    /// license.Refresh(token);
    /// 
    /// if(license.DataObjects.Contains("usagecount"))
    /// {
    ///     // attempt to increment. true means we succeed.
    ///     var dataObj = license.DataObjects.Get("usagecount");
    ///     if (dataObj.IncrementIntValue(token: token,
    ///                                  incrementValue: 1,
    ///                                  enableBound: true,
    ///                                  upperBound: 2))
    ///     {
    ///         // success, we can keep using this feature
    ///     }
    ///     else
    ///     {
    ///         // fail, the the user has already used it 10 times.
    ///     }
    /// 
    /// }
    /// else
    /// {
    ///     // if it does not exist, add a new one.
    ///     license.AddDataObject(tokenDObj, new DataObject { Name = "usagecount", IntValue = 0 });
    /// }
    /// </code>
    /// </example>
    public class Data
    {
        /// <summary>
        /// Creates a new <see cref="DataObject"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/AddDataObject">https://app.cryptolens.io/docs/api/v3/AddDataObject</a> </remarks>
        /// <returns>Returns <see cref="DataObjectIdResult"/> or null.</returns>
        public static DataObjectIdResult AddDataObject(string token, AddDataObjectModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<DataObjectIdResult>(parameters, "/data/adddataobject/", token);
        }

        /// <summary>
        /// Creates a new <see cref="DataObject"/> for a key.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/AddDataObject">https://app.cryptolens.io/docs/api/v3/AddDataObject</a> </remarks>
        /// <returns>Returns <see cref="DataObjectIdResult"/> or null.</returns>
        public static DataObjectIdResult AddDataObject(string token, AddDataObjectToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<DataObjectIdResult>(parameters, "/data/adddataobjecttokey/", token);
        }

        /// <summary>
        /// This method lists either all Data Object associated with a
        /// license key, a product or your entire account, or all of them at once.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/ListDataObjects">https://app.cryptolens.io/docs/api/v3/ListDataObjects</a> </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static ListOfDataObjectsResult ListDataObjects(string token, ListDataObjectsModel parameters)
        {
            if (parameters.ShowAll)
            {
                var result = HelperMethods.SendRequestToWebAPI3<ListOfDataObjectsResultWithReferencer>(parameters, "/data/listdataobjects/", token);
                return new ListOfDataObjectsResult
                {
                    Message = result.Message,
                    Result = result.Result,
                    DataObjects = result.DataObjects.Select(x => (DataObject)x).ToList()
                };
            }
            else
            {
                return HelperMethods.SendRequestToWebAPI3<ListOfDataObjectsResult>(parameters, "/data/listdataobjects/", token);
            }
        }
        /// <summary>
        /// This method lists all data objects associated with a license key only.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/ListDataObjects">https://app.cryptolens.io/docs/api/v3/ListDataObjects</a> </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static ListOfDataObjectsResult ListDataObjects(string token, ListDataObjectsToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<ListOfDataObjectsResult>(parameters, "/data/listdataobjectstokey/", token);
        }


        /// <summary>
        /// This method will set the int value to a new one.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/SetIntValue">https://app.cryptolens.io/docs/api/v3/SetIntValue</a> <br/>
        /// Note also: Integer overflows are not allowed. If you attempt to assign an int value that is beyond the limits of an int32, zero will be assigned to the data object's IntValue.</remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult SetIntValue(string token, ChangeIntValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setintvalue/", token);
        }

        /// <summary>
        /// This method will set the int value to a new one.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/SetIntValue">https://app.cryptolens.io/docs/api/v3/SetIntValue</a> <br/>
        /// Note also: Integer overflows are not allowed. If you attempt to assign an int value that is beyond the limits of an int32, zero will be assigned to the data object's IntValue.</remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult SetIntValue(string token, ChangeIntValueToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setintvaluetokey/", token);
        }

        /// <summary>
        /// This method will set the string value to a new one.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/SetStringValue">https://app.cryptolens.io/docs/api/v3/SetStringValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult SetStringValue(string token, ChangeStringValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setstringvalue/", token);
        }

        /// <summary>
        /// This method will set the string value to a new one.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/SetStringValue">https://app.cryptolens.io/docs/api/v3/SetStringValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult SetStringValue(string token, ChangeStringValueToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setstringvaluetokey/", token);
        }

        /// <summary>
        /// This method will increment the current int value by the one specified as an input parameter,
        /// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/IncrementIntValue">https://app.cryptolens.io/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult IncrementIntValue(string token, ChangeIntValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/incrementintvalue/", token);
        }


        /// <summary>
        /// This method will increment the current int value by the one specified as an input parameter,
        /// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/IncrementIntValue">https://app.cryptolens.io/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult IncrementIntValue(string token, ChangeIntValueToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/incrementintvaluetokey/", token);
        }


        /// <summary>
        /// This method will decrement the current int value by the one specified as an input parameter,
        /// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/DecrementIntValue">https://app.cryptolens.io/docs/api/v3/DecrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult DecrementIntValue(string token, ChangeIntValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/decrementintvalue/", token);
        }


        /// <summary>
        /// This method will decrement the current int value by the one specified as an input parameter,
        /// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/DecrementIntValue">https://app.cryptolens.io/docs/api/v3/DecrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult DecrementIntValue(string token, ChangeIntValueToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/decrementintvaluetokey/", token);
        }

        /// <summary>
        /// This method will remove an existing data object.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/RemoveDataObject">https://app.cryptolens.io/docs/api/v3/RemoveDataObject</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult RemoveDataObject(string token, RemoveDataObjectModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/removedataobject/", token);
        }

        /// <summary>
        /// This method will remove an existing data object.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/RemoveDataObject">https://app.cryptolens.io/docs/api/v3/RemoveDataObject</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult RemoveDataObject(string token, RemoveDataObjectToKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/removedataobjecttokey/", token);
        }
    }
}
