using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;

#if SYSTEM_MANAGEMENT
using System.Management;
#endif

using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

using SKM.V3.Models;
using SKM.V3.Internal;
using SKM.V3;

[assembly: AllowPartiallyTrustedCallers()]
[assembly: CLSCompliant(true)]
/// <summary>
/// Methods that are related to Web API 2 and other helper methods.
/// </summary>
namespace SKGL
{

    /// <summary>
    ///   <para>
    ///     Methods that that are used to communicate with Web API 2. For newer projects, please avoid
    ///     using this namespace. You can still use <see cref="SKGL.SKM.getMachineCode(Func{string, string}, bool)"/>,
    ///     <see cref="SKM.TimeCheck"/> and <see cref="SKM.getSHA256(string)"/>.
    ///   </para>
    /// </summary>
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// This class contains additional methods to ease serial key validation with Serial Key Manager. 
    /// Most of the methods will Web API 2, where "uid", "pid" and "hsum" are required in each request.
    /// These can be found here https://serialkeymanager.com/docs/api/v2/Activate (please make sure you are logged in).
    /// In addition to this, you need to explicitly set each product to be IsPublic and, for some methods,
    /// enable the functionality on your Security page (https://serialkeymanager.com/User/Security). 
    /// RSA public keys and your private key can also be found on the Security page.<br/>
    /// For Web API 3, you only need one token. You can find information on how it is generated here:
    /// https://serialkeymanager.com/docs/api/v3/Auth.
    /// </summary>
    /// <remarks>In Debug mode, the error is going to be displayed in the Output Window.
    /// </remarks>
    public static class SKM
    {
#region TimeCheck
        /// <summary>
        /// This method checks whether the network time is different from the local time (client computer). This helps to prevent date changes caused by a client.
        /// </summary>
        /// <example>
        /// The following code demonstrances the way TimeCheck can be used.
        /// <code language="cs">
        /// public void HasLocalTimeChanged()
        /// {
        ///    bool hasChanged = SKGL.SKM.TimeCheck();
        ///
        ///    if(hasChanged)
        ///    {
        ///        Debug.WriteLine("The local time was changed by the user. Validation fails.");
        ///    }
        ///    else
        ///    {
        ///        Debug.WriteLine("The local time hasn't been changed. Continue validation.");
        ///    }
        /// }
        /// </code>
        /// </example>
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

        /// <summary>
        /// Returns the number of days left for a given license (time left). This method is particularly useful 
        /// when KeyInfo is not updated regularly, because TimeLeft will not be affected (stay constant).
        /// If your implementation checks the license with the server periodically, this method should be used instead of TimeLeft.
        /// </summary>
        /// <param name="keyInfo"></param>
        /// <returns></returns>
        [Obsolete]
        public static int DaysLeft (KeyInformation keyInfo)
        {
            return DaysLeft(keyInfo, false);

        }

        /// <summary>
        /// Returns the number of days left for a given license (time left). This method is particularly useful 
        /// when KeyInfo is not updated regularly, because TimeLeft will not be affected (stay constant).
        /// If your implementation checks the license with the server periodically, this method should be used instead of TimeLeft.
        /// </summary>
        /// <param name="keyInfo"></param>
        /// <param name="zeroIfExpired">If true, when a license has expired, zero will be returned.</param>
        /// <returns></returns>
        [Obsolete]
        public static int DaysLeft(KeyInformation keyInfo, bool zeroIfExpired = false)
        {
            var days = keyInfo.ExpirationDate - DateTime.Today;

            if (zeroIfExpired)
                return days.Days < 0 ? 0 : days.Days;
            else
                return days.Days;

        }

#endregion

#region KeyValidation
        /// <summary>
        /// This method will check whether the key is valid or invalid against the Serial Key Manager database
        /// The method will return an object (KeyInformation) only if:<br/>
        ///  * the key exists in the database (it has been generated)<br/>
        ///  * the key is not blocked
        /// </summary>
        /// <param name="pid">pid</param>
        /// <param name="uid">uid</param>
        /// <param name="hsum">hsum</param>
        /// <param name="sid">Serial Key that is to be validated</param>
        /// <param name="secure">If true, the Key Information will contain a signature of itself that you can validate with IsKeyInformationGenuine</param>
        /// <param name="signPid">If true, the Key Information object will contain the Pid field. (Note, secure has to be true, since otherwise Pid will not be included into the signature.)</param>
        /// <param name="signUid">If true, the Key Information object will contain the Uid field. (Note, secure has to be true, since otherwise Uid will not be included into the signature.)</param>
        /// <param name="signDate">If true, the Key Information object will contain the Date field. (when validation was performed). (Note, secure has to be true, since otherwise Date will not be included into the signature.)</param>
        /// <remarks>In Debug mode, the error is going to be displayed in the Output Window.
        /// </remarks>
        /// <example>
        /// For pid, uid and hsum, please see <a href="https://serialkeymanager.com/Ext/Val">https://serialkeymanager.com/Ext/Val</a>. You can retreive them using <see cref="GetProductVariables"/>.
        /// <code language="cs">
        /// public void KeyValidation()
        /// {
        ///    var validationResult = SKGL.SKM.KeyValidation("pid", "uid", "hsum", "serial key to validate", "machine code", {sign the data}, {sign machine code});
        ///
        ///    if (validationResult.IsValid())
        ///    {
        ///        //valid key
        ///        var created = validationResult.CreationDate;
        ///        var expires = validationResult.ExpirationDate;
        ///        var setTime = validationResult.SetTime;
        ///        var timeLeft = validationResult.TimeLeft;
        ///        var features = validationResult.Features;
        ///    }
        ///    else
        ///    {
        ///        //invalid key
        ///        Assert.Fail();
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <returns>KeyInformation or null.</returns>
        [Obsolete]
        public static KeyInformation KeyValidation(string pid, string uid, string hsum, string sid, bool secure=false, bool signPid=false, bool signUid=false, bool signDate = false)
        {

            Dictionary<string,string> input = new Dictionary<string,string>();
            input.Add("uid", uid);
            input.Add("pid", pid);
            input.Add("hsum", hsum);
            input.Add("sid", sid);
            input.Add("sign", secure.ToString());
            input.Add("signPid", signPid.ToString());
            input.Add("signUid", signUid.ToString());
            input.Add("signDate", signDate.ToString());

            var result = GetParameters(input, "Validate");

            if(result.ContainsKey("error"))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: " + result["error"]);
#endif
                return null;
            }

            var keydata = GetKeyInformationFromParameters(result);
            keydata.Key = sid;
            return keydata;

        }


        /// <summary>
        /// This method will check whether the key is valid or invalid against the Serial Key Manager database.
        /// The method will return an object (KeyInformation) only if:<br/>
        ///  * the key exists in the database (it has been generated)<br/>
        ///  * the key is not blocked<br/>
        ///  * the machine code that is activated has not been activated before<br/>
        ///  * the limit for maximum number of machine codes has not been achieved<br/>
        ///  * the machine code exists in the Allowed Machine codes.<br/>
        ///  NOTE: In Addition, depending on the settings, this method will activate a machine code.<br/>
        ///  </summary>
        /// <param name="pid">pid</param>
        /// <param name="uid">uid</param>
        /// <param name="hsum">hsum</param>
        /// <param name="sid">Serial Key that is to be validated</param>
        /// <param name="mid">Machine code</param>
        /// <param name="json">If true, additional information is returned in JSON format</param>
        /// <param name="secure">If true, the key information will contain a signature of itself that you can validate with IsKeyInformationGenuine</param>
        /// <param name="signMid">if set to true, the mid parameter will be included into the signature (requires secure to be true).  (Note, secure has to be true, since otherwise machine code (mid) will not be included into the signature.)</param>
        /// <param name="signPid">If true, the Key Information object will contain the Pid field. (Note, secure has to be true, since otherwise Pid will not be included into the signature.)</param>
        /// <param name="signUid">If true, the Key Information object will contain the Uid field. (Note, secure has to be true, since otherwise Uid will not be included into the signature.)</param>
        /// <param name="signDate">If true, the Key Information object will contain the Date field. (when validation was performed). (Note, secure has to be true, since otherwise Date will not be included into the signature.)</param>
        /// <remarks>In Debug mode, the error is going to be displayed in the Output Window.
        /// </remarks>
        /// <example>
        /// For pid, uid and hsum, please see <a href="https://serialkeymanager.com/Ext/Val">https://serialkeymanager.com/Ext/Val</a>. You can also retreive them using <see cref="GetProductVariables"/>. NB: If trial activation is configured, the API can return a new key (read more at <a href="http://support.serialkeymanager.com/kb/trial-activation/">http://support.serialkeymanager.com/kb/trial-activation/</a>).
        /// <code language="cs">
        /// public void KeyActivation()
        /// {
        ///    var validationResult = SKGL.SKM.KeyActivation("pid", "uid", "hsum", "serial key to validate", "machine code", {sign the data}, {sign machine code});
        ///
        ///    if (validationResult != null)
        ///    {
        ///        //valid key
        ///        var created = validationResult.CreationDate;
        ///        var expires = validationResult.ExpirationDate;
        ///        var setTime = validationResult.SetTime;
        ///        var timeLeft = validationResult.TimeLeft;
        ///        var features = validationResult.Features;
        ///    }
        ///    else
        ///    {
        ///        //invalid key
        ///        Assert.Fail();
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <returns>Returns a KeyInformation object if all rules were satisfied and null if an error occured.</returns>
        [Obsolete("Please use Key.Activate in SKM.V3.Methods.")]
        public static KeyInformation KeyActivation(string pid, string uid, string hsum, string sid, string mid, bool secure = false, bool signMid = false, bool signPid=false, bool signUid=false, bool signDate = false )
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("uid", uid);
            input.Add("pid", pid);
            input.Add("hsum", hsum);
            input.Add("mid", mid);
            input.Add("sid", sid);
            input.Add("sign", secure.ToString());
            input.Add("signMid", signMid.ToString());
            input.Add("signPid", signPid.ToString());
            input.Add("signUid", signUid.ToString());
            input.Add("signDate", signDate.ToString());

            var result = GetParameters(input, "Activate");

            if (result.ContainsKey("error"))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: " + result["error"]);
#endif
                return null;
            }

            var keydata = GetKeyInformationFromParameters(result);
            keydata.Key = sid;
            return keydata;
            
        }


        /// <summary>
        /// This method will attempt to de-activate a machine code from the given key.
        /// If the given machine code was de-activated, KeyInformation confirming the key and the machine code will be returned.
        /// If something went wrong, for instance, if the machine code did not exist, null will be returned.</summary>
        /// <param name="pid">pid</param>
        /// <param name="uid">uid</param>
        /// <param name="hsum">hsum</param>
        /// <param name="sid">Machine code's serial key.</param>
        /// <param name="mid">Machine code</param>
        /// <remarks>In Debug mode, the error is going to be displayed in the Output Window.<br/>
        /// Note: The key is going to be stored in "NewKey" field, while the machine code is going to be stored in "mid".
        /// </remarks>
        /// <example>
        /// The following code demonstrates activation of a machine code followed by its deactivation.
        /// <code language="cs">
        /// public void KeyDeactivationTest()
        /// {
        ///     // first, we need to activate a machine code. In this case, it's "artem123"
        ///     var activationResult = SKGL.SKM.KeyActivation("2196", "2", "749172", "KTDOU-JZQUY-NOJCU-ECTAA", "artem123");
        /// 
        ///     if(!activationResult.IsValid())
        ///     {
        ///         Assert.Fail("Unable to activate");
        ///     }
        /// 
        ///     // now, let's deactivate it:
        /// 
        ///     var deactivationResult = SKGL.SKM.KeyDeactivation("2196", "2", "749172", "KTDOU-JZQUY-NOJCU-ECTAA", "artem123");
        /// 
        ///     if(!deactivationResult.IsValid())
        ///     {
        ///         Assert.Fail("Unable to deactivate");
        ///     }
        /// 
        ///     // if we are here, the machine code "artem123" was successfully deactivated.
        /// 
        /// }
        /// </code>
        /// </example>
        /// <returns>Returns a KeyInformation object (with a key and machine code only) or null.</returns>

        [Obsolete("Please use Key.Deactivate in SKM.V3.Methods.")]
        public static KeyInformation KeyDeactivation(string pid, string uid, string hsum, string sid, string mid)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("uid", uid);
            input.Add("pid", pid);
            input.Add("hsum", hsum);
            input.Add("mid", mid);
            input.Add("sid", sid);

            var result = GetParameters(input, "Deactivate");

            if (result.ContainsKey("error"))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: " + result["error"]);
#endif
                return null;
            }

            return new KeyInformation() { Key = result["key"], NewKey = result["key"], Mid = result["mid"]};

        }

        /// <summary>
        /// This method will retrieve the list of activated machines for a given serial key. <br />
        /// If successful, a list of type ActivatedData will be returned. If an error occurs, null is returned.
        /// </summary>
        /// <param name="productVariables">The object that contains Uid, Pid and Hsum</param>
        /// <param name="sid">Serial Key that is to be validated</param>
        /// <param name="privateKey">The private key of the user.</param>
        /// <returns>A list of ActivatedData or null.</returns>
        [Obsolete]
        public static List<ActivationData> GetActivatedMachines(ProductVariables productVariables,string privateKey, string sid)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();

                reqparm.Add("uid", productVariables.UID);
                reqparm.Add("pid", productVariables.PID);
                reqparm.Add("hsum", productVariables.HSUM);
                reqparm.Add("sid", sid);
                reqparm.Add("privateKey", privateKey);

                // make sure .NET uses the default proxy set up on the client device.
                client.Proxy = WebRequest.DefaultWebProxy;
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                byte[] responsebytes = client.UploadValues("https://serialkeymanager.com/Ext/GetActivatedMachines", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                if(responsebody.StartsWith("{error:"))
                {
                    return null;
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActivationData>>(responsebody);

            }
        }
  

        /// <summary>
        /// This method allows you to check if the key information (creation date, expiration date, etc.) in a request was modified on the way from Serial Key Manager server to the client application.
        /// </summary>
        /// <param name="keyInformation">The variable that contains the key information (including the signature)</param>
        /// <param name="rsaPublicKey">The public key (RSA)</param>
        /// <example>
        /// The code below demonstrates how IsKeyInformationGenueine can be used in offine key validation. Please read more about offline key validation at <a href="http://support.serialkeymanager.com/kb/passive-key-validation/">http://support.serialkeymanager.com/kb/passive-key-validation/</a>.
        /// <code language="cs">
        /// public static void OfflineKeyValidationWithPeriodicTimeCheck()
        /// {
        ///    var RSAPublicKey = "RSA public key";
        ///
        ///    var keyInfo = new KeyInformation().LoadFromFile("license2.txt");
        ///
        ///    if (keyInfo.HasValidSignature(RSAPublicKey, 30)
        ///               .IsOnRightMachine()
        ///               .IsValid())
        ///    {
        ///        // the signature is correct so
        ///        // the program can now launch
        ///    }
        ///    else
        ///    {
        ///        var machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
        ///        keyInfo = SKGL.SKM.KeyActivation("3", "2", "751963", "MJAWL-ITPVZ-LKGAN-DLJDN", machineCode, secure: true, signMid: true, signDate: true);
        ///
        ///        if (keyInfo.HasValidSignature(RSAPublicKey)
        ///                   .IsOnRightMachine()
        ///                   .IsValid())
        ///        {
        ///            // the signature is correct and the key is valid.
        ///            // save to file.
        ///            keyInfo.SaveToFile("license2.txt");
        ///
        ///            // the program can now launch
        ///        }
        ///        else
        ///        {
        ///            // failure. close the program.
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <returns>True, if no changes were detected. False, otherwise.</returns>
        public static bool IsKeyInformationGenuine(KeyInformation keyInformation, string rsaPublicKey)
        {
            if(keyInformation == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: The KeyInformation object is null. False result will be returned.");
#endif
                return false;
            }

            // try catch require more, so better to simply check if features are null, etc.
            try
            {
                byte[] data = HelperMethods.GetBytes(keyInformation.Valid.ToString() + keyInformation.CreationDate.ToString("yyyy-MM-dd") + keyInformation.ExpirationDate.ToString("yyyy-MM-dd")
                     + keyInformation.SetTime.ToString() + keyInformation.TimeLeft.ToString() +
                     keyInformation.Features[0].ToString() +
                     keyInformation.Features[1].ToString() +
                     keyInformation.Features[2].ToString() +
                     keyInformation.Features[3].ToString() +
                     keyInformation.Features[4].ToString() +
                     keyInformation.Features[5].ToString() +
                     keyInformation.Features[6].ToString() +
                     keyInformation.Features[7].ToString() +
                     (keyInformation.Notes == null ? "" : keyInformation.Notes) +
                     (keyInformation.Mid == null ? "" : keyInformation.Mid) +
                     (keyInformation.Pid == null ? "" : keyInformation.Pid) +
                     (keyInformation.Uid == null ? "" : keyInformation.Uid) +
                     (keyInformation.Date == null ? "" : keyInformation.Date.Value.ToString("yyy-MM-dd")) +
                     (keyInformation.Customer == null ? "" : keyInformation.Customer.Value.ToString())
                     );

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

                rsa.FromXmlString(rsaPublicKey);

                byte[] signature = Convert.FromBase64String(keyInformation.Signature);

                return rsa.VerifyData(data, "SHA256", signature);
            }
            catch
            {
                return false;
            }
            
        }

        /// <summary>
        /// This method saves all information inside key information into a file.
        /// </summary>
        /// <param name="keyInformation">The key infromation that should be saved into a file</param>
        /// <param name="file">The entire path including file name, i.e. c:\folder\file.txt</param>
        /// <param name="json">Save the file using JSON format.</param>
        /// <returns>If successful, true will be returned. False otherwise.</returns>
        /// <remarks>This method does not use the same JSON format structure as activation files. Instead,
        /// if you want to read these files using <see cref="LoadKeyInformationFromFile"/>, then activationFile has
        /// to be set to FALSE.</remarks>
        public static bool SaveKeyInformationToFile(KeyInformation keyInformation, string file, bool json=false)
        {
            if (json)
            {
                System.IO.StreamWriter sw = null;
                bool state =false;
                try
                {
                    sw = new System.IO.StreamWriter(file);
                    sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(keyInformation));
                    state = true;
                }
                catch {
                    state = false;

                }
                finally
                {
                    if (sw != null)
                        sw.Dispose();
                }

                return state;

            }
            else
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.FileStream fs = null;

                bool state = false;

                try
                {
                    fs = new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate);
                    bf.Serialize(fs, keyInformation);
                    state = true;
                }
                catch (Exception e)
                {
                    state = false;
                }
                finally
                {
                    if (fs != null)
                        fs.Dispose();
                }

                return state;
            }
        }
        /// <summary>
        /// This method loads key information stored in a file into a key information variable.
        /// </summary>
        /// <param name="file">The entire path including file name, i.e. c:\folder\file.txt</param>
        /// <param name="json">If the file is stored in JSON (eg. an activation file with .skm extension), set this parameter to TRUE.</param>
        /// <param name="activationFile">If you obtained this file from an Activation Form (.skm extension), this should be set to true.</param>
        /// <remarks>If you want to read a file that uses the JSON format created by <see cref="SaveKeyInformationToFile"/>, activationFile has to be set to FALSE while
        /// json is set to TRUE.</remarks>
        /// <returns>If successful, this method returns a KeyInformation object. Null otherwise.</returns>
        public static KeyInformation LoadKeyInformationFromFile(string file, bool json = false, bool activationFile = false)
        {
            if (json || activationFile)
            {
                if (activationFile)
                {
                    System.IO.StreamReader sr = null;
                    Dictionary<string, string> ki = null;
                    try
                    {
                        sr = new System.IO.StreamReader(file);
                        ki = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                    }
                    catch { }
                    finally
                    {
                        if (sr != null)
                            sr.Dispose();
                    }

                    if (ki == null)
                    {
                        return null;
                    }
                    else
                    {
                        return GetKeyInformationFromParameters(ki);
                    }
                }
                else
                {
                    System.IO.StreamReader sr = null;
                    KeyInformation ki = null;
                    try
                    {
                        sr = new System.IO.StreamReader(file);
                        ki = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyInformation>(sr.ReadToEnd());
                    }
                    catch { }
                    finally
                    {
                        if (sr != null)
                            sr.Dispose();
                    }

                    return ki;
                }
            }
            else
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.FileStream fs = null;

                KeyInformation keyInfo  = null;
                try
                {
                    fs = new System.IO.FileStream(file, System.IO.FileMode.Open);
                    keyInfo = (KeyInformation)bf.Deserialize(fs);
                }
                catch { }
                finally
                {
                    if (fs != null)
                        fs.Dispose();
                }
              
                return keyInfo;
            }
        }

#endregion

#region OtherAPIRequests

        public static string GenerateKey()
        {
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productVariables">The object that contains Uid, Pid and Hsum</param>
        /// <param name="sid">Serial Key that is to be validated</param>
        /// <param name="todo">Action to perform. Either Get or Set.</param>
        /// <param name="decrement">If <paramref name="todo"/> is set to "Set", this method will try to decrease the current value of the optional field by this value.<br/>Note, it has to be a positive integer.</param>
        /// <example>
        /// The code below first checks the value of the optional field and then decreases it by 1. The Assert.True will be true in this case.
        /// <code language="cs">
        /// public void TestOptionalField()
        /// {
        ///     // let's assume that the following key has an optional field of the value 5.
        ///     // edit: this will pass several thousand times. then, it has to be increased again.
        /// 
        ///     var productVariables = new SKGL.ProductVariables() { UID = "2", PID = "2196", HSUM = "749172" };
        /// 
        ///     int currentvalue = SKGL.SKM.OptionalField(productVariables, "KTDOU-JZQUY-NOJCU-ECTAA");
        /// 
        ///     int newValue = SKGL.SKM.OptionalField(productVariables, "KTDOU-JZQUY-NOJCU-ECTAA", SKGL.SKM.Todo.Set, 1);
        /// 
        ///     Assert.IsTrue(newValue == currentvalue - 1);
        /// 
        /// }
        /// </code>
        /// </example>
        /// <returns>An intger that is currently stored in the optional field.</returns>
        public static int OptionalField(ProductVariables productVariables, string sid, Todo todo = Todo.Get, int decrement = 0)
        {

            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("uid", productVariables.UID);
            input.Add("pid", productVariables.PID);
            input.Add("hsum", productVariables.HSUM);
            input.Add("sid", sid);

            input.Add("todo", todo == Todo.Get ? "get" : "set");
            input.Add("decrement", decrement.ToString());

            var result = GetParameters(input, "OptionalField");

            if (result.ContainsKey("error"))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Error: " + result["error"]);
#endif
                return 0;
            }

            return Convert.ToInt32(result["optva"]);

        }

        /// <summary>
        /// Action to perform when using the <see cref="OptionalField"/>.
        /// </summary>
        public enum Todo
        {
            /// <summary>
            /// This will retrieve the current value stored in the optional field.
            /// </summary>
            Get,
            /// <summary>
            /// This will attempt to decrease the optional field value by the value in "decrement". The new value will then be returned.
            /// </summary>
            Set
        }


        /// <summary>
        /// This method will take in a set of parameters (input parameters) and send them to the given action. You can find them here: <a href="http://docs.serialkeymanager.com/web-api/">http://docs.serialkeymanager.com/web-api/</a>.
        /// </summary>
        /// <param name="inputParameters">A dictionary that contains data such as "uid", "pid", etc.</param>
        /// <param name="typeOfAction">A string that tells what to do, i.e. "validate", "activate" etc.</param>
        /// <param name="proxy">(Optional) The proxy settings.</param>
        /// <example>
        /// If you would like to access a method in the Web API manually, please use GetParameters method. A list of them can be found at <a href="http://docs.serialkeymanager.com/web-api/">http://docs.serialkeymanager.com/web-api/</a>.
        /// <code language="cs">
        /// public void GetParamtersExample()
        /// {
        ///    var input = new System.Collections.Generic.Dictionary&lt;string, string &gt;();
        ///    input.Add("uid", "1");
        ///    input.Add("pid", "1");
        ///    input.Add("hsum", "11111");
        ///    input.Add("sid", "ABCD-EFGHI-GKLMN-OPQRS");
        ///    input.Add("sign","true");
        ///
        ///    var result = SKGL.SKM.GetParameters(input, "Validate");
        ///
        ///    var keyinfo = SKGL.SKM.GetKeyInformationFromParameters(result);
        ///
        ///    if(result.ContainsKey("error") &amp;&amp; result["error"] != "")
        ///    {
        ///        // if we are here, something went wrong.
        ///    }
        /// }
        /// </code>
        /// </example>
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

                // make sure .NET uses the default proxy set up on the client device.
                client.Proxy = WebRequest.DefaultWebProxy;
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;

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

            if (parameters.ContainsKey("pid"))
            {
                ki.Pid = parameters["pid"];
            }

            if (parameters.ContainsKey("uid"))
            {
                ki.Uid = parameters["uid"];
            }

            if (parameters.ContainsKey("date"))
            {
                // risky here.
                ki.Date = DateTime.Parse(parameters["date"]);
            }

            if (parameters.ContainsKey("customer"))
            {
                ki.Customer = Convert.ToInt32(parameters["customer"]);
            }

            return ki;

        }

        /// <summary>
        /// Lists all your products associated with your account. Each product name is accompanied with a product id.
        /// </summary>
        /// <param name="username">Your username</param>
        /// <param name="password">Your password</param>
        /// <param name="proxy">(Optional) The proxy settings.</param>
        /// <returns>All products as a dictionary. The "key" is the product name and the "value" is the product id.</returns>
        public static Dictionary<string, string> ListUserProducts(string username, string password)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();
                reqparm.Add("usern", username);
                reqparm.Add("passw", password);

                //client.Credentials = System.Net.CredentialCache.DefaultCredentials;

                // make sure .NET uses the default proxy set up on the client device.
                client.Proxy = WebRequest.DefaultWebProxy;
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;

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
        /// <param name="proxy">(Optional) The proxy settings.</param>
        /// <example>
        /// This will get pid, uid and hsum.
        /// <code language="cs">
        /// public void GetProductVariables()
        /// {
        ///    var listOfProducts = SKGL.SKM.ListUserProducts("username", "password");
        ///
        ///    //variables needed in for instance validation/activation
        ///    //note, First requires System.Linq.
        ///    var productVar = SKGL.SKM.GetProductVariables("username","password", listOfProducts.First().Value);
        ///
        ///    Debug.WriteLine("The uid=" + productVar.UID + ", pid=" + productVar.PID + " and hsum=" + productVar.HSUM);
        /// }
        /// </code>
        /// </example>
        /// <returns>The "uid","pid", and "hsum" variables</returns>
        public static ProductVariables GetProductVariables(string username, string password, string productID)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();
                reqparm.Add("usern", username);
                reqparm.Add("passw", password);
                reqparm.Add("productid", productID);

                //client.Credentials = System.Net.CredentialCache.DefaultCredentials;

                // make sure .NET uses the default proxy set up on the client device.
                client.Proxy = WebRequest.DefaultWebProxy;
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                byte[] responsebytes = client.UploadValues("https://serialkeymanager.com/Ext/GetProductVariables", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,string>>(responsebody);

                return new ProductVariables() { ProductName = obj["productName"], UID = obj["uid"], PID = obj["pid"] , HSUM = obj["hsum"]};
            }  
        }

        /// <summary>
        /// This method will load ProductVariables data from a json serialized string. (see Example below.)
        /// </summary>
        /// <param name="productVariablesString">The json version of Product Variables, i.e. {"uid":"111", "pid":"111", "hsum":"111"}</param>
        /// <example>
        /// An example of a string that contains the the serialized json string is shown below:
        /// <code language="cs">
        /// public void LoadProductVariablesFromString()
        /// {
        ///    var productVariables = SKGL.SKM.LoadProductVariablesFromString("{\"pid\":\"test\", \"uid\":\"test1\", \"hsum\":\"test2\"}");
        ///    Assert.AreEqual(productVariables.PID, "test");
        ///    Assert.AreEqual(productVariables.UID, "test1");
        ///    Assert.AreEqual(productVariables.HSUM, "test2");
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public static ProductVariables LoadProductVariablesFromString(string productVariablesString)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(productVariablesString);

            return new ProductVariables() { ProductName = obj.ContainsKey("productName") ? obj["productName"] : "", UID = obj["uid"], PID = obj["pid"], HSUM = obj["hsum"] };
        }

#endregion

#region WebAPI3

        ///// <summary>
        ///// This method will extend a license by a certain amount of days. 
        ///// If the key algorithm in the product is SKGL, the key string 
        ///// will be changed if necessary. Otherwise, if SKM15 is used, 
        ///// the key will stay the same.
        ///// If the key is changed, the new key will be stored in the message.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version.</param>
        ///// <param name="parameters">The parameters that the method needs.</param>
        ///// <example>
        ///// Here is an example that demonstrates the use of the method.
        ///// <code language="cs" title="C#">
        ///// public void ExtendLicenseExample()
        ///// {
        /////    var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 30, ProductId = 3349 };
        /////    var auth = new AuthDetails() { Token = "WyI0IiwiY0E3aHZCci9FWFZtOWJYNVJ5eTFQYk8rOXJSNFZ5TTh1R25YaDVFUiJd" };
        /////
        /////    var result = SKM.ExtendLicense(auth, keydata);
        /////
        /////    if (result != null &amp;&amp; result.Result == ResultType.Success)
        /////    {
        /////        // the license was successfully extended with 30 days.
        /////    }
        ///// }
        ///// </code>
        ///// </example>
        ///// <remarks>This method may, in rare cases, return null if an error has occurred.
        ///// Null should be seen as an unsuccessful result.
        ///// </remarks>
        ///// <returns>A BasicResult object or null.</returns>
        //public static BasicResult ExtendLicense(AuthDetails auth, ExtendLicenseModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/extendlicense/", auth.Token);
        //}


        ///// <summary>
        ///// Moved to SKM.V3 namespace into the Key class.
        ///// </summary>
        ///// <param name="auth"></param>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static BasicResult AddFeature(AuthDetails auth, FeatureModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/addfeature/", auth.Token);
        //}

        ///// <summary>
        ///// Moved to SKM.V3 namespace into the Key class.
        ///// </summary>
        ///// <param name="auth"></param>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static BasicResult RemoveFeature(AuthDetails auth, FeatureModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/removefeature/", auth.Token);
        //}

        ///// <summary>
        ///// Creates a new <see cref="DataObject"/>.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/AddDataObject">https://serialkeymanager.com/docs/api/v3/AddDataObject</a> </remarks>
        ///// <returns>Returns <see cref="DataObjectIdResult"/> or null.</returns>
        //public static DataObjectIdResult AddDataObject(AuthDetails auth, AddDataObjectModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<DataObjectIdResult>(parameters, "/data/adddataobject/", auth.Token);
        //}

        ///// <summary>
        ///// This method lists either all Data Object associated with a
        ///// license key, a product or your entire account, or all of them at once.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/ListDataObjects">https://serialkeymanager.com/docs/api/v3/ListDataObjects</a> </remarks>
        ///// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        //public static ListOfDataObjectsResult ListDataObjects(AuthDetails auth, ListDataObjectsModel parameters)
        //{
        //    if (parameters.ShowAll)
        //    {
        //        var result = HelperMethods.SendRequestToWebAPI3<ListOfDataObjectsResultWithReferencer>(parameters, "/data/listdataobjects/", auth.Token);
        //        return new ListOfDataObjectsResult
        //        {
        //            Message = result.Message,
        //            Result = result.Result,
        //            DataObjects = result.DataObjects.Select(x => (DataObject)x).ToList()
        //        };
        //    }
        //    else
        //    {
        //        return HelperMethods.SendRequestToWebAPI3<ListOfDataObjectsResult>(parameters, "/data/listdataobjects/", auth.Token);
        //    }
        //}


        ///// <summary>
        ///// This method will set the int value to a new one.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/SetIntValue">https://serialkeymanager.com/docs/api/v3/SetIntValue</a> <br/>
        ///// Note also: Integer overflows are not allowed. If you attempt to assign an int value that is beyond the limits of an int32, zero will be assigned to the data object's IntValue.</remarks>
        ///// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        //public static BasicResult SetIntValue(AuthDetails auth, ChangeIntValueModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setintvalue/", auth.Token);
        //}

        ///// <summary>
        ///// This method will set the string value to a new one.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/SetStringValue">https://serialkeymanager.com/docs/api/v3/SetStringValue</a> <br/>
        ///// </remarks>
        ///// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        //public static BasicResult SetStringValue(AuthDetails auth, ChangeStringValueModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/setstringvalue/", auth.Token);
        //}


        ///// <summary>
        ///// This method will increment the current int value by the one specified as an input parameter,
        ///// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        ///// </remarks>
        ///// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        //public static BasicResult IncrementIntValue(AuthDetails auth, ChangeIntValueModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/incrementintvalue/", auth.Token);
        //}


        ///// <summary>
        ///// This method will decrement the current int value by the one specified as an input parameter,
        ///// i.e. <see cref="ChangeIntValueModel.IntValue"/>.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/DecrementIntValue">https://serialkeymanager.com/docs/api/v3/DecrementIntValue</a> <br/>
        ///// </remarks>
        ///// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        //public static BasicResult DecrementIntValue(AuthDetails auth, ChangeIntValueModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/decrementintvalue/", auth.Token);
        //}

        ///// <summary>
        ///// This method will remove an existing data object.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/RemoveDataObject">https://serialkeymanager.com/docs/api/v3/RemoveDataObject</a> <br/>
        ///// </remarks>
        ///// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        //public static BasicResult RemoveDataObject(AuthDetails auth, RemoveDataObjectModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/data/removedataobject/", auth.Token);
        //}

        ///// <summary>
        ///// This method will, given a license key, generate a new access token
        ///// that is locked to that particular key and return the Id of that key.
        ///// The scope of the access token is preserved (i.e. all methods that were
        ///// enabled in the access token used to access this method will be copied
        ///// to the new access token) except for the key lock, which is going to be
        ///// changed to the id of the license key. Note, for this method to work,
        ///// the access token used to access this method has to have key lock set
        ///// to -1. All of these details are described in Remarks.
        ///// </summary>
        ///// <param name="auth">Details such as Token and Version</param>
        ///// <param name="parameters">The parameters that the method needs</param>
        ///// <remarks>Note: for more details, please see 
        ///// <a href="https://serialkeymanager.com/docs/api/v3/KeyLock">https://serialkeymanager.com/docs/api/v3/KeyLock</a> <br/>
        ///// </remarks>
        ///// <returns>Returns <see cref="KeyLockResult"/> or null.</returns>
        //public static KeyLockResult KeyLock(AuthDetails auth, KeyLockModel parameters)
        //{
        //    return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(parameters, "/auth/keylock/", auth.Token);
        //}

#endregion


#region NewMachineCode



        /// <summary>
        /// This method will calculate a machine code
        /// </summary>
        /// <param name="hashFunction">The hash function that is to be used. getEightDigitLongHash or SHA1 can be used as a default hash function.</param>
        /// <code language="VB.NET">
        /// 'eg. "61843235" (getEightDigitsLongHash)
        /// 'eg. "D38F13CAB8938AC3C393BC111E1A85BB4BA2CCC9" (getSHA1)
        /// Dim machineCode = SKGL.SKM.getMachineCode(AddressOf SKGL.SKM.getEightDigitsLongHash)
        /// Dim machineCode = SKGL.SKM.getMachineCode(AddressOf SKGL.SKM.getSHA1)
        /// </code>
        /// <code language="cs" title="C#">
        /// //eg. "61843235" (getEightDigitsLongHash)
        /// //eg. "D38F13CAB8938AC3C393BC111E1A85BB4BA2CCC9" (getSHA1)
        /// string machineID1 = SKGL.SKM.getMachineCode(SKGL.SKM.getEightDigitsLongHash);
        /// string machineID2 = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
        /// </code>
        /// </example>
        /// <returns>A machine code</returns>
        /// <remarks>On platforms other than .NET Framework 4.0 and 4.6, includeUserName value will be false.</remarks>
        [SecuritySafeCritical]
        public static string getMachineCode(Func<string, string> hashFunction)
        {
            return getMachineCode(hashFunction, false);
        }

        /// <summary>
        /// This method will calculate a machine code
        /// </summary>
        /// <param name="hashFunction">The hash function that is to be used. getEightDigitLongHash or SHA1 can be used as a default hash function.</param>
        /// <param name="includeUserName">If set to TRUE, the user name of the current user will be be taken into account int he signature (.NET Framework only).</param>
        /// <example>
        /// Machine code can be calculated with the function below. Any other hash algorithm will do, as long as it only contains letters and digits only.
        /// 
        /// <code language="VB.NET">
        /// 'eg. "61843235" (getEightDigitsLongHash)
        /// 'eg. "D38F13CAB8938AC3C393BC111E1A85BB4BA2CCC9" (getSHA1)
        /// Dim machineCode = SKGL.SKM.getMachineCode(AddressOf SKGL.SKM.getEightDigitsLongHash)
        /// Dim machineCode = SKGL.SKM.getMachineCode(AddressOf SKGL.SKM.getSHA1)
        /// </code>
        /// <code language="cs" title="C#">
        /// //eg. "61843235" (getEightDigitsLongHash)
        /// //eg. "D38F13CAB8938AC3C393BC111E1A85BB4BA2CCC9" (getSHA1)
        /// string machineID1 = SKGL.SKM.getMachineCode(SKGL.SKM.getEightDigitsLongHash);
        /// string machineID2 = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
        /// </code>
        /// </example>
        /// <returns>A machine code</returns>
        /// <remarks>On platforms other than .NET Framework 4.0 and 4.6, includeUserName value will be false.</remarks>
        [SecuritySafeCritical]
        public static string getMachineCode(Func<string,string> hashFunction, bool includeUserName = false)
        {
            // please see https://skgl.codeplex.com/workitem/2246 for a list of developers of this code.

#if SYSTEM_MANAGEMENT

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

            // In case we have message "To be filled by O.E.M." - there is incorrect motherboard/BIOS serial number 
            // - we should relay to NIC
            if (collectedInfo.Contains("To be filled by O.E.M."))
            {
              var nic = GetNicInfo();

              if (!string.IsNullOrWhiteSpace(nic))
                collectedInfo += nic;
            }

#if (NET40 || NET46)
            if(includeUserName)
                collectedInfo += System.Security.Principal.WindowsIdentity.GetCurrent().Name;
#endif

            return hashFunction(collectedInfo);//m.getEightByteHash(collectedInfo, 100000);
#else
            throw new NotImplementedException("System.Management (required to collect device info) is not included in this build).");
#endif

        }

        /// <summary>
        /// Enumerate all Nic adapters, take first one, who has MAC address and return it.
        /// </summary>
        /// <remarks> Function MUST! be updated to select only real NIC cards (and filter out USB and PPTP etc interfaces).
        /// Otherwise user can run in this scenario: a) Insert USB NIC b) Generate machine code c) Remove USB NIC...
        /// </remarks>
        /// <returns>MAC address of NIC adapter</returns>
        [SecuritySafeCritical]
        private static string GetNicInfo()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            var mac = string.Empty;

            foreach (var adapter in nics.Where(adapter => string.IsNullOrWhiteSpace(mac)))
                mac = adapter.GetPhysicalAddress().ToString();

            return mac;
        }

        [SecuritySafeCritical]
        private static string getHddSerialNumber()
        {
#if SYSTEM_MANAGEMENT

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
#else
            throw new NotImplementedException("System.Management (required to collect device info) is not included in this build).");
#endif

        }

        /// <summary>
        /// This method will generate an 8 digit long hash which can be stored as an Int32.
        /// </summary>
        /// <param name="s">The string value of the infromation that is to be hashed.</param>
        /// <returns>A string with the hash value</returns>
        /// <remarks>Please see <see cref="getMachineCode"/> for a code example of how this method can be used.</remarks>
        public static string getEightDigitsLongHash(string s)
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

            //we want the result to not be zero, as this would thrown an exception in check.
            if (result == 0)
                result = 1;


            int check = MUST_BE_LESS_THAN / result;

            if (check > 1)
            {
                result *= check;
            }

            //when result is less than MUST_BE_LESS_THAN, multiplication of result with check will be in that boundary.
            //otherwise, we have to divide by 10.
            if (MUST_BE_LESS_THAN == result)
                result /= 10;
                

            return result.ToString();
        }

        /// <summary>
        /// This method will generate a SHA1 hash.
        /// </summary>
        /// <param name="s">The string value of the infromation that is to be hashed.</param>
        /// <returns>A string with the hash value</returns>
        /// <remarks>Please see <see cref="getMachineCode"/> for a code example of how this method can be used.</remarks>
        public static string getSHA1(string s)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(System.Text.Encoding.Unicode.GetBytes(s));
                //return Convert.ToBase64String(hash);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// This method will generate a SHA256 hash.
        /// </summary>
        /// <param name="s">The string value of the information that is to be hashed.</param>
        /// <returns>A string with the hash value</returns>
        /// <remarks>Please see <see cref="getMachineCode"/> for a code example of how this method can be used.</remarks>
        public static string getSHA256(string s)
        {
            using (SHA256 sha256 = new SHA256Managed())
            {
                var hash = sha256.ComputeHash(System.Text.Encoding.Unicode.GetBytes(s));
                //return Convert.ToBase64String(hash);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

#endregion

    }
}
