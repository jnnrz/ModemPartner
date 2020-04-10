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

        public int NumberFoundDevices { get => cbDevices.Items.Count; }

        public void ClearDeviceList()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.cbDevices.Items.Clear()));
            }
            else
            {
                cbDevices.Items.Clear();
            }
        }

        public void AddDevicesToList(Dictionary<string, FoundModem> devices)
        {
            foreach (var device in devices)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => this.cbDevices.Items.Add(device.Key)));
                }
                else
                {
                    cbDevices.Items.Add(device.Key);
                }                
            }

            if (devices.Count > 0)
                cbDevices.SelectedIndex = 0;
        }

        public void UpdateToolStripStatus(string status)
        {
            tslblStatus.Text = status;
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
