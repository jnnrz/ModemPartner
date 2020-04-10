using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModemPartner.View;
using ModemPartner.Core;

namespace ModemPartner.Presenter
{
    public class MainPresenter
    {
        private IMainView _view;

        public MainPresenter(IMainView view)
        {
            _view = view;

            view.RefreshDevicesClicked += View_RefreshDevicesClicked;
        }

        private void View_RefreshDevicesClicked(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                var devices = Modem.GetModems();
                _view.ClearDeviceList();
                _view.AddDevicesToList(devices);
            });
        }
    }
}
