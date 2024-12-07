using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashPrinter.CashServices.Datecs
{
    public struct DirectIOCommand 
    {
        public int Command;
        public int ReturnParameter;
        public string Parameters;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("DirectIO(");
            sb.Append(String.Format("{0:X}h", Command));
            sb.Append(", ");
            sb.Append(ReturnParameter);
            sb.Append(", \"");
            sb.Append(Parameters);
            sb.Append("\")");

            return sb.ToString();
        }

        public DirectIOCommand(int Command, params object[] Parameters)
        {
            this.Command = Command;
            this.ReturnParameter = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("000000;");
            foreach (object param in Parameters)
            {
                sb.Append(param.ToString());
                sb.Append(';');
            }

            this.Parameters = sb.ToString();
        }
    }

    public class CommandList : List<DirectIOCommand>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (DirectIOCommand command in this)
                sb.AppendLine(command.ToString());

            return sb.ToString();
        }

        public void Add(int Command, params object[] Parameters)
        {
            this.Add(new DirectIOCommand(Command, Parameters));
        }
    }
}
