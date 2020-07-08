using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModemPartner.Properties;

namespace ModemPartner.View
{
    /// <summary>
    /// Defines form where device information will appear.
    /// </summary>
    public partial class DeviceInfoForm : Form, IDeviceInfoView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoForm"/> class.
        /// </summary>
        public DeviceInfoForm()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public event EventHandler LoadForm;

        /// <inheritdoc/>
        public void ClearFields()
        {
            lblManufacturer.Text = Resources.NoRealValue;
            lblModel.Text = Resources.NoRealValue;
            lblIMEI.Text = Resources.NoRealValue;
        }

        /// <inheritdoc/>
        public void UpdateIMEI(string IMEI)
        {
            lblIMEI.Text = IMEI;
        }

        /// <inheritdoc/>
        public void UpdateManufacturer(string manufacturer)
        {
            lblManufacturer.Text = manufacturer;
        }

        /// <inheritdoc/>
        public void UpdateModel(string model)
        {
            lblModel.Text = model;
        }

        private void DeviceInfoForm_Load(object sender, EventArgs e)
        {
            LoadForm?.Invoke(sender, e);
        }
    }
}
