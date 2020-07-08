using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModemPartner.View;

namespace ModemPartner.Presenter
{
    /// <summary>
    /// Defines presenter for <see cref="DeviceInfoForm"/>.
    /// </summary>
    public class DeviceInfoPresenter
    {
        private readonly IDeviceInfoView _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoPresenter"/> class.
        /// </summary>
        /// <param name="view">Instance of <see cref="IDeviceInfoView"/>.</param>
        public DeviceInfoPresenter(IDeviceInfoView view)
        {
            _view = view;
            view.LoadForm += View_LoadForm;
        }

        private void View_LoadForm(object sender, EventArgs e)
        {
            _view.UpdateManufacturer(ConnectedDeviceInfo.Manufacturer);
            _view.UpdateModel(ConnectedDeviceInfo.Model);
            _view.UpdateIMEI(ConnectedDeviceInfo.IMEI);
        }
    }
}
