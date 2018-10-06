using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beastrack.Utility
{
    public static class Extension
    {
        public static bool IsBetween<U>(this U val, U low, U high, bool includeBorder = true) where U : IComparable
        {
            return includeBorder ?
                (low.CompareTo(val) <= 0 && val.CompareTo(high) <= 0) :
                (low.CompareTo(val) < 0 && val.CompareTo(high) < 0);
        }
    }
}
