using CashPrinter.CashDocuments;
using CashPrinter.Core;
using System.Text;
using System.Windows;

namespace CashPrinter.TextLayouts
{
    public class InvoiceTextFormatter_w57 : TextFormatter
	{
		private const int MaxWidth = 38;

		public override string GetTextFromReceipt(CashDocument cashDocument)
		{
			Invoice invoice = cashDocument as Invoice;
			StringBuilder sb = new StringBuilder();

			if (invoice != null)
			{
				if (invoice.isFOP)
				{
					#region Макет РН безнал для ФОП 
					//Шапка чека
					sb.AppendLineWrap(string.Format("\\hВидаткова накладна № {0} від {1}", invoice.Key.Number, invoice.Date.ToString("dd.MM.yyyy")), MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap(string.Format("Постачальник: {0}", invoice.SupplierName), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("IBAN: {0}", invoice.SupplierIBAN), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("{0} {1}", invoice.SupplierBankMFO, invoice.SupplierBankName), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("{0}", invoice.SupplierAddress), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("ЄДРПОУ {0}", invoice.SupplierOKPO), MaxWidth, TextAlignment.Left);
                    if (!string.IsNullOrEmpty(invoice.TaxPayerInfo))
						sb.AppendLineWrap(string.Format(invoice.TaxPayerInfo), MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap(string.Format("Покупець: {0}", invoice.BuyerName), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("ЄДРПОУ {0}", invoice.BuyerOKPO), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("Адрес доставки: {0}", invoice.BuyerAddress), MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

					//Информация о товаре
					foreach (CashDocumentLine rl in invoice.Lines)
					{
						// 55 - общая ширина
						sb.AppendLineWrap(rl.GetGoodNameWithCodes(invoice.isPrintGoodCode), MaxWidth, TextAlignment.Left);
						sb.AppendFormat("{0,-28}{1,10:0}\n", "Кількість, шт.", rl.Quantity);
						if (invoice.IsGeneralTaxPayer && rl.isVAT)
						{
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Ціна, без ПДВ грн.", rl.PriceWoVAT);
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума, без ПДВ грн.", rl.AmountWoVAT);
							if (rl.Discount != 0)
							{
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Знижка без ПДВ, грн.", rl.DiscountWoVAT);
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума з урах. знижки без ПДВ", (rl.AmountWoVAT - rl.DiscountWoVAT));
							}
						}
						else
						{
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Ціна, грн.", rl.Price);
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума, грн.", rl.Amount);
							if (rl.Discount != 0)
							{
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Знижка, грн.", rl.Discount);
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума з урах. знижки, грн.", (rl.Amount - rl.Discount));
							}
						}

					}
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

					//Сумма к оплате 
					// 27- общая ширина 
					sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Кількість, шт", invoice.Quantity);
					if (invoice.IsGeneralTaxPayer && invoice.isVAT)
					{
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума без ПДВ", invoice.AmountWoVAT);
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "ПДВ", invoice.VAT);
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума з ПДВ", invoice.Amount);
						sb.AppendLineWrap(string.Format("\\h{0}", NumInWords.GrivnaPhrase(invoice.Amount)), MaxWidth, TextAlignment.Left);
					}
					else
					{
						//sb.AppendFormat("{0,-28}{1,10}\n", "ПДВ*", "-");
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума, грн.", invoice.Amount);
						sb.AppendLineWrap(string.Format("\\h{0}", NumInWords.GrivnaPhrase(invoice.Amount)), MaxWidth, TextAlignment.Left);
						sb.AppendLineWrap(string.Format("{0}", invoice.TaxReference), MaxWidth, TextAlignment.Left);
					}
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

					//Служебная информация
					//
					sb.AppendLineWrap("Відповідальний за здійснення та правильність оформлення операції по відвантаженню товарів/послуг - матеріально відповідальна особа:", MaxWidth, TextAlignment.Left);

					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap("ФОП", MaxWidth, TextAlignment.Center);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '_'));
					sb.AppendLineWrap("посада", MaxWidth, TextAlignment.Center);
					sb.AppendLineWrap(invoice.StorekeeperName, MaxWidth, TextAlignment.Right);
					sb.AppendLine("___________________/________________");
					sb.AppendLine("      Підпис             ПІБ");


					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap("Товар (послугу/и) отримав, працездатність товарів, які передаються, та їх комплектність перевірено споживачем. Товари в справному стані. Претензій щодо якості та кількості товарів, наданих послуг (у разі надання) не маю.:", MaxWidth, TextAlignment.Left);
					
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '_'));
					sb.AppendLineWrap("посада", MaxWidth, TextAlignment.Center);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLine("___________________/________________");
					sb.AppendLine(" Підпис споживача   ПІБ споживача");

					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap("Товар (послугу) отримав за дорученням:", MaxWidth, TextAlignment.Left);
					sb.AppendLine("Серія_____№__________від____________");
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLine("Через_______________________________");

					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));

					sb.AppendFormat("{0}\n", invoice.Key.Number);
					sb.AppendFormat("\\bcl{0}\n", invoice.Key.Number);
					#endregion
				}
				else
				{
					#region Макет РН безнал для АЛЛО
					//Шапка чека
					sb.AppendLineWrap(string.Format("\\hВидаткова накладна № {0} від {1}", invoice.Key.Number, invoice.Date.ToString("dd.MM.yyyy")), MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap(string.Format("Постачальник: {0}", invoice.SupplierName), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("ЄДРПОУ {0}, тел. {1}", invoice.SupplierOKPO, invoice.SupplierPhone), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("IBAN: {0}", invoice.SupplierIBAN), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("{0} МФО {1}", invoice.SupplierBankName, invoice.SupplierBankMFO), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("ІПН {0}", invoice.SupplierINN), MaxWidth, TextAlignment.Left);
					if (!string.IsNullOrEmpty(invoice.TaxPayerInfo))
						sb.AppendLineWrap(string.Format(invoice.TaxPayerInfo), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("Адреса {0}", invoice.SupplierAddress), MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap(string.Format("Покупець: {0}", invoice.BuyerName), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("ЄДРПОУ {0}", invoice.BuyerOKPO), MaxWidth, TextAlignment.Left);
					sb.AppendLineWrap(string.Format("Адрес доставки: {0}", invoice.BuyerAddress), MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

					//Информация о товаре
					foreach (CashDocumentLine rl in invoice.Lines)
					{
						// 55 - общая ширина
						sb.AppendLineWrap(rl.GetGoodNameWithCodes(invoice.isPrintGoodCode), MaxWidth, TextAlignment.Left);
						sb.AppendFormat("{0,-28}{1,10:0}\n", "Кількість, шт.", rl.Quantity);

						if (invoice.IsGeneralTaxPayer && rl.isVAT)
						{
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Ціна, без ПДВ грн.", rl.PriceWoVAT);
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума, без ПДВ грн.", rl.AmountWoVAT);
							if (rl.Discount != 0)
							{
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Знижка без ПДВ, грн.", rl.DiscountWoVAT);
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума з урах. знижки без ПДВ", (rl.AmountWoVAT - rl.DiscountWoVAT));
							}
						}
						else
						{
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Ціна, грн.", rl.Price);
							sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума, грн.", rl.Amount);
							if (rl.Discount != 0)
							{
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Знижка, грн.", rl.Discount);
								sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума з урах. знижки, грн.", (rl.Amount - rl.Discount));
							}
						}

					}
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

					//Сумма к оплате 
					// 27 - общая ширина 
					sb.AppendFormat("{0,-28}{1,10}\n", "Кількість, шт", invoice.Quantity);
					if (invoice.IsGeneralTaxPayer && invoice.isVAT)
					{
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума без ПДВ", invoice.AmountWoVAT);
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "ПДВ", invoice.VAT);
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума з ПДВ", invoice.Amount);
						sb.AppendLineWrap(string.Format("\\h{0}", NumInWords.GrivnaPhrase(invoice.Amount)), MaxWidth, TextAlignment.Left);
					}
					else 
					{
						//sb.AppendFormat("{0,-28}{1,10}\n", "ПДВ*", "-");
						sb.AppendFormat("{0,-28}{1,10:0.00}\n", "Сума, грн.", invoice.Amount);
						sb.AppendLineWrap(string.Format("\\h{0}", NumInWords.GrivnaPhrase(invoice.Amount)), MaxWidth, TextAlignment.Left);
						sb.AppendLineWrap(string.Format("{0}", invoice.TaxReference), MaxWidth, TextAlignment.Left);
					}
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '-'));

					//Служебная информация
					//
					sb.AppendLineWrap("Відповідальний за здійснення та правильність оформлення операції по відвантаженню товарів/послуг - матеріально відповідальна особа:", MaxWidth, TextAlignment.Left);

					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap("Комірник", MaxWidth, TextAlignment.Center);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '_'));
					sb.AppendLineWrap("посада", MaxWidth, TextAlignment.Center);
					sb.AppendLineWrap(invoice.StorekeeperName, MaxWidth, TextAlignment.Right);
					sb.AppendLine("___________________/________________");
					sb.AppendLine("     Підпис                  ПІБ");

					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap("Товар (послугу/и) отримав, працездатність товарів, які передаються, та їх комплектність перевірено споживачем. Товари в справному стані. Претензій щодо якості та кількості товарів, наданих послуг (у разі надання) не маю:", MaxWidth, TextAlignment.Left);
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLine(string.Empty.PadRight(MaxWidth, '_'));
					sb.AppendLineWrap("посада", MaxWidth, TextAlignment.Center);
					sb.AppendLine("___________________/________________");
					sb.AppendLine("  Підпис споживача    ПІБ споживача");

					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLineWrap("Товар (послугу) отримав за дорученням:", MaxWidth, TextAlignment.Left);
					sb.AppendLine("Серія_____№__________від____________");
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendLine("Через_______________________________");
					
					sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
					sb.AppendFormat("{0}\n", invoice.Key.Number);
					sb.AppendFormat("\\bcl{0}\n", invoice.Key.Number);
					#endregion
				}
			}

			return sb.ToString();
		}
	}
}
