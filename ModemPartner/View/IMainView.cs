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

        int NumberFoundDevices { get; }
        string SelectedModem { get; }
        bool DisableControls { set; }

        void ClearDeviceList();
        void AddDevicesToList(Dictionary<string, FoundModem> devices);
        void UpdateToolStripStatus(string status);
        void UpdateOpenPortBtn(Bitmap icon, string tooltipText);
        void UpdateModeSelection(Modem.Mode mode);
        void UpdateRSSI(float rssi);
        void UpdatePSNetwork(int status);
        void UpdateCSNetwork(int status);
    }
}
