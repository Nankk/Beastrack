using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SZK.Model;

namespace MUR.VmdNS
{
    public class Vmd
    {
        public Vmd()
        {
            Frames = new List<Frame>();
        }
        public Vmd(string modelName) : this()
        {
            ModelName = modelName;
        }
        public Vmd(string modelName, IDictionary<int, Doll> dolls) : this(modelName)
        {
            foreach (var numDollPair in dolls)
            {
                var frame = new Frame { Index = numDollPair.Key };
                var doll = numDollPair.Value;
                foreach (var pair in _bodyPartNameMap)
                {
                    var part = pair.Key;
                    var bone = new Bone();
                    bone.Name = pair.Value;
                    bone.Position = doll.GetPosition(part);
                    var angle = doll.GetEulerAngle(part);
                    // I dunno why but *heading and pitch* are inverted in vmd file
                    angle.Heading = -angle.Heading;
                    angle.Pitch = -angle.Pitch;
                    bone.Quaternion = angle.ToQuaternion();

                    frame.Bones.Add(bone.Name, bone);
                }
                Frames.Add(frame);
            }
            ItemsTotal = Frames.Select(f => f.Bones.Count).Sum();
        }

        public string Header { get; set; } = "Vocaloid Motion Data 0002";
        public string ModelName { get; set; }
        public int ItemsTotal { get; set; } // Need to be mutable: set when reading a vmd file
        public List<Frame> Frames { get; set; }

        IDictionary<BodyPart, string> _bodyPartNameMap = new Dictionary<BodyPart, string>
        {
            { BodyPart.Neck,      "首" },
            { BodyPart.RightEye,  "右目" },
            { BodyPart.LeftEye,   "左目" },
            { BodyPart.RightArm,  "右腕" },
            { BodyPart.RightElbow,"右ひじ" },
            { BodyPart.RightWrist,"右手首" },
            { BodyPart.LeftArm,   "左腕" },
            { BodyPart.LeftElbow, "左ひじ" },
            { BodyPart.LeftWrist, "左手首" },
            { BodyPart.RightLeg,  "右足" },
            { BodyPart.RightKnee, "右ひざ" },
            { BodyPart.RightAnkle,"右足首" },
            { BodyPart.RightToe,  "右つま先ＩＫ" },
            { BodyPart.LeftLeg,   "左足" },
            { BodyPart.LeftKnee,  "左ひざ" },
            { BodyPart.LeftAnkle, "左足首" },
            { BodyPart.LeftToe,   "左つま先ＩＫ" },
            { BodyPart.Upper,     "上半身" },
            { BodyPart.Center,    "センター" },
        };
    }
}
