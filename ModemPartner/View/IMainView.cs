using System;
using System.Collections.Generic;
using System.Drawing;
using DotRas;
using ModemPartner.Core;

namespace ModemPartner.View
{
    /// <summary>
    /// Inteface for the main form view.
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Occurs when <see cref="MainForm"/> loads.
        /// </summary>
        event EventHandler LoadForm;

        /// <summary>
        /// Occurs when the app is about to close.
        /// </summary>
        event EventHandler AppClosing;

        /// <summary>
        /// Occurs when the refresh devices button is clicked.
        /// </summary>
        event EventHandler RefreshDevicesClicked;

        /// <summary>
        /// Occurs when the 'open port' button is clicked.
        /// </summary>
        event EventHandler OpenPortClicked;

        /// <summary>
        /// Occurs when the 'apply mode' is clicked.
        /// </summary>
        event EventHandler ApplyModeClicked;

        /// <summary>
        /// Occurs when the 'connect' button is clicked.
        /// </summary>
        event EventHandler ConnectionClicked;

        /// <summary>
        /// Occurs when the 'reset session' button is clicked.
        /// </summary>
        event EventHandler ResetSessionClicked;

        /// <summary>
        /// Occurs when the 'reset' button is clicked.
        /// </summary>
        event EventHandler ResetClicked;

        /// <summary>
        /// Gets the NumberFoundDevices.
        /// </summary>
        int NumberFoundDevices { get; }

        /// <summary>
        /// Gets or sets the SelectedModem.
        /// </summary>
        string SelectedModem { get; set; }

        /// <summary>
        /// Gets the SelectedMode.
        /// </summary>
        int SelectedMode { get; }

        /// <summary>
        /// Gets the SelectedProfile.
        /// </summary>
        string SelectedProfile { get; }

        /// <summary>
        /// Sets a value indicating whether DisableDeviceRelatedControls.
        /// </summary>
        /// <param name="value">Controls wether to show the controls or not.</param>
        void DisableDeviceRelatedControls(bool value);

        /// <summary>
        /// Clears the device list.
        /// </summary>
        void ClearDeviceList();

        /// <summary>
        /// Adds devices to the device list.
        /// </summary>
        /// <param name="devices">The devices<see cref="Dictionary{string, FoundModem}"/>.</param>
        void AddDevicesToList(Dictionary<string, FoundModem> devices);

        /// <summary>
        /// Adds profiles to the profile list.
        /// </summary>
        /// <param name="profiles">The profiles<see cref="RasEntryCollection"/>.</param>
        void AddProfilesToList(RasEntryCollection profiles);

        /// <summary>
        /// Updates the status label on the tool strip.
        /// </summary>
        /// <param name="status">The status<see cref="string"/>.</param>
        void UpdateToolStripStatus(string status);

        /// <summary>
        /// Updates the 'open port' button.
        /// </summary>
        /// <param name="icon">The icon<see cref="Bitmap"/>.</param>
        /// <param name="tooltipText">The tooltip text<see cref="string"/>.</param>
        void UpdateOpenPortBtn(Bitmap icon, string tooltipText);

        /// <summary>
        /// Updates the current mode on the mode combo box.
        /// </summary>
        /// <param name="mode">The mode<see cref="Modem.Mode"/>.</param>
        void UpdateModeSelection(Modem.Mode mode);

        /// <summary>
        /// Updates the RSSI progress bar and status label.
        /// </summary>
        /// <param name="rssi">The rssi<see cref="float"/>.</param>
        void UpdateRSSI(float rssi);

        /// <summary>
        /// Updates the PS network label.
        /// </summary>
        /// <param name="status">The status<see cref="int"/>.</param>
        void UpdatePSNetwork(int status);

        /// <summary>
        /// Updates the CS network label.
        /// </summary>
        /// <param name="status">The status<see cref="int"/>.</param>
        void UpdateCSNetwork(int status);

        /// <summary>
        /// Updates the PS network attachment label.
        /// </summary>
        /// <param name="status">The status<see cref="int"/>.</param>
        void UpdatePSAttachment(int status);

        /// <summary>
        /// Updates the provider label.
        /// </summary>
        /// <param name="provider">The provider<see cref="string"/>.</param>
        void UpdateProvider(string provider);

        /// <summary>
        /// Updates the sub mode label on the tool strip.
        /// </summary>
        /// <param name="mode">The mode<see cref="Modem.SubMode"/>.</param>
        void UpdateSubMode(Modem.SubMode mode);

        /// <summary>
        /// Updates elements of the UI when a connection is dialed.
        /// </summary>
        void UpdateUIWhenConnected();

        /// <summary>
        /// Updates elements of the UI when a connection is interrupted.
        /// </summary>
        void UpdateUIWhenDisconnected();

        /// <summary>
        /// Updates elements of the UI when a connection is been dialed.
        /// </summary>
        void UpdateUIWhenDialing();

        /// <summary>
        /// Updates download speed label.
        /// </summary>
        /// <param name="speed">Current speed.</param>
        void UpdateDownloadSpeed(string speed);

        /// <summary>
        /// Updates upload speed label.
        /// </summary>
        /// <param name="speed">Current speed.</param>
        void UpdateUploadSpeed(string speed);

        /// <summary>
        /// Updates total download label.
        /// </summary>
        /// <param name="total">Current total.</param>
        void UpdateTotalDownloaded(string total);

        /// <summary>
        /// Updates total uploaded label.
        /// </summary>
        /// <param name="total">Current total.</param>
        void UpdateTotalUploaded(string total);

        /// <summary>
        /// Updates session download label.
        /// </summary>
        /// <param name="total">Session download total.</param>
        void UpdateSessionDownload(string total);

        /// <summary>
        /// Updates session upload label.
        /// </summary>
        /// <param name="total">Session upload total.</param>
        void UpdateSessionUpload(string total);

        /// <summary>
        ///  Updates chart points.
        /// </summary>
        /// <param name="downloadValue">New download point.</param>
        /// <param name="uploadValue">New upload point.</param>
        void UpdateChart(double downloadValue, double uploadValue);

        /// <summary>
        /// Updates connection duration label.
        /// </summary>
        /// <param name="duration">Duration</param>
        void UpdateConnDuration(string duration);
    }
}
