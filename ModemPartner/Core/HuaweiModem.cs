using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ModemPartner.Core
{
    class HuaweiModem : Modem
    {
        private string[] _separator = { ":" };
        private ModemEventArgs _modemEventArgs;
        private SerialDataReceivedEventHandler _receivedDataEventHandler;

        public HuaweiModem(string comPort)
        {
            _modemEventArgs = new ModemEventArgs();
            _receivedDataEventHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            _serialPort = new SerialPort();
            _serialPort.PortName = comPort;
            _serialPort.DataReceived += _receivedDataEventHandler;
        }

        public HuaweiModem()
        {
            _modemEventArgs = new ModemEventArgs();
            _receivedDataEventHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            _serialPort = new SerialPort();
            _serialPort.DataReceived += _receivedDataEventHandler;
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
                    SendEvent(Modem.Event.Model, message);
                }

                if (message.Contains("Manufacturer"))
                {
                    SendEvent(Modem.Event.Manufacturer, message);
                }

                if (message.Contains("IMEI"))
                {
                    SendEvent(Modem.Event.IMEI, message);
                }
            }
        }

        private void SendEvent(Modem.Event e, string message)
        {
            var splitMessage = message.Split(_separator, StringSplitOptions.None);
            var args = new ModemEventArgs(e, splitMessage[1].Trim());
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

                _serialPort.Open();
                _serialPort.DiscardOutBuffer();
                _serialPort.DiscardInBuffer();

                // Get modem information
                AddCommandToQueue("ATI\r");
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
