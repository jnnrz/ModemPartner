﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Windows.Forms;
using DotRas;
using ModemPartner.Core;
using ModemPartner.Properties;

namespace ModemPartner.View
{
    /// <summary>
    /// Defines the app's main form.
    /// </summary>
    public partial class MainForm : Form, IMainView
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_SEPARATOR = 0x800;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MF_STRING = 0x0;

        public const Int32 _AboutSysMenuID = 1001;

        /// <summary>
        /// Previous Y axis value.
        /// </summary>
        private double _oldAxisYValue = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public event EventHandler LoadForm;

        /// <inheritdoc/>
        public event EventHandler AppClosing;

        /// <inheritdoc/>
        public event EventHandler RefreshDevicesClicked;

        /// <inheritdoc/>
        public event EventHandler ApplyModeClicked;

        /// <inheritdoc/>
        public event EventHandler ConnectionClicked;

        /// <inheritdoc/>
        public event EventHandler ResetSessionClicked;

        /// <inheritdoc/>
        public event EventHandler ResetClicked;

        /// <inheritdoc/>
        public event EventHandler OpenDeviceInfoFormClicked;

        /// <inheritdoc/>
        public event EventHandler SelectedDeviceChanged;

        /// <inheritdoc/>
        public int NumberFoundDevices => cbDevices.Items.Count;

        /// <inheritdoc/>
        public string SelectedModem
        {
            get => cbDevices.SelectedItem.ToString();
            set
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => cbDevices.SelectedIndex = cbDevices.Items.IndexOf(value)));
                }
                else
                {
                    cbDevices.SelectedIndex = cbDevices.Items.IndexOf(value);
                }
            }
        }

        /// <inheritdoc/>
        public int SelectedMode => cbModes.SelectedIndex;

        /// <inheritdoc/>
        public string SelectedProfile => cbProfiles.SelectedItem.ToString();

        /// <inheritdoc/>
        public bool Retry => ckbRetry.Checked;

        /// <inheritdoc/>
        public void DisableDeviceRelatedControls(bool value)
        {
            cbDevices.Enabled = !value;
            btnDeviceRefresh.Enabled = !value;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void AddDevicesToList(List<Device> devices)
        {
            if (devices == null)
            {
                UpdateToolStripStatus("There are no devices that can be added to the list.");
                return;
            }

            foreach (var device in devices)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => this.cbDevices.Items.Add(device.Model)));
                }
                else
                {
                    cbDevices.Items.Add(device.Model);
                }
            }
        }

        /// <inheritdoc/>
        public void AddProfilesToList(RasEntryCollection profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException();
            }

            var defaultProfile = Settings.Default.DefaultProfile;

            for (var i = 0; i < profiles.Count; i++)
            {
                if (this.InvokeRequired)
                {
                    var i1 = i;
                    this.Invoke(new MethodInvoker(() =>
                    {
                        cbProfiles.Items.Add(profiles[i1].Name);

                        if (profiles[i1].Name.Equals(defaultProfile))
                        {
                            cbProfiles.SelectedIndex = i1;
                        }
                    }));
                }
                else
                {
                    cbProfiles.Items.Add(profiles[i].Name);

                    if (profiles[i].Name.Equals(defaultProfile))
                    {
                        cbProfiles.SelectedIndex = i;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void UpdateToolStripStatus(string status)
        {
            tslblStatus.Text = status;
        }

        /// <inheritdoc/>
        public void UpdateModeSelection(Modem.Mode mode)
        {
            int index;
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

        /// <inheritdoc/>
        public void UpdateRSSI(float rssi)
        {
            Color lblColor = Color.Black;
            string lblText = Resources.NoRealValue;

            if (rssi >= 2 && rssi < 10)
            {
                lblColor = Color.Crimson;
                lblText = Resources.Bad;
            }

            if (rssi >= 10 && rssi < 15)
            {
                lblColor = Color.DeepSkyBlue;
                lblText = Resources.OK;
            }

            if (rssi >= 15 && rssi < 20)
            {
                lblColor = Color.Green;
                lblText = Resources.Good;
            }

            if (rssi >= 20 && rssi <= 30)
            {
                lblColor = Color.FromArgb(0, 192, 0);
                lblText = Resources.Excellent;
            }

            lblRSSI.ForeColor = lblColor;

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    lblRSSI.Text = $@"{lblText} {(rssi > 1 ? $"- {rssi}" : string.Empty)}";

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
                lblRSSI.Text = $@"{lblText} {(rssi > 1 ? $"- {rssi}" : string.Empty)}";
                pbRSSI.Value = Convert.ToInt32(rssi);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void UpdatePSAttachment(int status)
        {
            Color lblColor = Color.Black;
            string lblText = Resources.NoRealValue;

            switch (status)
            {
                case 0:
                    lblText = Resources.NotAttached;
                    break;

                case 1:
                    lblColor = Color.DeepSkyBlue;
                    lblText = Resources.Attached;
                    break;

                case 2:
                    lblColor = Color.Black;
                    lblText = Resources.NoRealValue;
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void UpdateSubMode(Modem.SubMode mode)
        {
            var text = string.Empty;

            switch (mode)
            {
                case Modem.SubMode.NoService:
                    text = "NA";
                    break;

                case Modem.SubMode.Gsm:
                    text = "GSM";
                    break;

                case Modem.SubMode.Gprs:
                    text = "GPRS";
                    break;

                case Modem.SubMode.Edge:
                    text = "EDGE";
                    break;

                case Modem.SubMode.Wcdma:
                    text = "WCDMA 3G";
                    break;

                case Modem.SubMode.Hsdpa:
                    text = "HSDPA 3.5G";
                    break;

                case Modem.SubMode.Hsupa:
                    text = "HSUPA 3.7G";
                    break;

                case Modem.SubMode.Hspa:
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

        /// <inheritdoc/>
        public void UpdateUIWhenConnected()
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    btnConnect.Text = Resources.Disconnect;
                    tslblDialStatus.Image = Resources.green_ball;
                    cbProfiles.Enabled = false;
                }));
            }
            else
            {
                btnConnect.Text = Resources.Disconnect;
                tslblDialStatus.Image = Resources.green_ball;
                cbProfiles.Enabled = false;
            }
        }

        /// <inheritdoc/>
        public void UpdateUIWhenDisconnected()
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    btnConnect.Text = Resources.Connect;
                    tslblDialStatus.Image = Resources.red_ball;
                    cbProfiles.Enabled = true;
                }));
            }
            else
            {
                btnConnect.Text = Resources.Connect;
                tslblDialStatus.Image = Resources.red_ball;
                cbProfiles.Enabled = true;
            }
        }

        /// <inheritdoc/>
        public void UpdateUIWhenDialing()
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    btnConnect.Text = Resources.Cancel;
                    tslblDialStatus.Image = Resources.red_ball;
                    cbProfiles.Enabled = false;
                }));
            }
            else
            {
                btnConnect.Text = Resources.Cancel;
                tslblDialStatus.Image = Resources.red_ball;
                cbProfiles.Enabled = false;
            }
        }

        /// <inheritdoc/>
        public void UpdateDownloadSpeed(string speed)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblDownloadSpeed.Text = speed));
            }
            else
            {
                lblDownloadSpeed.Text = speed;
            }
        }

        /// <inheritdoc/>
        public void UpdateUploadSpeed(string speed)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblUploadSpeed.Text = speed));
            }
            else
            {
                lblUploadSpeed.Text = speed;
            }
        }

        /// <inheritdoc/>
        public void UpdateTotalDownloaded(string total)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblDownloaded.Text = total));
            }
            else
            {
                lblDownloaded.Text = total;
            }
        }

        /// <inheritdoc/>
        public void UpdateTotalUploaded(string total)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblUploaded.Text = total));
            }
            else
            {
                lblUploaded.Text = total;
            }
        }

        /// <inheritdoc/>
        public void UpdateSessionDownload(string total)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblSessionDownload.Text = total));
            }
            else
            {
                lblSessionDownload.Text = total;
            }
        }

        /// <inheritdoc/>
        public void UpdateSessionUpload(string total)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblSessionUpload.Text = total));
            }
            else
            {
                lblSessionUpload.Text = total;
            }
        }

        /// <inheritdoc/>
        public void UpdateChart(double downloadValue, double uploadValue)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    var downSeries = chart.Series.FindByName("DownloadSeries");
                    if (downSeries == null)
                    {
                        throw new Exception("Could not get DownloadSeries");
                    }

                    downSeries.Points.Add(downloadValue);

                    var upSeries = chart.Series.FindByName("UploadSeries");
                    if (upSeries == null)
                    {
                        throw new Exception("Could not get UploadSeries");
                    }

                    upSeries.Points.Add(uploadValue);

                    var ay = chart.ChartAreas[0].AxisY;

                    if (downloadValue > _oldAxisYValue || uploadValue > _oldAxisYValue)
                    {
                        var max = uploadValue;
                        if (downloadValue > uploadValue)
                        {
                            max = downloadValue;
                        }

                        ay.Maximum = max;
                        _oldAxisYValue = max;
                    }

                    if (downSeries.Points.Count > 100)
                    {
                        downSeries.Points.RemoveAt(0);
                        upSeries.Points.RemoveAt(0);
                    }
                }));
            }
            else
            {
                var downSeries = chart.Series.FindByName("DownloadSeries");
                if (downSeries == null)
                {
                    throw new Exception("Could not get DownloadSeries");
                }

                downSeries.Points.Add(downloadValue);

                var upSeries = chart.Series.FindByName("UploadSeries");
                if (upSeries == null)
                {
                    throw new Exception("Could not get UploadSeries");
                }

                var ay = chart.ChartAreas[0].AxisY;

                if (downloadValue > _oldAxisYValue || uploadValue > _oldAxisYValue)
                {
                    var max = uploadValue;
                    if (downloadValue > uploadValue)
                    {
                        max = downloadValue;
                    }

                    ay.Maximum = max;
                    _oldAxisYValue = max;
                }

                if (downSeries.Points.Count > 100)
                {
                    downSeries.Points.RemoveAt(0);
                    upSeries.Points.RemoveAt(0);
                }
            }
        }

        /// <inheritdoc/>
        public void UpdateConnDuration(string duration)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => lblSessionDuration.Text = duration));
            }
            else
            {
                lblSessionDuration.Text = duration;
            }
        }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                switch (m.WParam.ToInt32())
                {
                    case _AboutSysMenuID:
                        var aboutForm = new AboutForm();
                        aboutForm.Show();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Updates network registration label.
        /// </summary>
        /// <param name="ctrl">Label control.</param>
        /// <param name="status">Registration status.</param>
        private void UpdateNetworkRegLabel(Control ctrl, int status)
        {
            switch (status)
            {
                case 0:
                    ctrl.Text = Resources.NotRegistered;
                    ctrl.ForeColor = Color.Black;
                    break;

                case 1:
                    ctrl.ForeColor = Color.DeepSkyBlue;
                    ctrl.Text = Resources.Registered;
                    break;

                case 2:
                    ctrl.ForeColor = Color.DarkBlue;
                    ctrl.Text = Resources.Searching;
                    break;

                case 3:
                    ctrl.ForeColor = Color.Crimson;
                    ctrl.Text = Resources.Denied;
                    break;

                case 4:
                    lblCS.Text = Resources.Unknown;
                    break;

                case 5:
                    ctrl.ForeColor = Color.Salmon;
                    ctrl.Text = Resources.Roaming;
                    break;

                case 6:
                    ctrl.ForeColor = Color.Black;
                    ctrl.Text = Resources.NoRealValue;
                    break;

                default:
                    ctrl.Text = Resources.NoRealValue;
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Get the Handle for the Forms System Menu
            IntPtr systemMenuHandle = GetSystemMenu(this.Handle, false);

            // Insert a separator and a 'about' button
            InsertMenu(systemMenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
            InsertMenu(systemMenuHandle, 6, MF_BYPOSITION, _AboutSysMenuID, "About...");

            var downSeries = chart.Series.FindByName("DownloadSeries");
            if (downSeries != null)
            {
                downSeries.Color = Color.FromArgb(134, 196, 63);
            }

            var upSeries = chart.Series.FindByName("UploadSeries");
            if (upSeries != null)
            {
                upSeries.Color = Color.FromArgb(50, 153, 255);
            }

            LoadForm?.Invoke(sender, e);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppClosing?.Invoke(sender, e);
        }

        private void BtnDeviceRefresh_Click(object sender, EventArgs e)
        {
            RefreshDevicesClicked?.Invoke(sender, e);
        }

        private void BtnModeApply_Click(object sender, EventArgs e)
        {
            ApplyModeClicked?.Invoke(sender, e);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            ConnectionClicked?.Invoke(sender, e);
        }

        private void BtnResetSession_Click(object sender, EventArgs e)
        {
            ResetSessionClicked?.Invoke(sender, e);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetClicked?.Invoke(sender, e);
        }

        private void BtnModemInfo_Click(object sender, EventArgs e)
        {
            OpenDeviceInfoFormClicked?.Invoke(sender, e);
        }

        private void CbDevices_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SelectedDeviceChanged?.Invoke(sender, e);
        }
    }
}