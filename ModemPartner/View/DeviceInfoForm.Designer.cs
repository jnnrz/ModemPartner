namespace ModemPartner.View
{
    partial class DeviceInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.lblManufacturer_label = new System.Windows.Forms.Label();
            this.lblIMEI_label = new System.Windows.Forms.Label();
            this.lblModel_label = new System.Windows.Forms.Label();
            this.lblManufacturer = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblIMEI = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblManufacturer_label
            // 
            this.lblManufacturer_label.AutoSize = true;
            this.lblManufacturer_label.Location = new System.Drawing.Point(12, 18);
            this.lblManufacturer_label.Name = "lblManufacturer_label";
            this.lblManufacturer_label.Size = new System.Drawing.Size(80, 13);
            this.lblManufacturer_label.TabIndex = 0;
            this.lblManufacturer_label.Text = "Manufacturer:";
            // 
            // lblIMEI_label
            // 
            this.lblIMEI_label.AutoSize = true;
            this.lblIMEI_label.Location = new System.Drawing.Point(12, 62);
            this.lblIMEI_label.Name = "lblIMEI_label";
            this.lblIMEI_label.Size = new System.Drawing.Size(32, 13);
            this.lblIMEI_label.TabIndex = 1;
            this.lblIMEI_label.Text = "IMEI:";
            // 
            // lblModel_label
            // 
            this.lblModel_label.AutoSize = true;
            this.lblModel_label.Location = new System.Drawing.Point(12, 40);
            this.lblModel_label.Name = "lblModel_label";
            this.lblModel_label.Size = new System.Drawing.Size(43, 13);
            this.lblModel_label.TabIndex = 2;
            this.lblModel_label.Text = "Model:";
            // 
            // lblManufacturer
            // 
            this.lblManufacturer.AutoSize = true;
            this.lblManufacturer.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblManufacturer.Location = new System.Drawing.Point(153, 18);
            this.lblManufacturer.Name = "lblManufacturer";
            this.lblManufacturer.Size = new System.Drawing.Size(15, 13);
            this.lblManufacturer.TabIndex = 3;
            this.lblManufacturer.Text = "--";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblModel.Location = new System.Drawing.Point(153, 40);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(15, 13);
            this.lblModel.TabIndex = 4;
            this.lblModel.Text = "--";
            // 
            // lblIMEI
            // 
            this.lblIMEI.AutoSize = true;
            this.lblIMEI.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblIMEI.Location = new System.Drawing.Point(153, 62);
            this.lblIMEI.Name = "lblIMEI";
            this.lblIMEI.Size = new System.Drawing.Size(15, 13);
            this.lblIMEI.TabIndex = 5;
            this.lblIMEI.Text = "--";
            // 
            // DeviceInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(276, 206);
            this.Controls.Add(this.lblIMEI);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.lblManufacturer);
            this.Controls.Add(this.lblModel_label);
            this.Controls.Add(this.lblIMEI_label);
            this.Controls.Add(this.lblManufacturer_label);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.Name = "DeviceInfoForm";
            this.Text = "Device Information";
            this.Load += new System.EventHandler(this.DeviceInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblIMEI;
        private System.Windows.Forms.Label lblIMEI_label;
        private System.Windows.Forms.Label lblManufacturer;
        private System.Windows.Forms.Label lblManufacturer_label;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblModel_label;

        #endregion
    }
}