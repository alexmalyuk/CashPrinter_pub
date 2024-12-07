using System;

namespace CashPrinter.Core
{
    public abstract class CashService : IDisposable
    {
        public abstract void Dispose();

        public abstract string DeviceName { get; }
        public abstract string SerialNumber { get; }
        public abstract string ErrorMessage { get; }
        public abstract int ResultCode { get; }
        public abstract bool DeviceEnabled { get; }

        public abstract bool Test();
        public abstract void PrintText(string Text, int Copies = 1);
        public abstract void PrintPDF(byte[] Pdf, int Copies = 1);
    }
}
