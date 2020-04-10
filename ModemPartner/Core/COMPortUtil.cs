using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemPartner.Core
{
    class COMPortUtil
    {
        public static string ExtractCOMPortFromName(string name)
        {
            int openBracket = name.IndexOf('(');
            int closeBracket = name.IndexOf(')');
            return name.Substring(openBracket + 1, closeBracket - openBracket - 1);
        }
    }
}
