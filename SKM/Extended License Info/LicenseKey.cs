using SKM.V3.Internal;
using SKM.V3.Methods;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SKM.V3
{
    public class LicenseKey
    {
        public int ProductId { get; set; }

        public int ID { get; set; }

        public string Key { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public int Period { get; set; }

        public bool F1 { get; set; }
        public bool F2 { get; set; }
        public bool F3 { get; set; }
        public bool F4 { get; set; }
        public bool F5 { get; set; }
        public bool F6 { get; set; }
        public bool F7 { get; set; }
        public bool F8 { get; set; }

        public string Notes { get; set; }

        public bool Block { get; set; }

        public long GlobalId { get; set; }

        public CustomerData Customer { get; set; }

        public List<ActivationData> ActivatedMachines { get; set; }

        public bool TrialActivation { get; set; }

        public int MaxNoOfMachines { get; set; }

        public string AllowedMachines { get; set; }

        public List<DataObject> DataObjects { get; set; }

        public DateTime SignDate { get; set; }

        public string Signature { get; set; }


        /// <summary>
        /// Returns the number of days left for a given license (time left). This method is particularly useful 
        /// when KeyInfo is not updated regularly, because TimeLeft will not be affected (stay constant).
        /// If your implementation checks the license with the server periodically, this method should be used instead of TimeLeft.
        /// </summary>
        /// <returns></returns>
        public int DaysLeft()
        {
            return DaysLeft(false);

        }

        /// <summary>
        /// Returns the number of days left for a given license (time left). This method is particularly useful 
        /// when KeyInfo is not updated regularly, because TimeLeft will not be affected (stay constant).
        /// If your implementation checks the license with the server periodically, this method should be used instead of TimeLeft.
        /// </summary>
        /// <param name="zeroIfExpired">If true, when a license has expired, zero will be returned.</param>
        /// <returns></returns>
        public int DaysLeft(bool zeroIfExpired = false)
        {
            var days = Expires - DateTime.Today;

            if (zeroIfExpired)
                return days.Days < 0 ? 0 : days.Days;
            else
                return days.Days;

        }

        /// <summary>
        /// Creates a new <see cref="DataObject"/>.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="dataObject">The data object to add to the license key.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/AddDataObject">https://serialkeymanager.com/docs/api/v3/AddDataObject</a> </remarks>
        /// <returns>Returns the id of the data object (and updates the <see cref="DataObjects"/>) if successful, or -1 otherwise.</returns>
        public long AddDataObject(string token, DataObject dataObject)
        {
            var parameters = new AddDataObjectToKeyModel
            {
                IntValue = dataObject.IntValue,
                StringValue = dataObject.StringValue,
                Key = this.Key,
                Name = dataObject.Name,
                ProductId = ProductId
            };

            var result = Data.AddDataObject(token, parameters);

            if(result != null && result.Result == ResultType.Success)
            {
                dataObject.Id = result.Id;
                DataObjects.Add(dataObject);
                return dataObject.Id;
            }
            return -1;
        }

        /// <summary>
        /// This method will remove an existing data object.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool RemoveDataObject(string token)
        {
            var parameters = new RemoveDataObjectToKeyModel
            {
                ProductId = this.ProductId,
                Key = this.Key,
         
            };

            var result = Data.RemoveDataObject(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the new version of this license from SKM. By default, if the <see cref="Signature"/> field
        /// ís not null and not empty, SKM will sign the new data too. You can also require this explicitly
        /// by calling <see cref="Refresh(string, bool)"/>, and setting the second parameter to 'true'.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <remarks>This method uses <see cref="GetKeysModel"/></remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool Refresh(string token)
        {
            if (Signature != null && Signature != "")
            {
                return Refresh(token, true);
            }
            return Refresh(token, false);
        }

        /// <summary>
        /// Gets the new version of this license from SKM.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="sign">If true, SKM will sign the current license key object.</param>
        /// <remarks>This method uses <see cref="GetKeysModel"/></remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool Refresh(string token, bool sign)
        {
            var parameters = new KeyInfoModel
            {
                ProductId = ProductId,
                Key = Key,
                Sign = sign
            };

            var result = SKM.V3.Methods.Key.GetKey(token, parameters);

            if(result != null && result.Result == ResultType.Success)
            {
                this.ActivatedMachines = result.LicenseKey.ActivatedMachines;
                this.AllowedMachines = result.LicenseKey.AllowedMachines;
                this.Block = result.LicenseKey.Block;
                this.Created = result.LicenseKey.Created;
                this.Customer = result.LicenseKey.Customer;
                this.DataObjects = result.LicenseKey.DataObjects;
                this.Expires = result.LicenseKey.Expires;
                this.F1 = result.LicenseKey.F1;
                this.F2 = result.LicenseKey.F2;
                this.F3 = result.LicenseKey.F3;
                this.F4 = result.LicenseKey.F4;
                this.F5 = result.LicenseKey.F5;
                this.F6 = result.LicenseKey.F6;
                this.F7 = result.LicenseKey.F7;
                this.F8 = result.LicenseKey.F8;
                this.GlobalId = result.LicenseKey.GlobalId;
                this.ID = result.LicenseKey.ID;
                this.Key = result.LicenseKey.Key;
                this.MaxNoOfMachines = result.LicenseKey.MaxNoOfMachines;
                this.Notes = result.LicenseKey.Notes;
                this.Period = result.LicenseKey.Period;
                this.ProductId = result.LicenseKey.ProductId;
                this.Signature = result.LicenseKey.Signature;
                this.SignDate = result.LicenseKey.SignDate;
                this.TrialActivation = result.LicenseKey.TrialActivation;
                return true;
            }
            return false;
        
        }

        /// <summary>
        /// Gets the new version of this license from SKM. Note, you need to manually assign the new value to this object, eg,
        /// by license = license.Refresh("token");
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="feature">The feature number, eg. 1,2,...,8. </param>
        /// <returns>Returns a newer version of<see cref="LicenseKey"/> or null if unsuccessful.</returns>
        public LicenseKey AddFeature(string token, int feature)
        {
            var parameters = new FeatureModel
            {
                ProductId = ProductId,
                Key = Key,
                Feature = feature
                
            };

            var result = Methods.Key.AddFeature(token, parameters);

            
            //if (result != null && result.Result == ResultType.Success )
            //{
            //    ExtensionMethods.SetFeatureByNumber()
            //}
            return null;
        }



    }
}
