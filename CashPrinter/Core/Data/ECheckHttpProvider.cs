using System;
using System.IO;
using System.Net;
using System.Web;

namespace CashPrinter.Core.Data
{
    public static class ECheckHttpProvider
    {
        public static byte[] GetReceiptPDF(string FiscalRRONumber, string FiscalNumber, DateTime FiscalDate)
        {
            try
            {
                // <EcheckUrl>?rro=4000028987&receipt=3219091&date=20210216
                var uriBuilder = new UriBuilder(AppSettings.EcheckUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add("rro", FiscalRRONumber);
                query.Add("receipt", FiscalNumber);
                query.Add("date", FiscalDate.ToString("yyyyMMdd"));
                uriBuilder.Query = query.ToString();
                string longurl = uriBuilder.ToString();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(longurl);
                request.Method = "GET";
                request.ContentType = "application/pdf";
                request.Headers.Add("Authorization", AppSettings.EcheckAuth);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(memoryStream);
                    byte[] data = memoryStream.ToArray();
                    return data;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new CashPrinterException("Fiscal check not found", ex);
                else 
                    throw new CashPrinterException(string.Format("E-check error: {0}", ex.Message), ex);

                throw new CashPrinterException(string.Format("E-check error: {0}", ex.Message), ex);
            }
            catch (Exception ex)
            {
                throw new CashPrinterException(string.Format("E-check error: {0}", ex.Message), ex);
            }
        }
    }
}
