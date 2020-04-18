using System;
using System.Collections.Generic;
using System.Drawing;
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

        public event EventHandler AppClosing;

        public event EventHandler RefreshDevicesClicked;

        public event EventHandler OpenPortClicked;

        public event EventHandler ApplyModeClicked;

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

        public int SelectedMode { get => cbModes.SelectedIndex; }

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

            if (rssi >= 20 && rssi <= 30)
            {
                lblColor = Color.FromArgb(0, 192, 0);
                lblText = "Excellent";
            }

            lblRSSI.ForeColor = lblColor;

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    lblRSSI.Text = $"{lblText} {(rssi > 1 ? $"- {rssi}" : "")}";

                    var value = Convert.ToInt32(rssi);

                    if (value > pbRSSI.Maximum)
                    {
                        pbRSSI.Value = 1;
                    }
                    else
                    {
                        pbRSSI.Value = value;
                    }
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

        public void UpdateProvider(string provider)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.lblProvider.Text = provider));
            }
            else
            {
                this.lblProvider.Text = provider;
            }
        }

        private void UpdateNetworkRegLabel(Control ctrl, int status)
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

        public void UpdateSubMode(Modem.SubMode mode)
        {
            var text = "";

            switch (mode)
            {
                case Modem.SubMode.NoService:
                    text = "NA";
                    break;

                case Modem.SubMode.GSM:
                    text = "GSM";
                    break;

                case Modem.SubMode.GPRS:
                    text = "GPRS";
                    break;

                case Modem.SubMode.EDGE:
                    text = "EDGE";
                    break;

                case Modem.SubMode.WCDMA:
                    text = "WCDMA 3G";
                    break;

                case Modem.SubMode.HSDPA:
                    text = "HSDPA 3.5G";
                    break;

                case Modem.SubMode.HSUPA:
                    text = "HSUPA 3.7G";
                    break;

                case Modem.SubMode.HSPA:
                    text = "HSPA 3.9G";
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => tslblSubMode.Text = text));
            }
            else
            {
                tslblSubMode.Text = text;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadForm?.Invoke(sender, e);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppClosing?.Invoke(sender, e);
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

        private void btnModeApply_Click(object sender, EventArgs e)
        {
            ApplyModeClicked?.Invoke(sender, e);
        }
    }
}