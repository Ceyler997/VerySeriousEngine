using SharpDX;
using System;

namespace VerySeriousEngine.Utils
{
    public class VSEMath
    {
        public static float PointToSegmentDistance2D(Vector2 Point, Vector2 SegmentPoint1, Vector2 SegmentPoint2)
        {
            var segmentLengthSq = (SegmentPoint1 - SegmentPoint2).LengthSquared();

            if (segmentLengthSq < MathUtil.ZeroTolerance)
                return (Point - SegmentPoint1).Length();


            float projectPoint = Vector2.Dot(Point - SegmentPoint1, SegmentPoint2 - SegmentPoint1) / segmentLengthSq;
            projectPoint = MathUtil.Clamp(projectPoint, 0.0f, 1.0f);

            var projection = SegmentPoint1 + projectPoint * (SegmentPoint2 - SegmentPoint1);

            return (projection - Point).Length();
        }

        public static Vector2 RotatePointOnAngle(Vector2 Point, float Angle)
        {
            return new Vector2((float)(Point.X * Math.Cos(Angle) - Point.Y * Math.Sin(Angle)),
                (float)(Point.Y * Math.Cos(Angle) + Point.X * Math.Sin(Angle)));
        }
    }
}
