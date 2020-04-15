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

        public event EventHandler LoadForm;
        public event EventHandler RefreshDevicesClicked;
        public event EventHandler OpenPortClicked;

        public int NumberFoundDevices { get => cbDevices.Items.Count; }
        public string SelectedModem { get => cbDevices.SelectedItem.ToString(); }

        public bool DisableControls
        {
            set
            {
                cbDevices.Enabled = !value;
                cbProfiles.Enabled = !value;
                btnDeviceRefresh.Enabled = !value;
            }
        }

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
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(() => cbDevices.SelectedIndex = 0));
            }
                
        }

        public void UpdateToolStripStatus(string status)
        {
            tslblStatus.Text = status;
        }

        public void UpdateOpenPortBtn(Bitmap icon, string tooltipText)
        {
            btnOpen.Image = icon;
        }

        public void UpdateModeSelection(Modem.Mode mode)
        {
            int index = 0;
            switch (mode)
            {
                case Modem.Mode.TwoGOnly:
                    index = 0;
                    break;
                case Modem.Mode.TwoGPref:
                    index = 1;
                    break;
                case Modem.Mode.ThreeGOnly:
                    index = 2;
                    break;
                case Modem.Mode.ThreeGPref:
                    index = 3;
                    break;
                default:
                    index = 2;
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.cbModes.SelectedIndex = index));
            }
            else
            {
                cbModes.SelectedIndex = index;
            }
        }

        public void UpdateRSSI(float rssi)
        {
            Color lblColor = Color.Black;
            string lblText = "--";

            if (rssi >= 2 && rssi < 10)
            {
                lblColor = Color.Crimson;
                lblText = "Bad";
            }

            if (rssi >= 10 && rssi < 15)
            {
                lblColor = Color.DeepSkyBlue;
                lblText = "OK";
            }

            if (rssi >= 15 && rssi < 20)
            {
                lblColor = Color.Green;
                lblText = "Good";
            }

            if (rssi >= 20 && rssi <= 30 )
            {
                lblColor = Color.FromArgb(0, 192, 0);
                lblText = "Excellent";
            }

            lblRSSI.ForeColor = lblColor;

            if (InvokeRequired)
            {                
                this.Invoke(new MethodInvoker(() => {
                    lblRSSI.Text = $"{lblText} {(rssi > 1 ? $"- {rssi}" : "")}";
                    pbRSSI.Value = Convert.ToInt32(rssi);
                }));
            }
            else
            {
                lblRSSI.Text = $"{lblText} {(rssi > 1 ? $"- {rssi}" : "")}";
                pbRSSI.Value = Convert.ToInt32(rssi);
            }
        }

        public void UpdatePSNetwork(int status)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.UpdateNetworkRegLabel(lblPS, status)));
            }
            else
            {
                UpdateNetworkRegLabel(lblPS, status);
            }
        }

        public void UpdateCSNetwork(int status)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.UpdateNetworkRegLabel(lblCS, status)));                
            }
            else
            {
                UpdateNetworkRegLabel(lblCS, status);
            }
        }

        public void UpdatePSAttachment(int status)
        {
            Color lblColor = Color.Black;
            string lblText = "--";

            switch (status)
            {
                case 0:
                    lblText = "Not attached";
                    break;
                case 1:
                    lblColor = Color.DeepSkyBlue;
                    lblText = "Attached";
                    break;
                case 2:
                    lblColor = Color.Black;
                    lblText = "--";
                    break;
                default:
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    lblPSAttach.ForeColor = lblColor;
                    lblPSAttach.Text = lblText;
                }));
            }
            else
            {
                lblPSAttach.ForeColor = lblColor;
                lblPSAttach.Text = lblText;
            }
        }

        void UpdateNetworkRegLabel(Control ctrl, int status)
        {
            switch (status)
            {
                case 0:
                    ctrl.Text = "Not Registered";
                    ctrl.ForeColor = Color.Black;
                    break;
                case 1:
                    ctrl.ForeColor = Color.DeepSkyBlue;
                    ctrl.Text = "Registered";
                    break;
                case 2:
                    ctrl.ForeColor = Color.DarkBlue;
                    ctrl.Text = "Searching";
                    break;
                case 3:
                    ctrl.ForeColor = Color.Crimson;
                    ctrl.Text = "Denied";
                    break;
                case 4:
                    lblCS.Text = "Unknown";
                    break;
                case 5:
                    ctrl.ForeColor = Color.Salmon;
                    ctrl.Text = "Roaming";
                    break;
                case 6:
                    ctrl.ForeColor = Color.Black;
                    ctrl.Text = "--";
                    break;
                default:
                    ctrl.Text = "--";
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadForm?.Invoke(sender, e);
        }

        private void btnDeviceRefresh_Click(object sender, EventArgs e)
        {
            if (RefreshDevicesClicked != null)
            {
                RefreshDevicesClicked(sender, e);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (OpenPortClicked != null)
            {
                OpenPortClicked(sender, e);
            }
        }
    }
}
