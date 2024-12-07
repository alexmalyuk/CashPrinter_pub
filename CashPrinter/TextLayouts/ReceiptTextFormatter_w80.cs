using CashPrinter.Core;
using System;
using System.Text;
using System.Windows;

namespace CashPrinter.TextLayouts
{
    public class ReceiptTextFormatter_w80 : TextFormatter
    {
        private const int MaxWidth = 55;

        public override string GetTextFromReceipt(CashDocument cashDocument)
        {
            Receipt receipt = cashDocument as Receipt;
            StringBuilder sb = new StringBuilder();

            if (receipt != null)
            {
                if (receipt.isFOP)
                {
                    // ФОП Шапка чека
                    sb.AppendLineWrap(receipt.SupplierName, MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(string.Format("ІНН: {0}", receipt.SupplierINN), MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(receipt.SupplierAddress, MaxWidth, TextAlignment.Center);
                    var str = string.Format("ЗН {0}  ФН {0}", receipt.SupplierINN);
                    sb.AppendLineWrap(str, MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap("Оператор 0   Касса 0", MaxWidth, TextAlignment.Center);
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

                    // Информация по товарам
                    foreach (CashDocumentLine rl in receipt.Lines)
                        if (receipt.IsGeneralTaxPayer && rl.isVAT)
                        {
                            sb.AppendLineWrap(rl.GetGoodNameWithCodes(receipt.isPrintGoodCode), MaxWidth, TextAlignment.Left);
                            string str1 = "Продаж з ПДВ";
                            string str2 = string.Format("{1:0.00}х{0}={2:0.00}", rl.Quantity, rl.Price, rl.Amount);
                            sb.AppendFormat("{0,-15}{1,39}А\n", str1, str2);
                            if (rl.Discount != 0)
                                sb.AppendFormat("{0,-8}{1,47:0.00}\n", "Знижка", rl.Discount);
                        }
                        else
                        {
                            sb.AppendLineWrap(rl.GetGoodNameWithCodes(receipt.isPrintGoodCode), MaxWidth, TextAlignment.Left);
                            string str1 = "";
                            string str2 = string.Format("{1:0.00}х{0}={2:0.00}", rl.Quantity, rl.Price, rl.Amount);
                            sb.AppendFormat("{0,-15}{1,40}\n", str1, str2);
                            if (rl.Discount != 0)
                                sb.AppendFormat("{0,-8}{1,47:0.00}\n", "Знижка", rl.Discount);
                        }

                    //Служебная информация 
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                    sb.AppendFormat("{0}\n", receipt.Key.Number);
                    // Алло-гроші
                    if (!string.IsNullOrEmpty(receipt.AGDescription))
                    {
                        sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                        sb.AppendLineWrap(receipt.AGDescription, MaxWidth, TextAlignment.Left);
                    }

                    // Сумма к оплате
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                    sb.AppendFormat("\\w\\hСума{0,23:0.00}\n", receipt.AmountDiscounted);
                    if (receipt.IsGeneralTaxPayer && receipt.isVAT)
                        sb.AppendFormat("{0,-15}{1,40:0.00}\n", "ПДВ А А=20,00%", receipt.VAT);
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                    // по формам оплаты
                    foreach (var pfLine in receipt.PayFormLines)
                        sb.AppendFormat("{0,-15}{1,40:0.00}\n", pfLine.Desription, pfLine.Sum);

                    // Расшифровка
                    sb.AppendFormat("{0,-10:000000000}{1,-6:00000}{2,39}\n", receipt.StoreId, 2, DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"));
                    sb.AppendFormat("\\bcl{0}\n", receipt.Key.Number);
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                    sb.AppendLineWrap(@"\wЧЕК", MaxWidth / 2, TextAlignment.Center);
                }
                else
                {
                    // АЛЛО Шапка чека
                    sb.AppendLineWrap(receipt.SupplierName, MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(receipt.SupplierAddress, MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(string.Format("ПН: {0}", receipt.SupplierINN), MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap("Оператор 0   Касса 0", MaxWidth, TextAlignment.Center);
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

                    // Информация по товарам
                    foreach (CashDocumentLine rl in receipt.Lines)
                        if (receipt.IsGeneralTaxPayer && rl.isVAT)
                        {
                            sb.AppendLineWrap(rl.GetGoodNameWithCodes(receipt.isPrintGoodCode), MaxWidth, TextAlignment.Left);
                            string str1 = "Продаж з ПДВ";
                            string str2 = string.Format("{1:0.00}х{0}={2:0.00}", rl.Quantity, rl.Price, rl.Amount);
                            sb.AppendFormat("{0,-15}{1,39}А\n", str1, str2);
                            if (rl.Discount != 0)
                                sb.AppendFormat("{0,-8}{1,47:0.00}\n", "Знижка", rl.Discount);
                        }
                        else
                        {
                            sb.AppendLineWrap(rl.GetGoodNameWithCodes(receipt.isPrintGoodCode), MaxWidth, TextAlignment.Left);
                            string str1 = "";
                            string str2 = string.Format("{1:0.00}х{0}={2:0.00}", rl.Quantity, rl.Price, rl.Amount);
                            sb.AppendFormat("{0,-15}{1,40}\n", str1, str2);
                            if (rl.Discount != 0)
                                sb.AppendFormat("{0,-8}{1,47:0.00}\n", "Знижка", rl.Discount);
                        }

                    //Служебная информация 
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                    sb.AppendFormat("{0}\n", receipt.Key.Number);
                    // Алло-гроші
                    if (!string.IsNullOrEmpty(receipt.AGDescription))
                    {
                        sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                        sb.AppendLineWrap(receipt.AGDescription, MaxWidth, TextAlignment.Left);
                    }

                    // Сумма к оплате
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                    sb.AppendFormat("\\w\\hСума{0,23:0.00}\n", receipt.AmountDiscounted);
                    if (receipt.IsGeneralTaxPayer && receipt.isVAT)
                        sb.AppendFormat("{0,-15}{1,40:0.00}\n", "ПДВ А А=20,00%", receipt.VAT);
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));
                    // по формам оплаты
                    foreach (var pfLine in receipt.PayFormLines)
                        sb.AppendFormat("{0,-15}{1,40:0.00}\n", pfLine.Desription, pfLine.Sum);

                    // Расшифровка
                    sb.AppendFormat("{0,-10:000000000}{1,-6:00000}{2,39}\n", receipt.StoreId, 2, DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"));
                    sb.AppendFormat("\\bcl{0}\n", receipt.Key.Number);
                    sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                    sb.AppendLineWrap(@"\wЧЕК", MaxWidth / 2, TextAlignment.Center);
                }

                sb.AppendLine(@"\logo");
            }

            return sb.ToString();
        }
    }
}
