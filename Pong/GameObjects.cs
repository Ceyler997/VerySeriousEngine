using SharpDX;
using System.Linq;
using VerySeriousEngine.Components;
using VerySeriousEngine.Components.Physics2D;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Shaders;
using VerySeriousEngine.Utils;

namespace Pong
{
    class Platform : WorldObject
    {
        public RectangleColliderComponent Collider { get; }
        public SimplePointsMeshComponent Mesh { get; }

        public Platform(float width, float height, Color color, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            var shaderSetup = new VertexColorShader();
            var indices = new int[] { 0, 1, 2, 1, 2, 3 };
            var points = new Vertex[]
            {
                new Vertex(){Location = new Vector3(-width/2,  height/2, .0f), Color = color.ToVector4() }, // upper left
                new Vertex(){Location = new Vector3( width/2,  height/2, .0f), Color = color.ToVector4() }, // upper right
                new Vertex(){Location = new Vector3(-width/2, -height/2, .0f), Color = color.ToVector4() }, // lower left
                new Vertex(){Location = new Vector3( width/2, -height/2, .0f), Color = color.ToVector4() }, // lower right
            };
            Mesh = new SimplePointsMeshComponent(this, shaderSetup, points, indices);

            Collider = new RectangleColliderComponent(this, width, height);
            Collider.OnOverlapBegin += Collider_OnOverlapBegin;
        }

        private void Collider_OnOverlapBegin(Physics2DComponent thisComponent, Physics2DComponent otherComponent)
        {
            if (otherComponent.Owner is Ball ball)
                ball.MovementDirection = ball.Collider.Location - Collider.Location;
        }
    }

    class Ball : WorldObject
    {

        private Vector2 movementDirection;

        public CircleColliderComponent Collider { get; }
        public SimplePointsMeshComponent Mesh { get; }

        public Vector2 MovementDirection {
            get => movementDirection;
            set => movementDirection = Vector2.Normalize(value);
        }
        public float MovementSpeed { get; set; } = 250;

        public Ball(float radius, Color color, int segmentsAmount = 12, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            var shaderSetup = new VertexColorShader();

            var points = new Vertex[segmentsAmount + 1];
            points[0] = new Vertex()
            {
                Location = Vector3.Zero,
                Color = color.ToVector4()
            };
            foreach (int segment in Enumerable.Range(0, segmentsAmount))
            {
                var screenLocation = VSEMath.RotatePointOnAngle(new Vector2(.0f, radius), MathUtil.TwoPi / segmentsAmount * segment); //turn up vector on corresponding angle

                var pointLocation = new Vector3(screenLocation, .0f);

                points[segment + 1] = new Vertex()
                {
                    Location = pointLocation,
                    Color = color.ToVector4(),
                };
            }

            var indices = new int[segmentsAmount * 3];
            foreach (int segment in Enumerable.Range(0, segmentsAmount))
            {
                indices[segment * 3] = 0;
                indices[segment * 3 + 1] = segment + 1;
                indices[segment * 3 + 2] = (segment + 1) % segmentsAmount + 1;
            }

            Mesh = new SimplePointsMeshComponent(this, shaderSetup, points, indices);

            Collider = new CircleColliderComponent(this, radius);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            WorldLocation = WorldLocation + new Vector3(MovementDirection * MovementSpeed * frameTime, 0);
        }
    }
}
