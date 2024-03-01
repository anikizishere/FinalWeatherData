using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Väderdata___Inlämning
{
    public static class ExtensionMethods
    {
        public static string Tab(this string str)
        {
            return new string(str + "\t");
        }

        public static string DegreesC(this string str)
        {
            return new string(str + '\u00B0' + 'C');
        }
    }
}
