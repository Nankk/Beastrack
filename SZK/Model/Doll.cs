using Beastrack.Utility;
using KMR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using SZK.Utility;

namespace SZK.Model
{
    public class Doll
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
        public EulerAngle GetEulerAngle(BodyPart bodyPart)
        {
            return _nodes[bodyPart].Angle;
        }
        public Range<EulerAngle> GetEulerAngleLimit(BodyPart bodyPart)
        {
            return _nodes[bodyPart].Limit;
        }
        public void SetEulerAngle(BodyPart bodyPart, EulerAngle angles)
        {
            _nodes[bodyPart].Angle = angles;
        }
        public void SetEulerAngles(IDictionary<BodyPart, EulerAngle> angles)
        {
            foreach (var angle in angles)
            {
                if (!_nodes.ContainsKey(angle.Key))
                    ErrorHandler.Terminate($"Estimation doll does not contains a body part {angle.Key}");

                _nodes[angle.Key].Angle = angle.Value;
            }
        }

        // Renew all node's positions based on their own and parent's state
        public void RenewAllPositions()
        {
            _nodes[_root].Propagate();
        }
        public double CalculateResidualSum(float w, float h, float fovRad, List<Keypoint> keypoints)
        {
            double sum = 0;
            for (int i = 0; i < keypoints.Count; i++)
            {
                if (!_body25Map.ContainsKey(i)) continue;
                BodyPart bodyPart = _body25Map[i];
                var projected = Project(w, h, fovRad, new Vector3(
                    _nodes[bodyPart].Position.X,
                    _nodes[bodyPart].Position.Y,
                    _nodes[bodyPart].Position.Z));

                double diffX = projected.X - keypoints[i].X;
                double diffY = projected.Y - keypoints[i].Y;
                sum += Math.Sqrt(diffX * diffX + diffY * diffY);
            }

            return sum;
        }

        public Node RootNode
        {
            get { return _nodes[_root]; }
        }

        private int _MyProperty;

        // --- private ---
        Vector2 Project(float w, float h, float fovRad, Vector3 v)
        {
            // Add the w value for the calculation
            var v4 = new Vector4(v, 1f);

            // System.Numerics assumes the right-hand coordinate system
            var modelViewMatrix = Matrix4x4.CreateLookAt(new Vector3(0, 1, 4), new Vector3(0, 0, 0), Vector3.UnitY);
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fovRad, w / h, 0.1f, 1000.0f);
            var viewportMatrix = new Matrix4x4(w / 2, 0, 0, 0,
                                                 0, -h / 2, 0, 0,
                                                 0, 0, 1, 0,
                                               w / 2, h / 2, 0, 1);

            v4 = Vector4.Transform(v4, modelViewMatrix);
            v4 = Vector4.Transform(v4, projectionMatrix);
            v4 = Vector4.Multiply(1f / v4.W, v4);  // Manual normalization required
            v4 = Vector4.Transform(v4, viewportMatrix);

            return new Vector2(v4.X, v4.Y);
        }
        void InitializeOpenGl()
        {
            int w = 800;
            int h = 600;

            OpenTK.Graphics.OpenGL.GL.Viewport(0, 0, w, h);
            OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4,
                (float)w / h,
                0.1f,
                1000.0f);
            OpenTK.Graphics.OpenGL.GL.LoadMatrix(ref projection);

        }

        void SetConnectionInformation()
        {
            // Center -> [Upper RightLeg LeftLeg]
            _nodes[BodyPart.Center].AddChild(_nodes[BodyPart.Upper], new Vector3(0.00f, 5.47f, -0.31f));
            _nodes[BodyPart.Center].AddChild(_nodes[BodyPart.RightLeg], new Vector3(-1.01f, 3.36f, -0.37f));
            _nodes[BodyPart.Center].AddChild(_nodes[BodyPart.LeftLeg], new Vector3(1.01f, 3.36f, -0.37f));
            // Neck -> [RightEye LeftEye RightArm LeftArm]
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.RightEye], new Vector3(-0.41f, 1.09f, -0.60f));
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.LeftEye], new Vector3(0.41f, 1.09f, -0.60f));
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.RightArm], new Vector3(-1.35f, -0.71f, -0.14f));
            _nodes[BodyPart.Neck].AddChild(_nodes[BodyPart.LeftArm], new Vector3(1.35f, -0.71f, -0.14f));

            _nodes[BodyPart.Upper].AddChild(_nodes[BodyPart.Neck], new Vector3(0.00f, 3.60f, 0.40f));

            _nodes[BodyPart.RightArm].AddChild(_nodes[BodyPart.RightElbow], new Vector3(-2.19f, -1.71f, 0.00f));
            _nodes[BodyPart.RightElbow].AddChild(_nodes[BodyPart.RightWrist], new Vector3(-1.75f, -1.36f, 0.14f));

            _nodes[BodyPart.LeftArm].AddChild(_nodes[BodyPart.LeftElbow], new Vector3(2.19f, -1.71f, 0.00f));
            _nodes[BodyPart.LeftElbow].AddChild(_nodes[BodyPart.LeftWrist], new Vector3(1.75f, -1.36f, 0.14f));

            _nodes[BodyPart.RightLeg].AddChild(_nodes[BodyPart.RightKnee], new Vector3(0.29f, -4.68f, 0.02f));
            _nodes[BodyPart.RightKnee].AddChild(_nodes[BodyPart.RightAnkle], new Vector3(-0.05f, -4.93f, 0.39f));
            _nodes[BodyPart.RightAnkle].AddChild(_nodes[BodyPart.RightToe], new Vector3(-0.04f, -1.19f, -2.18f));

            _nodes[BodyPart.LeftLeg].AddChild(_nodes[BodyPart.LeftKnee], new Vector3(-0.29f, -4.68f, 0.02f));
            _nodes[BodyPart.LeftKnee].AddChild(_nodes[BodyPart.LeftAnkle], new Vector3(0.05f, -4.93f, 0.39f));
            _nodes[BodyPart.LeftAnkle].AddChild(_nodes[BodyPart.LeftToe], new Vector3(0.04f, -1.19f, -2.18f));
        }
        void SetAngleLimits()
        {
            _nodes[BodyPart.LeftLeg].Limit = new Range<EulerAngle>(new EulerAngle(-90, -130, -90), new EulerAngle(5, 5, 10));
            _nodes[BodyPart.LeftKnee].Limit = new Range<EulerAngle>(new EulerAngle(-5, -2, 0), new EulerAngle(0, 170, 0));
            _nodes[BodyPart.LeftAnkle].Limit = new Range<EulerAngle>(new EulerAngle(0, -15, 0), new EulerAngle(0, 75, 0));

            _nodes[BodyPart.RightLeg].Limit = new Range<EulerAngle>(new EulerAngle(-5, -130, -10), new EulerAngle(90, 5, 90));
            _nodes[BodyPart.RightKnee].Limit = new Range<EulerAngle>(new EulerAngle(0, -2, 0), new EulerAngle(5, 170, 0));
            _nodes[BodyPart.RightAnkle].Limit = new Range<EulerAngle>(new EulerAngle(0, -15, 0), new EulerAngle(0, 75, 0));

            _nodes[BodyPart.Upper].Limit = new Range<EulerAngle>(new EulerAngle(-45, -10, -20), new EulerAngle(45, 90, 20));
            _nodes[BodyPart.Neck].Limit = new Range<EulerAngle>(new EulerAngle(-80, -45, -50), new EulerAngle(80, 45, 50));
        }

        BodyPart _root = BodyPart.Center;
        IDictionary<BodyPart, Node> _nodes { get; set; }
        IDictionary<int, BodyPart> _body25Map = new Dictionary<int, BodyPart>
        {
            {  2, BodyPart.RightArm },
            {  3, BodyPart.RightElbow },
            {  4, BodyPart.RightWrist },
            {  5, BodyPart.LeftArm },
            {  6, BodyPart.LeftElbow },
            {  7, BodyPart.LeftWrist },
            {  9, BodyPart.RightLeg },
            { 10, BodyPart.RightKnee },
            { 11, BodyPart.RightAnkle },
            { 12, BodyPart.LeftLeg },
            { 13, BodyPart.LeftKnee },
            { 14, BodyPart.LeftAnkle },
            { 15, BodyPart.RightEye },
            { 16, BodyPart.LeftEye },
            { 19, BodyPart.LeftToe },
            { 22, BodyPart.RightToe },
        };
    }
}
