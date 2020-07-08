namespace ModemPartner
{
    /// <summary>
    /// Represents the current connected device information.
    /// </summary>
    public static class ConnectedDeviceInfo
    {
        /// <summary>
        /// Gets or sets the Manufacturer.
        /// </summary>
        public static string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the Model.
        /// </summary>
        public static string Model { get; set; }

        /// <summary>
        /// Gets or sets the IMEI.
        /// </summary>
        public static string IMEI { get; set; }

        /// <summary>
        /// Gets or sets the Revision.
        /// </summary>
        public static string Revision { get; set; }

        /// <summary>
        /// Gets or sets the HardwareVersion.
        /// </summary>
        public static string HardwareVersion { get; set; }

        /// <summary>
        /// Clears class fields.
        /// </summary>
        public static void Clear()
        {
            Manufacturer = string.Empty;
            Model = string.Empty;
            IMEI = string.Empty;
            Revision = string.Empty;
            HardwareVersion = string.Empty;
        }
    }
}
