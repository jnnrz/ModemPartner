using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModemPartner.Core;
using ModemPartner.View;

namespace ModemPartner.Presenter
{
    public class MainPresenter
    {
        private IMainView _view;

        private Dictionary<string, FoundModem> _modemList = new Dictionary<string, FoundModem>();

        private Modem _modem;

        public MainPresenter(IMainView view)
        {
            _view = view;

            view.LoadForm += View_Load;
            view.RefreshDevicesClicked += View_RefreshDevicesClicked;
            view.OpenPortClicked += View_OpenPortClicked;
            view.ApplyModeClicked += View_ApplyModeClicked;
        }

        private void View_ApplyModeClicked(object sender, EventArgs e)
        {
            if (_modem.IsOpen)
            {
                var mode = "";

                switch (_view.SelectedMode)
                {
                    case 0:
                        mode = "AT^SYSCFG=13,1,3FFFFFFF,2,4\r"; // 2G only
                        break;

                    case 1:
                        mode = "AT^SYSCFG=2,1,3FFFFFFF,2,4\r"; // 2G preferred
                        break;

                    case 2:
                        mode = "AT^SYSCFG=14,2,3FFFFFFF,2,4\r"; // 3G only
                        break;

                    case 3:
                        mode = "AT^SYSCFG=2,2,3FFFFFFF,2,4\r"; // 3G preferred
                        break;

                    default:
                        mode = "AT^SYSCFG=2,2,3FFFFFFF,2,4\r"; // 3G preferred
                        break;
                }

                try
                {
                    _modem.AddCommandToQueue(mode);
                    _modem.AddCommandToQueue("AT^SYSCFG?\r");
                    _modem.AddCommandToQueue("AT+CGATT?\r");
                    _modem.ExecuteNextCommand();
                }
                catch (Exception ex)
                {
                    _view.UpdateToolStripStatus(ex.Message);
                }
            }
        }

        private void View_OpenPortClicked(object sender, EventArgs e)
        {
            try
            {
                if (_modem.IsOpen)
                {
                    _modem.ModemEvent -= Modem_ReceiveEvent;
                    _modem.Close();
                    _view.DisableControls = false;
                    _view.UpdateOpenPortBtn(Properties.Resources.unplugged, "");
                    _view.UpdateProvider("--");
                    _view.UpdateRSSI(1);
                    _view.UpdateCSNetwork(6);
                    _view.UpdatePSNetwork(6);
                    _view.UpdatePSAttachment(2);
                    _view.UpdateToolStripStatus($"Disconnected from {_view.SelectedModem}");
                }
                else
                {
                    OpenModemPort();

                    if (_modem.IsOpen)
                    {
                        _view.DisableControls = true;
                        _view.UpdateOpenPortBtn(Properties.Resources.plug, "");
                        _view.UpdateToolStripStatus($"Connected to {_view.SelectedModem}");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        private void View_Load(object sender, EventArgs e)
        {
            _modem = new HuaweiModem();

            try
            {
                LookForDevices();
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        private void View_RefreshDevicesClicked(object sender, EventArgs e)
        {
            try
            {
                LookForDevices();
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        private void Modem_ReceiveEvent(object sender, ModemEventArgs e)
        {
            switch (e.Event)
            {
                case Modem.Event.Model:
                    break;

                case Modem.Event.ModemMode:
                    _view.UpdateModeSelection((Modem.Mode)e.Value);
                    break;

                case Modem.Event.RSSI:
                    _view.UpdateRSSI(float.Parse(e.Value.ToString()));
                    break;

                case Modem.Event.PSNetwork:
                    _view.UpdatePSNetwork(int.Parse(e.Value.ToString()));
                    break;

                case Modem.Event.CSNetwork:
                    _view.UpdateCSNetwork(int.Parse(e.Value.ToString()));
                    break;

                case Modem.Event.PSAttach:
                    _view.UpdatePSAttachment(int.Parse(e.Value.ToString()));
                    break;

                case Modem.Event.Provider:
                    _view.UpdateProvider(e.Value.ToString());
                    break;

                case Modem.Event.SysMode:
                    _view.UpdateSubMode((Modem.SubMode)e.Value);
                    break;
            }
        }

        private void OpenModemPort()
        {
            var selected = _view.SelectedModem;

            if (_modem.IsOpen)
                return;

            if (selected.Equals(String.Empty))
                return;

            var foundModem = _modemList[selected];
            if (foundModem == null)
                return;

            _modem.SetPort(foundModem.Port);
            _modem.ModemEvent += Modem_ReceiveEvent;
            _modem.Open();
        }

        private void LookForDevices()
        {
            Task.Run(() =>
            {
                _view.UpdateToolStripStatus("Finding devices...");

                _modemList = Modem.GetModems();
                _view.ClearDeviceList();
                _view.AddDevicesToList(_modemList);

                _view.UpdateToolStripStatus($"{_view.NumberFoundDevices} devices found.");
            });
        }
    }
}