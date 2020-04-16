using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModemPartner.Core;

namespace ModemPartner.View
{
    public interface IMainView
    {
        event EventHandler LoadForm;
        event EventHandler RefreshDevicesClicked;
        event EventHandler OpenPortClicked;
        event EventHandler ApplyModeClicked;

        int NumberFoundDevices { get; }
        string SelectedModem { get; }
        bool DisableControls { set; }
        int SelectedMode { get; }

        void ClearDeviceList();
        void AddDevicesToList(Dictionary<string, FoundModem> devices);
        void UpdateToolStripStatus(string status);
        void UpdateOpenPortBtn(Bitmap icon, string tooltipText);
        void UpdateModeSelection(Modem.Mode mode);
        void UpdateRSSI(float rssi);
        void UpdatePSNetwork(int status);
        void UpdateCSNetwork(int status);
        void UpdatePSAttachment(int status);
        void UpdateProvider(string provider);
        void UpdateSubMode(Modem.SubMode mode);
    }
}
