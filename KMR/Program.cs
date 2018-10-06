using KMR.IO;
using System;

namespace KMR
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            string path = @"C:\tmp\Sample_000000000132_keypoints.json";
#else

#endif
            var opr = new OpenPoseReader(path);
            var keypoints = opr.Read();
            Console.WriteLine($"Keypoints:");
            int count = 0;
            foreach (var kp in keypoints)
            {
                Console.WriteLine($"Point {count++, 10}: ({kp.X}, {kp.Y}, {kp.Reliability})");
            }
            Console.ReadKey(true);
        }
    }
}
