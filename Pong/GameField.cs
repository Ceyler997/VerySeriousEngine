using SharpDX;
using VerySeriousEngine.Components.Physics2D;
using VerySeriousEngine.Core;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace Pong
{
    enum ESide
    {
        Left,
        Right,
    }

    delegate void OnGoal(ESide lostSide);

    class GameField : WorldObject
    {
        public float FieldWidth { get; }
        public float FieldHeight { get; }

        public event OnGoal Goal;

        private FieldEdge LeftEdge { get; }
        private FieldEdge RightEdge { get; }
        private FieldEdge TopEdge { get; }
        private FieldEdge BottomEdge { get; }

        public GameField(float width, float height, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            FieldWidth = width;
            FieldHeight = height;

            LeftEdge = new FieldEdge(0, height, this, "Left Edge")
            {
                Location = Vector3.Left * width / 2,
            };
            RightEdge = new FieldEdge(0, height, this, "Right Edge")
            {
                Location = Vector3.Right * width / 2,
            };
            TopEdge = new FieldEdge(width, 0, this, "Top Edge")
            {
                Location = Vector3.Up * height / 2,
            };
            BottomEdge = new FieldEdge(width, 0, this, "Bottom Edge")
            {
                Location = Vector3.Down * height / 2,
            };

            LeftEdge.Collider.OnOverlapBegin += EdgeOverlapBegin;
            RightEdge.Collider.OnOverlapBegin += EdgeOverlapBegin;
            TopEdge.Collider.OnOverlapBegin += EdgeOverlapBegin;
            BottomEdge.Collider.OnOverlapBegin += EdgeOverlapBegin;
        }

        private void EdgeOverlapBegin(Physics2DComponent thisComponent, Physics2DComponent otherComponent)
        {
            if(otherComponent.Owner is Ball ball)
            {
                if(thisComponent.Owner == LeftEdge)
                {
                    Goal?.Invoke(ESide.Left);
                    return;
                }

                if(thisComponent.Owner == RightEdge)
                {
                    Goal?.Invoke(ESide.Right);
                    return;
                }

                if(thisComponent.Owner == TopEdge)
                {
                    ball.MovementDirection = VSEMath.MirrorPoint(ball.MovementDirection, Vector2.UnitY) * -1;
                    return;
                }

                if(thisComponent.Owner == BottomEdge)
                {
                    ball.MovementDirection = VSEMath.MirrorPoint(ball.MovementDirection, Vector2.UnitY) * -1;
                    return;
                }
            }
        }
    }

    class FieldEdge : WorldObject
    {
        public RectangleColliderComponent Collider { get; }
        public VerySeriousEngine.Components.SimplePointsMeshComponent Mesh { get; }

        public FieldEdge(float width, float height, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            Collider = new RectangleColliderComponent(this, width, height);
        }
    }
}
