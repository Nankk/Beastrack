using Beastrack.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KMR.IO
{
    public class OpenPoseReader
    {
        public OpenPoseReader(string openPoseOutDir)
        {
            if (openPoseOutDir.Last() != '/')
                openPoseOutDir = openPoseOutDir + "/";

            _openPoseOutDir = openPoseOutDir;
        }

        public List<Keypoint> ReadOneFile(string filename)
        {
            List<List<Keypoint>> people = new List<List<Keypoint>>();

            string fullPath = _openPoseOutDir + filename;
            string content = "";
            using (var sr = new StreamReader(fullPath))
            {
                content = sr.ReadToEnd();
            }

            int lastFoundIdx = 0;
            while (true)
            {
                // Search data "pose_keypoints_2d"
                int begin = content.IndexOf("pose_keypoints_2d", lastFoundIdx);
                if (begin == -1) break;
                begin = content.IndexOf("[", begin) + 1;
                int end = content.IndexOf("]", begin);
                lastFoundIdx = end;

                // Parse single person data
                string arrayString = content.Substring(begin, end - begin);
                var person = ParseKeypoints(arrayString);
                people.Add(person);
            }

            // For now only single person tracking is implemented
            return people[0];
        }
        public List<List<Keypoint>> ReadAll()
        {
            var keypointsCollection = new List<List<Keypoint>>();
            var jsonFileNames = Directory.GetFiles(_openPoseOutDir, "*.json");
            foreach (var fileName in jsonFileNames)
            {
                keypointsCollection.Add(ReadOneFile(_openPoseOutDir + fileName));
            }
            return keypointsCollection;
        }

        // --- private ---
        List<Keypoint> ParseKeypoints(string arrayString)
        {
            var strings = arrayString.Split(new char[] { ' ', '\t', ',', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            var values = strings.Select(s => float.Parse(s)).ToList();

            if (values.Count != 75)
                ErrorHandler.Terminate($"Only Pose25 format is supported. Keypoints count is {values.Count}");

            var keypoints = new List<Keypoint>();
            int count = 0;
            for (int i = 0; i < 25; i++)
            {
                keypoints.Add(new Keypoint(values[count++], values[count++], values[count++]));
            }

            return keypoints;
        }

        string _openPoseOutDir;
    }
}
