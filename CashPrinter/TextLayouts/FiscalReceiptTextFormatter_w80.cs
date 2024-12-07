using CashPrinter.Core;
using System;
using System.Text;
using System.Windows;

namespace CashPrinter.TextLayouts
{
    public class FiscalReceiptTextFormatter_w80 : TextFormatter
    {
        private const int MaxWidth = 55;

        public override string GetTextFromReceipt(CashDocument cashDocument)
        {
            FiscalReceipt receipt = cashDocument as FiscalReceipt;
            StringBuilder sb = new StringBuilder();

            if (receipt != null)
            {
                // Шапка фискального чека
                sb.AppendLineWrap(receipt.SupplierName, MaxWidth, TextAlignment.Center);
                sb.AppendLineWrap(receipt.SupplierAddress, MaxWidth, TextAlignment.Center);
                sb.AppendLineWrap("Оператор 0   Касса 0", MaxWidth, TextAlignment.Center);
                sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

                // Информация по товарам
                foreach (CashDocumentLine rl in receipt.Lines)
                {
                    sb.AppendLineWrap(rl.GoodNameWithCodes, MaxWidth, TextAlignment.Left);
                    string str1 = "Продаж з ПДВ";
                    string str2 = string.Format("{1:0.00}х{0}={2:0.00}", rl.Quantity, rl.Price, rl.Amount);
                    sb.AppendFormat("{0,-15}{1,22}А\n", str1, str2);
                    if (rl.Discount != 0)
                        sb.AppendFormat("Знижка  {0,30:0.00}\n", rl.Discount);
                }

                //Служебная информация 
                sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                sb.AppendFormat("{0}\n", receipt.Key.Number);

                // Сумма к оплате
                sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                sb.AppendFormat("\\w\\hСума{0,15:0.00}\n", receipt.AmountDiscounted);
                sb.AppendFormat("{0,-15}{1,23:0.00}\n", "ПДВ А А=20,00%", receipt.VAT);
                sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                sb.AppendFormat("{0,-15}{1,23:0.00}\n", receipt.PaymentType, receipt.PaymentSum);

                // Расшифровка
                sb.AppendFormat("{0}\n", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"));
                sb.AppendFormat("ФН чека: {0}\n", receipt.FiscalNumber);
                sb.AppendFormat("ФН ПРРО: {0}\n", receipt.FiscalRRONumber);
                sb.AppendFormat("{0} allo e-check\n", receipt.OnlineMode);
                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLineWrap(@"\wФіскальний чек", MaxWidth / 2, TextAlignment.Center);
            }

            return sb.ToString();
        }
    }
}
