using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                    _view.UpdateRSSI(1);
                    _view.UpdateToolStripStatus($"Disconnected from {_view.SelectedModem}");
                }
                else
                {
                    OpenModemPort();

                    if (_modem.IsOpen)
                    {
                        _view.DisableControls = true;
                        _view.UpdateOpenPortBtn(Properties.Resources.modem, "");
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
                    _view.UpdateToolStripStatus($"Model: {e.Value}");
                    break;
                case Modem.Event.ModemMode:
                    _view.UpdateModeSelection((Modem.Mode)e.Event);
                    break;
                case Modem.Event.RSSI:
                    _view.UpdateRSSI(float.Parse(e.Value.ToString()));
                    break;
                case Modem.Event.PSNetwork:
                    _view.UpdatePSNetwork(int.Parse(e.Value.ToString()));
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