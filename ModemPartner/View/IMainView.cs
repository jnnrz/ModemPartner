using System;
using System.Collections.Generic;
using System.Drawing;
using DotRas;
using ModemPartner.Core;

namespace ModemPartner.View
{
    public interface IMainView
    {
        event EventHandler LoadForm;

        event EventHandler AppClosing;

        event EventHandler RefreshDevicesClicked;

        event EventHandler OpenPortClicked;

        event EventHandler ApplyModeClicked;

        event EventHandler ConnectionClicked;

        int NumberFoundDevices { get; }
        string SelectedModem { get; }
        bool DisableDeviceRelatedControls { set; }
        int SelectedMode { get; }
        string SelectedProfile { get; }

        void ClearDeviceList();

        void AddDevicesToList(Dictionary<string, FoundModem> devices);

        void AddProfilesToList(RasEntryCollection profiles);

        void UpdateToolStripStatus(string status);

        void UpdateOpenPortBtn(Bitmap icon, string tooltipText);

        void UpdateModeSelection(Modem.Mode mode);

        void UpdateRSSI(float rssi);

        void UpdatePSNetwork(int status);

        void UpdateCSNetwork(int status);

        void UpdatePSAttachment(int status);

        void UpdateProvider(string provider);

        void UpdateSubMode(Modem.SubMode mode);

        void UpdateUIWhenConnected();

        void UpdateUIWhenDisconnected();

        void UpdateUIWhenDialing();
    }
}