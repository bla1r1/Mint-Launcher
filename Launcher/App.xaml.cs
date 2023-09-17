using System;
using System.Threading;
using System.Windows;

namespace Minty
{
    public partial class App : Application
    {
        private Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "Minty";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                Environment.Exit(0);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
            }

            base.OnExit(e);
        }
    }

}
