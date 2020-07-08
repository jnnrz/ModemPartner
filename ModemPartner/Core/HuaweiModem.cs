using System;
using System.IO.Ports;

namespace ModemPartner.Core
{
    /// <summary>
    /// Defines the <see cref="HuaweiModem" />.
    /// </summary>
    internal class HuaweiModem : Modem
    {
        /// <summary>
        /// Defines the separator used while splitting message string received from the modem.
        /// </summary>
        private readonly string[] _separator = { ":", "," };

        /// <summary>
        /// Instance of <see cref="SerialDataReceivedEventHandler"/>.
        /// </summary>
        private readonly SerialDataReceivedEventHandler _receivedDataEventHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HuaweiModem"/> class.
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        public HuaweiModem(string comPort)
        {
            _receivedDataEventHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            _serialPort = new SerialPort(comPort);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HuaweiModem"/> class.
        /// </summary>
        public HuaweiModem()
        {
            _receivedDataEventHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            _serialPort = new SerialPort();
        }

        /// <summary>
        /// The OnMessageReceived.
        /// </summary>
        /// <param name="e">The e<see cref="Modem.ReceivedEventArgs"/>.</param>
        public override void OnMessageReceived(ReceivedEventArgs e)
        {
            Received?.Invoke(this, e);
        }

        /// <summary>
        /// The OnErrorReceived.
        /// </summary>
        /// <param name="e">The e<see cref="Modem.ErrorEventArgs"/>.</param>
        public override void OnErrorReceived(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        /// <summary>
        /// Establishes the connection with the modem.
        /// </summary>
        public override void Open()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    OnErrorReceived(new ErrorEventArgs("Port is already open."));
                    return;
                }

                _serialPort.DataReceived += _receivedDataEventHandler;
                _serialPort.Open();

                // Get modem information
                AddCommandToQueue("AT\r");
                AddCommandToQueue("ATI\r");
                AddCommandToQueue("AT+CSQ\r");
                AddCommandToQueue("AT+COPS=0,0\r");
                AddCommandToQueue("AT+COPS?\r");
                AddCommandToQueue("AT+CREG=1\r");
                AddCommandToQueue("AT+CGREG=1\r");
                AddCommandToQueue("AT+CREG?\r");
                AddCommandToQueue("AT+CGREG?\r");
                AddCommandToQueue("AT+CGATT?\r");
                AddCommandToQueue("AT^SYSCFG?\r");
                ExecuteNextCommand();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("in use"))
                {
                    _serialPort.Close();
                    return;
                }

                OnErrorReceived(new ErrorEventArgs(e.Message));
            }
        }

        /// <summary>
        /// Closes the connection with the modem.
        /// </summary>
        public override void Close()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    OnErrorReceived(new ErrorEventArgs("SerialPort is not open."));
                    return;
                }

                _serialPort.DataReceived -= _receivedDataEventHandler;
                _serialPort.Close();

                ClearCommandQueue();
            }
            catch (Exception e)
            {
                OnErrorReceived(new ErrorEventArgs(e.Message));
            }
        }

        /// <summary>
        /// Handles <see cref="SerialPort.DataReceived"/>.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SerialDataReceivedEventArgs"/>.</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var receivedData = serialPort.ReadExisting().Trim();

            if (receivedData != string.Empty)
            {
                ProcessMessage(receivedData);
            }
        }

        /// <summary>
        /// Processes the incoming message and sends the proper event to the app.
        /// </summary>
        /// <param name="receivedMessage">The receivedMessage<see cref="string"/>.</param>
        private void ProcessMessage(string receivedMessage)
        {
            string[] sep = { "\r", "\n" };
            string[] splitMessage = receivedMessage.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            foreach (var message in splitMessage)
            {
                if (message.Contains("BOOT:"))
                {
                    return;
                }

                if (message.Contains("OK"))
                {
                    ExecuteNextCommand();
                    return;
                }

                if (message.Contains("ERROR"))
                {
                    ExecuteNextCommand();
                    return;
                }

                if (message.Contains("Model"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(ModemEvent.Model, sp[1].Trim());
                }

                if (message.Contains("Manufacturer"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(ModemEvent.Manufacturer, sp[1].Trim());
                }

                if (message.Contains("IMEI"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(ModemEvent.Imei, sp[1].Trim());
                }

                if (message.Contains("CSQ:") || message.Contains("RSSI:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(ModemEvent.Rssi, sp[1].Trim());
                }

                if (message.Contains("^SYSCFG:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                    var only = sp[1];
                    var pref = sp[2];

                    var mode = Mode.Any;
                    var preferred = true;

                    switch (only)
                    {
                        case "13":
                            mode = Mode.TwoGOnly;
                            preferred = false;
                            break;

                        case "14":
                            mode = Mode.ThreeGOnly;
                            preferred = false;
                            break;
                    }

                    if (preferred)
                    {
                        switch (pref)
                        {
                            case "1":
                                mode = Mode.TwoGPref;
                                break;

                            case "2":
                                mode = Mode.ThreeGPref;
                                break;
                        }
                    }

                    SendEvent(ModemEvent.ModemMode, mode);
                }

                if (message.Contains("+CGREG:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    var status = sp.Length > 2 ? sp[2] : sp[1];

                    SendEvent(ModemEvent.PsNetwork, status);
                }

                if (message.Contains("+CREG:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    var status = sp.Length > 2 ? sp[2] : sp[1];

                    SendEvent(ModemEvent.CsNetwork, status);
                }

                if (message.Contains("+CGATT:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    SendEvent(ModemEvent.PsAttach, sp[1].Trim());
                }

                if (message.Contains("COPS:"))
                {
                    char[] trimChars = { '"', ' ' };
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                    if (sp.Length < 3)
                    {
                        return;
                    }

                    SendEvent(ModemEvent.Provider, sp[3].Trim(trimChars));
                }

                if (message.Contains("MODE:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                    var msgSubMode = sp[2];
                    var resSubMode = SubMode.NoService;

                    switch (msgSubMode)
                    {
                        case "0":
                            resSubMode = SubMode.NoService;
                            break;

                        case "1":
                            resSubMode = SubMode.Gsm;
                            break;

                        case "2":
                            resSubMode = SubMode.Gprs;
                            break;

                        case "3":
                            resSubMode = SubMode.Edge;
                            break;

                        case "4":
                            resSubMode = SubMode.Wcdma;
                            break;

                        case "5":
                            resSubMode = SubMode.Hsdpa;
                            break;

                        case "6":
                            resSubMode = SubMode.Hsupa;
                            break;

                        case "7":
                            resSubMode = SubMode.Hspa;
                            break;
                    }

                    SendEvent(ModemEvent.SysMode, resSubMode);
                }
            }
        }

        /// <summary>
        /// Sends a <see cref="Modem.ModemEvent"/> to the app so it can make the proper changes to the UI.
        /// </summary>
        /// <param name="e">The e<see cref="Modem.ModemEvent"/>.</param>
        /// <param name="message">The message<see cref="object"/>.</param>
        private void SendEvent(ModemEvent e, object message)
        {
            var args = new ReceivedEventArgs(e, message);
            OnMessageReceived(args);
        }
    }
}
