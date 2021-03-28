using System;
using System.Net;
using System.Windows.Forms;
using ConnectionCheck.Properties;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ConnectionCheck
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new App());
        }
    }


    public class App : ApplicationContext
    {
        private static System.Timers.Timer _timer;
        public static NotifyIcon Tray = new NotifyIcon();
        public static bool PastValue = true;

        public App()
        {
            Tray.Icon = Resources.Green;
            Tray.ContextMenu = new ContextMenu(new[]
            {
                new MenuItem("Exit", Exit)
            });
            Tray.Visible = true;

            SetTimer();
        }

        void Exit(object sender, EventArgs e)
        {
            Tray.Visible = false;
            Application.Exit();
        }


        private static void SetTimer()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (sender, e) => { OnTimedEvent(); };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        

        private static void OnTimedEvent()
        {
            new Toasts().SendToast();
        }
    }

    public class Toasts
    {
        bool CheckForInternetConnection()
        {
            try
            {
                using (var Client = new WebClient())
                using (Client.OpenRead("http://connorcode.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public void SendToast()
        {
            bool CurrentValue = CheckForInternetConnection();
            if (App.PastValue == CurrentValue) return;
            App.PastValue = CurrentValue;

            if (CurrentValue)
            {
                App.Tray.Icon = Resources.Green;
                new ToastContentBuilder().AddText("🌐 Internet Connected.").Show();
            }

            if (CurrentValue) return;
            App.Tray.Icon = Resources.Red;
            new ToastContentBuilder().AddText("❌ Internet Disconnected.").Show();
        }
    }
}
