using System;
using System.Windows.Forms;
using ModemPartner.Presenter;
using ModemPartner.View;

namespace ModemPartner
{
    /// <summary>
    /// Defines the <see cref="Program" />.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new MainForm();
            _ = new MainPresenter(mainForm);

            Application.Run(mainForm);
        }
    }
}
