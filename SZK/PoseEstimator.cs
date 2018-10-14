using Beastrack.Utility;
using KMR;
using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Numerics;
using SZK.Model;

namespace SZK
{
    public class PoseEstimator
    {
        public Doll EstimateSingleFrame(List<Keypoint> keypoints)
        {
            // Setup solver
            var solver = SolverContext.GetContext();
            var model = solver.CreateModel();

            // Setup decisions
            var doll = new Doll();
            var centerPositionDecision = new List<Decision>
            {
                new Decision(Domain.RealNonnegative, "CenterPositionX"),
                new Decision(Domain.RealNonnegative, "CenterPositionY"),
                new Decision(Domain.RealNonnegative, "CenterPositionZ"),
            };
            model.AddDecision(centerPositionDecision[0]);
            model.AddDecision(centerPositionDecision[1]);
            model.AddDecision(centerPositionDecision[2]);
            var angleDecisions = CreateAngleDecisions(doll);
            foreach (var decision in angleDecisions)
            {
                model.AddDecision(decision.Value[0]);
                model.AddDecision(decision.Value[1]);
                model.AddDecision(decision.Value[2]);
            }

            // Setup goal
            model.AddGoal("Goal", GoalKind.Maximize,
                          CalculateObjectiveFunction(doll, centerPositionDecision, angleDecisions, keypoints));

            // Solve
            var solution = solver.Solve();

            // Output
            ReflectDecisionsToDoll(doll, centerPositionDecision, angleDecisions);

            return doll;
        }

        Dictionary<BodyPart, List<Decision>> CreateAngleDecisions(Doll doll)
        {
            var decisions = new Dictionary<BodyPart, List<Decision>>();

            var targetParts = new List<BodyPart>
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

            foreach (var part in targetParts)
            {
                var limit = doll.GetEulerAngleLimit(part);
                decisions.Add(part,
                    new List<Decision>
                    {
                    new Decision(Domain.RealRange(limit.Min.HeadingDeg, limit.Max.HeadingDeg), $"{part.ToString()}-heading"),
                    new Decision(Domain.RealRange(limit.Min.PitchDeg, limit.Max.PitchDeg), $"{part.ToString()}-pitch"),
                    new Decision(Domain.RealRange(limit.Min.BankDeg, limit.Max.BankDeg), $"{part.ToString()}-bank"),
                    });
            }

            return decisions;
        }
        void ReflectDecisionsToDoll(Doll doll, List<Decision> centerPos, IDictionary<BodyPart, List<Decision>> angles)
        {
            doll.SetRootLocation(new Vector3(
                    (float)centerPos[0].GetDouble(),
                    (float)centerPos[1].GetDouble(),
                    (float)centerPos[2].GetDouble()));

            foreach (var angle in angles)
                doll.SetEulerAngle(angle.Key, new EulerAngle(
                    (float)angle.Value[0].GetDouble(),
                    (float)angle.Value[1].GetDouble(),
                    (float)angle.Value[2].GetDouble()));
        }
        double CalculateObjectiveFunction(Doll doll, List<Decision> centerPos, IDictionary<BodyPart, List<Decision>> angles, List<Keypoint> keypoints)
        {
            ReflectDecisionsToDoll(doll, centerPos, angles);
            doll.RenewAllPositions();
            return doll.CalculateResidualSum(keypoints);
        }

        public List<Doll> EstimateFrames(List<List<Keypoint>> keypointsCollection)
        {
            var estimatedDolls = new List<Doll>();
            foreach (var keypoints in keypointsCollection)
            {
                estimatedDolls.Add(EstimateSingleFrame(keypoints));
            }
            return estimatedDolls;
        }
    }
}
