using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUR.VmdNS
{
    public class Frame
    {
        public Frame()
        {
            Bones = new Dictionary<string, Bone>();
        }

        public int Index { get; set; }
        public IDictionary<string, Bone> Bones { get; set; }
    }
}
