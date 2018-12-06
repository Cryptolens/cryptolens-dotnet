using SKM.V3;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Helper methods to ease interaction with Web API 3. These methods require .NET Framework 4.0 or 4.6.
    /// </summary>
    public static class Helpers
    {

        /// <summary>
        /// Returns the machine code of the current device with SHA-256 as the hash function.
        /// </summary>
        public static string GetMachineCode()
        {
            return SKGL.SKM.getMachineCode(SKGL.SKM.getSHA256);
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


    }
}
