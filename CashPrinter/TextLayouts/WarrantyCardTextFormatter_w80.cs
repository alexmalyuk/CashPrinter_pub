using CashPrinter.CashDocuments;
using CashPrinter.Core;
using System.Text;
using System.Windows;

namespace CashPrinter.TextLayouts
{
    public class WarrantyCardTextFormatter_w80 : TextFormatter
    {
        private const int MaxWidth = 55;    //38

        public override string GetTextFromReceipt(CashDocument cashDocument)
        {
            WarrantyCard warrantyCard = cashDocument as WarrantyCard;
            StringBuilder sb = new StringBuilder();

            if (warrantyCard != null)
            {
                if (warrantyCard.isFOP)
                {
                    // ФОП Шапка чека
                    sb.AppendLineWrap(string.Format("Продавець: {0}", warrantyCard.SupplierName), MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(warrantyCard.SupplierFiscalAddress, MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(string.Format("ІПН: {0}", warrantyCard.SupplierINN), MaxWidth, TextAlignment.Center);
                }
                else
                {
                    // АЛЛО Шапка чека
                    sb.AppendLineWrap(string.Format("Продавець: {0}", warrantyCard.SupplierName), MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(warrantyCard.SupplierFiscalAddress, MaxWidth, TextAlignment.Center);
                    sb.AppendLineWrap(string.Format("ЄДРПОУ: {0}", warrantyCard.SupplierINN), MaxWidth, TextAlignment.Center);
                }

                // Заголовок
                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLineWrap(@"\w\hГАРАНТІЙНИЙ ТАЛОН", MaxWidth / 2, TextAlignment.Center);
                sb.AppendLineWrap(string.Format(@"\w\h№ {0} {1} {2}", warrantyCard.Key.Number, warrantyCard.Key.Type, warrantyCard.Key.Company), MaxWidth / 2, TextAlignment.Center);
                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));

                // Информация о товаре
                sb.AppendLineWrap(string.Format("Найменування та модель товару: {0}", warrantyCard.GoodName), MaxWidth, TextAlignment.Left);
                sb.AppendLineWrap(string.Format("Код товару: {0}", (warrantyCard.Key as WarrantyCardRecordKey).GoodCode), MaxWidth, TextAlignment.Left);
                sb.AppendLineWrap(string.Format("Заводський номер(IMEI): {0}", (warrantyCard.Key as WarrantyCardRecordKey).SerialNumber), MaxWidth, TextAlignment.Left);
                sb.AppendLineWrap(string.Format("Дата продажу: {0:dd.MM.yyyy}", warrantyCard.DateOfSale), MaxWidth, TextAlignment.Left);
                sb.AppendLineWrap(string.Format("Ціна товару: {0:0.00} грн.", warrantyCard.Price), MaxWidth, TextAlignment.Left);
                sb.AppendLineWrap(string.Format("Строк гарантії: {0} з дати продажу", warrantyCard.WarrantyPeriod), MaxWidth, TextAlignment.Left);

                // Уценка
                if (warrantyCard.MarkdownType == "у1")
                {
                    sb.AppendLineWrap(" Товар, вказаний в цьому гарантійному талоні, є уцінений по одній або декільком причинам:", MaxWidth, TextAlignment.Left);
                    sb.AppendLineWrap("  - на товарі присутні дрібні подряпини, потертості, сколи;", MaxWidth, TextAlignment.Left);
                    sb.AppendLineWrap("  - у комплектації товару відсутні деякі комплектуючі (карта пам'яті, акумуляторна батарея, зарядний пристрій);", MaxWidth, TextAlignment.Left);
                    sb.AppendLineWrap("  - коробка товару не відповідає моделі товару;", MaxWidth, TextAlignment.Left);
                    sb.AppendLineWrap("  - пошкодження упаковки, яка порушує її цілісність, форму крім випадків якщо вийняти товар з упаковки без її пошкодження неможливо (наприклад: блістер для аксесуарів).", MaxWidth, TextAlignment.Left);
                }
                else if (warrantyCard.MarkdownType == "у2")
                {
                    sb.AppendLineWrap(" Товар, вказаний в цьому гарантійному талоні, є уцінений по одній або декільком причинам: численні потертості, багато дрібних відколів, глибоких подряпин, тріщин на корпусних частинах і дисплеї(відшарування фарби на значній площі поверхні: корпусу, дисплея, клавіатури), які порушують цілісність товару або комплектуючих.", MaxWidth, TextAlignment.Left);
                }

                // Бла-Бла-Бла
                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLineWrap(" Перед початком експлуатації товару(ів) Споживач зобов'язаний ознайомитися з інструкцією з користування.", MaxWidth, TextAlignment.Left);
                sb.AppendLineWrap(" Споживач своїм підписом підтверджує, що працездатність та комплектність товарів, зовнішній вигляд ним перевірені претензій немає:", MaxWidth, TextAlignment.Left);

                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLine("_________________________________/_____________________");
                sb.AppendLineWrap("(ПІБ та підпис Споживача)", MaxWidth, TextAlignment.Center);
                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLine("_______________________________________________________");
                sb.AppendLineWrap("(печатка)", MaxWidth, TextAlignment.Center);

                sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
                sb.AppendLineWrap(" Результати технічного обслуговування та гарантійного ремонту надаватимуться в окремій Довідці / Акті Сервісного центру.", MaxWidth, TextAlignment.Left);
            }

            return sb.ToString();
        }
    }
}
