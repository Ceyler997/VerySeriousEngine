using SharpDX;
using System;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Components.Physics2D
{
    public class RectangleComponent : Physics2DComponent
    {

        public override float Radius => Convert.ToSingle(Math.Sqrt(Width * Width + Height * Height));

        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2[] Points {
            get {
                var points = new Vector2[]
                {
                    new Vector2( Width/2,  Height/2),
                    new Vector2( Width/2, -Height/2),
                    new Vector2(-Width/2, -Height/2),
                    new Vector2(-Width/2,  Height/2),
                };

                return Array.ConvertAll(points, 
                    point => VSEMath.RotatePointOnAngle(point, Angle) + Location);
            }
        }

        public RectangleComponent(WorldObject owner, float width, float height, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            Width = width;
            Height = height;
        }

        public override bool IsOverlappedWith(Physics2DComponent other)
        {
            if (other is CircleComponent)
                return other.IsOverlappedWith(this);

            if (IsReachable(other) == false)
                return false;

            if (other is RectangleComponent otherRectangle)
                return IsProjectionIntersets(otherRectangle) && otherRectangle.IsProjectionIntersets(this);

            return false;
        }

        public override bool IsPointInside(Vector2 point)
        {
            var localPoint = GlobalToLocal(point);

            if (Math.Abs(localPoint.X) > Width / 2)
                return false;

            if (Math.Abs(localPoint.Y) > Height / 2)
                return false;

            return true;
        }

        protected Vector2 GlobalToLocal(Vector2 point)
        {
            return VSEMath.RotatePointOnAngle(point - Location, -Angle);
        }

        protected bool IsProjectionIntersets(RectangleComponent other)
        {
            var projectedPoints = Array.ConvertAll(other.Points, point => GlobalToLocal(point));

            float maxX = Math.Max(Math.Max(projectedPoints[0].X, projectedPoints[1].X), Math.Max(projectedPoints[2].X, projectedPoints[3].X));
            if (maxX < -Width / 2)
                return false;

            float minX = Math.Min(Math.Min(projectedPoints[0].X, projectedPoints[1].X), Math.Min(projectedPoints[2].X, projectedPoints[3].X));
            if (minX > Width / 2)
                return false;

            float maxY = Math.Max(Math.Max(projectedPoints[0].Y, projectedPoints[1].Y), Math.Max(projectedPoints[2].Y, projectedPoints[3].Y));
            if (maxY < -Height / 2)
                return false;

            float minY = Math.Min(Math.Min(projectedPoints[0].Y, projectedPoints[1].Y), Math.Min(projectedPoints[2].Y, projectedPoints[3].Y));
            if (minY > Height / 2)
                return false;

            return true;
        }
    }
}
