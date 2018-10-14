using Beastrack.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SZK.Model
{
    class Node
    {
        public Node()
        {
            Children = new Dictionary<Node, Vector3>();
            Position = new Vector3();
            Angle = new EulerAngle();
            Limit = new Range<EulerAngle>(new EulerAngle(float.MinValue, float.MinValue, float.MinValue),
                                          new EulerAngle(float.MaxValue, float.MaxValue, float.MaxValue));
        }

        public void AddChild(Node node, Vector3 pointerToChild)
        {
            Children.Add(node, pointerToChild);
        }
        public void Propagate()
        {
            foreach (var pair in Children)
            {
                // Renew child's position based on parent's position and direction (and scaling factor)
                var child = pair.Key;
                var pointer = pair.Value;
                pointer = Vector3.Transform(pointer, Angle.ToQuaternion());
                pointer = Vector3.Multiply(LengthScale, pointer);
                child.Position = Vector3.Add(Position, pointer);

                // Let child do the same procedure
                child.Propagate();
            }
        }

        public Vector3 Position { get; set; }
        public float LengthScale { get; set; }

        public EulerAngle Angle
        {
            get { return _Angle; }
            set
            {
                if (_Angle == value) return;
                _Angle = value;

                if (!Angle.Heading.IsBetween(Limit.Min.Heading, Limit.Max.Heading) ||
                    !Angle.Pitch.IsBetween(Limit.Min.Pitch, Limit.Max.Pitch) ||
                    !Angle.Bank.IsBetween(Limit.Min.Bank, Limit.Max.Bank))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Bone.Angle: Specified angle exceeds the limit.");
                    sb.AppendLine($"  Input: Heading = {Angle.Heading}, Pitch = {Angle.Pitch}, Bank = {Angle.Bank}");
                    sb.AppendLine($"  Lower: Heading = {Limit.Min.Heading}, Pitch = {Limit.Min.Pitch}, Bank = {Limit.Min.Bank}");
                    sb.AppendLine($"  Upper: Heading = {Limit.Max.Heading}, Pitch = {Limit.Max.Pitch}, Bank = {Limit.Max.Bank}");
                    ErrorHandler.Throw(sb.ToString());
                }
            }
        }
        public Range<EulerAngle> Limit { get; set; }
        public IDictionary<Node, Vector3> Children { get; private set; }

        // --- private ---
        bool _hasChild
        {
            get
            {
                return Children.Count != 0;
            }
        }

        #region ...
        // Background fields
        private EulerAngle _Angle;
        #endregion
    }
}
