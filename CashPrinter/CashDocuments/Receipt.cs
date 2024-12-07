using System;
using System.Collections.Generic;

namespace CashPrinter.Core
{
    public class Receipt : CashDocument
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
        public string AGDescription { get; set; }
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
        public override string ToString() => string.Format("Чек {0}", Key.ToString());
        public Receipt()
        {
            Key = new ReceiptRecordKey();
            Lines = new List<CashDocumentLine>();
        }
    }
    public class ReceiptRecordKey : RecordKey
    {

    }
}
