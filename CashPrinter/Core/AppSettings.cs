using Core.CashPrinter;

namespace CashPrinter.Core
{
    public static class AppSettings
    {
        private static CashTypeEnum cashType;
        private static string connectionString;
        private static string echeckUrl;
        private static string echeckAuth;

        public static int DatabaseQueryinterval { get; set; }
        public static string WorkstationName { get; set; }
        public static TapeWidthEnum TapeWidth { get; set; }
        public static CashTypeEnum CashType
        {
            get => cashType;
            set
            {
                if (cashType != value)
                {
                    cashType = value;
                    CashPrinterApp.getInstance().SetCashTypeService(CashType);
                }
            }
        }
        public static string ThermalPrinterName { get; set; }
        public static string ConnectionString => connectionString;
        public static string EcheckUrl => echeckUrl;
        public static string EcheckAuth => echeckAuth;

        public static void Load()
        {
            DatabaseQueryinterval=Properties.Settings.Default.DatabaseQueryinterval;
            WorkstationName=Properties.Settings.Default.WorkstationName;
            cashType = (CashTypeEnum)Properties.Settings.Default.CashPrinterType;
            TapeWidth = Properties.Settings.Default.isDatecsTapeWidth_57mm ? TapeWidthEnum.Tape57mm : TapeWidthEnum.Tape80mm;
            ThermalPrinterName = Properties.Settings.Default.ThermalPrinterName;
            connectionString = Properties.Settings.Default.ConnectionString;
            echeckUrl = Properties.Settings.Default.EcheckUrl;
            echeckAuth = Properties.Settings.Default.EcheckAuth;
        }

        public static void Save()
        {
            Properties.Settings.Default.DatabaseQueryinterval = DatabaseQueryinterval;
            Properties.Settings.Default.WorkstationName = WorkstationName;
            Properties.Settings.Default.isDatecsTapeWidth_57mm = (TapeWidth == TapeWidthEnum.Tape57mm);
            Properties.Settings.Default.CashPrinterType = (int)cashType;
            Properties.Settings.Default.ThermalPrinterName = ThermalPrinterName;
            Properties.Settings.Default.ConnectionString = ConnectionString;
            Properties.Settings.Default.EcheckUrl = EcheckUrl;
            Properties.Settings.Default.EcheckAuth = EcheckAuth;

            Properties.Settings.Default.Save();
        }

        static AppSettings()
        {
            Load();
        }
    }
}
