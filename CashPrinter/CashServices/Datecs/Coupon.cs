using CashPrinter.Core;
using Core.CashPrinter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CashPrinter.CashServices.Datecs
{
	public class Coupon
	{
		private const int nonPrintHeaderHeight = 45;
		private const int lineHeight = 15;
		private const int lineInterval = 6;
		private const decimal pixelRatio = 2.045455M;
		private const int maxCouponHeight = 1440;
		private int currentCouponHeight = 0;
		private int couponCount = 0;
		private int currentPositionY = 0;
		
		private List<CouponLine> couponLines;

		public struct CouponLine 
		{
			public string Text;
			public bool isDoubleHeight;
			public bool isDoubleWidth;
			public int PositionX;
			public int PositionY;
			public int CouponNumber;
			public bool isBarCode;

			public CouponLine(string Text, bool isDoubleHeight, bool isDoubleWidth, int PositionX)
			{
				this.Text = Text;
				this.isDoubleHeight = isDoubleHeight;
				this.isDoubleWidth = isDoubleWidth;
				this.PositionX = PositionX;
				this.PositionY = 0;
				this.CouponNumber = 0;
				this.isBarCode = false;
			}
		}

		public Coupon()
		{
			couponLines = new List<CouponLine>();
			couponCount = 1;
		}

		public Coupon(string text) : this()
		{
			string[] lines = text.Split('\n');
			
			foreach (string line in lines)
				AddTextLine(new CouponLine(line/*.TrimEnd()*/, false, false, 10));
		}

		public void AddTextLine(CouponLine couponLine)
		{
			if (couponLine.Text == string.Empty)
				return;

			couponLine.Text = Regex.Replace(couponLine.Text, ";", " ");

			string dblWidthPattern = @"(\\w)";
			if (Regex.IsMatch(couponLine.Text, dblWidthPattern))
			{
				couponLine.isDoubleWidth = true;
				couponLine.Text = Regex.Replace(couponLine.Text, dblWidthPattern, "");
			}
			string dblHeightPattern = @"(\\h)";
			if (Regex.IsMatch(couponLine.Text, dblHeightPattern))
			{
				couponLine.isDoubleHeight = true;
				couponLine.Text = Regex.Replace(couponLine.Text, dblHeightPattern, "");
			}
			string isBarcodePattern = @"(\\bcl)";
			if (Regex.IsMatch(couponLine.Text, isBarcodePattern))
			{
				couponLine.isBarCode = true;
				couponLine.Text = Regex.Replace(couponLine.Text, isBarcodePattern, "");
			}
			string isLogo = @"(\\logo)";
			if (Regex.IsMatch(couponLine.Text, isLogo))
			{
				currentPositionY -= (lineHeight + lineInterval) * 2;
				currentPositionY = Math.Max(currentPositionY, 0);
				couponLine.PositionX = AppSettings.TapeWidth == TapeWidthEnum.Tape57mm ? 290 : 450;
				couponLine.PositionY = currentPositionY;
				couponLine.Text = "Atlas";
				couponLine.isDoubleHeight = true;
			}

			int _wantedPositionY = currentPositionY;

			if (couponLine.isBarCode)
				_wantedPositionY += 45;
			else
			{
				if (couponLine.isDoubleHeight)
					_wantedPositionY += lineHeight;

				if (couponLines.Count > 0)
					_wantedPositionY += (lineHeight + lineInterval);
			}

			currentCouponHeight = (int)Math.Round(_wantedPositionY * pixelRatio) + nonPrintHeaderHeight;
			if (currentCouponHeight > maxCouponHeight)
			{
				couponCount++;
				currentPositionY = 0;
				currentCouponHeight = nonPrintHeaderHeight;
			}
			else
				currentPositionY = _wantedPositionY;

			couponLine.CouponNumber = couponCount;
			couponLine.PositionY = currentPositionY;
			couponLines.Add(couponLine);
		}

		public CommandList GetCommandList()
		{
			CommandList directIOCommands = new CommandList();
			int currentCouponNumber = 0;

			if (couponCount > 0)
			{
				foreach(CouponLine couponLine in couponLines)
				{
					if (couponLine.CouponNumber > currentCouponNumber)
					{
						currentCouponNumber = couponLine.CouponNumber;

						// Команда 88h: Установить служебный счетчик. 24 – Установка длины купона в пикселях.
						directIOCommands.Add(0x0088, 24, currentCouponNumber == couponCount ? currentCouponHeight : maxCouponHeight);

						// Команда С8h: Печать купона 
						// 0 - Переход в режим купона и инициализация буфера
						// 3 - Переход в режим купона и инициализация буфера без печати начального темной полосы
						// 4 - Распечатать буфер и перейти к формированию нового без обрезки.
						directIOCommands.Add(0x00C8, currentCouponNumber == 1 ? 3 : 4);

					}
					// Команда С9h: Установка направления печати и позиции
					directIOCommands.Add(0x00C9, 0, couponLine.PositionX, couponLine.PositionY);

					// Команда САh: Данные для печати
					if (couponLine.isBarCode)
						directIOCommands.Add(0x00CA, 1, 3, 30, couponLine.Text);    // 3 - ширина ШК (1-4); 30 - высота ШК
					else
						directIOCommands.Add(0x00CA, 0, 1, string.Format("{0}{1}", couponLine.isDoubleWidth ? "1" : "0", couponLine.isDoubleHeight ? "1" : "0"), couponLine.Text);
				}
				// Команда С8h: Печать купона - 1 Печать буфера и возврат в основной режим
				directIOCommands.Add(0x00C8, 1);
			}

			return directIOCommands;
		}

	}
}
