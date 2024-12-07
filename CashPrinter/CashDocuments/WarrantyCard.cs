using CashPrinter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashPrinter.CashDocuments
{
    public class WarrantyCard : CashDocument
    {
        public override RecordKey Key { get; }
        public override List<CashDocumentLine> Lines => throw new NotImplementedException();
        public string GoodName { get; set; }
        public DateTime Date { get; set; }
        public string MarkdownType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string WarrantyPeriod { get; set; }
        public string SupplierName { get; set; }
        public string SupplierINN { get; set; }
        public string SupplierFiscalAddress { get; set; }
        public object GoodCode { get; set; }
        public DateTime DateOfSale { get; set; }
        public override string ToString() => string.Format("Гар.талон {0}", Key.ToString());
        public WarrantyCard()
        {
            Key = new WarrantyCardRecordKey();
        }
    }
    public class WarrantyCardRecordKey : RecordKey
    {
        public decimal GoodCode { get; set; }
        public string SerialNumber { get; set; }
        public override string ToString() => string.Format("{0}-{1}", base.ToString(), GoodCode);
    }
}
