using SharpDX;
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

        public Platform(float Width, float Height, Color color, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            var shaderSetup = new ShaderSetup("Shader.hlsl", SimplePoint.InputElements);
            var indices = new int[] { 0, 1, 2, 1, 2, 3 };
            var points = new SimplePoint[]
            {
                new SimplePoint { Location = new Vector4(-Width/2,  Height/2, .0f, 1.0f), Color = color.ToVector4() }, // upper left
                new SimplePoint { Location = new Vector4( Width/2,  Height/2, .0f, 1.0f), Color = color.ToVector4() }, // upper right
                new SimplePoint { Location = new Vector4(-Width/2, -Height/2, .0f, 1.0f), Color = color.ToVector4() }, // lower left
                new SimplePoint { Location = new Vector4( Width/2, -Height/2, .0f, 1.0f), Color = color.ToVector4() }, // lower right
            };
            Mesh = new SimplePointsMeshComponent(this, shaderSetup, points, indices);

            Collider = new RectangleComponent(this, Width, Height);
        }

    }
}
