using Beastrack.Utility;
using KMR;
using Microsoft.SolverFoundation.Solvers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using SZK.Model;

namespace SZK
{
    public class PoseEstimator
    {
        public List<Doll> EstimateFrames(float w, float h, List<List<Keypoint>> keypointsCollection)
        {
            var estimatedDolls = new List<Doll>();
            foreach (var keypoints in keypointsCollection)
            {
                estimatedDolls.Add(EstimateSingleFrame(w, h, keypoints));
            }
            return estimatedDolls;
        }
        public Doll EstimateSingleFrame(float w, float h, List<Keypoint> keypoints)
        {
            // Setup unknown variables' information
            var estimationDoll = new Doll();
            double[] unknowns;
            double[] lowerLimits;
            double[] upperLimits;
            InitializeInputArrays(estimationDoll, out unknowns, out lowerLimits, out upperLimits);
            double[] unknowns2 = new double[unknowns.Length];

            // Call Nelder-mead solver in Microsoft.Solver.Foundation librabry
            var solution = NelderMeadSolver.Solve(
                (input) =>
                {
                    ReflectDecisionsToDoll(input, estimationDoll);
                    estimationDoll.RenewAllPositions();
                    return estimationDoll.CalculateResidualSum(w, h, (float)input[0], keypoints);
                },
                unknowns, lowerLimits, upperLimits);

            Debug.WriteLine($"{solution.Result}");
            Debug.WriteLine($"Solution = {solution.GetSolutionValue(0)}");

            // Output
            return estimationDoll;
        }

        // --- private ---
        void InitializeInputArrays(Doll doll, out double[] unknowns, out double[] lowerLimits, out double[] upperLimits)
        {
            var unknownList = new List<double>();
            var lowerLimitList = new List<double>();
            var upperLimitList = new List<double>();

            // Field of view
            unknownList.Add(System.Math.PI / 4); // Initial assumption is 45 degree
            lowerLimitList.Add(30 * System.Math.PI / 180);
            upperLimitList.Add(50 * System.Math.PI / 180);

            // Center position
            for (int i = 0; i < 3; i++)
            {
                unknownList.Add(0.0); // Origin
                lowerLimitList.Add(double.MinValue);
                upperLimitList.Add(double.MaxValue);
            }

            // Angles
            foreach (var part in _targetParts)
            {
                // Heading-Pitch-Bank order
                for (int i = 0; i < 3; i++) unknownList.Add(0.0); // No rotation
                var limit = doll.GetEulerAngleLimit(part);
                lowerLimitList.Add(limit.Min.HeadingDeg);
                lowerLimitList.Add(limit.Min.PitchDeg);
                lowerLimitList.Add(limit.Min.BankDeg);
                upperLimitList.Add(limit.Max.HeadingDeg);
                upperLimitList.Add(limit.Max.PitchDeg);
                upperLimitList.Add(limit.Max.BankDeg);
            }

            unknowns = unknownList.ToArray();
            lowerLimits = lowerLimitList.ToArray();
            upperLimits = upperLimitList.ToArray();
        }
        void ReflectDecisionsToDoll(double[] unknowns, Doll doll)
        {
            int idx = 1; // idx = 0 is used by the field of view

            // Center position
            doll.SetRootLocation(new Vector3(
                (float)unknowns[idx++],
                (float)unknowns[idx++],
                (float)unknowns[idx++]));

            // Angles
            foreach (var part in _targetParts)
            {
                doll.SetEulerAngle(part, new EulerAngle(
                    (float)unknowns[idx++],
                    (float)unknowns[idx++],
                    (float)unknowns[idx++]));
            }
        }

        List<BodyPart> _targetParts = new List<BodyPart>
        {
            BodyPart.Center,
            BodyPart.Neck,
            BodyPart.RightArm,
            BodyPart.RightElbow,
            BodyPart.LeftArm,
            BodyPart.LeftElbow,
            BodyPart.RightLeg,
            BodyPart.RightKnee,
            BodyPart.RightAnkle,
            BodyPart.LeftLeg,
            BodyPart.LeftKnee,
            BodyPart.LeftAnkle,
            BodyPart.Upper,
        };
    }
}
