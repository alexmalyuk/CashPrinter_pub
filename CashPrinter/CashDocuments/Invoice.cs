using CashPrinter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CashPrinter.CashDocuments
{
    public class Invoice : CashDocument
    {
        public override RecordKey Key { get; }
        public override List<CashDocumentLine> Lines { get; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public string SupplierINN { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierIBAN { get; set; }
        public string SupplierBankMFO { get; set; }
        public string SupplierBankName { get; set; }
        public string SupplierOKPO { get; set; }
        public string SupplierPhone { get; set; }
        public string BuyerName { get; set; }
        public string BuyerOKPO { get; set; }
        public string BuyerAddress { get; set; }
        public string TaxReference { get; set; }
        public string StorekeeperName { get; set; }
        public decimal AmountDiscounted { get; set; }
        public decimal VAT
        {
            // сумма НДС считается построчно
            get
            {
                decimal sumVAT = 0;
                foreach (var documentLine in Lines)
                    if (documentLine.isVAT)
                        sumVAT += Math.Round(documentLine.Amount * vatRate / (1 + vatRate) * 100);

                return sumVAT / 100;
            }
        }
        public decimal Amount
        {
            // сумма считается построчно
            get
            {
                decimal sumAmount = 0;
                foreach (var documentLine in Lines)
                    sumAmount += documentLine.Amount;

                return sumAmount;
            }
        }
        public decimal AmountWoVAT
        {
            get
            {
                decimal sumAmountWoVAT = 0;
                foreach (var documentLine in Lines)
                    sumAmountWoVAT += Math.Round(documentLine.Amount / (1 + CashDocument.vatRate), 2);

                return sumAmountWoVAT;
                
            }
        }
        public decimal Quantity
        {
            // сумма считается построчно
            get
            {
                decimal sumQuantity = 0;
                foreach (var documentLine in Lines)
                    sumQuantity += documentLine.Quantity;

                return sumQuantity;
            }
        }
        public override string ToString() => string.Format("РН {0}", Key.ToString());
        public Invoice()
        {
            Key = new InvoiceRecordKey();
            Lines = new List<CashDocumentLine>();
        }
    }
    public class InvoiceRecordKey : RecordKey
    {

    }
}

