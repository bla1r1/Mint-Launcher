using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;

namespace Minty
{
    public partial class App : Application
    {
        private Mutex _mutex = null;
        private const string PipeName = "MintyPipe";

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "Minty";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // Notify the first instance to activate itself.
                ActivateFirstInstance();

                // Exit the current instance.
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

        private void ActivateFirstInstance()
        {
            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    pipeClient.Connect(1000); // Timeout in milliseconds

                    using (var writer = new StreamWriter(pipeClient))
                    {
                        writer.WriteLine("Activate");
                    }
                }
            }
            catch (TimeoutException)
            {
                // Handle the case where the first instance doesn't respond in time.
            }
            catch (Exception ex)
            {
                // Handle other exceptions as needed.
            }
        }
    }
}
