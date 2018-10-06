using Beastrack.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Beastrack.Model
{
    class Doll
    {
        public Doll()
        {
            // Relate body parts and bones
            _nodes = new Dictionary<BodyPart, Node>();
            foreach (BodyPart bp in Enum.GetValues(typeof(BodyPart))) _nodes.Add(bp, new Node());

            SetConnectionInformation();
            SetAngleLimits();
        }

        public Vector3 GetPosition(BodyPart bodyPart)
        {
            return _nodes[bodyPart].Position;
        }
        public void SetRootLocation(Vector3 position)
        {
            _nodes[_root].Position = position;
        }
        public void SetRootEulerAngles(EulerAngle angles)
        {
            _nodes[_root].Angle = angles;
        }
        public void SetQuarterion(BodyPart bodyPart, EulerAngle angles)
        {
            _nodes[bodyPart].Angle = angles;
        }

        // Renew all node's positions based on their own and parent's state
        public void RenewAllPositions()
        {
            _nodes[_root].Propagate();
        }

        // --- private ---
        void SetConnectionInformation()
        {
            // Center -> [Upper RightLeg LeftLeg]
            _nodes[BodyPart.Center].AddChild(_nodes[BodyPart.Upper], new Vector3(0.00f, 5.47f, -0.31f));
            _nodes[BodyPart.Center].AddChild(_nodes[BodyPart.RightLeg], new Vector3(-1.01f, 3.36f, -0.37f));
            _nodes[BodyPart.Center].AddChild(_nodes[BodyPart.LeftLeg], new Vector3(1.01f, 3.36f, -0.37f));
            // Neck -> [RightEye LeftEye RightArm LeftArm]
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.RightEye], new Vector3(-0.41f, 1.09f, -0.60f));
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.LeftEye],  new Vector3(0.41f, 1.09f, -0.60f  ));
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.RightArm], new Vector3(-1.35f, -0.71f, -0.14f));
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.LeftArm],  new Vector3(1.35f, -0.71f, -0.14f ));

            _nodes[BodyPart.Upper].AddChild(_nodes[BodyPart.Neck], new Vector3(0.00f, 3.60f, 0.40f));
             
            _nodes[BodyPart.RightArm  ].AddChild(_nodes[BodyPart.RightElbow], new Vector3(-2.19f, -1.71f, 0.00f));
            _nodes[BodyPart.RightElbow].AddChild(_nodes[BodyPart.RightWrist], new Vector3(-1.75f, -1.36f, 0.14f));

            _nodes[BodyPart.LeftArm  ].AddChild(_nodes[BodyPart.LeftElbow], new Vector3(2.19f, -1.71f, 0.00f));
            _nodes[BodyPart.LeftElbow].AddChild(_nodes[BodyPart.LeftWrist], new Vector3(1.75f, -1.36f, 0.14f));

            _nodes[BodyPart.RightLeg  ].AddChild(_nodes[BodyPart.RightKnee ], new Vector3(0.29f, -4.68f, 0.02f));
            _nodes[BodyPart.RightKnee ].AddChild(_nodes[BodyPart.RightAnkle], new Vector3(-0.05f, -4.93f, 0.39f ));
            _nodes[BodyPart.RightAnkle].AddChild(_nodes[BodyPart.RightToe  ], new Vector3(-0.04f, -1.19f, -2.18f));

            _nodes[BodyPart.LeftLeg  ].AddChild(_nodes[BodyPart.LeftKnee ], new Vector3(-0.29f, -4.68f, 0.02f));
            _nodes[BodyPart.LeftKnee ].AddChild(_nodes[BodyPart.LeftAnkle], new Vector3(0.05f, -4.93f, 0.39f ));
            _nodes[BodyPart.LeftAnkle].AddChild(_nodes[BodyPart.LeftToe  ], new Vector3(0.04f, -1.19f, -2.18f));
        }
        void SetAngleLimits()
        {
            _nodes[BodyPart.LeftLeg].Limit = new Range<EulerAngle>(new EulerAngle(), new EulerAngle());
        }

        BodyPart _root = BodyPart.Center;
        IDictionary<BodyPart, Node> _nodes { get; set; }
    }
}
