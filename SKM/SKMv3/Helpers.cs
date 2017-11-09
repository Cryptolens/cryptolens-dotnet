using SKM.V3;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Helper methods to ease interaction with Web API 3.
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
        /// Checks if the current license key is on the correct device.
        /// </summary>
        /// <param name="licenseKey">The license key object.</param>
        /// <returns></returns>
        public static bool IsOnRightMachine(LicenseKey licenseKey)
        {
            return licenseKey.IsOnRightMachine(SKGL.SKM.getSHA256).IsValid();
        }
    }
}
