using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ModemPartner.Core
{
    /// <summary>
    /// Represents a USB modem.
    /// </summary>
    public abstract class Modem
    {
        /// <summary>
        /// Defines the serial port object.
        /// </summary>
        protected SerialPort _serialPort;

        /// <summary>
        /// Queue to store all the AT commands to be executed.
        /// </summary>
        private readonly Queue<string> _commandQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Modem"/> class.
        /// </summary>
        public Modem()
        {
            _commandQueue = new Queue<string>();
        }

        /// <summary>
        /// Modem's mode.
        /// </summary>
        public enum Mode
        {
            Any,
            TwoGPref,
            TwoGOnly,
            ThreeGPref,
            ThreeGOnly,
        }

        /// <summary>
        /// Modem events.
        /// </summary>
        public enum ModemEvent
        {
            Duration,
            UploadSpeed,
            DownloadSpeed,
            DownloadedData,
            SysMode,
            Rssi,
            Provider,
            Apn,
            Manufacturer,
            Port,
            Model,
            Imei,
            Firmware,
            Hardware,
            ModemMode,
            ModemBand,
            PsNetwork,
            CsNetwork,
            PsAttach,
        }

        /// <summary>
        /// Submodes
        /// </summary>
        public enum SubMode
        {
            NoService,
            Gsm,
            Gprs,
            Edge,
            Wcdma,
            Hsdpa,
            Hsupa,
            Hspa,
        }

        /// <summary>
        /// Gets or sets data received from the modem.
        /// </summary>
        public EventHandler<ReceivedEventArgs> Received { get; set; }

        /// <summary>
        /// Gets or sets errors received from the modem.
        /// </summary>
        public EventHandler<ErrorEventArgs> Error { get; set; }

        /// <summary>
        /// Gets a value indicating whether serial port is open.
        /// </summary>
        public bool IsOpen => _serialPort.IsOpen;

        /// <summary>
        /// The Open.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// The Close.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// The OnErrorReceived.
        /// </summary>
        /// <param name="e">The e<see cref="ErrorEventArgs"/>.</param>
        public abstract void OnErrorReceived(ErrorEventArgs e);

        /// <summary>
        /// The OnMessageReceived.
        /// </summary>
        /// <param name="e">The e<see cref="ReceivedEventArgs"/>.</param>
        public abstract void OnMessageReceived(ReceivedEventArgs e);

        /// <summary>
        /// Retrieves all the modems currently connected to the computer.
        /// </summary>
        /// <returns>The a list of all connected modems to the PC.</returns>
        public static Dictionary<string, string> GetModems()
        {
            List<string> comList = new List<string>();
            Dictionary<string, string> modemList = new Dictionary<string, string>();

            // Looks for COM ports using ClassGuid=COM and contains 'PC UI' in its name
            // 'PC UI' will work for certain Huawei modems, I don't know if it can work for all models
            // It's hardcoded and kinda sh*tty but ¯\_(ツ)_/¯ <quote>This is the way</quote>
            string getPortsQuery =
                "SELECT Name FROM Win32_PnPEntity WHERE Name LIKE '%PC UI%' AND ClassGuid='{4d36e978-e325-11ce-bfc1-08002be10318}'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", getPortsQuery);

            // Looks for COM port
            foreach (var obj in searcher.Get())
            {
                var deviceName = obj["Name"].ToString();

                // Extract COM port from device name
                var port = ComPortUtil.ExtractComPortFromName(deviceName);
                comList.Add(port);
            }

            // Connect to each COM port and get modem information
            foreach (var port in comList)
            {
                var modem = new HuaweiModem(port);
                modem.Received = (sender, e) =>
                {
                    // We wait for the Model event
                    if (e.Event != ModemEvent.Model)
                    {
                        return;
                    }

                    // e.Value represents the modem model
                    // so here we add (model, port)
                    modemList.Add(e.Value.ToString(), port);
                    modem.Close();
                };
                modem.Open();

                // Hold it for a bit while the event arrives
                while (modem.IsOpen)
                {
                }
            }

            return modemList;
        }

        /// <summary>
        /// Sets the mode.
        /// </summary>
        /// <param name="mode">The mode<see cref="int"/>.</param>
        public async void SetMode(int mode)
        {
            string m = string.Empty;

            switch (mode)
            {
                case 0:
                    m = "AT^SYSCFG=13,1,3FFFFFFF,2,4\r"; // 2G only
                    break;

                case 1:
                    m = "AT^SYSCFG=2,1,3FFFFFFF,2,4\r"; // 2G preferred
                    break;

                case 2:
                    m = "AT^SYSCFG=14,2,3FFFFFFF,2,4\r"; // 3G only
                    break;

                case 3:
                    m = "AT^SYSCFG=2,2,3FFFFFFF,2,4\r"; // 3G preferred
                    break;

                default:
                    m = "AT^SYSCFG=2,2,3FFFFFFF,2,4\r"; // 3G preferred
                    break;
            }

            await Task.Run(() =>
            {
                AddCommandToQueue(m);
                AddCommandToQueue("AT^SYSCFG?\r");
                ExecuteNextCommand();

                Thread.Sleep(10000);
                AddCommandToQueue("AT+CGATT?\r");
                ExecuteNextCommand();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the COM port to be connected.
        /// </summary>
        /// <param name="comPort">The comPort<see cref="string"/>.</param>
        public void SetPort(string comPort)
        {
            _serialPort.PortName = comPort;
        }

        /// <summary>
        /// Adds a AT command to the queue so it can be executed.
        /// </summary>
        /// <param name="command">The command<see cref="string"/>.</param>
        protected void AddCommandToQueue(string command)
        {
            if (IsOpen)
            {
                _commandQueue.Enqueue(command);
            }
        }

        /// <summary>
        /// Executes the next command in queue.
        /// </summary>
        protected void ExecuteNextCommand()
        {
            if (_commandQueue.Count <= 0)
            {
                return;
            }

            _serialPort.WriteLine(_commandQueue.Peek());
            _commandQueue.Dequeue();
        }

        /// <summary>
        /// Clears the command queue.
        /// </summary>
        protected void ClearCommandQueue()
        {
            _commandQueue.Clear();
        }

        /// <summary>
        /// Defines the <see cref="ErrorEventArgs" />.
        /// </summary>
        public class ErrorEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
            /// </summary>
            public ErrorEventArgs()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ErrorEventArgs"/> class.
            /// </summary>
            /// <param name="errorMessage">The errorMessage<see cref="string"/>.</param>
            public ErrorEventArgs(string errorMessage)
            {
                Error = errorMessage;
            }

            /// <summary>
            /// Gets or sets the Error.
            /// </summary>
            public string Error { get; set; }
        }

        /// <summary>
        /// Defines the <see cref="ReceivedEventArgs" />.
        /// </summary>
        public class ReceivedEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReceivedEventArgs"/> class.
            /// </summary>
            public ReceivedEventArgs()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ReceivedEventArgs"/> class.
            /// </summary>
            /// <param name="e">The e<see cref="Modem.ModemEvent"/>.</param>
            /// <param name="value">The value<see cref="object"/>.</param>
            public ReceivedEventArgs(ModemEvent e, object value)
            {
                Event = e;
                Value = value;
            }

            /// <summary>
            /// Gets or sets the Event.
            /// </summary>
            public ModemEvent Event { get; set; }

            /// <summary>
            /// Gets or sets the Value.
            /// </summary>
            public object Value { get; set; }
        }
    }
}