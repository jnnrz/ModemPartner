using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace ModemPartner.Core
{
    class HuaweiModem : Modem
    {
        private string[] _separator = { ":", "," };
        private ModemEventArgs _modemEventArgs;
        private SerialDataReceivedEventHandler _receivedDataEventHandler;

        public HuaweiModem(string comPort)
        {
            _modemEventArgs = new ModemEventArgs();
            _receivedDataEventHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            _serialPort = new SerialPort();
            _serialPort.PortName = comPort;
        }

        public HuaweiModem()
        {
            _modemEventArgs = new ModemEventArgs();
            _receivedDataEventHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            _serialPort = new SerialPort();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var receivedData = serialPort.ReadExisting().Trim();

            if (receivedData != String.Empty)
            {
                ProcessMessage(receivedData);
            }
        }

        private void ProcessMessage(string receivedMessage)
        {
            String[] splitMessage = receivedMessage.Split('\r');

            foreach (var message in splitMessage)
            {
                if (message.Contains("OK"))
                {
                    ExecuteNextCommand();
                }

                if (message.Contains("ERROR"))
                {
                }

                if (message.Contains("Model"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(Modem.Event.Model, sp[1].Trim());
                }

                if (message.Contains("Manufacturer"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(Modem.Event.Manufacturer, sp[1].Trim());
                }

                if (message.Contains("IMEI"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(Modem.Event.IMEI, sp[1].Trim());
                }

                if (message.Contains("CSQ:") || message.Contains("RSSI:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.None);
                    SendEvent(Modem.Event.RSSI, sp[1].Trim());
                }
                
                if (message.Contains("^SYSCFG:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                    var only = sp[1];
                    var pref = sp[2];
                    var band = sp[3];
                    var roam = sp[4];
                    var doma = sp[5];

                    var mode = Modem.Mode.Any;
                    var preferred = true;

                    switch (only)
                    {
                        case "13":
                            mode = Modem.Mode.TwoGOnly;
                            preferred = false;
                            break;
                        case "14":
                            mode = Modem.Mode.ThreeGOnly;
                            preferred = false;
                            break;
                    }

                    if (preferred)
                    {
                        switch (pref)
                        {
                            case "1":
                                mode = Modem.Mode.TwoGPref;
                                break;
                            case "2":
                                mode = Modem.Mode.ThreeGPref;
                                break;
                        }
                    }

                    SendEvent(Modem.Event.ModemMode, mode);
                }

                if (message.Contains("CGREG:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    SendEvent(Modem.Event.PSNetwork, sp[2].Trim());
                }

                if (message.Contains("CREG:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    SendEvent(Modem.Event.CSNetwork, sp[2].Trim());
                }

                if (message.Contains("CGATT:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    SendEvent(Modem.Event.PSAttach, sp[1].Trim());
                }

                if (message.Contains("COPS:"))
                {
                    char[] trimChars = { '"', ' ' };
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    SendEvent(Modem.Event.Provider, sp[3].Trim(trimChars));
                }

                if (message.Contains("MODE:"))
                {
                    var sp = message.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    SendEvent(Modem.Event.SysMode, sp[2].Trim());
                }
            }
        }

        private void SendEvent(Modem.Event e, object message)
        {
            var args = new ModemEventArgs(e, message);
            OnMessageReceived(args);
        }

        public override void OnMessageReceived(ModemEventArgs e)
        {
            EventHandler<ModemEventArgs> handler = ModemEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override void OnErrorReceived(ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = Error;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override void Open()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    MessageBox.Show("Port is already open.");
                    return;
                }

                _serialPort.DataReceived += _receivedDataEventHandler;
                _serialPort.Open();
                _serialPort.DiscardOutBuffer();
                _serialPort.DiscardInBuffer();

                // Get modem information
                AddCommandToQueue("AT\r");
                AddCommandToQueue("ATI\r");
                AddCommandToQueue("AT+CSQ\r");
                AddCommandToQueue("AT+COPS=0,0\r");
                AddCommandToQueue("AT+COPS?\r");
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
                    return;
                }

                OnErrorReceived(new ErrorEventArgs(e.Message));
            }
        }

        public override void Close()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= _receivedDataEventHandler;
                    _serialPort.Close();

                    ClearCommandQueue();
                }
            }
            catch (Exception e)
            {
                OnErrorReceived(new ErrorEventArgs(e.Message));
            }
        }

    }
}
