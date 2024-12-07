using CashPrinter.Core;
using System;
using System.Diagnostics;

namespace CashPrinter.CashServices.Datecs
{
	class DatecsCashService : CashService
	{
		private object _oposDriver;
		private object oposDriver
        {
			get
            {
                try
                {
					return _oposDriver;
				}
				catch (Exception e)
                {
                    throw new CashPrinterException(e.Message, e);
                }
            }
        }
		private Type oposType;

		public DatecsCashService()
		{
			try
			{
				oposType = Type.GetTypeFromProgID("OPOS.FiscalPrinter");
				_oposDriver = Activator.CreateInstance(oposType);

				InvokeMethod("Open", "FiscPrinter");
				Connect();
			}
			catch (Exception)
			{
				_oposDriver = null;
			}
		}

		public override string DeviceName
		{
			get
			{
				if (oposDriver == null)
					return "???";
				else
					return (string)GetProperty("DeviceName");
			}
		}

		public override int ResultCode
		{
			get
			{
				if (oposDriver == null)
					return -999;
				else
					return (int)GetProperty("ResultCode");
			}
		}

		public override string ErrorMessage
		{
			get
			{
				if (oposDriver == null)
					return "\"OPOS.FiscalPrinter\" - клас не зареєстровано";
				else
				{
					int _resultCode = ResultCode;
					string _errorString = (string)GetProperty("ErrorString");

					if (_resultCode == 0)
						return "Ok";
					else
						return string.Format("{0} - {1}", _resultCode, _errorString);
				}
			}
		}
		public override string SerialNumber 
		{
			get 
			{
				if (oposDriver == null)
					return "???";
				else
				{
					InvokeMethod("DirectIO", 0x0042, 0, "000000;");
					return (string)GetProperty("ReservedWord");
				}
			}
		}

		public override bool DeviceEnabled 
		{
			get
			{
				return oposDriver == null ? false : ResultCode == 0;
			}
		}

		public override void PrintText(string Text, int Copies = 1)
		{
			Coupon coupon = new Coupon(Text);
            for (int i = 0; i < Copies; i++)
				PrintCoupon(coupon);
		}

		private object InvokeMethod(string MethodName, params object[] parameters)
		{
			if (oposDriver == null)
				return null;

			oposType.InvokeMember(MethodName, System.Reflection.BindingFlags.InvokeMethod, null, oposDriver, parameters);
			return GetProperty("ReservedWord");
		}

		private object GetProperty(string PropertyName)
		{
			return oposType?.InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty, null, oposDriver, null);
		}
		private void SetProperty(string PropertyName, object value)
		{
			if (oposDriver == null)
				return;

			object[] parameter = new object[1];
			parameter[0] = value;

			oposType?.InvokeMember(PropertyName, System.Reflection.BindingFlags.SetProperty, null, oposDriver, parameter);
		}

		private bool Connect()
		{
			if (oposDriver == null)
				return false;

			InvokeMethod("Claim", 20);
			SetProperty("DeviceEnabled", true);
			int res = (int)GetProperty("ResultCode");

			return (res == 0);
		}

		public void Disconnect()
		{
			if (oposDriver == null)
				return;

			InvokeMethod("ResetPrinter");
			SetProperty("DeviceEnabled", false);
			InvokeMethod("ReleaseDevice");
		}

		public override bool Test()
		{
			if (oposDriver == null)
				return false;

			InvokeMethod("ResetPrinter");
			return true;
		}
		private bool PrintCoupon(Coupon coupon)
		{
			if (oposDriver == null)
				return false;

			///TODO: Сохранение, сброс и восстановление параметров печати чека
			/// 1. сохраненить шапку, подвал чека, пераметры прогона и обрезки
			/// 2. установка необходимых для печати купона (сброс)
			/// 3. печать купона
			/// 4. восстановление обратно сохраненных значений
			/// Это нужно при печати купонов на ФИСКАЛИЗИРОВАННОМ аппарате

			SaveOPOSSettings();
				
			CommandList commandList = coupon.GetCommandList();
			Debug.Print(commandList.ToString());

			try
			{
				foreach (DirectIOCommand command in commandList)
				{
					InvokeMethod("DirectIO", command.Command, command.ReturnParameter, command.Parameters);
					if (ResultCode != 0)
						throw new Exception($"{command.ToString()}\n{ErrorMessage}");
				}
			}
			catch (Exception e)
			{
				// Команда С8h: Печать купона - 2 Сброс буфера и возврат в основной режим
				InvokeMethod("DirectIO", 0x00C8, 0, "000000;2;");
				InvokeMethod("ResetPrinter");

				///TODO Logging the OPOS exceptions
				///
				//Debug.Print(e.Message);

				//return false;
				throw new CashPrinterException(e.Message, e);
			}
			finally
			{
				RestoreOPOSSettings();
			}

			return true;
		}

		private void RestoreOPOSSettings()
		{
			// прогонка под обрезчик пустыми строками
			// шапка и подвал чека
			// 


		}

		private void SaveOPOSSettings()
		{
			//throw new NotImplementedException();
		}

		public override void Dispose()
		{
			Disconnect();
		}

        public override void PrintPDF(byte[] Pdf, int Copies = 1)
        {
            throw new NotImplementedException();
        }
    }
}
