using SharpDX;
using System.Linq;
using VerySeriousEngine.Components;
using VerySeriousEngine.Components.Physics2D;
using VerySeriousEngine.Core;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace Pong
{
    class Platform : WorldObject
    {
        public RectangleComponent Collider { get; }
        public SimplePointsMeshComponent Mesh { get; }

        public Platform(float width, float height, Color color, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            var shaderSetup = new ShaderSetup("Shader.hlsl", SimplePoint.InputElements);
            var indices = new int[] { 0, 1, 2, 1, 2, 3 };
            var points = new SimplePoint[]
            {
                new SimplePoint { Location = new Vector4(-width/2,  height/2, .0f, 1.0f), Color = color.ToVector4() }, // upper left
                new SimplePoint { Location = new Vector4( width/2,  height/2, .0f, 1.0f), Color = color.ToVector4() }, // upper right
                new SimplePoint { Location = new Vector4(-width/2, -height/2, .0f, 1.0f), Color = color.ToVector4() }, // lower left
                new SimplePoint { Location = new Vector4( width/2, -height/2, .0f, 1.0f), Color = color.ToVector4() }, // lower right
            };
            Mesh = new SimplePointsMeshComponent(this, shaderSetup, points, indices);

            Collider = new RectangleComponent(this, width, height);
        }

    }

    class Ball : WorldObject
    {

        public CircleComponent Collider { get; }
        public SimplePointsMeshComponent Mesh { get; }

        public Vector2 MovementDirection { get; set; }
        public float MovementSpeed { get; set; } = 250;

        public Ball(float radius, Color color, int segmentsAmount = 12, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            var shaderSetup = new ShaderSetup("Shader.hlsl", SimplePoint.InputElements);

            var points = new SimplePoint[segmentsAmount + 1];
            points[0] = new SimplePoint() { Location = new Vector4(Vector3.Zero, 1.0f), Color = color.ToVector4() };
            foreach (int segment in Enumerable.Range(0, segmentsAmount))
            {
                Vector2 screenLocation = VSEMath.RotatePointOnAngle(new Vector2(.0f, radius), MathUtil.TwoPi / segmentsAmount * segment); //turn up vector on corresponding angle

                Vector4 pointLocation = new Vector4(screenLocation, .0f, 1.0f);

                points[segment + 1] = new SimplePoint() { Location = pointLocation, Color = color.ToVector4() };
            }

            var indices = new int[segmentsAmount * 3];
            foreach (int segment in Enumerable.Range(0, segmentsAmount))
            {
                indices[segment * 3] = 0;
                indices[segment * 3 + 1] = segment + 1;
                indices[segment * 3 + 2] = (segment + 1) % segmentsAmount + 1;
            }

            Mesh = new SimplePointsMeshComponent(this, shaderSetup, points, indices);

            Collider = new CircleComponent(this, radius);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            WorldLocation = WorldLocation + new Vector3(MovementDirection * MovementSpeed * frameTime, 0);
        }
    }
}
