using CashPrinter.CashDocuments;
using CashPrinter.CashServices.Datecs;
using CashPrinter.CashServices.Test;
using CashPrinter.CashServices.ThermalPrinter;
using CashPrinter.Core.DataBase;
using CashPrinter.TextLayouts;
using Core.CashPrinter;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace CashPrinter.Core
{
    public class CashPrinterApp
    {
        private IContext _context;

        #region Singleton Support
        private static CashPrinterApp instance;
        public static CashPrinterApp getInstance()
        {
            if (instance == null)
                instance = new CashPrinterApp();

            return instance;
        }
        private CashPrinterApp()
        {
            _context = new WpfDispatcherContext();

            messageLog = new ObservableCollection<string>();
            messageLog.Add(string.Format("Старт: {0}", DateTime.Now));

            СashService = null;
        }
        public void SetCashTypeService(CashTypeEnum cashType)
        {
            if (СashService != null)
            {
                СashService.Dispose();
                СashService = null;
            }

            switch (cashType)
            {
                case CashTypeEnum.Datecs_FP320:
                    СashService = new DatecsCashService();
                    break;
                case CashTypeEnum.FakeCash:
                    СashService = new FakeCashService();
                    break;
                case CashTypeEnum.ThermalPrinter:
                    СashService = new ThermalPrinterCashService();
                    break;
                default:
                    throw new CashPrinterException("Тип принтеру не визначений");
            }
        }
        #endregion

        private ObservableCollection<string> messageLog;

        public CashService СashService { get; private set; }

        public ObservableCollection<string> MessageLog
        {
            get { return messageLog; }
        }

        public void AddMessageToLog(string Msg)
        {
            _context.Invoke(() => messageLog.Add(string.Format("{0} {1}", Msg, DateTime.Now.ToString())));
        }

        public void ClearMessageLog()
        {
            messageLog.Clear();
        }

        public void PrintReceiptFromQueue()
        {
            PrintReceiptFromQueue(null);
        }

        public void PrintReceiptFromQueue(string number)
        {
            using (var eRP = new DataERPProvider())
            {
                try
                {
                    var receiptQueue = eRP.GetReceiptQueue(number);
                    receiptQueue.AddRange(eRP.GetWarrantyCardQueue(number));
                    receiptQueue.Sort();

                    foreach (CashDocument cashDocument in receiptQueue)
                    {
                        // Основна логіка друку залежно від типу документа
                        switch (cashDocument)
                        {
                            case FiscalReceipt fiscalReceipt when cashDocument.IsPrintReceipt:
                                СashService.PrintPDF(fiscalReceipt.ReceiptPdf);
                                break;

                            case Receipt receipt when cashDocument.IsPrintReceipt:
                            case WarrantyCard warrantyCard:
                                var text = TextLayoutFormatter.GetText(cashDocument);
                                СashService.PrintText(text);
                                break;

                            case Invoice invoice:
                                var textInvoice = TextLayoutFormatter.GetText(cashDocument);
                                СashService.PrintText(textInvoice, 2);
                                break;
                        }

                        // Друк пам'ятки, якщо потрібно
                        if (cashDocument is Receipt || cashDocument is Invoice || cashDocument is FiscalReceipt)
                        {
                            if (cashDocument.IsPrintMemo)
                            {
                                var textMemo = TextLayoutFormatter.GetTextMemo(cashDocument);
                                СashService.PrintText(textMemo);
                            }
                        }

                        // Оновлення статусу та логування
                        eRP.SetCashDocumentStatusPrinted(cashDocument.Key);
                        AddMessageToLog(cashDocument.ToString());
                    }
                }
                catch (CashPrinterException ex)
                {
                    AddMessageToLog(ex.Message);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("{0}\n{1}\n{2}", ex.Message, ex.TargetSite, ex.ToString()));
                }
            }
        }

        public void RunQueueProcessing()
        {
            while (true)
            {
                if (AppSettings.DatabaseQueryinterval != 0)
                {
                    PrintReceiptFromQueue();
                    Thread.Sleep(AppSettings.DatabaseQueryinterval);
                }
                else
                    Thread.Sleep(3000);
            }
        }

        public void ClearDocumentsQueue()
        {
            using (var eRP = new DataERPProvider())
            {
                try
                {
                    eRP.ClearDocumentsQueue();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("{0}", ex.Message));
                }
            }
        }
    }
}
