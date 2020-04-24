using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotRas;
using ModemPartner.Core;
using ModemPartner.View;

namespace ModemPartner.Presenter
{
    /// <summary>
    /// Defines the presenter for <see cref="MainForm"/>.
    /// </summary>
    public class MainPresenter : IDisposable
    {
        /// <summary>
        /// Instance of RasDialer.
        /// </summary>
        private RasDialer _dialer;

        /// <summary>
        /// Instance of IMainView.
        /// </summary>
        private readonly IMainView _view;

        /// <summary>
        /// Instance of RasConnectionWatcher.
        /// </summary>
        private readonly RasConnectionWatcher _watcher;

        /// <summary>
        /// Instance of RasHandle.
        /// </summary>
        private RasHandle _handle;

        /// <summary>
        /// Instance of Modem.
        /// </summary>
        private Modem _modem;

        /// <summary>
        /// A list of all the modems that are connected to the PC.
        /// </summary>
        private Dictionary<string, FoundModem> _modemList = new Dictionary<string, FoundModem>();

        /// <summary>
        /// Keeps track of Ras connection status.
        /// </summary>
        private bool _rasConnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPresenter"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="IMainView"/>.</param>
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

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">
        ///  Indicates whether the method call comes from a Dispose method (its value is true)
        ///  or from a finalizer (its value is false).
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dialer != null)
                {
                    _dialer.Dispose();
                    _dialer = null;
                }

                if (_handle != null)
                {
                    _handle.Dispose();
                    _handle = null;
                }
            }
        }

        /// <summary>
        /// Closes modem's serial port connection and updates UI accordingly.
        /// </summary>
        private void CloseShop()
        {
            if (_modem.IsOpen)
            {
                _modem.Received -= Modem_ReceiveEvent;
                _modem.Error -= Modem_ErrorEvent;
                _modem.Close();
                _view.DisableDeviceRelatedControls(false);
                _view.UpdateOpenPortBtn(Properties.Resources.unplugged, string.Empty);
                _view.UpdateProvider("--");
                _view.UpdateRSSI(1);
                _view.UpdateCSNetwork(6);
                _view.UpdatePSNetwork(6);
                _view.UpdatePSAttachment(2);
                _view.UpdateToolStripStatus($"Disconnected from {_view.SelectedModem}");
            }
        }

        /// <summary>
        /// Handles the event when dialing has been completed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DialCompletedEventArgs"/>.</param>
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

        /// <summary>
        /// Handles the event when the state of the dialer has been changed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DotRas.StateChangedEventArgs"/>.</param>
        private void Dialer_StateChanged(object sender, DotRas.StateChangedEventArgs e)
        {
            _view.UpdateToolStripStatus(e.State.ToString());
        }

        /// <summary>
        /// Loads profiles from Windows' phone book.
        /// </summary>
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

        /// <summary>
        /// Search for plugged-in USB modems.
        /// </summary>
        private async void LookForDevices()
        {
            await Task.Run(() =>
            {
                _view.UpdateToolStripStatus("Finding devices...");

                _modemList = Modem.GetModems();
                _view.ClearDeviceList();
                _view.AddDevicesToList(_modemList);

                _view.UpdateToolStripStatus($"{_view.NumberFoundDevices} devices found.");
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles any errors the modem may have.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ErrorEventArgs"/>.</param>
        private void Modem_ErrorEvent(object sender, Modem.ErrorEventArgs e)
        {
            _view.UpdateToolStripStatus(e.Error);
        }

        /// <summary>
        /// Handles the modem's events.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ReceivedEventArgs"/>.</param>
        private void Modem_ReceiveEvent(object sender, Modem.ReceivedEventArgs e)
        {
            switch (e.Event)
            {
                case Modem.ModemEvent.Model:
                    break;

                case Modem.ModemEvent.ModemMode:
                    _view.UpdateModeSelection((Modem.Mode)e.Value);
                    break;

                case Modem.ModemEvent.RSSI:
                    _view.UpdateRSSI(float.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.PSNetwork:
                    _view.UpdatePSNetwork(int.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.CSNetwork:
                    _view.UpdateCSNetwork(int.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.PSAttach:
                    _view.UpdatePSAttachment(int.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.Provider:
                    _view.UpdateProvider(e.Value.ToString());
                    break;

                case Modem.ModemEvent.SysMode:
                    _view.UpdateSubMode((Modem.SubMode)e.Value);
                    break;
            }
        }

        /// <summary>
        /// Establish a connection with the modem's serial port.
        /// </summary>
        private void OpenModemPort()
        {
            var selected = _view.SelectedModem;

            if (_modem.IsOpen)
                return;

            if (selected.Equals(string.Empty))
                return;

            var foundModem = _modemList[selected];
            if (foundModem == null)
                return;

            _modem.SetPort(foundModem.Port);
            _modem.Received += Modem_ReceiveEvent;
            _modem.Error += Modem_ErrorEvent;
            _modem.Open();
        }

        /// <summary>
        /// Establish a internet connection with RAS.
        /// </summary>
        private void Ras_Connect()
        {
            var selectedProfile = _view.SelectedProfile;

            if (selectedProfile.Equals(string.Empty))
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

        /// <summary>
        /// Interrupts connection made with RAS.
        /// </summary>
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
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles what happens <see cref="IMainView.AppClosing"/> ocurrs.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void View_AppClosing(object sender, EventArgs e)
        {
            CloseShop();
        }

        /// <summary>
        /// Handles what happens <see cref="IMainView.ApplyModeClicked"/> ocurrs.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
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

        /// <summary>
        /// Handles what happens <see cref="IMainView.ConnectionClicked"/> ocurrs.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
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

        /// <summary>
        /// Handles what happens when the main form loads.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
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

        /// <summary>
        /// Handles what happens when <see cref="IMainView.OpenPortClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
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
                        _view.DisableDeviceRelatedControls(true);
                        _view.UpdateOpenPortBtn(Properties.Resources.plug, string.Empty);
                        _view.UpdateToolStripStatus($"Connected to {_view.SelectedModem}");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        /// <summary>
        /// Handles what happens when <see cref="IMainView.RefreshDevicesClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
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

        /// <summary>
        /// Handles what happens when RAS connection is interrupted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DotRas.RasConnectionEventArgs"/>.</param>
        private void Watcher_Disconnected(object sender, DotRas.RasConnectionEventArgs e)
        {
            _view.UpdateUIWhenDisconnected();
            _view.UpdateToolStripStatus("Disconnected");
            _rasConnected = false;
        }
    }
}
