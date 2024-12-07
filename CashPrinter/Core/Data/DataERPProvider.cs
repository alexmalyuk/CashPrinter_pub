using CashPrinter.CashDocuments;
using Core.CashPrinter;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace CashPrinter.Core.DataBase
{
    public class DataERPProvider : IDisposable
	{
		private OracleConnection _oracon;

		public DataERPProvider()
		{
			Connect();
		}

		public void Dispose()
		{
			_oracon?.Close();
			_oracon?.Dispose();
		}

		private void Connect()
		{
			try
			{
				_oracon = new OracleConnection(AppSettings.ConnectionString);
				_oracon.Open();
			}
			catch (Exception)
			{
				_oracon = null;
			}
		}

		public List<CashDocument> GetReceiptQueue()
        {
			return GetReceiptQueue(null);
		}
		public List<CashDocument> GetReceiptQueue(string number)
		{
			var receiptList = new List<CashDocument>();

			if (_oracon != null)
			{
				DataTable dt = new DataTable();
				string sql;
				if (number != null)
					sql = string.Format(@"SELECT * FROM PRODDTA.F55FRC WHERE FHDOCO IN ({0}) ORDER BY FHDOCO", number);
                else
					sql = string.Format(@"SELECT * FROM PRODDTA.F55FRC WHERE FHYN = 'N' AND FHPHYD = '{0}' ORDER BY FHDOCO", string.IsNullOrWhiteSpace(AppSettings.WorkstationName) ? " " : AppSettings.WorkstationName);

				try
                {
					OracleDataAdapter oda = new OracleDataAdapter(sql, _oracon);
					oda.Fill(dt);
				}
				catch (Exception ex)
                {
                    throw new CashPrinterException(string.Format("Error loading Receipt: {0}", ex.Message), ex);
                }

				foreach (DataRow dr in dt.Rows)
				{

					CashDocument doc;

					string _RYIN = ConvertToString(dr["FHRYIN"]).ToUpper();
					string _STOP = ConvertToString(dr["FHSTOP"]).ToUpper();
					string _EV01 = ConvertToString(dr["FHEV01"]).ToUpper();
					string _EV02 = ConvertToString(dr["FHEV02"]).ToUpper();
					string _EV03 = ConvertToString(dr["FHEV03"]).ToUpper();
					string _EV04 = ConvertToString(dr["FHEV04"]).ToUpper();
					string _EV05 = ConvertToString(dr["FHEV05"]).ToUpper();
					string _EV06 = ConvertToString(dr["FHEV06"]).ToUpper();

					if (_RYIN.StartsWith("N") || (_RYIN.StartsWith("P") && _STOP.StartsWith("N")))
						doc = new Invoice();
                    else if (_EV02.StartsWith("1"))
                        doc = new FiscalReceipt();
                    else 
						doc = new Receipt();

					doc.isFOP = _EV01.StartsWith("1");
					doc.isPrintGoodCode = _EV03.StartsWith("1");
					doc.TaxPayerInfo = ConvertToString(dr["FHDL011"]);
					doc.IsGeneralTaxPayer = _EV04.StartsWith("1");
					if (_RYIN.StartsWith("H"))
                    {
						doc.memoTypeEnum = MemoTypeEnum.Samsung;
					}
					doc.IsPrintReceipt = _EV05.StartsWith("1");
                    doc.IsPrintMemo = _EV06.StartsWith("1");

					#region Fields description
					//FHDOCO,   № документа
					//FHDCTO,   тип
					//FHKCOO,   компания
					//FHTRDJ,   Дата
					//FHRYIN,   способ оплаты
					//FHCRCD,   Валюта
					//FHEV01,   ФЛП\Алло
					//FHAN81,   код ФЛП
					//FHALPH,   Название ФЛП
					//FHTAX,    ИНН ФЛП
					//FHAEXP,   Сумма чека (в копейках) (надо делить на 100)
					//FHDAMT,   скидка
					//FHYN,     Признак что чек напечатан
					//FHUK01    Номер чека
					//FHDL011	Надпись "є платником податку на прибуток на загальних підставах"/"є платником єдиного податку"
					//FHEV04	Признак плательщика налога на общих основаниях 0/1
					//FHEV05	Флаг друку чека
					//FHEV06	Флаг друку пам'ятки
					#endregion

					if (doc is Invoice)
					{
						// РН Расходная накладная
						var invoice = doc as Invoice;
						invoice.Key.Number = ConvertToDecimal(dr["FHDOCO"]);
						invoice.Key.Type = ConvertToString(dr["FHDCTO"]);
						invoice.Key.Company = ConvertToString(dr["FHKCOO"]);

						invoice.Date = DateTime.Now;
						invoice.AmountDiscounted = ConvertToDecimal(dr["FHAEXP"]) / 100 - ConvertToDecimal(dr["FHDAMT"]) / 100;

						invoice.SupplierName = ConvertToString(dr["FHALPH"]);
						invoice.SupplierINN = ConvertToString(dr["FHTX2"]);
						invoice.SupplierAddress = ConvertToString(dr["FHADDR"]);

						invoice.SupplierIBAN = ConvertToString(dr["FHIBAN"]);
						invoice.SupplierBankMFO = ConvertToString(dr["FHAN84"]);
						invoice.SupplierBankName = ConvertToString(dr["FHBANNA"]);
						invoice.SupplierOKPO = ConvertToString(dr["FHTAX"]);
						invoice.SupplierPhone = ConvertToString(dr["FHPH1"]);
						invoice.BuyerName = ConvertToString(dr["FHALPH1"]);
						invoice.BuyerOKPO = ConvertToString(dr["FHTAX0"]);
						invoice.BuyerAddress = ConvertToString(dr["FHK74ADL2"]);
						invoice.TaxReference = ConvertToString(dr["FHSGTXT"]);
						invoice.StorekeeperName = ConvertToString(dr["FHSLNM"]);

                        LoadReceiptLinesFromDB(invoice);
						receiptList.Add(invoice);
					}
					else if (doc is Receipt)
					{
						// Чек
						var receipt = doc as Receipt;
						receipt.Key.Number = ConvertToDecimal(dr["FHDOCO"]);
						receipt.Key.Type = ConvertToString(dr["FHDCTO"]);
						receipt.Key.Company = ConvertToString(dr["FHKCOO"]);

						receipt.Date = DateTime.Now;
						receipt.AmountDiscounted = ConvertToDecimal(dr["FHAEXP"]) / 100 - ConvertToDecimal(dr["FHDAMT"]) / 100;
						receipt.Currency = ConvertToString(dr["FHCRCD"]);
						receipt.SupplierName = ConvertToString(dr["FHALPH"]);
						receipt.SupplierINN = ConvertToString(dr["FHTAX"]);
						receipt.SupplierAddress = ConvertToString(dr["FHADDR"]);
						receipt.PaymentType = ConvertToString(dr["FHDL01"]);
						receipt.PaymentSum = receipt.AmountDiscounted;

						string textStoreId = ConvertToString(dr["FHMCU"]);
						Regex regex = new Regex("\\d+$");
						Match match = regex.Match(textStoreId);
						receipt.StoreId = Convert.ToInt32(match.Value);

						LoadReceiptLinesFromDB(receipt);
						LoadReceiptAGLinesFromDB(receipt);
						LoadReceiptPayFormLinesFromDB(receipt);

						receiptList.Add(receipt);
					}
					else if (doc is FiscalReceipt)
					{
						// Фискальный Чек
						var fiscalReceipt = doc as FiscalReceipt;
						fiscalReceipt.Key.Number = ConvertToDecimal(dr["FHDOCO"]);
						fiscalReceipt.Key.Type = ConvertToString(dr["FHDCTO"]);
						fiscalReceipt.Key.Company = ConvertToString(dr["FHKCOO"]);

						fiscalReceipt.Date = DateTime.Now;
						//fiscalReceipt.AmountDiscounted = ConvertToDecimal(dr["FHAEXP"]) / 100 - ConvertToDecimal(dr["FHDAMT"]) / 100;
						//fiscalReceipt.Currency = dr["FHCRCD"].ToString();
						//fiscalReceipt.SupplierName = dr["FHALPH"].ToString();
						//fiscalReceipt.SupplierINN = dr["FHTAX"].ToString();
						//fiscalReceipt.SupplierAddress = dr["FHADDR"].ToString();
						//fiscalReceipt.PaymentType = dr["FHDL01"].ToString();
						//fiscalReceipt.PaymentSum = fiscalReceipt.AmountDiscounted;

						//string textStoreId = dr["FHMCU"].ToString();
						//Regex regex = new Regex("\\d+$");
						//Match match = regex.Match(textStoreId);
						//fiscalReceipt.StoreId = Convert.ToInt32(match.Value);

						//fiscalReceipt.FiscalNumber = dr["FHUK01"].ToString();
						fiscalReceipt.FiscalNumber = ConvertToString(dr["FHORDERID"]);
						fiscalReceipt.FiscalRRONumber = ConvertToString(dr["FHUK02"]);
						try
                        {
							fiscalReceipt.FiscalDate = (DateTime)dr["FHFDAT"];
                        }
                        catch (Exception ex)
                        {
                            var _fieldValue = dr["FHFDAT"];
                            string sFieldValue = string.Format("{0} '{1}'", _fieldValue.GetType().Name, _fieldValue.ToString());
                            throw new CashPrinterException(string.Format("{0} - Некоректне значення дати у полі FHFDAT: {1}", fiscalReceipt.ToString(), sFieldValue), ex);
                        }

                        //LoadReceiptLinesFromDB(fiscalReceipt);
						receiptList.Add(fiscalReceipt);
					}

					if (AppSettings.DatabaseQueryinterval == 0)
                        break;
                }
			}

			return receiptList;
		}

		public List<CashDocument> GetWarrantyCardQueue()
		{
			return GetWarrantyCardQueue(null);
		}
		public List<CashDocument> GetWarrantyCardQueue(string number)
		{
			var receiptList = new List<CashDocument>();

			if (_oracon != null)
			{
				DataTable dt = new DataTable();
				string sql;
				if (number != null)
					sql = string.Format(@"SELECT * FROM PRODDTA.F55FRWC WHERE FWDOCO IN ({0}) ORDER BY FWDOCO, FWITM", number);
				else
					sql = string.Format(@"SELECT * FROM PRODDTA.F55FRWC WHERE FWYN = 'N' AND FWPHYD = '{0}' ORDER BY FWDOCO, FWITM", string.IsNullOrWhiteSpace(AppSettings.WorkstationName) ? " " : AppSettings.WorkstationName);

                try
                {
					OracleDataAdapter oda = new OracleDataAdapter(sql, _oracon);
					oda.Fill(dt);
				}
				catch (Exception ex)
                {
                    throw new CashPrinterException(string.Format("Error loading WarrantyCard: {0}", ex.Message), ex);
                }

				foreach (DataRow dr in dt.Rows)
				{

#region Fields description
					//FWDOCO	номер документа
					//FWDCTO	тип
					//FWKCOO	компания
					//FWITM     ном.Номер позиции
					//FWAA30	имеи или серийник

					//FWADDJ	дата документа
					//FWMGTX	наименование позиции
					//FWSOQS	кол-во
					//FWUPRC	цена
					//FWDL01	срок гарантии(строка)
					//FWXRT		Тип позиции(У1, У2 или пусто)
					//FWADDR	нефискальный адрес
					//FWK74ADl1 фискальный адрес
					//FWEV01	алло\флп
					//FWAN81	код флп
					//FWALPH	наименование продавца
					//FWTAX		инн продавца
					
					//FWPHYD	имя принтера
					//FWYN		признак печати
#endregion

					var warrantyCard = new WarrantyCard();
					warrantyCard.Key.Number = ConvertToDecimal(dr["FWDOCO"]);
					warrantyCard.Key.Type = ConvertToStringNoTrim(dr["FWDCTO"].ToString());
					warrantyCard.Key.Company = ConvertToStringNoTrim(dr["FWKCOO"].ToString());
					(warrantyCard.Key as WarrantyCardRecordKey).GoodCode=ConvertToDecimal(dr["FWITM"]);
					(warrantyCard.Key as WarrantyCardRecordKey).SerialNumber = ConvertToStringNoTrim(dr["FWAA30"]);

					warrantyCard.Date = DateTime.Now;
					warrantyCard.GoodName = ConvertToString(dr["FWMGTX"]);
					warrantyCard.MarkdownType = ConvertToString(dr["FWXRT"]).ToLower();
					warrantyCard.Quantity = ConvertToDecimal(dr["FWSOQS"]);
					warrantyCard.Price = ConvertToDecimal(dr["FWUPRC"]) / 10000;
					warrantyCard.WarrantyPeriod = ConvertToString(dr["FWDL01"]); 
					warrantyCard.SupplierName = ConvertToString(dr["FWALPH"]);
					warrantyCard.SupplierINN = ConvertToString(dr["FWTAX"]);
					warrantyCard.SupplierFiscalAddress = ConvertToString(dr["FWADDR"]);
					warrantyCard.isFOP = ConvertToString(dr["FWEV01"]).Equals("1");
                    try
                    {
						warrantyCard.DateOfSale = (DateTime)dr["FWFDAT"];
					}
					catch (Exception ex)
                    {
						var _fieldValue = dr["FWFDAT"];
						string sFieldValue = string.Format("{0} '{1}'", _fieldValue.GetType().Name, _fieldValue.ToString());
						throw new CashPrinterException(string.Format("{0} - Некоректне значення дати у полі FWFDAT: {1}", warrantyCard.ToString(), sFieldValue), ex);
                    }

					receiptList.Add(warrantyCard);

                    if (AppSettings.DatabaseQueryinterval == 0)
                        break;
                }
			}

			return receiptList;
		}

		private void LoadReceiptLinesFromDB(CashDocument receipt)
		{
			if (receipt == null)
				return;

			receipt.Lines.Clear();

			DataTable dt = new DataTable();
			string sql = string.Format(@"SELECT * FROM PRODDTA.F55FRD WHERE FDDOCO = {0} AND FDDCTO = '{1}' AND FDKCOO = '{2}' ORDER BY FDLNID", receipt.Key.Number, receipt.Key.Type, receipt.Key.Company);

            try
            {
				OracleDataAdapter oda = new OracleDataAdapter(sql, _oracon);
				oda.Fill(dt);
			}
			catch (Exception ex)
            {
				throw new CashPrinterException(string.Format("{0} Error loading ReceiptLines: {1}", receipt.ToString(), ex.Message), ex);
			}

			foreach (DataRow dr in dt.Rows)
			{
				#region Fields description
				//FDLNID,    строка
				//FDITM,    код позиции(товара)
				//FDMGTX,   Название товара
				//FDSOQS,   количество
				//FDUPRC,   Цена (в копейках) (надо делить на 10000, т.е. 4 знака после запятой)
				//FDAEXP,   Сумма
				//FDDAMT,   скидка
				//FDEV01,   признак ПФ
				//FDEV02,   признак НДС
				//FDEV03    признак акциз
				//FDAA30	код УКТЗЕД
				#endregion

				var rLine = new CashDocumentLine();
				rLine.GoodCode = ConvertToString(dr["FDITM"]);
				rLine.GoodName = ConvertToString(dr["FDMGTX"]);
				rLine.GoodUKTZED = ConvertToString(dr["FDAA30"]);
				rLine.Quantity = ConvertToDecimal(dr["FDSOQS"]);
				rLine.Price = ConvertToDecimal(dr["FDUPRC"]) / 10000;
				rLine.Amount = ConvertToDecimal(dr["FDAEXP"]) / 100;
				rLine.Discount = ConvertToDecimal(dr["FDDAMT"]) / 100;
				rLine.isVAT = ConvertToString(dr["FDEV02"]).Equals("1");
				rLine.isPF = ConvertToString(dr["FDEV01"]).Equals("1");
				rLine.isExcise = ConvertToString(dr["FDEV03"]).Equals("1");

				receipt.Lines.Add(rLine);
			}
		}
		private void LoadReceiptAGLinesFromDB(Receipt receipt)
		{
			if (receipt == null)
				return;

			DataTable dt = new DataTable();
			string sql = string.Format(@"SELECT FPMGTX FROM proddta.F55FRP WHERE FPDOCO = {0} AND FPDCTO = '{1}' AND FPKCOO = '{2}' ORDER BY FPLNID", receipt.Key.Number, receipt.Key.Type, receipt.Key.Company);

			try
			{
				OracleDataAdapter oda = new OracleDataAdapter(sql, _oracon);
				oda.Fill(dt);
			}
			catch (Exception ex)
			{
				throw new CashPrinterException(string.Format("{0} Error loading ReceiptAGLines: {1}", receipt.ToString(), ex.Message), ex);
			}

			#region Fields description
			//FPLNID,   номер строки
			//FPMGTX,	Строка описания АГ (Алло-гроші)
			#endregion
			
			StringBuilder sb = new StringBuilder();
			
			foreach (DataRow dr in dt.Rows)
				sb.AppendLine(ConvertToString(dr["FPMGTX"]));
			
			receipt.AGDescription = sb.ToString().Trim();
		}
		private void LoadReceiptPayFormLinesFromDB(CashDocument receipt)
		{
			if (receipt == null)
				return;

			receipt.PayFormLines.Clear();

			DataTable dt = new DataTable();
			string sql = string.Format(@"SELECT * FROM proddta.F55FRCP WHERE FPDOCO = {0} AND FPDCTO = '{1}' AND FPKCOO = '{2}' ORDER BY FPKY", receipt.Key.Number, receipt.Key.Type, receipt.Key.Company);

			try
			{
				OracleDataAdapter oda = new OracleDataAdapter(sql, _oracon);
				oda.Fill(dt);
			}
			catch (Exception ex)
			{
				throw new CashPrinterException(string.Format("{0} Error loading ReceiptPayFormLines: {1}", receipt.ToString(), ex.Message), ex);
			}

			foreach (DataRow dr in dt.Rows)
			{
				#region Fields description
				//FPDL01,	Название формы оплаты
				//FPAEXP,	Сумма
				#endregion

				var pfLine = new CashDocumentPayFormLine();
				pfLine.Desription = ConvertToString(dr["FPDL01"]);
				pfLine.Sum = ConvertToDecimal(dr["FPAEXP"]) / 100;

				receipt.PayFormLines.Add(pfLine);
			}
		}

		public void SetCashDocumentStatusPrinted(RecordKey key)
		{
			if (App.IsNoDatecsMode || App.IsDebugMode)
				return;

			string sql = BuildUpdateSql(key);
			if (!string.IsNullOrEmpty(sql))
            {
				try
				{
					ExecuteUpdateQuery(sql);
				}
				catch (Exception ex)
				{
					throw new CashPrinterException(string.Format("{ 0} Error setting PrintStatus: {1}", key.ToString(), ex.Message), ex);
				}
			}
		}

		private string BuildUpdateSql(RecordKey key)
        {
            if (key is ReceiptRecordKey || key is FiscalReceiptRecordKey || key is InvoiceRecordKey)
            {
                return $@"
            UPDATE PRODDTA.F55FRH 
            SET FHYN = 'Y' 
            WHERE FHDOCO = {key.Number} 
              AND FHDCTO = '{key.Type}' 
              AND FHKCOO = '{key.Company}'";
            }

            if (key is WarrantyCardRecordKey warrantyKey)
			{
				var sql = $@"
            UPDATE PRODDTA.F55FRW 
            SET FWYN = 'Y' 
            WHERE FWDOCO = {warrantyKey.Number} 
              AND FWDCTO = '{warrantyKey.Type}' 
              AND FWKCOO = '{warrantyKey.Company}' 
              AND FWITM = {warrantyKey.GoodCode}";

				if (!string.IsNullOrWhiteSpace(warrantyKey.SerialNumber))
				{
					sql += $@"
              AND FWAA30 = '{warrantyKey.SerialNumber}'";
				}

				return sql;
			}

			return null;
		}

		public void ClearDocumentsQueue()
		{
            try
            {
				string sql;
				// чеки
				sql = string.Format(@"UPDATE PRODDTA.F55FRH H SET H.FHYN = 'Y' WHERE H.FHYN = 'N' AND H.FHPHYD = '{0}'", AppSettings.WorkstationName);
				ExecuteUpdateQuery(sql);
				// гарталоны
				sql = string.Format(@"UPDATE PRODDTA.F55FRW W SET W.FWYN = 'Y' WHERE W.FWYN = 'N' AND W.FWPHYD = '{0}'", AppSettings.WorkstationName);
				ExecuteUpdateQuery(sql);
			}
			catch (Exception ex)
            {
				throw new CashPrinterException(string.Format("Error clearing queue: {0}", ex.Message), ex);
			}
		}

		private void ExecuteUpdateQuery(string sql)
		{
			using (var cmd = new OracleCommand(sql, _oracon))
			{
				cmd.CommandTimeout = 15;
				cmd.ExecuteNonQuery();
			}
		}

		private decimal ConvertToDecimal(object value)
		{
			string strValue = value.ToString();
			if (string.IsNullOrEmpty(strValue))
				return 0;
			else
				return Convert.ToDecimal(strValue);
		}

		private string ConvertToString(object value)
		{
			return ConvertToStringNoTrim(value).Trim();
		}

		private string ConvertToStringNoTrim(object value)
		{
			string strValue = value.ToString();
			if (string.IsNullOrEmpty(strValue))
                return string.Empty;
			else
				return strValue;
		}

	}
}