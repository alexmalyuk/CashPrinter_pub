using CashPrinter.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CashPrinter
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;
        private static bool _isDebugMode = false;
        private static bool _isNoDatecsMode = false;
        public static bool IsDebugMode 
        { 
            get 
            {
                return _isDebugMode; 
            } 
        }
        public static bool IsNoDatecsMode
        {
            get
            {
                return _isNoDatecsMode;
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            base.OnStartup(e);

            foreach (string s in e.Args)
            {
                if (s.ToUpper() == "DEBUG")
                    _isDebugMode = true;
                else if (s.ToUpper() == "NODATECS")
                    _isNoDatecsMode = true;
            }


            MainWindow = new MainWindow();
            
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = CashPrinter.Properties.Resources.allo_icon;
            _notifyIcon.Visible = true;

            CreateContextMenu();

            MainWindow.Show();
        }

#if !DEBUG
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        System.Windows.Forms.MessageBox.Show(string.Format("{0} {1}\n{2}", ex.Message, ex.TargetSite, ex.ToString()));
    }
#endif

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
            new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Показати вікно програми").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Завершити роботу").Click += (s, e) => ExitApplication();
        }


        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                    MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
                _notifyIcon.ShowBalloonTip(5000, "Cash printer", "Друк чеків продовжує работу у фоновому режимі ", System.Windows.Forms.ToolTipIcon.Info);

            }
        }

    }
}
