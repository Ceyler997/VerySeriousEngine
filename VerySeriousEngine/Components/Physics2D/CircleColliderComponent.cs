using SharpDX;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Components.Physics2D
{
    public class CircleColliderComponent : Physics2DComponent
    {
        private float radius;

        public override float Radius { get => radius; }

        public void SetRadius(float value) { radius = value; }

        public CircleColliderComponent(WorldObject owner, float radius, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            this.radius = radius;
        }

        public override bool IsOverlappedWith(Physics2DComponent other)
        {
            if (IsReachable(other) == false)
                return false;

            if (other is CircleColliderComponent)
                return true;

            if (other is RectangleColliderComponent otherRectangle)
            {
                // if rectangle centre inside the circle
                if (IsPointInside(otherRectangle.Location))
                    return true;

                // if circle centre inside the rectangle
                if (otherRectangle.IsPointInside(Location))
                    return true;

                var points = otherRectangle.Points;

                // if any of the rectangle points inside the circle
                foreach (var point in points)
                {
                    if (IsPointInside(point))
                        return true;
                }

                // if edges inside the radius
                for(var i = 0; i < points.Length; ++i)
                {
                    var firstPoint = points[i];
                    var secondPoint = points[(i + 1) % points.Length];
                    if (VSEMath.PointToSegmentDistance2D(Location, firstPoint, secondPoint) < Radius)
                        return true;
                }

                return false;
            }

            return false;
        }

        public override bool IsPointInside(Vector2 point)
        {
            return (Location - point).LengthSquared() < (Radius * Radius);
        }
    }
}
