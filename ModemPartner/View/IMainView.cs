using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModemPartner.Core;

namespace ModemPartner.View
{
    public interface IMainView
    {
        event EventHandler RefreshDevicesClicked;

        void ClearDeviceList();
        void AddDevicesToList(Dictionary<string, FoundModem> devices);
    }
}
