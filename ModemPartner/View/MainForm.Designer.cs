namespace ModemPartner.View
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lblDevices = new System.Windows.Forms.Label();
            this.cbDevices = new System.Windows.Forms.ComboBox();
            this.btnDeviceRefresh = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tslblDialStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblSubMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabChart = new System.Windows.Forms.TabPage();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbProfiles = new System.Windows.Forms.ComboBox();
            this.lblProfile_label = new System.Windows.Forms.Label();
            this.lblMode_label = new System.Windows.Forms.Label();
            this.cbModes = new System.Windows.Forms.ComboBox();
            this.btnModeApply = new System.Windows.Forms.Button();
            this.lblDownloadSpeed_label = new System.Windows.Forms.Label();
            this.lblUploadSpeed_label = new System.Windows.Forms.Label();
            this.lblDownloadSpeed = new System.Windows.Forms.Label();
            this.lblUploadSpeed = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnResetSession = new System.Windows.Forms.Button();
            this.lblSessionUpload = new System.Windows.Forms.Label();
            this.lblSessionUpload_label = new System.Windows.Forms.Label();
            this.lblSessionDownload = new System.Windows.Forms.Label();
            this.lblSessionDownload_label = new System.Windows.Forms.Label();
            this.lblSessionDuration = new System.Windows.Forms.Label();
            this.lblSessionDuration_label = new System.Windows.Forms.Label();
            this.lblUploaded = new System.Windows.Forms.Label();
            this.lblUploaded_label = new System.Windows.Forms.Label();
            this.lblDownloaded = new System.Windows.Forms.Label();
            this.lblDownloaded_label = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblRSSI = new System.Windows.Forms.Label();
            this.lblPSAttach = new System.Windows.Forms.Label();
            this.lblPS = new System.Windows.Forms.Label();
            this.lblCS = new System.Windows.Forms.Label();
            this.lblProvider = new System.Windows.Forms.Label();
            this.pbRSSI = new System.Windows.Forms.ProgressBar();
            this.label12 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnModemInfo = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.chart)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDevices
            // 
            this.lblDevices.AutoSize = true;
            this.lblDevices.Location = new System.Drawing.Point(12, 15);
            this.lblDevices.Name = "lblDevices";
            this.lblDevices.Size = new System.Drawing.Size(50, 15);
            this.lblDevices.TabIndex = 0;
            this.lblDevices.Text = "Devices:";
            // 
            // cbDevices
            // 
            this.cbDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDevices.FormattingEnabled = true;
            this.cbDevices.Location = new System.Drawing.Point(68, 12);
            this.cbDevices.Name = "cbDevices";
            this.cbDevices.Size = new System.Drawing.Size(191, 23);
            this.cbDevices.TabIndex = 1;
            this.cbDevices.SelectionChangeCommitted += new System.EventHandler(this.CbDevices_SelectionChangeCommitted);
            // 
            // btnDeviceRefresh
            // 
            this.btnDeviceRefresh.Image = ((System.Drawing.Image) (resources.GetObject("btnDeviceRefresh.Image")));
            this.btnDeviceRefresh.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDeviceRefresh.Location = new System.Drawing.Point(263, 11);
            this.btnDeviceRefresh.Name = "btnDeviceRefresh";
            this.btnDeviceRefresh.Size = new System.Drawing.Size(28, 25);
            this.btnDeviceRefresh.TabIndex = 2;
            this.btnDeviceRefresh.UseVisualStyleBackColor = true;
            this.btnDeviceRefresh.Click += new System.EventHandler(this.BtnDeviceRefresh_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.tslblDialStatus, this.tslblSubMode, this.tslblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 487);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(651, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tslblDialStatus
            // 
            this.tslblDialStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tslblDialStatus.Image = global::ModemPartner.Properties.Resources.red_ball;
            this.tslblDialStatus.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.tslblDialStatus.Name = "tslblDialStatus";
            this.tslblDialStatus.Size = new System.Drawing.Size(16, 17);
            // 
            // tslblSubMode
            // 
            this.tslblSubMode.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.tslblSubMode.Name = "tslblSubMode";
            this.tslblSubMode.Size = new System.Drawing.Size(17, 17);
            this.tslblSubMode.Text = "--";
            // 
            // tslblStatus
            // 
            this.tslblStatus.Name = "tslblStatus";
            this.tslblStatus.Size = new System.Drawing.Size(17, 17);
            this.tslblStatus.Text = "--";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabChart);
            this.tabs.Controls.Add(this.tabHistory);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabs.Location = new System.Drawing.Point(0, 255);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(651, 232);
            this.tabs.TabIndex = 10;
            // 
            // tabChart
            // 
            this.tabChart.Controls.Add(this.chart);
            this.tabChart.Location = new System.Drawing.Point(4, 24);
            this.tabChart.Name = "tabChart";
            this.tabChart.Padding = new System.Windows.Forms.Padding(3);
            this.tabChart.Size = new System.Drawing.Size(643, 204);
            this.tabChart.TabIndex = 0;
            this.tabChart.Text = "Chart";
            this.tabChart.UseVisualStyleBackColor = true;
            // 
            // chart
            // 
            this.chart.BorderlineColor = System.Drawing.Color.Empty;
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisX.IntervalOffset = 1D;
            chartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            chartArea1.AxisX.LabelStyle.Interval = 1D;
            chartArea1.AxisX.LabelStyle.IntervalOffset = 1D;
            chartArea1.AxisX.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            chartArea1.AxisX2.TitleFont = new System.Drawing.Font("Segoe UI", 8.25F);
            chartArea1.AxisY.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.SteelBlue;
            chartArea1.AxisY.LabelStyle.Format = "{0} kbps";
            chartArea1.AxisY.LineColor = System.Drawing.Color.LightSalmon;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.SeaShell;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.AxisY.MajorTickMark.Interval = 0D;
            chartArea1.AxisY.MaximumAutoSize = 100F;
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 8.25F);
            chartArea1.AxisY2.TitleFont = new System.Drawing.Font("Segoe UI", 8.25F);
            chartArea1.Name = "MainChartArea";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 98F;
            chartArea1.Position.Width = 100F;
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Location = new System.Drawing.Point(0, 3);
            this.chart.Name = "chart";
            series1.BorderWidth = 2;
            series1.ChartArea = "MainChartArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            series1.IsVisibleInLegend = false;
            series1.Name = "DownloadSeries";
            series1.SmartLabelStyle.Enabled = false;
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series2.BorderWidth = 2;
            series2.ChartArea = "MainChartArea";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Name = "UploadSeries";
            this.chart.Series.Add(series1);
            this.chart.Series.Add(series2);
            this.chart.Size = new System.Drawing.Size(647, 201);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart";
            // 
            // tabHistory
            // 
            this.tabHistory.Location = new System.Drawing.Point(4, 24);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistory.Size = new System.Drawing.Size(643, 204);
            this.tabHistory.TabIndex = 1;
            this.tabHistory.Text = "History";
            this.tabHistory.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(551, 215);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(88, 34);
            this.btnConnect.TabIndex = 11;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // cbProfiles
            // 
            this.cbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfiles.FormattingEnabled = true;
            this.cbProfiles.Location = new System.Drawing.Point(411, 12);
            this.cbProfiles.Name = "cbProfiles";
            this.cbProfiles.Size = new System.Drawing.Size(149, 23);
            this.cbProfiles.TabIndex = 12;
            // 
            // lblProfile_label
            // 
            this.lblProfile_label.AutoSize = true;
            this.lblProfile_label.Location = new System.Drawing.Point(361, 15);
            this.lblProfile_label.Name = "lblProfile_label";
            this.lblProfile_label.Size = new System.Drawing.Size(44, 15);
            this.lblProfile_label.TabIndex = 13;
            this.lblProfile_label.Text = "Profile:";
            // 
            // lblMode_label
            // 
            this.lblMode_label.AutoSize = true;
            this.lblMode_label.Location = new System.Drawing.Point(20, 45);
            this.lblMode_label.Name = "lblMode_label";
            this.lblMode_label.Size = new System.Drawing.Size(41, 15);
            this.lblMode_label.TabIndex = 20;
            this.lblMode_label.Text = "Mode:";
            // 
            // cbModes
            // 
            this.cbModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModes.FormattingEnabled = true;
            this.cbModes.Items.AddRange(new object[] {"2G Only", "2G Preferred", "3G Only", "3G Preferred"});
            this.cbModes.Location = new System.Drawing.Point(67, 42);
            this.cbModes.Name = "cbModes";
            this.cbModes.Size = new System.Drawing.Size(121, 23);
            this.cbModes.TabIndex = 21;
            // 
            // btnModeApply
            // 
            this.btnModeApply.Location = new System.Drawing.Point(194, 41);
            this.btnModeApply.Name = "btnModeApply";
            this.btnModeApply.Size = new System.Drawing.Size(53, 25);
            this.btnModeApply.TabIndex = 22;
            this.btnModeApply.Text = "Apply";
            this.btnModeApply.UseVisualStyleBackColor = true;
            this.btnModeApply.Click += new System.EventHandler(this.BtnModeApply_Click);
            // 
            // lblDownloadSpeed_label
            // 
            this.lblDownloadSpeed_label.AutoSize = true;
            this.lblDownloadSpeed_label.Location = new System.Drawing.Point(336, 59);
            this.lblDownloadSpeed_label.Name = "lblDownloadSpeed_label";
            this.lblDownloadSpeed_label.Size = new System.Drawing.Size(98, 15);
            this.lblDownloadSpeed_label.TabIndex = 23;
            this.lblDownloadSpeed_label.Text = "Download (kb/s):";
            // 
            // lblUploadSpeed_label
            // 
            this.lblUploadSpeed_label.AutoSize = true;
            this.lblUploadSpeed_label.Location = new System.Drawing.Point(493, 59);
            this.lblUploadSpeed_label.Name = "lblUploadSpeed_label";
            this.lblUploadSpeed_label.Size = new System.Drawing.Size(82, 15);
            this.lblUploadSpeed_label.TabIndex = 24;
            this.lblUploadSpeed_label.Text = "Upload (kb/s):";
            // 
            // lblDownloadSpeed
            // 
            this.lblDownloadSpeed.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblDownloadSpeed.ForeColor = System.Drawing.Color.Green;
            this.lblDownloadSpeed.Location = new System.Drawing.Point(434, 55);
            this.lblDownloadSpeed.Margin = new System.Windows.Forms.Padding(0);
            this.lblDownloadSpeed.Name = "lblDownloadSpeed";
            this.lblDownloadSpeed.Size = new System.Drawing.Size(55, 20);
            this.lblDownloadSpeed.TabIndex = 25;
            this.lblDownloadSpeed.Text = "--";
            // 
            // lblUploadSpeed
            // 
            this.lblUploadSpeed.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblUploadSpeed.ForeColor = System.Drawing.Color.Firebrick;
            this.lblUploadSpeed.Location = new System.Drawing.Point(575, 55);
            this.lblUploadSpeed.Margin = new System.Windows.Forms.Padding(0);
            this.lblUploadSpeed.Name = "lblUploadSpeed";
            this.lblUploadSpeed.Size = new System.Drawing.Size(55, 20);
            this.lblUploadSpeed.TabIndex = 26;
            this.lblUploadSpeed.Text = "--";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.btnResetSession);
            this.groupBox1.Controls.Add(this.lblSessionUpload);
            this.groupBox1.Controls.Add(this.lblSessionUpload_label);
            this.groupBox1.Controls.Add(this.lblSessionDownload);
            this.groupBox1.Controls.Add(this.lblSessionDownload_label);
            this.groupBox1.Controls.Add(this.lblSessionDuration);
            this.groupBox1.Controls.Add(this.lblSessionDuration_label);
            this.groupBox1.Controls.Add(this.lblUploaded);
            this.groupBox1.Controls.Add(this.lblUploaded_label);
            this.groupBox1.Controls.Add(this.lblDownloaded);
            this.groupBox1.Controls.Add(this.lblDownloaded_label);
            this.groupBox1.Location = new System.Drawing.Point(265, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(373, 134);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stats";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(272, 100);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(93, 23);
            this.btnReset.TabIndex = 18;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnResetSession
            // 
            this.btnResetSession.Location = new System.Drawing.Point(272, 74);
            this.btnResetSession.Name = "btnResetSession";
            this.btnResetSession.Size = new System.Drawing.Size(93, 23);
            this.btnResetSession.TabIndex = 17;
            this.btnResetSession.Text = "Reset Session";
            this.btnResetSession.UseVisualStyleBackColor = true;
            this.btnResetSession.Click += new System.EventHandler(this.BtnResetSession_Click);
            // 
            // lblSessionUpload
            // 
            this.lblSessionUpload.AutoSize = true;
            this.lblSessionUpload.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSessionUpload.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSessionUpload.Location = new System.Drawing.Point(138, 50);
            this.lblSessionUpload.Name = "lblSessionUpload";
            this.lblSessionUpload.Size = new System.Drawing.Size(13, 15);
            this.lblSessionUpload.TabIndex = 16;
            this.lblSessionUpload.Text = "0";
            // 
            // lblSessionUpload_label
            // 
            this.lblSessionUpload_label.AutoSize = true;
            this.lblSessionUpload_label.Location = new System.Drawing.Point(18, 49);
            this.lblSessionUpload_label.Name = "lblSessionUpload_label";
            this.lblSessionUpload_label.Size = new System.Drawing.Size(103, 15);
            this.lblSessionUpload_label.TabIndex = 15;
            this.lblSessionUpload_label.Text = "Session Uploaded:";
            // 
            // lblSessionDownload
            // 
            this.lblSessionDownload.AutoSize = true;
            this.lblSessionDownload.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSessionDownload.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSessionDownload.Location = new System.Drawing.Point(138, 29);
            this.lblSessionDownload.Name = "lblSessionDownload";
            this.lblSessionDownload.Size = new System.Drawing.Size(13, 15);
            this.lblSessionDownload.TabIndex = 14;
            this.lblSessionDownload.Text = "0";
            // 
            // lblSessionDownload_label
            // 
            this.lblSessionDownload_label.AutoSize = true;
            this.lblSessionDownload_label.Location = new System.Drawing.Point(18, 28);
            this.lblSessionDownload_label.Name = "lblSessionDownload_label";
            this.lblSessionDownload_label.Size = new System.Drawing.Size(119, 15);
            this.lblSessionDownload_label.TabIndex = 13;
            this.lblSessionDownload_label.Text = "Session Downloaded:";
            // 
            // lblSessionDuration
            // 
            this.lblSessionDuration.AutoSize = true;
            this.lblSessionDuration.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblSessionDuration.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblSessionDuration.Location = new System.Drawing.Point(307, 26);
            this.lblSessionDuration.Name = "lblSessionDuration";
            this.lblSessionDuration.Size = new System.Drawing.Size(55, 15);
            this.lblSessionDuration.TabIndex = 12;
            this.lblSessionDuration.Text = "00:00:00";
            // 
            // lblSessionDuration_label
            // 
            this.lblSessionDuration_label.AutoSize = true;
            this.lblSessionDuration_label.Location = new System.Drawing.Point(223, 26);
            this.lblSessionDuration_label.Name = "lblSessionDuration_label";
            this.lblSessionDuration_label.Size = new System.Drawing.Size(78, 15);
            this.lblSessionDuration_label.TabIndex = 11;
            this.lblSessionDuration_label.Text = "Session Time:";
            // 
            // lblUploaded
            // 
            this.lblUploaded.AutoSize = true;
            this.lblUploaded.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUploaded.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUploaded.Location = new System.Drawing.Point(138, 100);
            this.lblUploaded.Name = "lblUploaded";
            this.lblUploaded.Size = new System.Drawing.Size(13, 15);
            this.lblUploaded.TabIndex = 10;
            this.lblUploaded.Text = "0";
            // 
            // lblUploaded_label
            // 
            this.lblUploaded_label.AutoSize = true;
            this.lblUploaded_label.Location = new System.Drawing.Point(18, 100);
            this.lblUploaded_label.Name = "lblUploaded_label";
            this.lblUploaded_label.Size = new System.Drawing.Size(89, 15);
            this.lblUploaded_label.TabIndex = 9;
            this.lblUploaded_label.Text = "Total Uploaded:";
            // 
            // lblDownloaded
            // 
            this.lblDownloaded.AutoSize = true;
            this.lblDownloaded.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDownloaded.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDownloaded.Location = new System.Drawing.Point(138, 79);
            this.lblDownloaded.Name = "lblDownloaded";
            this.lblDownloaded.Size = new System.Drawing.Size(13, 15);
            this.lblDownloaded.TabIndex = 8;
            this.lblDownloaded.Text = "0";
            // 
            // lblDownloaded_label
            // 
            this.lblDownloaded_label.AutoSize = true;
            this.lblDownloaded_label.Location = new System.Drawing.Point(18, 79);
            this.lblDownloaded_label.Name = "lblDownloaded_label";
            this.lblDownloaded_label.Size = new System.Drawing.Size(105, 15);
            this.lblDownloaded_label.TabIndex = 7;
            this.lblDownloaded_label.Text = "Total Downloaded:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblRSSI);
            this.groupBox3.Controls.Add(this.lblPSAttach);
            this.groupBox3.Controls.Add(this.lblPS);
            this.groupBox3.Controls.Add(this.lblCS);
            this.groupBox3.Controls.Add(this.lblProvider);
            this.groupBox3.Controls.Add(this.pbRSSI);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(14, 78);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(233, 168);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Network";
            // 
            // lblRSSI
            // 
            this.lblRSSI.AutoSize = true;
            this.lblRSSI.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblRSSI.Location = new System.Drawing.Point(49, 112);
            this.lblRSSI.Name = "lblRSSI";
            this.lblRSSI.Size = new System.Drawing.Size(17, 15);
            this.lblRSSI.TabIndex = 11;
            this.lblRSSI.Text = "--";
            // 
            // lblPSAttach
            // 
            this.lblPSAttach.AutoSize = true;
            this.lblPSAttach.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPSAttach.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblPSAttach.Location = new System.Drawing.Point(154, 91);
            this.lblPSAttach.Name = "lblPSAttach";
            this.lblPSAttach.Size = new System.Drawing.Size(17, 15);
            this.lblPSAttach.TabIndex = 10;
            this.lblPSAttach.Text = "--";
            // 
            // lblPS
            // 
            this.lblPS.AutoSize = true;
            this.lblPS.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblPS.Location = new System.Drawing.Point(154, 70);
            this.lblPS.Name = "lblPS";
            this.lblPS.Size = new System.Drawing.Size(17, 15);
            this.lblPS.TabIndex = 9;
            this.lblPS.Text = "--";
            // 
            // lblCS
            // 
            this.lblCS.AutoSize = true;
            this.lblCS.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblCS.Location = new System.Drawing.Point(154, 49);
            this.lblCS.Name = "lblCS";
            this.lblCS.Size = new System.Drawing.Size(17, 15);
            this.lblCS.TabIndex = 8;
            this.lblCS.Text = "--";
            // 
            // lblProvider
            // 
            this.lblProvider.AutoSize = true;
            this.lblProvider.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblProvider.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblProvider.Location = new System.Drawing.Point(73, 20);
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Size = new System.Drawing.Size(22, 21);
            this.lblProvider.TabIndex = 7;
            this.lblProvider.Text = "--";
            // 
            // pbRSSI
            // 
            this.pbRSSI.Location = new System.Drawing.Point(14, 130);
            this.pbRSSI.Maximum = 30;
            this.pbRSSI.Minimum = 1;
            this.pbRSSI.Name = "pbRSSI";
            this.pbRSSI.Size = new System.Drawing.Size(201, 23);
            this.pbRSSI.Step = 1;
            this.pbRSSI.TabIndex = 6;
            this.pbRSSI.Value = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 112);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 15);
            this.label12.TabIndex = 5;
            this.label12.Text = "RSSI:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 91);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(137, 15);
            this.label15.TabIndex = 4;
            this.label15.Text = "PS Network Attachment:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 70);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(137, 15);
            this.label14.TabIndex = 3;
            this.label14.Text = "PS Network Registration:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 49);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(138, 15);
            this.label13.TabIndex = 2;
            this.label13.Text = "CS Network Registration:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "Operator:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(575, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 25);
            this.button1.TabIndex = 32;
            this.button1.Text = "Settings";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnModemInfo
            // 
            this.btnModemInfo.Image = global::ModemPartner.Properties.Resources.information;
            this.btnModemInfo.Location = new System.Drawing.Point(325, 11);
            this.btnModemInfo.Name = "btnModemInfo";
            this.btnModemInfo.Size = new System.Drawing.Size(28, 25);
            this.btnModemInfo.TabIndex = 33;
            this.btnModemInfo.UseVisualStyleBackColor = true;
            this.btnModemInfo.Click += new System.EventHandler(this.BtnModemInfo_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Enabled = false;
            this.btnOpen.Image = global::ModemPartner.Properties.Resources.unplugged;
            this.btnOpen.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOpen.Location = new System.Drawing.Point(294, 11);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(28, 25);
            this.btnOpen.TabIndex = 34;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(651, 509);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnModemInfo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblUploadSpeed);
            this.Controls.Add(this.lblDownloadSpeed);
            this.Controls.Add(this.btnModeApply);
            this.Controls.Add(this.cbModes);
            this.Controls.Add(this.lblUploadSpeed_label);
            this.Controls.Add(this.lblMode_label);
            this.Controls.Add(this.lblDownloadSpeed_label);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblProfile_label);
            this.Controls.Add(this.cbProfiles);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnDeviceRefresh);
            this.Controls.Add(this.cbDevices);
            this.Controls.Add(this.lblDevices);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modem Partner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.chart)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDeviceRefresh;
        private System.Windows.Forms.Button btnModeApply;
        private System.Windows.Forms.Button btnModemInfo;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnResetSession;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cbDevices;
        private System.Windows.Forms.ComboBox cbModes;
        private System.Windows.Forms.ComboBox cbProfiles;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblCS;
        private System.Windows.Forms.Label lblDevices;
        private System.Windows.Forms.Label lblDownloaded;
        private System.Windows.Forms.Label lblDownloaded_label;
        private System.Windows.Forms.Label lblDownloadSpeed;
        private System.Windows.Forms.Label lblDownloadSpeed_label;
        private System.Windows.Forms.Label lblMode_label;
        private System.Windows.Forms.Label lblProfile_label;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.Label lblPS;
        private System.Windows.Forms.Label lblPSAttach;
        private System.Windows.Forms.Label lblRSSI;
        private System.Windows.Forms.Label lblSessionDownload;
        private System.Windows.Forms.Label lblSessionDownload_label;
        private System.Windows.Forms.Label lblSessionDuration;
        private System.Windows.Forms.Label lblSessionDuration_label;
        private System.Windows.Forms.Label lblSessionUpload;
        private System.Windows.Forms.Label lblSessionUpload_label;
        private System.Windows.Forms.Label lblUploaded;
        private System.Windows.Forms.Label lblUploaded_label;
        private System.Windows.Forms.Label lblUploadSpeed;
        private System.Windows.Forms.Label lblUploadSpeed_label;
        private System.Windows.Forms.ProgressBar pbRSSI;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabPage tabChart;
        private System.Windows.Forms.TabPage tabHistory;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.ToolStripStatusLabel tslblDialStatus;
        private System.Windows.Forms.ToolStripStatusLabel tslblStatus;
        private System.Windows.Forms.ToolStripStatusLabel tslblSubMode;

        #endregion
    }
}

