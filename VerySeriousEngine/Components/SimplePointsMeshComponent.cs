using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VerySeriousEngine.Core;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Components
{
    public class SimplePointsMeshComponent : GameComponent, IRenderable
    {
        private WorldObject worldOwner;

        private GeometrySetup[] setup;

        public SimplePointsMeshComponent(WorldObject owner, ShaderSetup shaderSetup, SimplePoint[] points, int[] indices, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            worldOwner = owner;
            var constructor = Game.GameInstance.GameConstructor;

            var indexBuffer = constructor.CreateBuffer(indices, BindFlags.IndexBuffer);
            var indexCount = indices.Length;

            var vertexBuffer = constructor.CreateBuffer(points, BindFlags.VertexBuffer);
            var vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Marshal.SizeOf<SimplePoint>(), 0);
            BufferSetup geometrySetup = new BufferSetup(indexBuffer, vertexBufferBinding, indexCount);

            setup = new GeometrySetup[]
            {
                new GeometrySetup(shaderSetup, geometrySetup),
            };
        }

        public Matrix WorldMatrix => worldOwner.WorldTransform;

        public bool IsRendered => IsActive;

        public IEnumerable<GeometrySetup> Geometry => setup;
    }
}
