using CashPrinter.Core;
using Core.CashPrinter;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CashPrinter.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string WorkstationName
        {
            get => AppSettings.WorkstationName;
            set
            {
                AppSettings.WorkstationName = value;
                OnPropertyChanged();
            }
        }
        public int DatabaseQueryinterval
        {
            get => AppSettings.DatabaseQueryinterval;
            set
            {
                AppSettings.DatabaseQueryinterval = value;
                OnPropertyChanged();
            }
        }
        public bool isTapeWidth_57mm
        {
            get => AppSettings.TapeWidth == TapeWidthEnum.Tape57mm;
            set
            {
                AppSettings.TapeWidth = value ? TapeWidthEnum.Tape57mm : TapeWidthEnum.Tape80mm;
                OnPropertyChanged();
            }
        }
        public CashTypeEnum CashPrinterType
        {
            get => AppSettings.CashType;
            set
            {
                AppSettings.CashType = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> CashPrinterType_Items 
        {
            get 
            {
                ObservableCollection<string> items = new ObservableCollection<string>();
                
                foreach (CashTypeEnum item in Enum.GetValues(typeof(CashTypeEnum)))
                    items.Add(item.ToString());
                
                return items;
            }
        }
        public string CashPrinterType_SelectedItem
        {
            get => AppSettings.CashType.ToString();
            set
            {
                foreach (CashTypeEnum item in Enum.GetValues(typeof(CashTypeEnum)))
                {
                    if (value == item.ToString())
                    {
                        AppSettings.CashType = item;
                        OnPropertyChanged("ThermalPrinterNameVisibility");
                        break;
                    }
                }
            }
        }

        public ObservableCollection<string> ThermalPrinterName_Items
        {
            get
            {
                ObservableCollection<string> items = new ObservableCollection<string>();

                LocalPrintServer printServer = new LocalPrintServer();
                PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues();
                foreach (PrintQueue printer in printQueuesOnLocalServer)
                    items.Add(printer.Name);

                return items;
            }
        }
        public string ThermalPrinterName_SelectedItem
        {
            get => AppSettings.ThermalPrinterName;
            set
            {
                AppSettings.ThermalPrinterName = value;
                OnPropertyChanged();
            }
        }

        public Visibility ThermalPrinterNameVisibility
        {
            get
            {
                return CashPrinterType == CashTypeEnum.ThermalPrinter ? Visibility.Visible : Visibility.Hidden;
            }
        }

    }
}
