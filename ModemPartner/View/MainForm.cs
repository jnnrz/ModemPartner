using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModemPartner.Core;
using ModemPartner.View;

namespace ModemPartner
{
    public partial class MainForm : Form, IMainView
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public event EventHandler RefreshDevicesClicked;

        public void ClearDeviceList()
        {
            cbDevices.Items.Clear();
        }

        public void AddDevicesToList(Dictionary<string, FoundModem> devices)
        {
            foreach (var device in devices)
            {
                cbDevices.Items.Add(device.Key);
            }

            cbDevices.SelectedIndex = 0;
        }

        private void btnDeviceRefresh_Click(object sender, EventArgs e)
        {
            if (RefreshDevicesClicked != null)
            {
                RefreshDevicesClicked(sender, e);
            }
        }
    }
}
