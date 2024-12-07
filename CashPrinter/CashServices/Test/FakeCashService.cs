using System.Diagnostics;
using CashPrinter.Core;
using System.IO;

namespace CashPrinter.CashServices.Test
{
	public class FakeCashService : CashService
	{
        public override string DeviceName => "Test cash service";

        public override string ErrorMessage => "Ok";

        public override string SerialNumber => "*12345*";

        public override bool DeviceEnabled => true;

        public override int ResultCode => 0;

        public override void PrintText(string text, int Copies = 1)
        {
            for (int i = 0; i < Copies; i++)
            {
				string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), "txt");
				File.WriteAllText(tempFileName, text);
				Process.Start(tempFileName);
			}
		}

        public override void Dispose()
        {
        }

        public override bool Test()
        {
            return true;
        }

        public override void PrintPDF(byte[] Pdf, int Copies = 1)
        {
            for (int i = 0; i < Copies; i++)
            {
                string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), "pdf");
                File.WriteAllBytes(tempFileName, Pdf);
                Process.Start(tempFileName);
            }
        }
    }
}
