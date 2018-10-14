using System.Collections.Generic;
using System.Numerics;

namespace MUR.VmdNS
{
    public class Bone
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Quaternion { get; set; }
        public List<byte> InterpolationParameters { get; set; } = new List<byte>
        {
            0x14,
            0x14,
            0x00,
            0x00,
            0x14,
            0x14,
            0x14,
            0x14,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x14,
            0x14,
            0x14,
            0x14,
            0x14,
            0x14,
            0x14,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x00,
            0x14,
            0x14,
            0x14,
            0x14,
            0x14,
            0x14,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x00,
            0x00,
            0x14,
            0x14,
            0x14,
            0x14,
            0x14,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x6B,
            0x00,
            0x00,
            0x00,
        };
    }
}
