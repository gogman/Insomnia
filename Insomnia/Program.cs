#region File Information

// Solution: Insomnia
// Project Name: Insomnia
// 
// File: Program.cs
// Created: 2021/12/14 @ 12:25 PM
// Updated: 2021/12/16 @ 3:30 PM

#endregion

#region using

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Win32;

using Timer = System.Windows.Forms.Timer;

#endregion

namespace Insomnia
{
    internal static class Program
    {
        private static readonly Timer INPUT_CHECK_TIMER = new Timer();

        private static ProcessIcon processIcon;

        private static void ExitMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static TimeSpan GetIdleTime()
        {
            DateTime bootTime = DateTime.UtcNow.AddMilliseconds(-Environment.TickCount);

            LastInputInfo lii = new LastInputInfo
                                    {
                                        cbSize = (uint)Marshal.SizeOf(typeof(LastInputInfo))
                                    };

            GetLastInputInfo(ref lii);

            DateTime lastInputTime = bootTime.AddMilliseconds(lii.dwTime);

            TimeSpan idleTime = DateTime.UtcNow.Subtract(lastInputTime);

            return idleTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);

        private static void InputCheckTimerTick(object sender, EventArgs e)
        {
            try
            {
                double seconds = GetIdleTime().TotalSeconds;

                // processIcon.notifyIcon.Text = $@"Idle for {seconds:###0} secs.";

                if (seconds < 300) return;

                SendKeys.SendWait("^{ESC}");

                Thread.Sleep(500);

                SendKeys.SendWait("^{ESC}");
            }
            catch (Exception exception)
            {
                // Console.WriteLine(exception);
                // throw;
                Console.Beep();
            }
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            INPUT_CHECK_TIMER.Tick += InputCheckTimerTick;
            INPUT_CHECK_TIMER.Interval = 1000;
            INPUT_CHECK_TIMER.Enabled = true;
            INPUT_CHECK_TIMER.Start();

            processIcon = new ProcessIcon();

            processIcon.DoubleClick += ProcessIconDoubleClick;

            ToolStripMenuItem exitMenu = new ToolStripMenuItem();
            exitMenu.Text = @"Exit";
            exitMenu.Click += ExitMenu_Click;

            InputCheckTimerTick(null, null);

            using (processIcon)
            {
                processIcon.RightClickContextMenu = new ContextMenuStrip();
                processIcon.RightClickContextMenu.Items.Add(exitMenu);
                processIcon.Display();
                Application.Run();
            }

            exitMenu.Click -= ExitMenu_Click;

            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;

            INPUT_CHECK_TIMER.Tick -= InputCheckTimerTick;
            INPUT_CHECK_TIMER.Enabled = false;
            INPUT_CHECK_TIMER.Stop();
        }

        private static void ProcessIconDoubleClick(object sender, EventArgs e)
        {
            Console.Beep();
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionUnlock:
                    INPUT_CHECK_TIMER.Enabled = true;
                    break;

                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.SessionLogoff:
                    INPUT_CHECK_TIMER.Enabled = false;
                    break;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LastInputInfo
        {
            public uint cbSize;

            public readonly int dwTime;
        }
    }
}