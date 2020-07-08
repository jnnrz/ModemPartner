using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemPartner.View
{
    /// <summary>
    /// Interface for <see cref="View.DeviceInfoForm"/>.
    /// </summary>
    public interface IDeviceInfoView
    {
        /// <summary>
        /// Occurs when the form loads.
        /// </summary>
        event EventHandler LoadForm;

        /// <summary>
        /// Updates manufacturer label.
        /// </summary>
        /// <param name="manufacturer">Device manufacturer.</param>
        void UpdateManufacturer(string manufacturer);

        /// <summary>
        /// Updates model label.
        /// </summary>
        /// <param name="model">Device model.</param>
        void UpdateModel(string model);

        /// <summary>
        /// Updates IMEI label.
        /// </summary>
        /// <param name="IMEI">Device's IMEI.</param>
        void UpdateIMEI(string IMEI);

        /// <summary>
        /// Clears labels.
        /// </summary>
        void ClearFields();
    }
}
