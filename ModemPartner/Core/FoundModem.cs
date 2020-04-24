namespace ModemPartner.Core
{
    /// <summary>
    /// Defines the <see cref="FoundModem" />.
    /// </summary>
    public class FoundModem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoundModem"/> class.
        /// </summary>
        public FoundModem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoundModem"/> class.
        /// </summary>
        /// <param name="port">The COM port.</param>
        /// <param name="modem">The modem.</param>
        public FoundModem(string port, string modem)
        {
            Port = port;
            Modem = modem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoundModem"/> class.
        /// </summary>
        /// <param name="port">The COM port.</param>
        public FoundModem(string port)
        {
            Port = port;
        }

        /// <summary>
        /// Gets the Port.
        /// </summary>
        public string Port { get; }

        /// <summary>
        /// Gets the Modem.
        /// </summary>
        public string Modem { get; }
    }
}
