using CashPrinter.Core;
using CashPrinter.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CashPrinter.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private CashPrinterApp cashPrinterApp;
        public CashPrinterApp CashPrinterApp { get => cashPrinterApp; set => cashPrinterApp = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            CashPrinterApp = CashPrinterApp.getInstance();
            CashPrinterApp.SetCashTypeService(AppSettings.CashType);

            Task.Run(CashPrinterApp.RunQueueProcessing);
        }

        public string DeviceName => CashPrinterApp.СashService.DeviceName;
        public string SerialNumber => CashPrinterApp.СashService.SerialNumber;
        public bool DeviceEnabled => CashPrinterApp.СashService.DeviceEnabled;
        public string ErrorMessage => CashPrinterApp.СashService.ErrorMessage;
        public ObservableCollection<string> MessageLog => CashPrinterApp.MessageLog;
        public string DocumentNumber { get; set; }
        public string WorkstationName => AppSettings.WorkstationName;
        public ICommand TestPrintCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    _testPrint();
                }, (obj) =>
                {
                    return CashPrinterApp.СashService.DeviceEnabled;
                });
            }
        }
        public Visibility DebugVisibility => App.IsDebugMode ? Visibility.Visible : Visibility.Hidden;

        private void _testPrint()
        {
            FiscalReceipt cashDocument = new FiscalReceipt();
            cashDocument.FiscalRRONumber = "4000028987";
            cashDocument.FiscalNumber = "3219091";
            cashDocument.FiscalDate = new DateTime(2021, 2, 16);
            try
            {
                CashPrinterApp.СashService.PrintPDF(cashDocument.ReceiptPdf, 1);
            }
            catch (Exception ex)
            {
                CashPrinterApp.AddMessageToLog(ex.Message);
            }
        }
        
        public ICommand GetQueueCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    CashPrinterApp.PrintReceiptFromQueue();
                }, (obj) =>
                {
                    return (CashPrinterApp.СashService.DeviceEnabled && AppSettings.DatabaseQueryinterval == 0);
                });
            }
        }

        public ICommand PrintReceiptByNumberCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    CashPrinterApp.PrintReceiptFromQueue(DocumentNumber);
                }, (obj) =>
                {
                    return CashPrinterApp.СashService.DeviceEnabled;
                });
            }
        }

        public ICommand ClearDocumentsQueueCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    string messageBoxText = string.Format("Очистити чергу другу для {0}?", AppSettings.WorkstationName);
                    string caption = "Cash printer";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Question;
                    MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                    if (result == MessageBoxResult.Yes)
                        CashPrinterApp.ClearDocumentsQueue();

                }, (obj) =>
                {
                    return CashPrinterApp.СashService.DeviceEnabled;
                });
            }
        }

        public ICommand ShowSettings
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    SettingsWindow win = new SettingsWindow();
                    win.ShowDialog();
                    AppSettings.Save();
                    OnSettingsChanged();
                }, (obj) =>
                {
                    return true;
                });
            }
        }
        
        private void OnSettingsChanged()
        {
            // UI element updates when the AppSettings has been changed
            OnPropertyChanged("DeviceName");
            OnPropertyChanged("SerialNumber");
            OnPropertyChanged("DeviceEnabled");
            OnPropertyChanged("ErrorMessage");
            OnPropertyChanged("WorkstationName");
        }

        public ICommand ClearMessageLog
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    CashPrinterApp.ClearMessageLog();
                }, (obj) =>
                {
                    return true;
                });
            }
        }

        public string Version 
        {
            get
            {
                return string.Format("{0}{1}", typeof(App).Assembly.GetName().Version, App.IsDebugMode ? " debug" : "");
            } 
        }

    }
}
