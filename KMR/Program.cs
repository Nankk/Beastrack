using KMR.IO;
using System;

// Kinematic Motionframe Reducer
namespace KMR
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            string dir = @"C:\tmp\";
#else

#endif
            var opr = new OpenPoseReader(dir);
            var keypoints = opr.ReadOneFile("Sample_000000000132_keypoints.json");
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
