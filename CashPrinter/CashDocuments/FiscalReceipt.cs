using CashPrinter.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashPrinter.Core
{
    public class FiscalReceipt : CashDocument
    {
        public override RecordKey Key { get ; }
        public override List<CashDocumentLine> Lines { get ; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public string SupplierName { get; set; }
        public string SupplierINN { get; set; }
        public string SupplierAddress { get; set; }
        public decimal AmountDiscounted { get; set; }
        public string PaymentType { get; set; }
        public decimal PaymentSum { get; set; }
        public int StoreId { get; set; }
        public decimal VAT
        {
            // сумма НДС считается построчно
            get
            {
                decimal sumVAT = 0;
                foreach (var couponLine in Lines)
                    if (couponLine.isVAT)
                        sumVAT += Math.Round(couponLine.Amount * vatRate / (1 + vatRate) * 100);

                return sumVAT / 100;
            }
        }
        public string FiscalNumber { get; set; }
        public string FiscalRRONumber { get; set; }
        public DateTime FiscalDate { get; set; }
        public bool IsOnlineMode { get; set; }
        public string OnlineMode => IsOnlineMode ? "режим онлайн" : "режим офлайн";
        private byte[] receiptPdf;
        public byte[] ReceiptPdf
        {
            get
            {
                if (receiptPdf == null)
                    receiptPdf = ECheckHttpProvider.GetReceiptPDF(FiscalRRONumber, FiscalNumber, FiscalDate);

                return receiptPdf;
            }
        }
        public override string ToString() => string.Format("Чек {0} ФН{1}", Key.ToString(), FiscalNumber);
        public FiscalReceipt()
        {
            Key = new FiscalReceiptRecordKey();
            Lines = new List<CashDocumentLine>();
            receiptPdf = null;
        }
    }
    public class FiscalReceiptRecordKey : RecordKey
    {

    }
}
