using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beastrack.Utility
{
    public struct Range<T>
    {
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public T Min { get; set; }
        public T Max { get; set; }
    }
}
