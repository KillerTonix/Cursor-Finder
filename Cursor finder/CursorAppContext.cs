using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cursor_finder
{
    public class CursorAppContext : ApplicationContext
    {
        private readonly LowLevelKeyboardProc _proc; // Delegate for the hook callback
        private readonly IntPtr _hookID = IntPtr.Zero; // Hook ID
        private readonly RippleForm _ripple = new RippleForm(); // Single instance of RippleForm
        private readonly NotifyIcon _trayIcon; // Tray icon
        private readonly List<DarkenerForm> _darkeners = new List<DarkenerForm>(); // Active darkener forms
        private DateTime _lastPress = DateTime.MinValue; // Last Ctrl key press time
        private bool _isAnimating = false; // Animation state flag

        public CursorAppContext()
        {
            _proc = HookCallback; // Assign the callback method
            _hookID = SetHook(_proc); // Set the keyboard hook

            _trayIcon = new NotifyIcon() // Initialize tray icon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Exit", Exit)
            }),
                Visible = true,
                Text = "Cursor Finder v1.0\nDouble-Ctrl to find cursor"
            };

            Application.ApplicationExit += (s, e) =>
            {
                _trayIcon.Visible = false;
                UnhookWindowsHookEx(_hookID);
            };
        }


        // Keyboard hook callback method 
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)0x0100) // WM_KEYDOWN
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == 162 || vkCode == 163) // Left or Right Ctrl
                {
                    if ((DateTime.Now - _lastPress).TotalMilliseconds < 400) // Double press detected and within 400ms
                    {
                        TriggerEffect(Cursor.Position); // Trigger the effect at current cursor position
                    }
                    _lastPress = DateTime.Now; // Update last press time
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam); // Call next hook
        }

        // Set the keyboard hook method
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess()) // Get current process
            using (ProcessModule curModule = curProcess.MainModule) // Get main module
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0); // WH_KEYBOARD_LL = 13
            }
        }


        // Tray icon exit handler 
        private void Exit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }


        // Trigger the cursor finding effect
        private void TriggerEffect(Point mousePos)
        {
            if (_isAnimating) return; // Prevent overlapping animations
            _isAnimating = true;

            foreach (var screen in Screen.AllScreens) // Create darkener forms for each screen
            {
                var df = new DarkenerForm(screen); // Initialize darkener form for the screen
                df.Show(); // Show the darkener form
                _darkeners.Add(df); // Add to the list of active darkeners
            }

            _ripple.ShowAt(mousePos); // Show ripple effect at cursor position

            Timer cleanup = new Timer { Interval = 500 }; // Timer for cleanup after animation
            cleanup.Tick += (s, e) =>
            {
                foreach (var df in _darkeners)
                {
                    if (!df.IsDisposed) df.Dispose();
                }
                _darkeners.Clear(); // Clear the list of active darkeners

                _ripple.Hide(); // Hide the ripple effect
                _isAnimating = false; // Reset animation state

                cleanup.Stop(); // Stop and dispose the timer
                cleanup.Dispose();
            };
            cleanup.Start();
        }


        #region DLL Imports and Hooking Logic
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }
}
