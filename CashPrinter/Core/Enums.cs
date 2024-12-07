using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CashPrinter
{
    public enum CashTypeEnum 
    {
        Datecs_FP320,
        FakeCash,
        ThermalPrinter
    }

    public enum TapeWidthEnum
    {
        Tape57mm,
        Tape80mm
    }

    public enum MemoTypeEnum
    {
        General,
        Samsung
    }
}
