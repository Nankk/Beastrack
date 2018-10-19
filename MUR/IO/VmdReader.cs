using Beastrack.Utility;
using MUR.VmdNS;
using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace MUR.IO
{
    public class VmdReader
    {
        public VmdReader(string path)
        {
            _path = path;
        }

        public Vmd Read()
        {
            var vmd = new Vmd();
            using (var br = new BinaryReader(File.OpenRead(_path)))
            {
                ReadHeader(br, ref vmd);
                ReadFrames(br, ref vmd);
            }
            return vmd;
        }

        // --- private ---
        void ReadHeader(BinaryReader br, ref Vmd vmd)
        {
            vmd.Header = ReadString(br, 30);
            vmd.ModelName = ReadString(br, 20);
            vmd.ItemsTotal = br.ReadInt32();
        }
        void ReadFrames(BinaryReader br, ref Vmd vmd)
        {
            Frame currentFrame = new Frame { Index = 0 };
            var baseStream = br.BaseStream;

            try
            {
                while (baseStream.Position != baseStream.Length)
                {
                    var bone = new Bone();
                    string boneName = ReadString(br, 15);
                    // EOF checking workaround: For some reason baseStream checking doesn't work
                    if (string.IsNullOrEmpty(boneName)) break;
                    int frameIdx = br.ReadInt32();

                    // Finalize and register the current frame and create a new instance when the frame index increases
                    if (currentFrame.Index != frameIdx)
                    {
                        if (currentFrame.Index > frameIdx)
                            throw new Exception("Frame indices are not sorted in ascending order.");

                        vmd.Frames.Add(currentFrame);
                        currentFrame = new Frame { Index = frameIdx };
                    }

                    // Read bone data and set to the current frame
                    bone.Name = boneName;
                    float x = br.ReadSingle();
                    float y = br.ReadSingle();
                    float z = br.ReadSingle();
                    bone.Position = new Vector3(x, y, z);
                    float qx = br.ReadSingle();
                    float qy = br.ReadSingle();
                    float qz = br.ReadSingle();
                    float qw = br.ReadSingle();
                    bone.Quaternion = new Quaternion(qx, qy, qz, qw);
                    bone.InterpolationParameters = br.ReadBytes(64).ToList();
                    currentFrame.Bones.Add(boneName, bone);
                }

                // Register the last frame
                vmd.Frames.Add(currentFrame);
            }
            catch (Exception ex)
            {
                string msg = $"Following error occured while reading {_path}:\n  {ex.Message}";
                ErrorHandler.AskTermination(msg);
            }
        }
        string ReadString(BinaryReader br, int length)
        {
            // TODO: Check if this procedure is really required
            var bytes = br.ReadBytes(length).ToList();
            bytes = bytes.GetRange(0, bytes.IndexOf((byte)'\0'));
            return System.Text.Encoding.GetEncoding(932).GetString(bytes.ToArray());
        }

        string _path;
    }
}
