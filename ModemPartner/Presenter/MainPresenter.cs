using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DotRas;
using ModemPartner.Core;
using ModemPartner.Core.Utils;
using ModemPartner.Properties;
using ModemPartner.View;

namespace ModemPartner.Presenter
{
    /// <summary>
    /// Defines the presenter for <see cref="MainForm"/>.
    /// </summary>
    public sealed class MainPresenter : IDisposable
    {
        /// <summary>
        /// Instance of IMainView.
        /// </summary>
        private readonly IMainView _view;

        /// <summary>
        /// Timer for saving statistics.
        /// </summary>
        private readonly Timer _saveStatsTimer;

        /// <summary>
        /// Timer for statistics updates.
        /// </summary>
        private readonly Timer _statisticsTimer;

        /// <summary>
        /// Instance of RasDialer.
        /// </summary>
        private RasDialer _dialer;

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
        /// It is stored like (model, port).
        /// </summary>
        private List<Device> _modemList = new List<Device>();

        /// <summary>
        /// Keeps track of Ras connection status.
        /// </summary>
        private bool _rasConnected;

        /// <summary>
        /// Represents the current connection.
        /// </summary>
        private RasConnection _currentConnection;

        /// <summary>
        /// Previous value from 'data received' stat.
        /// </summary>
        private long _oldReceivedStat = 0;

        /// <summary>
        /// Previous value from 'data sent' stat.
        /// </summary>
        private long _oldSentStat = 0;

        /// <summary>
        /// Total data downloaded.
        /// </summary>
        private long _totalDownloaded;

        /// <summary>
        /// Total data uploaded.
        /// </summary>
        private long _totalUploaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPresenter"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="IMainView"/>.</param>
        public MainPresenter(IMainView view)
        {
            _dialer = new RasDialer();
            _dialer.StateChanged += Dialer_StateChanged;
            _dialer.DialCompleted += Dialer_DialCompleted;

            var watcher = new RasConnectionWatcher();
            watcher.Disconnected += Watcher_Disconnected;
            watcher.EnableRaisingEvents = true;

            _statisticsTimer = new Timer();
            _saveStatsTimer = new Timer();
            _statisticsTimer.Interval = 1000;
            _statisticsTimer.Elapsed += StatisticsTimer_Elapsed;
            _saveStatsTimer.Interval = 10000;
            _saveStatsTimer.Elapsed += SaveStatsTimer_Elapsed;

            _statisticsTimer.Start();
            _saveStatsTimer.Start();

            if (view != null)
            {
                _view = view;

                view.LoadForm += View_Load;
                view.AppClosing += View_AppClosing;
                view.RefreshDevicesClicked += View_RefreshDevicesClicked;
                view.ApplyModeClicked += View_ApplyModeClicked;
                view.ConnectionClicked += View_ConnectionClicked;
                view.ResetSessionClicked += View_ResetSessionClicked;
                view.ResetClicked += View_ResetClicked;
                view.OpenDeviceInfoFormClicked += View_OpenDeviceInfoFormClicked;
                view.SelectedDeviceChanged += View_SelectedDeviceChanged;
            }
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
        private void Dispose(bool disposing)
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
                _view.UpdateProvider(Resources.NoRealValue);
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event of type <see cref="DialCompletedEventArgs"/>.</param>
        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            if (e.Connected)
            {
                _rasConnected = true;
                _currentConnection = RasConnection.GetActiveConnections()
                    .Single(p => p.EntryName == _view.SelectedProfile);

                _view.UpdateUIWhenConnected();
                _view.UpdateToolStripStatus($"Connected: [{_view.SelectedProfile}]");
            }
            else if (e.TimedOut)
            {
                _view.UpdateToolStripStatus("Connection attempt timed out");
            }
            else if (e.Error != null)
            {
                _view.UpdateToolStripStatus(e.Error.Message);
                _view.UpdateUIWhenDisconnected();

                // Retry connection if there's an error while connecting
                if (_view.Retry)
                {
                    TryRasConnection();
                }
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event of type <see cref="DotRas.StateChangedEventArgs"/>.</param>
        private void Dialer_StateChanged(object sender, StateChangedEventArgs e)
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
        private void LookForDevices()
        {
            Task.Run(() =>
            {
                // If the app is connected to a modem, close the connetion
                // before refreshing the list.
                if (_modem.IsOpen)
                {
                    DisconnectRasConn();
                    CloseShop();
                }

                // Update status
                _view.UpdateToolStripStatus("Finding devices...");

                // Get connected and <available> modems.
                _modemList = Modem.GetModems();

                _view.ClearDeviceList();
                _view.AddDevicesToList(_modemList);

                // Select the modem that was used most recently
                var lastUsedModem = Settings.Default.LastUsedModem;

                if (_modemList.Count == 0)
                {
                    _view.UpdateToolStripStatus("Devices not found");
                    return;
                }

                // Update status
                _view.UpdateToolStripStatus($"{_view.NumberFoundDevices} devices found.");

                // Select the first modem in the list if none has been used previously.
                if (lastUsedModem.Equals(string.Empty))
                {
                    _view.SelectedModem = _modemList.First().Model;
                }
                else
                {
                    // Check if lastUsedModem is connected, if it is, use that
                    // if not, use the first one found on the list.
                    if (_modemList.Find(device => device.Model.Equals(lastUsedModem)) != null)
                    {
                        _view.SelectedModem = _modemList
                            .FirstOrDefault(device => device.Model == lastUsedModem)
                            ?.Model;
                    }
                    else
                    {
                        _view.SelectedModem = _modemList.First().Model;
                    }
                }

                // Connect to modem
                OpenShop();
            });
        }

        /// <summary>
        /// Handles any errors the modem may have.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event of type <see cref="Modem.ErrorEventArgs"/>.</param>
        private void Modem_ErrorEvent(object sender, Modem.ErrorEventArgs e)
        {
            _view.UpdateToolStripStatus(e.Error);
        }

        /// <summary>
        /// Handles the modem's events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event of type <see cref="Modem.ReceivedEventArgs"/>.</param>
        private void Modem_ReceiveEvent(object sender, Modem.ReceivedEventArgs e)
        {
            switch (e.Event)
            {
                case Modem.ModemEvent.Model:
                    ConnectedDeviceInfo.Model = e.Value.ToString();
                    break;

                case Modem.ModemEvent.Manufacturer:
                    ConnectedDeviceInfo.Manufacturer = e.Value.ToString();
                    break;

                case Modem.ModemEvent.Imei:
                    ConnectedDeviceInfo.IMEI = e.Value.ToString();
                    break;

                case Modem.ModemEvent.ModemMode:
                    _view.UpdateModeSelection((Modem.Mode) e.Value);
                    break;

                case Modem.ModemEvent.Rssi:
                    _view.UpdateRSSI(float.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.PsNetwork:
                    _view.UpdatePSNetwork(int.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.CsNetwork:
                    _view.UpdateCSNetwork(int.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.PsAttach:
                    _view.UpdatePSAttachment(int.Parse(e.Value.ToString()));
                    break;

                case Modem.ModemEvent.Provider:
                    _view.UpdateProvider(e.Value.ToString());
                    break;

                case Modem.ModemEvent.SysMode:
                    _view.UpdateSubMode((Modem.SubMode) e.Value);
                    break;
            }
        }

        /// <summary>
        /// Connects to the modem thru the serial port.
        /// </summary>
        private void OpenShop()
        {
            var selected = _view.SelectedModem;

            // If somehow this method gets called when
            // the modem is already connected
            if (_modem.IsOpen)
            {
                return;
            }

            if (selected.Equals(string.Empty))
            {
                return;
            }

            var port = _modemList.Find(device => device.Model.Equals(selected)).Port;

            // Config and open port
            _modem.SetPort(port);
            _modem.Received += Modem_ReceiveEvent;
            _modem.Error += Modem_ErrorEvent;
            _modem.Open();

            Task.Delay(5000);

            if (!_modem.IsOpen)
            {
                _view.UpdateToolStripStatus("Could not open the port.");
                return;
            }

            /*
             *  Update RAS profile with the connected device
             */
            var book = new RasPhoneBook();
            book.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));

            // Selected profile must exist
            if (_view.SelectedProfile.Equals(string.Empty))
            {
                _view.UpdateToolStripStatus("Profile not valid.");
                return;
            }

            // Get profile from book, it needs to match the one selected
            var selectedEntry = book.Entries.First(x => x.Name == _view.SelectedProfile);

            // Get devices that are available to RAS
            var deviceList = RasDevice.GetDevices();

            // Get the device we're using
            var openedDevice = _modemList.First(x => x.Model.Equals(_view.SelectedModem));

            // Look for the device that matches the one that we're using
            var deviceToBeUsed = deviceList.First(x => x.Name == openedDevice.Name);

            // Update the profile with the device we're using
            selectedEntry.Device = deviceToBeUsed;
            selectedEntry.Update();

            _view.UpdateToolStripStatus($"Connected to {_view.SelectedModem}");
        }

        /// <summary>
        /// Attempts to establish an internet connection with RAS.
        /// </summary>
        private void TryRasConnection()
        {
            var selectedProfile = _view.SelectedProfile;

            if (selectedProfile.Equals(string.Empty))
            {
                _view.UpdateToolStripStatus("Profile not selected");
                return;
            }

            /*
             * Config the dialer
             * EntryName is the name of the dial-up conn on Windows.
             * PhoneBookPath is the path to Windows phone book, where the conns are stored.
             * Both needed.
             * */
            _dialer.EntryName = selectedProfile;
            _dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);

            try
            {
                _handle = _dialer.DialAsync();

                Task.Run(() =>
                {
                    while (_dialer.IsBusy)
                    {
                        // The button will show 'cancel' while the conn is been established
                        _view.UpdateUIWhenDialing();
                        Task.Delay(1000);
                    }
                });

                // Save selected modem and profile on settings
                Settings.Default.LastUsedModem = _view.SelectedModem;
                Settings.Default.DefaultProfile = selectedProfile;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        /// <summary>
        /// Interrupts connection made with RAS.
        /// </summary>
        private void DisconnectRasConn()
        {
            Task.Run(() =>
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

        /// <summary>
        /// Handles what happens <see cref="IMainView.AppClosing"/> ocurrs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void View_AppClosing(object sender, EventArgs e)
        {
            DisconnectRasConn();
            _statisticsTimer.Stop();
            _saveStatsTimer.Stop();
            CloseShop();
            SaveTotalStats();
        }

        /// <summary>
        /// Handles what happens <see cref="IMainView.ApplyModeClicked"/> ocurrs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void View_ConnectionClicked(object sender, EventArgs e)
        {
            try
            {
                // If the conn is been establish it's possible to cancel
                if (_dialer.IsBusy)
                {
                    Task.Run(() => { _dialer.DialAsyncCancel(); });

                    return;
                }

                if (!_rasConnected)
                {
                    TryRasConnection();
                }
                else
                {
                    DisconnectRasConn();
                    SaveTotalStats();
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void View_Load(object sender, EventArgs e)
        {
            _modem = new HuaweiModem();

            try
            {
                LookForDevices();
                LoadProfiles();

                _totalDownloaded = long.Parse(Settings.Default.Downloaded);
                _totalUploaded = long.Parse(Settings.Default.Uploaded);
                
                _view.UpdateSessionDownload(SizeUtil.SizeSuffix(0, 2));
                _view.UpdateSessionUpload(SizeUtil.SizeSuffix(0, 2));

                _view.UpdateTotalDownloaded(SizeUtil.SizeSuffix(_totalDownloaded, 2));
                _view.UpdateTotalUploaded(SizeUtil.SizeSuffix(_totalUploaded, 2));
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        /// <summary>
        /// Handles what happens when <see cref="IMainView.RefreshDevicesClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
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
        /// Handles what happens when <see cref="IMainView.ResetClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void View_ResetClicked(object sender, EventArgs e)
        {
            _statisticsTimer.Stop();
            Settings.Default.Downloaded = "0";
            Settings.Default.Uploaded = "0";
            Settings.Default.Save();
            _totalDownloaded = 0;
            _totalUploaded = 0;
            _view.UpdateTotalDownloaded(SizeUtil.SizeSuffix(0, 2));
            _view.UpdateTotalUploaded(SizeUtil.SizeSuffix(0, 2));
            _statisticsTimer.Start();
        }

        /// <summary>
        /// Handles what happens when <see cref="IMainView.ResetSessionClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void View_ResetSessionClicked(object sender, EventArgs e)
        {
            _view.UpdateSessionDownload(SizeUtil.SizeSuffix(0, 2));
            _view.UpdateSessionUpload(SizeUtil.SizeSuffix(0, 2));
        }

        private void View_OpenDeviceInfoFormClicked(object sender, EventArgs e)
        {
            DeviceInfoForm form = new DeviceInfoForm();
            _ = new DeviceInfoPresenter(form);
            form.Show();
        }

        /// <summary>
        /// Handles what happens when RAS connection is interrupted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The event of type <see cref="DotRas.RasConnectionEventArgs"/>.</param>
        private void Watcher_Disconnected(object sender, RasConnectionEventArgs e)
        {
            _view.UpdateUIWhenDisconnected();
            _view.UpdateToolStripStatus("Disconnected");
            _currentConnection = null;
            _rasConnected = false;
            SaveTotalStats();
        }

        /// <summary>
        /// Handles timer for stats saving.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void SaveStatsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveTotalStats();
        }

        /// <summary>
        /// Handles timer for stats updates.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void StatisticsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_rasConnected)
            {
                return;
            }

            try
            {
                if (_currentConnection == null)
                {
                    return;
                }

                // Retrieve stats from the current connection
                RasLinkStatistics statistics = _currentConnection.GetConnectionStatistics();

                // Compare new values to old ones and get the current
                // data size been downloaded or uploaded
                var received = statistics.BytesReceived - _oldReceivedStat;
                var sent = statistics.BytesTransmitted - _oldSentStat;

                // Add new data amount to the total
                _totalDownloaded += received;
                _totalUploaded += sent;

                // Format the total stat. Adds units at the end of the string
                // 1.01 -> 1.01 KB/MB/GB/etc
                var totalDownloadedFormatted = SizeUtil.SizeSuffix(_totalDownloaded, 2);
                var totalUploadedFormatted = SizeUtil.SizeSuffix(_totalUploaded, 2);

                // Format session stats
                var sessionDownloadedFormatted = SizeUtil.SizeSuffix(statistics.BytesReceived);
                var sessionUploadedFormatted = SizeUtil.SizeSuffix(statistics.BytesTransmitted);

                // Calculate speed
                var downSpeed = Math.Round(received / 1024d, 2);
                var upSpeed = Math.Round(sent / 1024d, 2);

                // Now the new data becomes the old data
                _oldReceivedStat = statistics.BytesReceived;
                _oldSentStat = statistics.BytesTransmitted;

                // Format the duration of the connection
                var connDuration = $"{statistics.ConnectionDuration:hh\\:mm\\:ss}";

                // Update the UI
                _view.UpdateDownloadSpeed($"{downSpeed}");
                _view.UpdateUploadSpeed($"{upSpeed}");
                _view.UpdateSessionDownload($"{sessionDownloadedFormatted}");
                _view.UpdateSessionUpload($"{sessionUploadedFormatted}");
                _view.UpdateTotalDownloaded($"{totalDownloadedFormatted}");
                _view.UpdateTotalUploaded($"{totalUploadedFormatted}");
                _view.UpdateChart(downSpeed, upSpeed);
                _view.UpdateConnDuration(connDuration);
            }
            catch (Exception ex)
            {
                _view.UpdateToolStripStatus(ex.Message);
            }
        }

        /// <summary>
        /// Handles the event when selected modem is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void View_SelectedDeviceChanged(object sender, EventArgs e)
        {
            if (_modem.IsOpen)
            {
                DisconnectRasConn();
                CloseShop();
            }

            if (_view.SelectedModem == null)
            {
                _view.UpdateToolStripStatus("Selected modem is not valid.");
            }

            OpenShop();
        }

        /// <summary>
        /// Saves total stats.
        /// </summary>
        private void SaveTotalStats()
        {
            Settings.Default.Downloaded = _totalDownloaded.ToString();
            Settings.Default.Uploaded = _totalUploaded.ToString();
            Settings.Default.Save();
        }
    }
}