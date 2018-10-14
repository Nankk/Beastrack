using MUR.VmdNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUR.IO
{
    public class VmdWriter
    {
        public VmdWriter(string path)
        {
            _path = path;
        }

        public void Write(Vmd vmd)
        {
            using (var bw = new BinaryWriter(File.OpenWrite(_path), System.Text.Encoding.GetEncoding("Shift_JIS")))
            {
                WriteHeader(bw, vmd);
                WriteFrames(bw, vmd);

                // Void data for expressions, camera, light, self-shadow
                for (int i = 0; i < 4; i++) bw.Write(0);
            }
        }

        // --- private ---
        void WriteHeader(BinaryWriter bw, Vmd vmd)
        {
            WriteString(bw, vmd.Header, 30);
            WriteString(bw, vmd.ModelName, 20);
            bw.Write(vmd.FramesTotal);
        }
        void WriteFrames(BinaryWriter bw, Vmd vmd)
        {
            foreach (var frame in vmd.Frames)
            {
                foreach (var bone in frame.Bones)
                {
                    WriteString(bw, bone.Key, 15);
                    bw.Write(frame.Index);
                    bw.Write(bone.Value.Position.X);
                    bw.Write(bone.Value.Position.Y);
                    bw.Write(bone.Value.Position.Z);
                    bw.Write(bone.Value.Quaternion.X);
                    bw.Write(bone.Value.Quaternion.Y);
                    bw.Write(bone.Value.Quaternion.Z);
                    bw.Write(bone.Value.Quaternion.W);
                    bw.Write(bone.Value.InterpolationParameters.ToArray());
                }
            }
        }
        void WriteString(BinaryWriter bw, string s, int length = -1)
        {
            var data = System.Text.Encoding.GetEncoding("Shift_JIS").GetBytes(s).ToList();

            int lackCount = 0;
            if (length > 0 && data.Count != length)
            {
                if (data.Count > length) data = data.Take(length - 1).ToList();
                if (data.Count < length - 1) lackCount = length - 1 - data.Count;
            }
            for (int i = 0; i < lackCount; i++)
            {
                data.Add(0);
            }
            data.Add(0);

            var aaa = data.ToArray();
            bw.Write(aaa);
        }

        string _path;
    }
}
