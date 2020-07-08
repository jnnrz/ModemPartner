using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DotRas;
using ModemPartner.Core;
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
        private readonly System.Timers.Timer _saveStatsTimer;

        /// <summary>
        /// Timer for statistics updates.
        /// </summary>
        private readonly System.Timers.Timer _statisticsTimer;

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
        /// </summary>
        private Dictionary<string, FoundModem> _modemList = new Dictionary<string, FoundModem>();

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

            _statisticsTimer = new System.Timers.Timer();
            _saveStatsTimer = new System.Timers.Timer();
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
                view.OpenPortClicked += View_OpenPortClicked;
                view.ApplyModeClicked += View_ApplyModeClicked;
                view.ConnectionClicked += View_ConnectionClicked;
                view.ResetSessionClicked += View_ResetSessionClicked;
                view.ResetClicked += View_ResetClicked;
                view.OpenDeviceInfoFormClicked += View_OpenDeviceInfoFormClicked;
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
                _view.UpdateOpenPortBtn(Properties.Resources.unplugged, string.Empty);
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
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DialCompletedEventArgs"/>.</param>
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
        private async void LookForDevices()
        {
            await Task.Run(() =>
            {
                _view.UpdateToolStripStatus("Finding devices...");

                _modemList = Modem.GetModems();

                _view.ClearDeviceList();
                _view.AddDevicesToList(_modemList);

                // Select the modem that was used most recently
                var defaultModem = Properties.Settings.Default.DefaultModem;
                var modemArray = _modemList.ToArray();

                for (var x = 0; x < modemArray.Length; x++)
                {
                    if (modemArray[x].Value.Modem == defaultModem)
                    {
                        _view.SelectedModem = x.ToString();
                        OpenModemPort();
                    }
                }

                _view.UpdateToolStripStatus($"{_view.NumberFoundDevices} devices found.");
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles any errors the modem may have.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="Modem.ErrorEventArgs"/>.</param>
        private void Modem_ErrorEvent(object sender, Modem.ErrorEventArgs e)
        {
            _view.UpdateToolStripStatus(e.Error);
        }

        /// <summary>
        /// Handles the modem's events.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="Modem.ReceivedEventArgs"/>.</param>
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

                case Modem.ModemEvent.IMEI:
                    ConnectedDeviceInfo.IMEI = e.Value.ToString();
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

            // Retrieve the modem model
            // it's needed to get the modem port
            var foundModem = _modemList[selected];
            if (foundModem == null)
            {
                return;
            }

            // Config and open port
            _modem.SetPort(foundModem.Port);
            _modem.Received += Modem_ReceiveEvent;
            _modem.Error += Modem_ErrorEvent;
            _modem.Open();
        }

        /// <summary>
        /// Establish a internet connection with RAS.
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
                Settings.Default.DefaultModem = _view.SelectedModem;
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
        private async void DisconnectRasConn()
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
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// Handles what happens <see cref="IMainView.AppClosing"/> ocurrs.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
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
        private async void View_ConnectionClicked(object sender, EventArgs e)
        {
            try
            {
                // If the conn is been establish it's possible to cancel
                if (_dialer.IsBusy)
                {
                    await Task.Run(() =>
                    {
                        _dialer.DialAsyncCancel();
                    }).ConfigureAwait(true);

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
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void View_Load(object sender, EventArgs e)
        {
            _modem = new HuaweiModem();

            try
            {
                LookForDevices();
                LoadProfiles();

                _totalDownloaded = long.Parse(Properties.Settings.Default.Downloaded);
                _totalUploaded = long.Parse(Properties.Settings.Default.Uploaded);

                _view.UpdateTotalDownloaded(SizeUtil.SizeSuffix(_totalDownloaded, 2));
                _view.UpdateTotalUploaded(SizeUtil.SizeSuffix(_totalUploaded, 2));
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
                    SaveTotalStats();
                    ConnectedDeviceInfo.Clear();
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
        /// Handles what happens when <see cref="IMainView.ResetClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The e.</param>
        private void View_ResetClicked(object sender, EventArgs e)
        {
            _statisticsTimer.Stop();
            Settings.Default.Downloaded = "0";
            Settings.Default.Uploaded = "0";
            Settings.Default.Save();
            _view.UpdateTotalDownloaded("0");
            _view.UpdateTotalUploaded("0");
            _statisticsTimer.Start();
        }

        /// <summary>
        /// Handles what happens when <see cref="IMainView.ResetSessionClicked"/> occurs.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The e.</param>
        private void View_ResetSessionClicked(object sender, EventArgs e)
        {
            _view.UpdateSessionDownload("0");
            _view.UpdateSessionUpload("0");
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
        /// <param name="e">The e<see cref="DotRas.RasConnectionEventArgs"/>.</param>
        private void Watcher_Disconnected(object sender, DotRas.RasConnectionEventArgs e)
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
        /// <param name="sender">The sender</param>
        /// <param name="e">The e</param>
        private void SaveStatsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveTotalStats();
        }

        /// <summary>
        /// Handles timer for stats updates.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The e</param>
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

                // Retrieve previous 'total' values from the settings
                var downloadedFromSettings = long.Parse(Settings.Default.Downloaded);
                var uploadedFromSettings = long.Parse(Settings.Default.Uploaded);

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
        /// Saves total stats.
        /// </summary>
        private void SaveTotalStats()
        {
            Settings.Default.Downloaded = $"{_totalDownloaded}";
            Settings.Default.Uploaded = $"{_totalUploaded}";
            Settings.Default.Save();
        }
    }
}
