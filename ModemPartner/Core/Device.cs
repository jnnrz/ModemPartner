namespace ModemPartner.Core
{
    /// <summary>
    /// Represents an available device on the PC.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device()
        {
        }

        /// <summary>
        /// Gets or sets name of the device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the PNPDeviceID of the device.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the model of the device.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the COM port of the device.
        /// </summary>
        public string Port { get; set; }
    }
}