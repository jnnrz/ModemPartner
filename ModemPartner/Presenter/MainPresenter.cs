using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotRas;
using ModemPartner.Core;
using ModemPartner.View;

namespace ModemPartner.Presenter
{
    public class MainPresenter
    {
        private readonly RasDialer _dialer;
        private readonly RasConnectionWatcher _watcher;
        private RasHandle _handle;
        private Modem _modem;
        private Dictionary<string, FoundModem> _modemList = new Dictionary<string, FoundModem>();
        private bool _rasConnected;
        private IMainView _view;

        public MainPresenter(IMainView view)
        {
            _dialer = new RasDialer();
            _dialer.StateChanged += Dialer_StateChanged;
            _dialer.DialCompleted += Dialer_DialCompleted;

            _watcher = new RasConnectionWatcher();
            _watcher.Disconnected += Watcher_Disconnected;
            _watcher.EnableRaisingEvents = true;

            _view = view;

            view.LoadForm += View_Load;
            view.AppClosing += View_AppClosing;
            view.RefreshDevicesClicked += View_RefreshDevicesClicked;
            view.OpenPortClicked += View_OpenPortClicked;
            view.ApplyModeClicked += View_ApplyModeClicked;
            view.ConnectionClicked += View_ConnectionClicked;
        }

        private void CloseShop()
        {
            _modem.ModemEvent -= Modem_ReceiveEvent;
            _modem.Error -= Modem_ErrorEvent;
            _modem.Close();
            _view.DisableDeviceRelatedControls = false;
            _view.UpdateOpenPortBtn(Properties.Resources.unplugged, "");
            _view.UpdateProvider("--");
            _view.UpdateRSSI(1);
            _view.UpdateCSNetwork(6);
            _view.UpdatePSNetwork(6);
            _view.UpdatePSAttachment(2);
            _view.UpdateToolStripStatus($"Disconnected from {_view.SelectedModem}");
        }

        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            if (e.Connected)
            {
                _rasConnected = true;
                _view.UpdateUIWhenConnected();
                _view.UpdateToolStripStatus($"Connected: [{_view.SelectedProfile}]");
            }
            else if (e.TimedOut)
            {
                _view.UpdateToolStripStatus("Connection attempt timed out");
            }
            else if (e.Error != null)
            {
                _view.UpdateToolStripStatus(e.Error.ToString());
            }
            else if (e.Cancelled)
            {
                _view.UpdateToolStripStatus("Connection attempt cancelled");
                _view.UpdateUIWhenDisconnected();
            }
            else if (!e.Connected)
            {
                _view.UpdateUIWhenDisconnected();
            }
        }

        private void Dialer_StateChanged(object sender, DotRas.StateChangedEventArgs e)
        {
            _view.UpdateToolStripStatus(e.State.ToString());
        }

        private void LoadProfiles()
        {
            try
            {
                RasPhoneBook phoneBook = new RasPhoneBook();
                phoneBook.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));

                _view.AddProfilesToList(phoneBook.Entries);
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        private async void LookForDevices()
        {
            await Task.Run(() =>
            {
                _view.UpdateToolStripStatus("Finding devices...");

                _modemList = Modem.GetModems();
                _view.ClearDeviceList();
                _view.AddDevicesToList(_modemList);

                _view.UpdateToolStripStatus($"{_view.NumberFoundDevices} devices found.");
            });
        }

        private void Modem_ErrorEvent(object sender, ErrorEventArgs e)
        {
            _view.UpdateToolStripStatus(e.Error);
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
            _modem.Error += Modem_ErrorEvent;
            _modem.Open();
        }

        private void Ras_Connect()
        {
            var selectedProfile = _view.SelectedProfile;

            if (selectedProfile.Equals(String.Empty))
            {
                _view.UpdateToolStripStatus("Profile not selected");
                return;
            }

            _dialer.EntryName = selectedProfile;
            _dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);

            try
            {
                _handle = _dialer.DialAsync();

                Task.Run(() =>
                {
                    while (_dialer.IsBusy)
                    {
                        _view.UpdateUIWhenDialing();
                        Thread.Sleep(1000);
                    }
                });

                Properties.Settings.Default.DefaultProfile = selectedProfile;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        private async void Ras_Disconnect()
        {
            await Task.Run(() =>
            {
                var connections = RasConnection.GetActiveConnections();

                foreach (var c in connections)
                {
                    if (c.Handle == _handle)
                    {
                        c.HangUp();
                    }
                }
            });
        }

        private void View_AppClosing(object sender, EventArgs e)
        {
            CloseShop();
        }

        private void View_ApplyModeClicked(object sender, EventArgs e)
        {
            if (_modem.IsOpen)
            {
                try
                {
                    _modem.SetMode(_view.SelectedMode);
                }
                catch (Exception ex)
                {
                    _view.UpdateToolStripStatus(ex.Message);
                }
            }
        }

        private void View_ConnectionClicked(object sender, EventArgs e)
        {
            try
            {
                if (_dialer.IsBusy)
                {
                    Task.Run(() =>
                    {
                        _dialer.DialAsyncCancel();
                    });

                    return;
                }

                if (!_rasConnected)
                {
                    Ras_Connect();
                }
                else
                {
                    Ras_Disconnect();
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
                LoadProfiles();
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        private void View_OpenPortClicked(object sender, EventArgs e)
        {
            try
            {
                if (_modem.IsOpen)
                {
                    CloseShop();
                }
                else
                {
                    OpenModemPort();

                    if (_modem.IsOpen)
                    {
                        _view.DisableDeviceRelatedControls = true;
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

        private void Watcher_Disconnected(object sender, DotRas.RasConnectionEventArgs e)
        {
            _view.UpdateUIWhenDisconnected();
            _view.UpdateToolStripStatus("Disconnected");
            _rasConnected = false;
        }
    }
}