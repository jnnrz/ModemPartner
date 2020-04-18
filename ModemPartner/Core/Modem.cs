using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ModemPartner.Core
{
    public abstract class Modem
    {
        public EventHandler<ModemEventArgs> ModemEvent { get; set; }
        public EventHandler<ErrorEventArgs> Error { get; set; }

        public bool IsOpen { get { return _serialPort.IsOpen; } }

        protected SerialPort _serialPort;

        private Queue<string> _commandQueue;

        public enum Event
        {
            Duration,
            UploadSpeed,
            DownloadSpeed,
            DownloadedData,
            SysMode,
            RSSI,
            Provider,
            APN,
            Manufacturer,
            Port,
            Model,
            IMEI,
            Firmware,
            Hardware,
            ModemMode,
            ModemBand,
            PSNetwork,
            CSNetwork,
            PSAttach
        };

        public enum Mode
        {
            Any,
            TwoGPref,
            TwoGOnly,
            ThreeGPref,
            ThreeGOnly
        };

        public enum SubMode
        {
            NoService,
            GSM,
            GPRS,
            EDGE,
            WCDMA,
            HSDPA,
            HSUPA,
            HSPA
        }

        public enum Band
        {
            GSM,
            EGSM,
            PGSM,
            DCS,
            PCS,
            WCDMA,
            Any
        }

        public Modem()
        {
            _commandQueue = new Queue<string>();
        }

        public abstract void Open();

        public abstract void Close();

        public abstract void OnMessageReceived(ModemEventArgs e);

        public abstract void OnErrorReceived(ErrorEventArgs e);

        public void ExecuteNextCommand()
        {
            if (_commandQueue.Count > 0)
            {
                _serialPort.WriteLine(_commandQueue.Peek());
                _commandQueue.Dequeue();
            }
        }

        public void AddCommandToQueue(string command)
        {
            if (IsOpen)
            {
                _commandQueue.Enqueue(command);
            }
        }

        protected void ClearCommandQueue()
        {
            _commandQueue.Clear();
        }

        public void SetPort(string comPort)
        {
            _serialPort.PortName = comPort;
        }

        public async void SetMode(int mode)
        {
            var m = "";

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
            });
        }

        public static Dictionary<string, FoundModem> GetModems()
        {
            List<string> comList = new List<string>();
            Dictionary<string, FoundModem> modemList = new Dictionary<string, FoundModem>();

            // Looks for COM ports using ClassGuid=COM and contains 'PC UI' in its name
            // 'PC UI' will work for certain Huawei modems, I don't know if it can work for all models
            // It's hardcoded and kinda sh*tty but ¯\_(ツ)_/¯ <quote>This is the way</quote>
            String getPortsQuery = "SELECT Name FROM Win32_PnPEntity WHERE Name LIKE '%PC UI%' AND ClassGuid='{4d36e978-e325-11ce-bfc1-08002be10318}'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", getPortsQuery);

            foreach (var obj in searcher.Get())
            {
                var deviceName = obj["Name"].ToString();
                // Extract COM port from device name
                var port = COMPortUtil.ExtractCOMPortFromName(deviceName);
                comList.Add(port);
            }

            // Connect to each COM port and get modem information
            foreach (var p in comList)
            {
                var modem = new HuaweiModem(p);
                modem.ModemEvent = new EventHandler<ModemEventArgs>((sender, e) =>
                {
                    if (e.Event == Event.Model)
                    {
                        modemList.Add(e.Value.ToString(), new FoundModem(p, e.Value.ToString()));
                        modem.Close();
                    }
                });
                modem.Open();

                // Hold it for a bit while the event arrives
                while (modem.IsOpen) { };
            }

            return modemList;
        }
    }

    public class ModemEventArgs : EventArgs
    {
        public ModemEventArgs()
        { }

        public ModemEventArgs(Modem.Event e, object value)
        {
            Event = e;
            Value = value;
        }

        public Modem.Event Event { get; set; }
        public object Value { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs()
        {
        }

        public ErrorEventArgs(string ErrorMessage)
        {
            Error = ErrorMessage;
        }

        public string Error { get; set; }
    }
}