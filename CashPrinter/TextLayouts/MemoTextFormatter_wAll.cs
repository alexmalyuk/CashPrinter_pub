using CashPrinter.Core;
using Core.CashPrinter;
using System;
using System.Text;
using System.Windows;

namespace CashPrinter.TextLayouts
{
    public class MemoTextFormatter_wAll : TextFormatter
    {
        public override string GetTextFromReceipt(CashDocument cashDocument)
        {
			int MaxWidth;

			if (AppSettings.TapeWidth == TapeWidthEnum.Tape57mm)
				MaxWidth = 38;
			else if (AppSettings.TapeWidth == TapeWidthEnum.Tape80mm)
				MaxWidth = 55;
			else
				throw new NotSupportedException();

			StringBuilder sb = new StringBuilder();

            if (cashDocument.memoTypeEnum == MemoTypeEnum.General)
            {
				sb.AppendLineWrap("\\w\\hШановний Споживач!", MaxWidth / 2, TextAlignment.Center);
				sb.AppendLineWrap(" Ви маєте право повернути товар належної якості, який придбали на сайті https://allo.ua з метою обміну на інший товар або повернення коштів згідно зі статтею 9 Закону України \"Про захист прав споживачів\", якщо він не влаштовує Вас за формою, габаритами, фасоном, кольором, розміром.", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" Перед поверненням потрібно переконатись що:", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - з дати придбання не пройшло 14 днів,", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - товар не використовувався, збережено його товарний вигляд, споживчі властивості та повна комплектація (як на момент придбання),", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - є в наявності розрахунковий документ, виданий Продавцем разом з товаром (акт приймання-передачі товару / накладна / чек або інший документ, що засвідчує факт купівлі)", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" Повернути товар можливо:", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - у магазині ТОВ \"АЛЛО\" попередньо узгодивши таку можливість  по телефону гарячої лінії Алло 0 800 300 100.", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - або відправивши на нашу адресу Новою Поштою", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" Для повернення товару Новою Поштою Вам потрібно:", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - оформити заяву (Зразок № 2 заяви, що міститься за посиланням", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("https://new.allo.ua/warranty-info-page/statement_9.pdf", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(") Реквізити для повернення коштів необхідно заповнити без помилок для запобігання непорозумінь", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - відправити товар у повній комплектації із розрахунковим документом, що свідчить про придбання товару, через відділення \"Нової Пошти\" на склад:", MaxWidth, TextAlignment.Left);

				sb.AppendLineWrap(" - Телефон Отримувача: 0 800 300 100", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Адреса доставки: 04073, місто Київ, проспект Степана Бандери, 26", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Отримувач: представник ІМ \"Алло\"", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Платник: 3-я особа за безготівковим розрахунком", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Доставка: Склад - Двері", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - В графі експрес-накладної Нової пошти \"Оголошена вартість\" вказати фактичну вартість придбаного товару.", MaxWidth, TextAlignment.Left);
				//sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" У випадку пред'явлення Споживачем вимог щодо здійснення обміну товару на аналогічний або повернення коштів, у зв'язку із тим, що у придбаному товарі протягом 14 днів з дня покупки з'явився недолік - такий товар (відповідно до ст. 8 Закону України \"Про захист прав споживачів\", а також умов Розділу 3 Угоди користувача, яка розміщена за посиланням: https://allo.ua/ua/terms_of_use/) передається до уповноваженого сервісного центру виробника для безкоштовного усунення недоліку, за умови дотримання Споживачем правил експлуатації товару.", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" Якщо такий товар при дотриманні Споживачем правил експлуатації вийшов з ладу в гарантійний термін для вирішення такого проблемного питання просимо Вас ознайомитись з інформацією, розміщеною на сайті https://allo.ua у Розділі \"Гарантія/Повернення\" (\"ГАРАНТІЙНЕ ОБСЛУГОВУВАННЯ\")", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" За будь-яких додаткових питань будемо раді Вам відповісти по телефону нашої гарячої лінії 0 800 300 100.", MaxWidth, TextAlignment.Left);
			}
			else if (cashDocument.memoTypeEnum == MemoTypeEnum.Samsung)
            {
				sb.AppendLineWrap("\\w\\hШановний Споживач!", MaxWidth / 2, TextAlignment.Center);
				sb.AppendLineWrap(" Ви маєте право повернути товар належної якості, який придбали на сайті https://allo.ua з метою обміну на інший товар або повернення коштів згідно зі статтею 9 Закону України \"Про захист прав споживачів\", якщо він не влаштовує Вас за формою, габаритами, фасоном, кольором, розміром.", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" Перед поверненням потрібно переконатись що:", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - з дати придбання не пройшло 14 днів,", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - товар не використовувався, збережено його товарний вигляд, споживчі властивості та повна комплектація (як на момент придбання),", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap("  - є в наявності розрахунковий документ, виданий Продавцем разом з товаром (акт приймання-передачі товару / накладна / чек або інший документ, що засвідчує факт купівлі)", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" Повернути товар можливо відправивши на нашу адресу Новою Поштою", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" Для повернення товару Новою Поштою Вам потрібно:", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - відправити товар у повній комплектації із розрахунковим документом, що свідчить про придбання товару, через відділення \"Нової Пошти\" на склад:", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Телефон Отримувача: 0 800 300 100", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Адреса доставки: 04073, місто Київ, проспект Степана Бандери, 26", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Отримувач: представник IM \"Алло\"", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Платник: 3-я особа за безготівковим розрахунком", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - Доставка: Склад - Двері", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" - В графі експрес-накладної Нової  пошти \"Оголошена вартість\" вказати фактичну вартість придбаного товару.", MaxWidth, TextAlignment.Left);
				//sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" У випадку пред'явлення Споживачем вимог щодо здійснення обміну товару на аналогічний або повернення коштів, у зв'язку із тим, що у придбаному товарі протягом 14 днів з дня покупки з'явився недолік - такий товар (відповідно до ст. 8 Закону України \"Про захист прав споживачів\", а також умов Розділу 3 Угоди користувача, яка розміщена за посиланням: https://allo.ua/ua/terms_of_use/) передається до уповноваженого сервісного центру виробника для безкоштовного усунення недоліку, за умови дотримання Споживачем правил експлуатації товару.", MaxWidth, TextAlignment.Left);
				sb.AppendLineWrap(" Якщо такий товар при дотриманні Споживачем правил експлуатації вийшов з ладу в гарантійний термін для вирішення такого проблемного питання просимо Вас ознайомитись з інформацією, розміщеною на сайті http://crp.in.ua/ у Розділі \"Гарантія/Повернення\" (\"ГАРАНТІЙНЕ ОБСЛУГОВУВАННЯ\")", MaxWidth, TextAlignment.Left);
				sb.AppendLine(string.Empty.PadRight(MaxWidth, ' '));
				sb.AppendLineWrap(" За будь-яких додаткових питань будемо раді Вам відповісти по телефону нашої гарячої лінії 0 800 300 100.", MaxWidth, TextAlignment.Left);
			}

			return sb.ToString();
		}
	}
}
