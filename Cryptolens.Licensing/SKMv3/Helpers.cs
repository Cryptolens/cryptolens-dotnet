using SKM.V3;

using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Helper methods to ease interaction with Web API 3. These methods require .NET Framework 4.0 or 4.6.
    /// </summary>
    public static class Helpers
    {
        public enum OSType {
            Undefined = 0,
            Windows = 1,
            Unix = 2,
            Linux = 3,
            Mac = 4
            
        }

        public static OSType GetPlatform()
        {
#if NETSTANDARD2_0 || NET46 || NET47 || NET471
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
            return OSType.Undefined;
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
        /// Returns the machine code of the current device with SHA-256 as the hash function.
        /// </summary>
        public static string GetMachineCode(bool platformIndependent = false, HashSet<OSType> supportedPlatforms = null)
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

                if(supportedPlatforms!= null && !supportedPlatforms.Contains(OSType.Linux))
                {
                    // must be mac
                    return SKGL.SKM.getSHA256(ExecCommand("/bin/bash", "system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'"));
                }
                else if(supportedPlatforms != null && !supportedPlatforms.Contains(OSType.Mac))
                {
                    // must be linux
                    return SKGL.SKM.getSHA256(ExecCommand("/bin/bash", "dmidecode -s system-uuid"));
                }

                // undetermined, use MAC?

                // ls /dev/disk/by-uuid

                // use MAC
                return null;
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
        public static bool IsOnRightMachine(LicenseKey licenseKey, bool isFloatingLicense = false, bool allowOverdraft = false)
        {
            return licenseKey.IsOnRightMachine(SKGL.SKM.getSHA256, isFloatingLicense, allowOverdraft).IsValid();
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
