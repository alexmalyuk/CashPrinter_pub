using Core.CashPrinter;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashPrinter.Core
{
    public abstract class CashDocument : IComparable
    {
        public static decimal vatRate
        {
            get
            {
                return 0.2M;
            }
        }

        public abstract RecordKey Key { get; }
        public abstract List<CashDocumentLine> Lines { get; }
        public List<CashDocumentPayFormLine> PayFormLines { get; }
        public bool isFOP { get; set; }
        public bool isPrintGoodCode { get; set; }
        public string TaxPayerInfo { get; set; }
        public bool IsGeneralTaxPayer { get; set; }
        public MemoTypeEnum memoTypeEnum { get; set; }
        public bool IsPrintReceipt { get; set; }
        public bool IsPrintMemo { get; set; }

        public bool isVAT
        {
            get
            {
                foreach (var documentLine in Lines)
                    if (documentLine.isVAT)
                        return true;
                return false;
            }
        }

        public decimal PayFormTotalSum
        {
            get
            {
                decimal sum = 0;
                
                foreach (CashDocumentPayFormLine pfLine in PayFormLines)
                    sum += pfLine.Sum;

                return sum;
            }
        }

        public CashDocument()
        {
            isPrintGoodCode = true;
            PayFormLines = new List<CashDocumentPayFormLine>();
            memoTypeEnum = MemoTypeEnum.General;
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            return ((IComparable)Key).CompareTo((obj as CashDocument).Key);
        }
    }
    public abstract class RecordKey : IComparable
    {
        public decimal Number;
        public string Type;
        public string Company;

        public int CompareTo(object obj)
        {
            RecordKey otherKey = obj as RecordKey;
            if (otherKey == null)
                return 1;
            else 
                return this.ToString().CompareTo(otherKey.ToString());
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Number, Type, Company);
        }

    }

    public class CashDocumentLine
    {
        public string GoodCode { get; set; }
        public string GoodName { get; set; }
        public string GoodUKTZED { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public bool isVAT { get; set; }
        public bool isPF { get; set; }
        public bool isExcise { get; set; }

        public decimal PriceWoVAT
        {
            // Цена без НДС
            get
            {
                if (isVAT)
                    return Math.Round(Price / (1 + CashDocument.vatRate), 2);
                else
                    return Price;
            }
        }

        public decimal AmountWoVAT
        {
            // сумма без НДС
            get
            {
                if (isVAT)
                    return Math.Round(Amount / (1 + CashDocument.vatRate), 2);
                else
                    return Amount;
            }
        }
        public decimal DiscountWoVAT
        {
            // сумма без НДС
            get
            {
                if (isVAT)
                    return Math.Round(Discount / (1 + CashDocument.vatRate), 2);
                else
                    return Discount;
            }
        }
        public string GoodNameWithCodes
        {
            get
            {
                if (string.IsNullOrEmpty(GoodUKTZED))
                    return string.Format("|{0}|{1}", GoodCode, GoodName);
                else
                    return string.Format("|{0}|{1}|УКТЗЕД: {2}", GoodCode, GoodName, GoodUKTZED);
            }
        }

        public string GetGoodNameWithCodes(bool IsPrintGoodCode)
        {
            StringBuilder sb = new StringBuilder();
            if (IsPrintGoodCode)
                sb.AppendFormat("|{0}", GoodCode);
            if (!string.IsNullOrEmpty(GoodUKTZED))
                sb.AppendFormat("|{0}", GoodUKTZED);
            sb.AppendFormat("|{0}", GoodName);

            return sb.ToString();
        }
    }

    public class CashDocumentPayFormLine
    {
        public string Desription { get; set; }
        public decimal Sum { get; set; }
    }
}
