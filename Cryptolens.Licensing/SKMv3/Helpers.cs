using SKM.V3;

using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Helper methods to ease interaction with Web API 3. These methods require .NET Framework 4.0 or 4.6.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Creates a JSON dictionary of the type <see cref="MachineInfo"/>, which contains OSVersion, OSName and Is64Bit (not available in NET 3.5).
        /// </summary>
        /// <returns>A string representation of the JSON dictionary.</returns>
        public static string GetOSStats()
        {
            var machineInfo = new MachineInfo { OSVersion = Environment.OSVersion.Version.ToString(), OSName = GetPlatform().ToString() };

#if !NET35
            machineInfo.Is64Bit = Environment.Is64BitOperatingSystem;
#endif

            return Newtonsoft.Json.JsonConvert.SerializeObject(machineInfo);
        }


        /// <summary>
        /// Checks if the result obtained from an API call is successful.
        /// </summary>
        public static bool IsSuccessful(BasicResult result)
        {
            return result != null && result.Result == ResultType.Success;
        }

        /// <summary>
        /// Computes the method of the calling assembly. This method is intended to be
        /// called from an SDK that you want to protect. The "Calling Assembly" is the
        /// first assembly that leads to a method being called in your SDK. <br></br>
        /// 
        /// Normally, if a customer uses your SDK in an assembly A, this method will
        /// compute the hash/fingerprint of that assembly. If they have developed two
        /// assemblies, where the A uses B and B uses your SDK (which calls this method),
        /// i.e. A -> B -> SDK, then the hash is computed of the first assembly, i.e. A.
        /// </summary>
        /// <returns></returns>
        public static AssemblySignature GetAssemblyHash()
        {
            return GetAssemblyHash(null);
        }

        /// <summary>
        /// Computes the method of the calling assembly. This method is intended to be
        /// called from an SDK that you want to protect. The "Calling Assembly" is the
        /// first assembly that leads to a method being called in your SDK. <br></br>
        /// 
        /// Normally, if a customer uses your SDK in an assembly A, this method will
        /// compute the hash/fingerprint of that assembly. If they have developed two
        /// assemblies, where the A uses B and B uses your SDK (which calls this method),
        /// i.e. A -> B -> SDK, then the hash is computed of the first assembly, i.e. A.
        /// </summary>
        /// <param name="path">Allows you to specify a custom path of the assembly to sign.
        /// If null, the default assembly will be signed, as described in the summary.</param>
        /// <returns></returns>
        public static AssemblySignature GetAssemblyHash(string path)
        {
            SHA512 sha = SHA512.Create();

            var sig = "";

            if (path == null)
            {
                var assembly = Assembly.GetCallingAssembly();

                path = assembly.Location;
            }

            using (var stream = File.OpenRead(path))
            {
                sig = Convert.ToBase64String(sha.ComputeHash(stream));
            }

            return new AssemblySignature { Path = path, Signature = sig };
        }

        /// <summary>
        /// Verifies that the certificate of the software using the SDK is valid.
        /// </summary>
        /// <param name="RSAPubKey">Your RSA Public Key, which can be found here:
        /// https://app.cryptolens.io/docs/api/v3/QuickStart</param>
        /// <returns></returns>
        public static bool VerifySDKLicenseCertificate(string RSAPubKey)
        {
            return VerifySDKLicenseCertificate(RSAPubKey, "certificate.json", null);
        }

        /// <summary>
        /// Verifies that the certificate of the software using the SDK is valid.
        /// </summary>
        /// <param name="RSAPubKey">Your RSA Public Key, which can be found here:
        /// https://app.cryptolens.io/docs/api/v3/QuickStart</param>
        /// <param name="certificate">The name of the certificate.</param>
        /// <param name="path">The path to the certificate and the assembly, whose hash is signed.
        /// Note, they need to be in the same folder.</param>
        /// <returns></returns>
        public static bool VerifySDKLicenseCertificate(string RSAPubKey, string certificate, string path)
        {
            if (path == null)
            {
                var assembly = Assembly.GetCallingAssembly();

                path = assembly.Location;
            }

            var dir = Path.GetDirectoryName(path);

            var certpath = Path.Combine(dir, certificate);

            var license = new LicenseKey().LoadFromFile(certpath, RSAPubKey);

            if (license == null)
                return false;

            if (license.DataObjects == null || license.DataObjects.Count == 0)
                return false;

            string assemblyHash = "";
            foreach (var dObj in license.DataObjects)
            {
                if (dObj.Name == "cryptolens_assemblyhash")
                {
                    assemblyHash = dObj.StringValue;
                    break;
                }
            }

            if(GetAssemblyHash(path).Signature != assemblyHash)
            {
                return false;
            }

            return true;

        } 

        public enum OSType {
            Undefined = 0,
            Windows = 1,
            Unix = 2,
            Linux = 3,
            Mac = 4
            
        }

        public static OSType GetPlatform()
        {
#if (NETSTANDARD2_0 || NET46 || NET47 || NET471) && SYSTEM_MANAGEMENT
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                return OSType.Linux;
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                return OSType.Mac;
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return OSType.Windows;
            }
            else
            {
                return OSType.Undefined;
            }

#else
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return OSType.Unix;
            }
            else
            {
                return OSType.Windows;
            }
#endif
        }

        private static string ExecCommand(string fileName, string args)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            StringBuilder sb = new StringBuilder();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                sb.Append(line);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Computes a platform independent machine code that works on
        /// Windows, Linux and Mac and does not require System.Management.
        /// 
        /// Note: On Linux, sudo access is necessary.
        /// </summary>
        public static string GetMachineCodePI()
        {
            return GetMachineCode(platformIndependent: true);
        }


        /// <summary>
        /// Returns the machine code of the current device with SHA-256 as the hash function.
        /// This method works differently depending on the binaries that you use. By default,
        /// machine code is computed using COM, which requires System.Management. This is only
        /// supported on Windows, it's better to set platformIndependent=true. If you use a version
        /// of library without System.Management, the platform independent machine code will be
        /// computed by default.
        /// 
        /// The supported platforms are Windows, Mac and Linux. Note, sudo access is required if
        /// Linux is used.
        /// 
        /// In newer projects, we recommend to always set platformIndependent=true or use 
        /// <see cref="GetMachineCodePI"/>.
        /// </summary>
        #if SYSTEM_MANAGEMENT
        [Obsolete]
        #endif
        public static string GetMachineCode(bool platformIndependent = false)
        {

            int p = (int)Environment.OSVersion.Platform;
            OSType os = GetPlatform();

#if !SYSTEM_MANAGEMENT
            platformIndependent = true;       
#endif

            if (os == OSType.Unix)
            {
                //unix

                if (os == OSType.Mac)
                {
                    return SKGL.SKM.getSHA256(ExecCommand("/bin/bash", "system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'"));
                }
                else if (os == OSType.Linux)
                {
                    // requires sudo
                    return SKGL.SKM.getSHA256(ExecCommand("/bin/bash", "dmidecode -s system-uuid"));
                }

                try
                {
                    // always assume it's MAC if it's unix
                    return SKGL.SKM.getSHA256(ExecCommand("/bin/bash", "system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'"));
                }
                catch (Exception ex)
                {
                    // but if we get an error, it must be Linux:
                    return SKGL.SKM.getSHA256(ExecCommand("/bin/bash", "dmidecode -s system-uuid"));
                }
            }
            else
            {
                // not unix --> windows

                if (platformIndependent)
                {
                    return SKGL.SKM.getSHA256(ExecCommand("cmd.exe", "/C wmic csproduct get uuid"));
                }
                else
                {
                    return SKGL.SKM.getMachineCode(SKGL.SKM.getSHA256);
                }

            }
        }

        /// <summary>
        /// Checks if the current license key is on the correct device with SHA-256 as the hash function.
        /// </summary>
        /// <param name="licenseKey">The license key object.</param>
        /// <param name="isFloatingLicense">If this is a floating license, this parameter has to be set to true.
        /// You can enable floating licenses by setting <see cref="V3.Models.ActivateModel.FloatingTimeInterval"/>
        /// to a value greater than 0.</param>
        /// <param name="allowOverdraft">If floating licensing is enabled with overdraft, this parameter should be set to true.
        /// You can enable overdraft by setting <see cref="ActivateModel.MaxOverdraft"/> to a value greater than 0.
        ///</param>
        /// <returns></returns>
        public static bool IsOnRightMachinePI(LicenseKey licenseKey, bool isFloatingLicense = false, bool allowOverdraft = false)
        {
            return licenseKey.IsOnRightMachine(GetMachineCodePI(), isFloatingLicense, allowOverdraft).IsValid();
        }


        /// <summary>
        /// Checks if the current license key is on the correct device with SHA-256 as the hash function.
        /// </summary>
        /// <param name="licenseKey">The license key object.</param>
        /// <param name="isFloatingLicense">If this is a floating license, this parameter has to be set to true.
        /// You can enable floating licenses by setting <see cref="V3.Models.ActivateModel.FloatingTimeInterval"/>
        /// to a value greater than 0.</param>
        /// <param name="allowOverdraft">If floating licensing is enabled with overdraft, this parameter should be set to true.
        /// You can enable overdraft by setting <see cref="ActivateModel.MaxOverdraft"/> to a value greater than 0.
        ///</param>
        /// <param name="platformIndependent">Allows you to specify if you want to use the old machine code method that is based on COM and requires
        /// System.Management or use the new platform independent method, i.e. <see cref="GetMachineCodePI"/>.
        /// </param>
        /// <returns></returns>
        public static bool IsOnRightMachine(LicenseKey licenseKey, bool isFloatingLicense = false, bool allowOverdraft = false, bool platformIndependent = false)
        {
#if !SYSTEM_MANAGEMENT
            platformIndependent = true;
#endif

            if (platformIndependent)
            {
                return licenseKey.IsOnRightMachine(GetMachineCodePI(), isFloatingLicense, allowOverdraft).IsValid();
            }
            else
            {
                return licenseKey.IsOnRightMachine(SKGL.SKM.getSHA256, isFloatingLicense, allowOverdraft).IsValid();
            }
        }

        /// <summary>
        /// Checks if the current license key is on the correct device with SHA-256 as the hash function.
        /// </summary>
        /// <param name="licenseKey">The license key object.</param>
        /// <param name="machineCode">A unique identifier of the machine.</param>
        /// <param name="isFloatingLicense">If this is a floating license, this parameter has to be set to true.
        /// You can enable floating licenses by setting <see cref="V3.Models.ActivateModel.FloatingTimeInterval"/>
        /// to a value greater than 0.</param>
        /// <param name="allowOverdraft">If floating licensing is enabled with overdraft, this parameter should be set to true.
        /// You can enable overdraft by setting <see cref="ActivateModel.MaxOverdraft"/> to a value greater than 0.
        ///</param>
        /// <returns></returns>
        public static bool IsOnRightMachine(LicenseKey licenseKey, string machineCode, bool isFloatingLicense = false, bool allowOverdraft = false)
        {
            return licenseKey.IsOnRightMachine(machineCode, isFloatingLicense, allowOverdraft).IsValid();
        }

        /// <summary>
        /// Returns floating license related information.
        /// </summary>
        public static FloatingLicenseInformation GetFloatingLicenseInformation(ActivateModel activationModel, KeyInfoResult activationResult)
        {
            return GetFloatingLicenseInformation(activationModel.MaxOverdraft,
                activationResult.LicenseKey.MaxNoOfMachines,
                activationResult.Metadata.UsedFloatingMachines);
        }

        /// <summary>
        /// Returns floating license related information.
        /// </summary>
        public static FloatingLicenseInformation GetFloatingLicenseInformation(int MaxOverdraft, int MaxNoOfMachines, int UsedFloatingMachines)
        {
            return new FloatingLicenseInformation
            {
                AvailableDevices = MaxNoOfMachines + MaxOverdraft - UsedFloatingMachines,
                UsedDevices = UsedFloatingMachines,
                OverdraftDevices = System.Math.Max(0, UsedFloatingMachines - MaxNoOfMachines)
            };
        }
    }

    public class FloatingLicenseInformation
    {
        /// <summary>
        /// The total number of floating devices.
        /// </summary>
        public int UsedDevices { get; set; }
        /// <summary>
        /// The number of available floating devices (taking into account potential overdraft).
        /// </summary>
        public int AvailableDevices { get; set; }
        /// <summary>
        /// The number of devices that exceed the MaxNoOfMachines.
        /// </summary>
        public int OverdraftDevices { get; set; }

    }
}
