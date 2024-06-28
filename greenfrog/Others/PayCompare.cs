using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Globalization;

namespace greenfrog.Others
{
    public class PayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var Compare = CompareInfo.GetCompareInfo("en-US");
            return Compare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}