using Beastrack.Utility;
using MUR.IO;
using System;

// Mmd Utilities/Unified/ Regulator??
// 何の略にしよう？役割は正直MMD関連ファイルとのインターフェース以外の何物でもない
namespace MUR
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            string path = @"C:\tmp\Sample_000000000213_keypoints.vmd";
#else
            string path = args[0];
#endif

            var vr = new VmdReader(path);
            var vmd = vr.Read();

            var vw = new VmdWriter(path.Substring(0, path.LastIndexOf('.')) + "_rewrite.vmd");
            vw.Write(vmd);

            foreach (var frame in vmd.Frames)
            {
                Console.WriteLine($"Frame #{frame.Index}");
                Console.WriteLine($"-----------------------------------------");

                foreach (var bone in frame.Bones)
                {
                    Console.WriteLine($"Bone {bone.Key}:");

                    var p = bone.Value.Position;
                    Console.WriteLine($"  Position     (x,y,z)   = ({p.X}, {p.Y}, {p.Z})");
                    var q = bone.Value.Quaternion;
                    Console.WriteLine($"  Quartenion   (x,y,z,w) = ({q.X}, {q.Y}, {q.Z}, {q.W})");
                    var e = EulerAngle.FromQuaternion(q);
                    Console.WriteLine($"  Euler angles (h,p,b)   = ({e.HeadingDeg}, {e.PitchDeg}, {e.BankDeg})");
                }
                Console.WriteLine($"");
            }

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
#else

#endif
        }
    }
}
