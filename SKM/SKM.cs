using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;

using System.Management;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

[assembly: AllowPartiallyTrustedCallers()]
[assembly: CLSCompliant(true)]
namespace SKGL
{
    /// <summary>
    /// This class contains additional methods to ease serial key validation with Serial Key Manager. For definitions of some variables, please go to http://serialkeymanager.com/Key/ProductValidation.
    /// </summary>
    public static class SKM
    {
        #region TimeCheck
        /// <summary>
        /// This method checks whether the network time is different from the local time (client computer). This helps to prevent date changes caused by a client.
        /// </summary>
        /// <returns>Returns FALSE if time was NOT changed and TRUE if the time was changed.</returns>
        public static bool TimeCheck()
        {
            if (GetNetworkTime().ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                // continue validation after this
                return false;
            }
            else
            {
                // the date was altered. stop validation.
                return true;
            }

        }
        private static DateTime GetNetworkTime()
        {
            //From: http://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c
            //By: @Nasreddine

            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = System.Net.Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new System.Net.IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        // stackoverflow.com/a/3294698/162671
        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        #endregion

        #region KeyValidation
        /// <summary>
        /// This method will check whether the key is valid or invalid against the Serial Key Manager database
        /// The method will return TRUE only if:
        ///  * the key exists in the database (it has been generated)
        ///  * the key is not blocked
        /// </summary>
        /// <param name="pid">pid</param>
        /// <param name="uid">uid</param>
        /// <param name="hsum">hsum</param>
        /// <param name="sid">Serial Key that is to be validated</param>
        /// <param name="secure">If true, the key information will contain a signature of itself that you can validate with IsKeyInformationGenuine</param>
        /// <returns>KeyInformation</returns>
        public static KeyInformation KeyValidation(string pid, string uid, string hsum, string sid, bool secure=false)
        {

            Dictionary<string,string> input = new Dictionary<string,string>();
            input.Add("uid", uid);
            input.Add("pid", pid);
            input.Add("hsum", hsum);
            input.Add("sid", sid);
            input.Add("sign", secure.ToString());

            var result = GetParameters(input, "Validate");

            if(result.ContainsKey("error"))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: " + result["error"]);
#endif
                return null;
            }

            return GetKeyInformationFromParameters(result);

        }


        /// <summary>
        /// This method will check whether the key is valid or invalid against the Serial Key Manager database.
        /// The method will return TRUE only if:
        ///  * the key exists in the database (it has been generated)</summary>
        ///  * the key is not blocked
        ///  * the machine code that is activated has not been activated before
        ///  * the limit for maximum number of machine codes has not been achieved
        ///  * the machine code exists in the Allowed Machine codes.
        ///  NOTE: In Addition, depending on the settings, this method will activate a machine code.
        /// <param name="pid">pid</param>
        /// <param name="uid">uid</param>
        /// <param name="hsum">hsum</param>
        /// <param name="sid">Serial Key that is to be validated</param>
        /// <param name="mid">Machine code</param>
        /// <param name="json">If true, additional information is returned in JSON format</param>
        /// <param name="secure">If true, the key information will contain a signature of itself that you can validate with IsKeyInformationGenuine</param>
        /// <param name="signMid">if set to true, the mid parameter will be included into the signature (requires secure to be true)</param>
        /// <returns>Returns TRUE if the key follows the defined rules.</returns>
        public static KeyInformation KeyActivation(string pid, string uid, string hsum, string sid, string mid, bool secure = false, bool signMid = false )
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("uid", uid);
            input.Add("pid", pid);
            input.Add("hsum", hsum);
            input.Add("mid", mid);
            input.Add("sid", sid);
            input.Add("sign", secure.ToString());
            input.Add("signMid", signMid.ToString());

            var result = GetParameters(input, "Activate");

            if (result.ContainsKey("error"))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: " + result["error"]);
#endif
                return null;
            }

            return GetKeyInformationFromParameters(result);
            
        }

        /// <summary>
        /// This method allows you to check if the key information (creation date, expiration date, etc.) in a request was modified on the way from Serial Key Manager server to the client application.
        /// </summary>
        /// <param name="keyInformation">The variable that contains the key information (including the signature)</param>
        /// <param name="rsaPublicKey">The public key (RSA)</param>
        /// <returns>True, if no changes were detected. False, otherwise.</returns>
        public static bool IsKeyInformationGenuine(KeyInformation keyInformation, string rsaPublicKey)
        {
            byte[] data = GetBytes(keyInformation.Valid.ToString() + keyInformation.CreationDate.ToString("yyyy-MM-dd") + keyInformation.ExpirationDate.ToString("yyyy-MM-dd")
                 + keyInformation.SetTime.ToString() + keyInformation.TimeLeft.ToString() + 
                 keyInformation.Features[0].ToString() +
                 keyInformation.Features[1].ToString() +
                 keyInformation.Features[2].ToString() +
                 keyInformation.Features[3].ToString() +
                 keyInformation.Features[4].ToString() +
                 keyInformation.Features[5].ToString() +
                 keyInformation.Features[6].ToString() +
                 keyInformation.Features[7].ToString() +
                 (keyInformation.Notes == null ? "": keyInformation.Notes) + 
                 (keyInformation.Mid == null ? "" : keyInformation.Mid)
                 );

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

            rsa.FromXmlString(rsaPublicKey);

            byte[] signature = Convert.FromBase64String(keyInformation.Signature);

            return rsa.VerifyData(data, "SHA256", signature);
            
        }

        /// <summary>
        /// This method saves all information inside key information into a file.
        /// </summary>
        /// <param name="keyInformation">The key infromation that should be saved into a file</param>
        /// <param name="file">The entire path including file name, i.e. c:\folder\file.txt</param>
        public static void SaveKeyInformationToFile( KeyInformation keyInformation, string file)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var fs = new System.IO.FileStream (file, System.IO.FileMode.OpenOrCreate);
            bf.Serialize(fs, keyInformation);
            fs.Close();
        }
        /// <summary>
        /// This method loads key information stored in a file into a key information variable.
        /// </summary>
        /// <param name="file">The entire path including file name, i.e. c:\folder\file.txt</param>
        /// <returns></returns>
        public static KeyInformation LoadKeyInformationFromFile(string file)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var fs = new System.IO.FileStream(file, System.IO.FileMode.Open);
            KeyInformation keyInfo = (KeyInformation)bf.Deserialize(fs);
            fs.Close();
            return keyInfo;
        }

        // useful snippettes by @Mehrdad
        // http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        #endregion

        #region OtherAPIRequests

        /// <summary>
        /// This method will take in a set of parameters (input parameters) and send them to the given action. You can find them here: http://docs.serialkeymanager.com/web-api/
        /// </summary>
        /// <param name="inputParameters">A dictionary that contains data such as "uid", "pid", etc.</param>
        /// <param name="typeOfAction">A string that tells what to do, i.e. "validate", "activate" etc.</param>
        /// <returns>A dictionary of the JSON elements returned for that particular request.</returns>
        public static Dictionary<string, string> GetParameters(Dictionary<string, string> inputParameters, string typeOfAction)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();

                foreach (var input in inputParameters)
                {
                    reqparm.Add(input.Key, input.Value);
                }

                client.Proxy = WebRequest.DefaultWebProxy;
                client.Credentials = System.Net.CredentialCache.DefaultCredentials;
                client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                byte[] responsebytes = client.UploadValues("https://serialkeymanager.com/Ext/" + typeOfAction, "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(responsebody);

            }

        }

        /// <summary>
        /// This method will interpret the input from the dictionary that was returned through "GetParameters" method, if the action was either "activate" or "validate".
        /// </summary>
        /// <param name="parameters">The dictionary array returned in "GetParameters" method</param>
        /// <returns>A Key Information object</returns>
        public static KeyInformation GetKeyInformationFromParameters(Dictionary<string,string> parameters)
        {
            var ki = new KeyInformation();

            if(parameters.ContainsKey("error"))
            {
                return null;
            }

            ki.Valid = true;
            ki.CreationDate = Convert.ToDateTime(parameters["created"]);
            ki.ExpirationDate = Convert.ToDateTime(parameters["expires"]);
            ki.SetTime = Convert.ToInt32(parameters["settime"]);
            ki.TimeLeft = Convert.ToInt32(parameters["timeleft"]);
            ki.Features = new bool[] { Convert.ToBoolean(parameters["f1"]), Convert.ToBoolean(parameters["f2"]),
                                       Convert.ToBoolean(parameters["f3"]), Convert.ToBoolean(parameters["f4"]),
                                       Convert.ToBoolean(parameters["f5"]), Convert.ToBoolean(parameters["f6"]),
                                       Convert.ToBoolean(parameters["f7"]), Convert.ToBoolean(parameters["f8"])};
            if(parameters.ContainsKey("notes"))
            {
                ki.Notes = parameters["notes"];
            }

            if(parameters.ContainsKey("signature"))
            {
                ki.Signature = parameters["signature"];
            }

            if (parameters.ContainsKey("newkey"))
            {
                ki.NewKey = parameters["newkey"];
            }

            if (parameters.ContainsKey("mid"))
            {
                ki.Mid = parameters["mid"];
            }


            return ki;

        }

        /// <summary>
        /// Lists all your products associated with your account. Each product name is accompanied with a product id.
        /// </summary>
        /// <param name="username">Your username</param>
        /// <param name="password">Your password</param>
        /// <returns>All products as a dictionary. The "key" is the product name and the "value" is the product id.</returns>
        public static Dictionary<string, string> ListUserProducts(string username, string password)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();
                reqparm.Add("usern", username);
                reqparm.Add("passw", password);

                client.Proxy = WebRequest.DefaultWebProxy;
                client.Credentials = System.Net.CredentialCache.DefaultCredentials;
                client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                byte[] responsebytes = client.UploadValues("https://serialkeymanager.com/Ext/ListProducts", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,string>>(responsebody);
                
            }   
        }

        /// <summary>
        /// Returns the product variables such as "uid", "pid", and "hsum".
        /// </summary>
        /// <param name="username">Your username</param>
        /// <param name="password">Your password</param>
        /// <param name="productID">The desired product ID</param>
        /// <returns>The "uid","pid", and "hsum" variables</returns>
        public static ProductVariables GetProductVariables(string username, string password, string productID)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();
                reqparm.Add("usern", username);
                reqparm.Add("passw", password);
                reqparm.Add("productid", productID);

                client.Proxy = WebRequest.DefaultWebProxy;
                client.Credentials = System.Net.CredentialCache.DefaultCredentials;
                client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                byte[] responsebytes = client.UploadValues("https://serialkeymanager.com/Ext/GetProductVariables", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,string>>(responsebody);

                return new ProductVariables() { ProductName = obj["productName"], UID = obj["uid"], PID = obj["pid"] , HSUM = obj["hsum"]};
            }  
        }

        #endregion

        #region NewMachineCode

        /// <summary>
        /// This method will calculates a machine code
        /// </summary>
        /// <param name="hashFunction">The hash function that is to be used. getEightByteHash can be used as a default hash function.</param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public static string getMachineCode(Func<string,string> hashFunction)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            string collectedInfo = "";
            // here we will put the informa
            foreach (ManagementObject share in searcher.Get())
            {
                // first of all, the processorid
                collectedInfo += share.GetPropertyValue("ProcessorId");
            }

            searcher.Query = new ObjectQuery("select * from Win32_BIOS");
            foreach (ManagementObject share in searcher.Get())
            {
                //then, the serial number of BIOS
                collectedInfo += share.GetPropertyValue("SerialNumber");
            }

            searcher.Query = new ObjectQuery("select * from Win32_BaseBoard");
            foreach (ManagementObject share in searcher.Get())
            {
                //finally, the serial number of motherboard
                collectedInfo += share.GetPropertyValue("SerialNumber");
            }

            // patch luca bernardini
            if (string.IsNullOrEmpty(collectedInfo) | collectedInfo == "00" | collectedInfo.Length <= 3)
            {
                collectedInfo += getHddSerialNumber();
            }

            return hashFunction(collectedInfo);
        }

        [SecuritySafeCritical]
        private static string getHddSerialNumber()
        {
            // --- Win32 Disk 
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\cimv2", "select * from Win32_DiskPartition WHERE BootPartition=True");

            uint diskIndex = 999;
            foreach (ManagementObject partition in searcher.Get())
            {
                diskIndex = Convert.ToUInt32(partition.GetPropertyValue("DiskIndex")); // should be DiskIndex
                break; // TODO: might not be correct. Was : Exit For
            }

            // I haven't found the bootable partition. Fail.
            if (diskIndex == 999)
                return string.Empty;



            // --- Win32 Disk Drive
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive where Index = " + diskIndex.ToString());

            string deviceName = "";
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                deviceName = wmi_HD.GetPropertyValue("Name").ToString();
                break; // TODO: might not be correct. Was : Exit For
            }


            // I haven't found the disk drive. Fail
            if (string.IsNullOrEmpty(deviceName.Trim()))
                return string.Empty;

            // -- Some problems in query parsing with backslash. Using like operator
            if (deviceName.StartsWith("\\\\.\\"))
            {
                deviceName = deviceName.Replace("\\\\.\\", "%");
            }


            // --- Physical Media
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia WHERE Tag like '" + deviceName + "'");
            string serial = string.Empty;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                serial = wmi_HD.GetPropertyValue("SerialNumber").ToString();
                break; // TODO: might not be correct. Was : Exit For
            }

            return serial;

        }

        /// <summary>
        /// This method will generate a 8 digit long hash which can be stored as an Int32.
        /// </summary>
        /// <param name="s">The string value of the infromation that is to be hashed.</param>
        /// <returns>A stiring with the hash value</returns>
        public static string getEightByteHash(string s)
        {
            //This function generates a eight byte hash

            //The length of the result might be changed to any length
            //just set the amount of zeroes in MUST_BE_LESS_THAN
            //to any length you want
            uint hash = 0;

            int MUST_BE_LESS_THAN = 100000000;//1000000;

            foreach (byte b in System.Text.Encoding.Unicode.GetBytes(s))
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            int result = (int)(hash % MUST_BE_LESS_THAN);
            int check = MUST_BE_LESS_THAN / result;

            if (check > 1)
            {
                result *= check;
            }

            return result.ToString();
        }


        #endregion

    }
}
