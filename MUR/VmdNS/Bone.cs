using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MUR.VmdNS
{
    public class Bone
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Quaternion { get; set; }
        public List<byte> InterpolationParameters { get; set; }
    }
}
