using System;
using System.Threading;
using System.Windows.Forms;

namespace Cursor_finder
{
    static class Program
    {
        private static readonly string _mutexGuid = "Global\\MyCursorFinderApp-12345"; // Unique identifier for the mutex
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(true, _mutexGuid, out bool createdNew)) // Try to create the mutex
            {
                if (!createdNew) // If the mutex already exists, another instance is running
                {
                    MessageBox.Show("Another instance of the application is already running.", "Instance Already Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CursorAppContext()); // Run the application with the custom ApplicationContext
            }
        }
    }


}
