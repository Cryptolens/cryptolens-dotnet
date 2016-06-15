using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Internal
{
    /// <summary>
    /// Methods that perform operations on a data object. A complete list
    /// can be found here: https://serialkeymanager.com/docs/api/v3/Data
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Creates a new <see cref="DataObject"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/AddDataObject">https://serialkeymanager.com/docs/api/v3/AddDataObject</a> </remarks>
        /// <returns>Returns <see cref="DataObjectIdResult"/> or null.</returns>
        public static DataObjectIdResult AddDataObject(string token, AddDataObjectModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<DataObjectIdResult>(parameters, "/data/adddataobject/", token);
        }

        /// <summary>
        /// This method lists either all Data Object associated with a
        /// license key, a product or your entire account, or all of them at once.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/ListDataObjects">https://serialkeymanager.com/docs/api/v3/ListDataObjects</a> </remarks>
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
        /// This method will set the int value to a new one.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/SetIntValue">https://serialkeymanager.com/docs/api/v3/SetIntValue</a> <br/>
        /// Note also: Integer overflows are not allowed. If you attempt to assign an int value that is beyond the limits of an int32, zero will be assigned to the data object's IntValue.</remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult SetIntValue(string token, ChangeIntValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setintvalue/", token);
        }

        /// <summary>
        /// This method will set the string value to a new one.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/SetStringValue">https://serialkeymanager.com/docs/api/v3/SetStringValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult SetStringValue(string token, ChangeStringValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setstringvalue/", token);
        }


        /// <summary>
        /// This method will increment the current int value by the one specified as an input parameter,
        /// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult IncrementIntValue(string token, ChangeIntValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/incrementintvalue/", token);
        }


        /// <summary>
        /// This method will decrement the current int value by the one specified as an input parameter,
        /// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/DecrementIntValue">https://serialkeymanager.com/docs/api/v3/DecrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult DecrementIntValue(string token, ChangeIntValueModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/decrementintvalue/", token);
        }

        /// <summary>
        /// This method will remove an existing data object.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/RemoveDataObject">https://serialkeymanager.com/docs/api/v3/RemoveDataObject</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public static BasicResult RemoveDataObject(string token, RemoveDataObjectModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/removedataobject/", token);
        }
    }
}
