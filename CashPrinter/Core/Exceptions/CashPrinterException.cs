using System;
using System.Runtime.Serialization;

namespace CashPrinter.Core
{
    class CashPrinterException : Exception
    {
        public CashPrinterException()
        {
        }

        public CashPrinterException(string message) : base(message)
        {
        }

        public CashPrinterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CashPrinterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
